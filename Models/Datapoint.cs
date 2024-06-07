namespace BricksAIDemo.Models
{
    public class Datapoint
    {
        public long timeStamp { get; set; }
        public int numberOfRequests { get; set; }
        public int successCount { get; set; }
        public double costInUsd { get; set; }
        public double latencyInMs { get; set; }
        public int promptTokenCount { get; set; }
        public int completionTokenCount { get; set; }
        public string model { get; set; }
        public string keyId { get; set; }
    }
}
