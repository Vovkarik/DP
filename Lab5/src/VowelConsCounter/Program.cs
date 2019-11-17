using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace VowelConsCounter
{
    class Program
    {
        static HashSet<char> Vowels = new HashSet<char> {'a', 'e', 'i', 'u', 'o'};
        static HashSet<char> Consonants = new HashSet<char> {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("vowel-cons-counter-jobs-hints", (channel, message) => {
                string countMessege = redis.GetDatabase().ListRightPop("vowel-cons-counter-jobs");
                Console.WriteLine(countMessege);
                string[] serializedMessege = countMessege.Split(" : ");
                string id = serializedMessege[1];
                string value = db.StringGet(id);
                var msg = BuildJobMessage(GetVowelsAndCons(value) + " : " + id);
                SendMessage(msg, redis);
                //sub.Publish("vowel-cons-rater-jobs", "RateVowelConsJob : " + id + " : " + GetVowelsAndCons(value));
            });
            Console.ReadLine();
        }

        static string GetVowelsAndCons(string value)
        {
            value = value.ToLower();
            int vowels = 0, consonants = 0;
            foreach (var letter in value)
            {                
                if(Vowels.Contains(letter))
                {
                    vowels++;
                }
                if(Consonants.Contains(letter))
                {
                    consonants++;
                }
            }
            Console.WriteLine(vowels + " : " + consonants); 
            return (vowels + " : " + consonants);
        }

        
        private static void SendMessage(string message, IConnectionMultiplexer redis )
        {
            // put message to queue
            redis.GetDatabase().ListLeftPush("vowel-cons-rater-jobs", message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            redis.GetSubscriber().Publish( "vowel-cons-rater-jobs-hints", "RateVowelConsJob" );
        }

        private static string BuildJobMessage(string data)
        {
            return $"RateVowelConsJob : {data}";
        }
    }
}
