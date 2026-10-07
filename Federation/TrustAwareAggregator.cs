using UAVFLSecureSim.Learning;
using UAVFLSecureSim.Trust;

namespace UAVFLSecureSim.Federation
{
    internal static class TrustAwareAggregator
    {
        public static LogisticModel Aggregate(
            IReadOnlyList<ModelUpdate> updates,
            IReadOnlyList<CombinedTrustResult> trustResults)
        {
            if (updates.Count == 0)
            {
                throw new ArgumentException(
                    "At least one model update is required.");
            }

            double weightedWeightSum = 0.0;
            double weightedBiasSum = 0.0;
            double totalAggregationWeight = 0.0;

            foreach (ModelUpdate update in updates)
            {
                CombinedTrustResult? trust =
                    trustResults.FirstOrDefault(
                        result =>
                            result.DroneId ==
                            update.DroneId);

                if (trust == null)
                {
                    continue;
                }

                // Combine the amount of local training data
                // with the UAV's calculated trust score.
                double aggregationWeight =
                    update.SampleCount *
                    trust.OverallTrust;

                weightedWeightSum +=
                    update.Weight *
                    aggregationWeight;

                weightedBiasSum +=
                    update.Bias *
                    aggregationWeight;

                totalAggregationWeight +=
                    aggregationWeight;
            }

            if (totalAggregationWeight <= 0.0)
            {
                throw new InvalidOperationException(
                    "Trust-aware aggregation requires " +
                    "at least one positively weighted update.");
            }

            double aggregatedWeight =
                weightedWeightSum /
                totalAggregationWeight;

            double aggregatedBias =
                weightedBiasSum /
                totalAggregationWeight;

            return new LogisticModel(
                aggregatedWeight,
                aggregatedBias);
        }
    }
}