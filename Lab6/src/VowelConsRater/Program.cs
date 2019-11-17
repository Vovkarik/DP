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
            IDatabase map = redis.GetDatabase(0);
            sub.Subscribe("vowel-cons-rater-jobs-hints", (channel, message) => {
                string msg = redis.GetDatabase().ListRightPop("vowel-cons-rater-jobs");
                string[] serializedMessage = msg.Split(" : ");
                string vowels = serializedMessage[1];
                string consonants = serializedMessage[2];
                string id = serializedMessage[4];
                string databaseNumberString = map.StringGet(id);
                int databaseNumber = Convert.ToInt32(databaseNumberString);
                IDatabase db = redis.GetDatabase(databaseNumber);
                float rank = CalculateRank(Convert.ToInt32(vowels), Convert.ToInt32(consonants));
                db.StringSet(id, rank);
                Console.WriteLine(id + " : " + rank + " : " + databaseNumber);
            });
            Console.ReadLine();
        }

        static public float CalculateRank(int vowels, int consonants)
        {
            return (float)vowels/consonants;
        }
    }
}
