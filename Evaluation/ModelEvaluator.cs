using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Evaluation
{
    internal static class ModelEvaluator
    {
        public static ModelEvaluationResult Evaluate(
            LogisticModel model,
            IReadOnlyList<ValidationSample> validationSamples)
        {
            int truePositives = 0;
            int trueNegatives = 0;
            int falsePositives = 0;
            int falseNegatives = 0;

            double totalLogLoss = 0.0;

            foreach (ValidationSample sample in validationSamples)
            {
                double probability =
                    model.PredictProbability(
                        sample.Temperature);

                bool predictedFire =
                    probability >= 0.5;

                bool actualFire =
                    sample.Label == 1;

                if (predictedFire && actualFire)
                {
                    truePositives++;
                }
                else if (!predictedFire && !actualFire)
                {
                    trueNegatives++;
                }
                else if (predictedFire && !actualFire)
                {
                    falsePositives++;
                }
                else
                {
                    falseNegatives++;
                }

                double safeProbability =
                    Math.Clamp(
                        probability,
                        1e-15,
                        1.0 - 1e-15);

                totalLogLoss +=
                    -(sample.Label *
                      Math.Log(safeProbability) +
                      (1 - sample.Label) *
                      Math.Log(1.0 - safeProbability));
            }

            double averageLogLoss =
                validationSamples.Count == 0
                    ? 0.0
                    : totalLogLoss /
                      validationSamples.Count;

            return new ModelEvaluationResult(
                truePositives,
                trueNegatives,
                falsePositives,
                falseNegatives,
                averageLogLoss);
        }
    }
}