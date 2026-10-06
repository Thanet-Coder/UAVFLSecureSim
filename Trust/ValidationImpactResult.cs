namespace UAVFLSecureSim.Trust
{
    internal class ValidationImpactResult
    {
        public int DroneId { get; }
        public double LogLoss { get; }
        public double TrustScore { get; }

        public ValidationImpactResult(
            int droneId,
            double logLoss,
            double trustScore)
        {
            DroneId = droneId;
            LogLoss = logLoss;
            TrustScore = trustScore;
        }
    }
}