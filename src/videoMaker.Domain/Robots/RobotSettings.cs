namespace videoMaker.Domain.Robots
{
    public class RobotSettings
    {
        public RobotSettings(string apiKey, string WikipidiaAlgorithm)
        {
            ApiKey = apiKey;
            WikipidiaAlgorithm = WikipidiaAlgorithm;
        }

        public string ApiKey { get; private set; }
        public string WikipidiaAlgorithm { get; private set; }


    }
}