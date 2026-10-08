
namespace UAVFLSecureSim.Detection
{
    internal class DroneDetectionResult
    {
        public int DroneId { get; }

        public double TrustScore { get; }

        public bool IsActuallyCompromised { get; }

        public bool IsPredictedSuspicious { get; }

        public DroneDetectionResult(
            int droneId,
            double trustScore,
            bool isActuallyCompromised,
            bool isPredictedSuspicious)
        {
            DroneId = droneId;
            TrustScore = trustScore;
            IsActuallyCompromised = isActuallyCompromised;
            IsPredictedSuspicious = isPredictedSuspicious;
        }
    }
}
