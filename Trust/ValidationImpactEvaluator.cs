using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Trust
{
    internal static class ValidationImpactEvaluator
    {
        public static IReadOnlyList<ValidationImpactResult> Evaluate(
            IReadOnlyList<ModelUpdate> updates,
            IReadOnlyList<ValidationSample> validationSamples)
        {
            if (updates.Count == 0)
            {
                return Array.Empty<ValidationImpactResult>();
            }

            List<(ModelUpdate Update, double LogLoss)>
                evaluatedUpdates = new();

            foreach (ModelUpdate update in updates)
            {
                LogisticModel submittedModel =
                    new LogisticModel(
                        update.Weight,
                        update.Bias);

                ModelEvaluationResult evaluation =
                    ModelEvaluator.Evaluate(
                        submittedModel,
                        validationSamples);

                evaluatedUpdates.Add(
                    (update, evaluation.LogLoss));
            }

            double minimumLogLoss =
                evaluatedUpdates.Min(
                    item => item.LogLoss);

            double maximumLogLoss =
                evaluatedUpdates.Max(
                    item => item.LogLoss);

            List<ValidationImpactResult> results = new();

            foreach (var item in evaluatedUpdates)
            {
                double trustScore;

                if (maximumLogLoss == minimumLogLoss)
                {
                    trustScore = 1.0;
                }
                else
                {
                    trustScore =
                        1.0 -
                        ((item.LogLoss - minimumLogLoss) /
                         (maximumLogLoss - minimumLogLoss));
                }

                results.Add(
                    new ValidationImpactResult(
                        item.Update.DroneId,
                        item.LogLoss,
                        trustScore));
            }

            return results;
        }
    }
}