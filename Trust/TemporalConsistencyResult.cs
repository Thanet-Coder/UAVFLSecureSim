namespace UAVFLSecureSim.Trust
{
    internal class TemporalConsistencyResult
    {
        public int DroneId { get; }
        public double Similarity { get; }
        public double TrustScore { get; }

        public TemporalConsistencyResult(
            int droneId,
            double similarity,
            double trustScore)
        {
            DroneId = droneId;
            Similarity = similarity;
            TrustScore = trustScore;
        }
    }
}