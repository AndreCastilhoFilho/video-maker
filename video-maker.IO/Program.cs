using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using videoMaker.Domain.Models;
using videoMaker.Domain.Robots;

namespace video_maker.IO
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var apiKey = Configuration["AlgorithmiaConfig:apikey"];
            var wikipidiaAlgorithm = Configuration["AlgorithmiaConfig:wikipidia_algorithm"];
            var watsonApiKey = Configuration["WatsonConfig:apikey"];
            var watsonUrl = Configuration["WatsonConfig:url"];

            var googleApi = Configuration["Google:apiKey"];
            var searchEngine = Configuration["Google:search_engine_key"];

            var settings = new RobotSettings(apiKey, wikipidiaAlgorithm, watsonApiKey, watsonUrl);

            Input.Robot();
            new Text(settings).Robot();
            new Image(googleApi, searchEngine).Robot();

            var content = State.Load();

            Console.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            Console.ReadKey(true);
        }


    }
}
