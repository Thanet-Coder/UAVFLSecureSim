
namespace UAVFLSecureSim.Detection
{
    internal class DetectionMetrics
    {
        public int TruePositives { get; }
        public int FalsePositives { get; }
        public int TrueNegatives { get; }
        public int FalseNegatives { get; }

        public int TotalEvaluated =>
            TruePositives +
            FalsePositives +
            TrueNegatives +
            FalseNegatives;

        public double Precision =>
            TruePositives + FalsePositives == 0
                ? 0.0
                : (double)TruePositives /
                  (TruePositives + FalsePositives);

        public double Recall =>
            TruePositives + FalseNegatives == 0
                ? 0.0
                : (double)TruePositives /
                  (TruePositives + FalseNegatives);

        public double F1Score =>
            Precision + Recall == 0.0
                ? 0.0
                : 2.0 * Precision * Recall /
                  (Precision + Recall);

        public double FalsePositiveRate =>
            FalsePositives + TrueNegatives == 0
                ? 0.0
                : (double)FalsePositives /
                  (FalsePositives + TrueNegatives);

        public DetectionMetrics(
            int truePositives,
            int falsePositives,
            int trueNegatives,
            int falseNegatives)
        {
            TruePositives = truePositives;
            FalsePositives = falsePositives;
            TrueNegatives = trueNegatives;
            FalseNegatives = falseNegatives;
        }
    }
}
