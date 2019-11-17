using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace TextStatistics
{
    class Program
    {
        private static int textNum = 0;

        private static int highRankPart = 0;

        private static double avgRank = 0;

        private static double sumRanks = 0;
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            ISubscriber sub = redis.GetSubscriber();
            IDatabase map = redis.GetDatabase(0);
            sub.Subscribe("events", (channel, message) => {
                string messageString = (string)message;
                string[] serializedMessage = messageString.Split(" : ");
                string rank = serializedMessage[1];
                double convertedRank = Convert.ToDouble(rank);
                sumRanks+=convertedRank;
                textNum++;
                if (convertedRank > 0.5)
                {
                    highRankPart++;
                }
                avgRank = sumRanks / textNum;
                string resultString = "TextNum : " + textNum + " HighRankPart : " + highRankPart + " AverageRank : " + avgRank;
                Console.WriteLine(resultString);
                IDatabase db = redis.GetDatabase(1);
                db.StringSet("TextStatistics", resultString);
            });
            Console.ReadLine();
        }
    }
}
