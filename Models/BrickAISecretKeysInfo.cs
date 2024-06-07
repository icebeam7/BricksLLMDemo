namespace BricksAIDemo.Models
{
    public class BricksAISecretKeysInfo
    {
        public string name { get; set; }
        public string keyId { get; set; }
        public string[] tags { get; set; }
        public double costLimitInUsd { get; set; }
        public double costLimitInUsdOverTime { get; set; }
        public string costLimitInUsdUnit { get; set; }
        public double rateLimitOverTime { get; set; }
        public string rateLimitUnit { get; set; }
        public string ttl { get; set; }
        public string[] settingIds { get; set; }
        public Datapoint[] dataPoints { get; set; }
        public double latencyInMsMedian { get; set; }
        public double latencyInMs99th { get; set; }
    }
}
