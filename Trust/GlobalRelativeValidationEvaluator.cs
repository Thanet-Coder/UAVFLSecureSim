using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Trust
{
    internal static class GlobalRelativeValidationEvaluator
    {
        public static IReadOnlyList<GlobalRelativeValidationResult>
            Evaluate(
                IReadOnlyList<ModelUpdate> updates,
                LogisticModel globalModel,
                IReadOnlyList<ValidationSample> validationSamples)
        {
            if (updates.Count == 0)
            {
                return Array.Empty<GlobalRelativeValidationResult>();
            }

            if (validationSamples.Count == 0)
            {
                throw new ArgumentException(
                    "Validation samples must not be empty.",
                    nameof(validationSamples));
            }

            // Evaluate the current global model once.
            ModelEvaluationResult globalEvaluation =
                ModelEvaluator.Evaluate(
                    globalModel,
                    validationSamples);

            double globalLogLoss =
                globalEvaluation.LogLoss;

            List<GlobalRelativeValidationResult> results =
                new();

            // Evaluate each UAV's submitted model.
            foreach (ModelUpdate update in updates)
            {
                LogisticModel submittedModel =
                    new LogisticModel(
                        update.Weight,
                        update.Bias);

                ModelEvaluationResult submittedEvaluation =
                    ModelEvaluator.Evaluate(
                        submittedModel,
                        validationSamples);

                results.Add(
                    new GlobalRelativeValidationResult(
                        update.DroneId,
                        globalLogLoss,
                        submittedEvaluation.LogLoss));
            }

            return results;
        }
    }
}