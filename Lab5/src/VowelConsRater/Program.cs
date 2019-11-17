using System;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            ISubscriber sub = redis.GetSubscriber();
            IDatabase db = redis.GetDatabase();
            sub.Subscribe("vowel-cons-rater-jobs-hints", (channel, message) => {
                string msg = redis.GetDatabase().ListRightPop("vowel-cons-rater-jobs");
                string[] serializedMessege = msg.Split(" : ");
                string vowels = serializedMessege[1];
                string consonants = serializedMessege[2];
                string id = serializedMessege[3];
                float rank = CalculateRank(Convert.ToInt32(vowels), Convert.ToInt32(consonants));
                db.StringSet(id, rank);
                Console.WriteLine(id + " : " + rank);
            });
            Console.ReadLine();
        }

        static public float CalculateRank(int vowels, int consonants)
        {
            return (float)vowels/consonants;
        }
    }
}
