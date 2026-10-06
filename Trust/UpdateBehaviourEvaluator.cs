using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Trust
{
    internal static class UpdateBehaviourEvaluator
    {
        public static IReadOnlyList<UpdateBehaviourResult> Evaluate(
            IReadOnlyList<ModelUpdate> updates,
            LogisticModel globalModel)
        {
            if (updates.Count == 0)
            {
                return Array.Empty<UpdateBehaviourResult>();
            }

            List<(ModelUpdate Update,
                  double WeightDelta,
                  double BiasDelta)> deltas = new();

            foreach (ModelUpdate update in updates)
            {
                double weightDelta =
                    update.Weight - globalModel.Weight;

                double biasDelta =
                    update.Bias - globalModel.Bias;

                deltas.Add(
                    (update, weightDelta, biasDelta));
            }

            double centreWeight =
                deltas.Average(item => item.WeightDelta);

            double centreBias =
                deltas.Average(item => item.BiasDelta);

            List<(ModelUpdate Update,
                  double Distance)> distances = new();

            foreach (var item in deltas)
            {
                double weightDifference =
                    item.WeightDelta - centreWeight;

                double biasDifference =
                    item.BiasDelta - centreBias;

                double distance =
                    Math.Sqrt(
                        (weightDifference * weightDifference) +
                        (biasDifference * biasDifference));

                distances.Add(
                    (item.Update, distance));
            }

            double maximumDistance =
                distances.Max(item => item.Distance);

            List<UpdateBehaviourResult> results = new();

            foreach (var item in distances)
            {
                double trustScore;

                if (maximumDistance == 0.0)
                {
                    trustScore = 1.0;
                }
                else
                {
                    trustScore =
                        1.0 -
                        (item.Distance / maximumDistance);
                }

                results.Add(
                    new UpdateBehaviourResult(
                        item.Update.DroneId,
                        item.Distance,
                        trustScore));
            }

            return results;
        }
    }
}