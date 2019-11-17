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
            ISubscriber sub = redis.GetSubscriber();
            IDatabase map = redis.GetDatabase(0);
            sub.Subscribe("vowel-cons-counter-jobs-hints", (channel, message) => {
                string countMessage = redis.GetDatabase().ListRightPop("vowel-cons-counter-jobs");
                string[] serializedMessage = countMessage.Split(" : ");
                string id = serializedMessage[1];
                string databaseNumberString = map.StringGet(id);
                int databaseNumber = Convert.ToInt32(databaseNumberString);
                IDatabase db = redis.GetDatabase(databaseNumber);
                string value = db.StringGet(id);
                Console.WriteLine(countMessage + databaseNumber);
                var msg = BuildJobMessage(GetVowelsAndCons(value) + " : " + databaseNumber + " : " + id);
                SendMessage(msg, redis);
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
