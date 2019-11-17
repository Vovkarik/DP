using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            ISubscriber sub = redis.GetSubscriber();
            IDatabase map = redis.GetDatabase(0);
            sub.Subscribe("events", (channel, message) => {
                string messageString = (string)message;
                string[] serializedMessage = messageString.Split(" : ");
                string id = serializedMessage[1];
                string databaseNumberString = map.StringGet(id);
                int databaseNumber = Convert.ToInt32(databaseNumberString);
                IDatabase db = redis.GetDatabase(databaseNumber);
                Console.WriteLine(id + " : " + databaseNumber);
                var msg = BuildJobMessage(id);
                SendMessage(msg, redis);
            });
            Console.ReadLine();
        }

        private static void SendMessage(string message, IConnectionMultiplexer redis )
        {
            // put message to queue
            redis.GetDatabase().ListLeftPush("vowel-cons-counter-jobs", message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            redis.GetSubscriber().Publish( "vowel-cons-counter-jobs-hints", "CalculateVowelConsJob" );
        }

        private static string BuildJobMessage(string data)
        {
            return $"CalculateVowelConsJob : {data}";
        }
    }
}
