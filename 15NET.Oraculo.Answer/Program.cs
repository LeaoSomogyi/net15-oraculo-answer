using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;

namespace _15NET.Oraculo.Answer
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();

            var ip = configuration.GetSection("ConnectionStrings")["Ip"];

            var conn = ConnectionMultiplexer.Connect(ip);

            var database = conn.GetDatabase();

            var subscriber = conn.GetSubscriber();

            subscriber.Subscribe("Perguntas", (ch, msg) =>
            {
                var question = msg.ToString().Split(":")[0];

                HashEntry[] answer =
                {
                    new HashEntry("Leao-Kleber", "Resposta que faça sentido com a pergunta")
                };

                database.HashSet(question, answer);

                var myAnswer = database.HashKeys(question);

                myAnswer.ToList().ForEach(a =>
                {
                    Console.WriteLine(a.ToString());
                });
            });

            Console.ReadKey();
        }
    }
}
