namespace UAVFLSecureSim.Trust
{
    internal class CommunicationReliabilityResult
    {
        public int DroneId { get; }
        public int ExpectedRounds { get; }
        public int SuccessfulRounds { get; }
        public double TrustScore { get; }

        public CommunicationReliabilityResult(
            int droneId,
            int expectedRounds,
            int successfulRounds,
            double trustScore)
        {
            DroneId = droneId;
            ExpectedRounds = expectedRounds;
            SuccessfulRounds = successfulRounds;
            TrustScore = trustScore;
        }
    }
}