using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("values", (channel, message) => {
                string messegeString = (string)message;
                string id = messegeString.Remove(0, 13);
                string value = db.StringGet(id);
                Console.WriteLine(id + " : " + value);
            });
            Console.ReadLine();
        }
    }
}
