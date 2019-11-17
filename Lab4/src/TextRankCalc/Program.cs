using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace TextRankCalc
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
            sub.Subscribe("values", (channel, message) => {
                string messegeString = (string)message;
                string id = messegeString.Remove(0, 13);
                string value = db.StringGet(id);
                float rank = GetRank(value);
                db.StringSet(id, rank);
                Console.WriteLine("Rank is " + rank);
             });
            Console.ReadLine();
        }
        static float GetRank(string value)
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
            Console.WriteLine(vowels + ":" + consonants);
            return (float)vowels/consonants;
        }
    }
}
