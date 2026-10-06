namespace UAVFLSecureSim.Evaluation
{
    internal class ModelEvaluationResult
    {
        public int TruePositives { get; }
        public int TrueNegatives { get; }
        public int FalsePositives { get; }
        public int FalseNegatives { get; }

        public double LogLoss { get; }

        public int Total =>
            TruePositives +
            TrueNegatives +
            FalsePositives +
            FalseNegatives;

        public double Accuracy =>
            Total == 0
                ? 0.0
                : (double)(TruePositives + TrueNegatives) / Total;

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

        public ModelEvaluationResult(
            int truePositives,
            int trueNegatives,
            int falsePositives,
            int falseNegatives,
            double logLoss)
        {
            TruePositives = truePositives;
            TrueNegatives = trueNegatives;
            FalsePositives = falsePositives;
            FalseNegatives = falseNegatives;
            LogLoss = logLoss;
        }
    }
}