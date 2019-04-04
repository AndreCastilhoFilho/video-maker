using Newtonsoft.Json;
using System;
using System.IO;
using videoMaker.Domain.Models;

namespace videoMaker.Domain.Robots
{
    public static class State
    {
        public static void Save(Content content)
        {
            var jsonString = JsonConvert.SerializeObject(content);

            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\content.json", jsonString);
        }

        public static Content Load()
        {
            var contentText = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\content.json");

            var content = JsonConvert.DeserializeObject<Content>(contentText);

            return content;
        }


    }
}
