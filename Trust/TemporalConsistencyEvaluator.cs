using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Trust
{
    internal class TemporalConsistencyEvaluator
    {
        private readonly Dictionary<
            int,
            (double WeightDelta, double BiasDelta)>
            previousUpdates = new();

        public IReadOnlyList<TemporalConsistencyResult> Evaluate(
            IReadOnlyList<ModelUpdate> updates,
            LogisticModel globalModel)
        {
            if (updates.Count == 0)
            {
                return Array.Empty<TemporalConsistencyResult>();
            }

            List<TemporalConsistencyResult> results = new();

            foreach (ModelUpdate update in updates)
            {
                double weightDelta =
                    update.Weight - globalModel.Weight;

                double biasDelta =
                    update.Bias - globalModel.Bias;

                double similarity;
                double trustScore;

                if (previousUpdates.TryGetValue(
                    update.DroneId,
                    out var previous))
                {
                    similarity =
                        CalculateCosineSimilarity(
                            previous.WeightDelta,
                            previous.BiasDelta,
                            weightDelta,
                            biasDelta);

                    // Convert cosine similarity from
                    // the range -1..+1 into trust 0..1.
                    trustScore =
                        (similarity + 1.0) / 2.0;
                }
                else
                {
                    // No previous round exists yet.
                    // Use neutral evidence.
                    similarity = 0.0;
                    trustScore = 0.5;
                }

                results.Add(
                    new TemporalConsistencyResult(
                        update.DroneId,
                        similarity,
                        trustScore));

                previousUpdates[update.DroneId] =
                    (weightDelta, biasDelta);
            }

            return results;
        }

        private static double CalculateCosineSimilarity(
            double previousWeightDelta,
            double previousBiasDelta,
            double currentWeightDelta,
            double currentBiasDelta)
        {
            double dotProduct =
                (previousWeightDelta * currentWeightDelta) +
                (previousBiasDelta * currentBiasDelta);

            double previousMagnitude =
                Math.Sqrt(
                    (previousWeightDelta * previousWeightDelta) +
                    (previousBiasDelta * previousBiasDelta));

            double currentMagnitude =
                Math.Sqrt(
                    (currentWeightDelta * currentWeightDelta) +
                    (currentBiasDelta * currentBiasDelta));

            if (previousMagnitude == 0.0 ||
                currentMagnitude == 0.0)
            {
                return 0.0;
            }

            double similarity =
                dotProduct /
                (previousMagnitude * currentMagnitude);

            // Protect against tiny floating-point errors
            // producing values slightly outside -1..+1.
            return Math.Clamp(
                similarity,
                -1.0,
                1.0);
        }
    }
}