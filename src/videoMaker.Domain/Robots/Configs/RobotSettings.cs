namespace videoMaker.Domain.Robots
{
    public class RobotSettings
    {
        public RobotSettings(string apiKey, string wikipidiaAlgorithm, string nluApiKey, string nluUrl)
        {
            ApiKey = apiKey;
            WikipidiaAlgorithm = wikipidiaAlgorithm;
            NluApiKey = nluApiKey;
            NluUrl = nluUrl;
            NluVersionDate = "2018-11-16";
        }

        public string ApiKey { get; private set; }
        public string WikipidiaAlgorithm { get; private set; }
        public string NluApiKey { get; internal set; }
        public string NluVersionDate { get; set; }
        public string NluUrl { get; internal set; }


    }
}