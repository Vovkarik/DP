using System;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace TextProcessingLimiter
{
    class Program
    {

        static void Main(string[] args)
        {
            int wordsCounter = 0;
            bool result = false;
            int textProcessingLimit = int.Parse(args[0]);  
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                string messageString = (string)message;
                string[] serializedMessage = messageString.Split(" : ");
                string messegeName = serializedMessage[0];
                if(messegeName == "Text Created")
                {
                    wordsCounter++;
                    result = wordsCounter <= textProcessingLimit;
                    string id = serializedMessage[1];
                    sub.Publish("events", "ProcessingAccepted : " + id + " : " + result);
                    Console.WriteLine(messageString);
                    Console.WriteLine(wordsCounter + ":" + textProcessingLimit + ":" + result);
                    if (!result)
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(60000);
                            wordsCounter = 0;
                        });
                    }
                }
                else if(messegeName == "TextStatistics")
                {
                    double rank = Convert.ToDouble(serializedMessage[1]);
                    if(rank <= 0.5)
                    {
                        wordsCounter--;
                        Console.WriteLine(messageString);
                        Console.WriteLine(wordsCounter + ":" + textProcessingLimit + ":" + result);
                    }
                }
            });
            Console.ReadLine();
        }
    }
}
