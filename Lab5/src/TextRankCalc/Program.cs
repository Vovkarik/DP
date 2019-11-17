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
            IDatabase db = redis.GetDatabase();
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                string messegeString = (string)message;
                string id = messegeString.Remove(0, 13);
                Console.WriteLine(id);
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
