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

        private static int numberRejectedElements = 0;
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            ISubscriber sub = redis.GetSubscriber();
            IDatabase map = redis.GetDatabase(0);
            sub.Subscribe("events", (channel, message) => {
                string messageString = (string)message;
                string[] serializedMessage = messageString.Split(" : ");
                string messegeName = serializedMessage[0];
                if(messegeName == "TextStatistics")
                {
                    string rank = serializedMessage[1];
                    double convertedRank = Convert.ToDouble(rank);
                    sumRanks+=convertedRank;
                    textNum++;
                    if (convertedRank > 0.5)
                    {
                        highRankPart++;
                    }
                    avgRank = sumRanks / textNum;
                    UpdateStatistics(redis);
                }
                else if(messegeName == "ProcessingAccepted" && serializedMessage[2] == "False")
                {
                    bool status = Convert.ToBoolean(serializedMessage[2]);
                    if(!status)
                    {
                        numberRejectedElements++;
                    }
                    UpdateStatistics(redis);
                    string id = serializedMessage[1];
                    string databaseNumberString = map.StringGet(id);
                    int databaseNumber = Convert.ToInt32(databaseNumberString);
                    IDatabase db = redis.GetDatabase(databaseNumber);
                    db.StringSet(id, "Processing limit achived");
                }
                
            });
            Console.ReadLine();
        }

        static void UpdateStatistics(ConnectionMultiplexer redis)
        {
            string resultString = "TextNum : " + textNum + " HighRankPart : " + highRankPart + " AverageRank : " + avgRank + " RejectedElements : " + numberRejectedElements;
            Console.WriteLine(resultString);
            IDatabase statisticsDB = redis.GetDatabase(1);
            statisticsDB.StringSet("TextStatistics", resultString);
        }
    }
}
