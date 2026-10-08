
namespace UAVFLSecureSim.Detection
{
    internal static class DetectionThresholdAnalysis
    {
        public static DetectionMetrics Evaluate(
            IReadOnlyList<DroneDetectionResult> results,
            double threshold)
        {
            if (double.IsNaN(threshold) ||
                threshold < 0.0 ||
                threshold > 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(threshold),
                    "Threshold must be between 0 and 1.");
            }

            List<DroneDetectionResult> classifiedResults =
                new();

            foreach (DroneDetectionResult result in results)
            {
                bool isSuspicious =
                    result.TrustScore < threshold;

                classifiedResults.Add(
                    new DroneDetectionResult(
                        result.DroneId,
                        result.TrustScore,
                        result.IsActuallyCompromised,
                        isSuspicious));
            }

            return DetectionMetricsCalculator.Calculate(
                classifiedResults);
        }
    }
}
