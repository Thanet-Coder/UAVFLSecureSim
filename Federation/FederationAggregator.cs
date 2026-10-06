using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Federation
{
    internal static class FedAvgAggregator
    {
        public static LogisticModel Aggregate(
            IReadOnlyList<ModelUpdate> updates)
        {
            if (updates.Count == 0)
            {
                throw new ArgumentException(
                    "At least one model update is required.");
            }

            int totalSamples =
                updates.Sum(update => update.SampleCount);

            if (totalSamples == 0)
            {
                throw new ArgumentException(
                    "Model updates must contain training samples.");
            }

            double weightedWeight = 0.0;
            double weightedBias = 0.0;

            foreach (ModelUpdate update in updates)
            {
                double contribution =
                    (double)update.SampleCount / totalSamples;

                weightedWeight +=
                    contribution * update.Weight;

                weightedBias +=
                    contribution * update.Bias;
            }

            return new LogisticModel(
                weightedWeight,
                weightedBias);
        }



    }// end of class
}// end of namespace