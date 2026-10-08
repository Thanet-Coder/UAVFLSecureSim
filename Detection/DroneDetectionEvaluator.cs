
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Trust;

namespace UAVFLSecureSim.Detection
{
    internal static class DroneDetectionEvaluator
    {
        public static IReadOnlyList<DroneDetectionResult> Evaluate(
            IReadOnlyList<CombinedTrustResult> trustResults,
            IReadOnlyList<Drone> drones,
            double detectionThreshold = 0.50)
        {
            if (detectionThreshold < 0.0 ||
                detectionThreshold > 1.0 ||
                double.IsNaN(detectionThreshold))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(detectionThreshold),
                    "Detection threshold must be between 0 and 1.");
            }

            List<DroneDetectionResult> results = new();

            foreach (CombinedTrustResult trust in trustResults)
            {
                Drone? drone = drones.FirstOrDefault(
                    drone => drone.Id == trust.DroneId);

                if (drone == null)
                {
                    throw new InvalidOperationException(
                        $"No UAV found for trust result {trust.DroneId}.");
                }

                bool isPredictedSuspicious =
                    trust.OverallTrust < detectionThreshold;

                results.Add(
                    new DroneDetectionResult(
                        trust.DroneId,
                        trust.OverallTrust,
                        drone.IsCompromised,
                        isPredictedSuspicious));
            }

            return results;
        }
    }
}
