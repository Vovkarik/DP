using System;
using StackExchange.Redis;

namespace TextListener
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
                string value = db.StringGet(id);
                Console.WriteLine(id + " : " + value + " : " + databaseNumber);
            });
            Console.ReadLine();
        }
    }
}
