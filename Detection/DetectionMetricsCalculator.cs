
namespace UAVFLSecureSim.Detection
{
    internal static class DetectionMetricsCalculator
    {
        public static DetectionMetrics Calculate(
            IReadOnlyList<DroneDetectionResult> results)
        {
            int truePositives = 0;
            int falsePositives = 0;
            int trueNegatives = 0;
            int falseNegatives = 0;

            foreach (DroneDetectionResult result in results)
            {
                if (result.IsActuallyCompromised &&
                    result.IsPredictedSuspicious)
                {
                    truePositives++;
                }
                else if (!result.IsActuallyCompromised &&
                         result.IsPredictedSuspicious)
                {
                    falsePositives++;
                }
                else if (!result.IsActuallyCompromised &&
                         !result.IsPredictedSuspicious)
                {
                    trueNegatives++;
                }
                else
                {
                    falseNegatives++;
                }
            }

            return new DetectionMetrics(
                truePositives,
                falsePositives,
                trueNegatives,
                falseNegatives);
        }
    }
}
