namespace UAVFLSecureSim.Trust
{
    internal class UpdateBehaviourResult
    {
        public int DroneId { get; }
        public double Distance { get; }
        public double TrustScore { get; }

        public UpdateBehaviourResult(
            int droneId,
            double distance,
            double trustScore)
        {
            DroneId = droneId;
            Distance = distance;
            TrustScore = trustScore;
        }
    }
}