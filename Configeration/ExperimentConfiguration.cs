using UAVFLSecureSim.Federation;

namespace UAVFLSecureSim.Configuration
{
    internal class ExperimentConfiguration
    {
        public int NumberOfDrones { get; set; }

        public int NumberOfCompromisedDrones { get; set; }

        public double AttackStrength { get; set; }

        public AggregationMethod AggregationMethod
        { get; set; }

        public double CommunicationReliability
        { get; set; }

        public int TotalRounds { get; set; }

        public int LocalEpochs { get; set; }

        public double LearningRate { get; set; }

        public int RandomSeed { get; set; }

        public ExperimentConfiguration()
        {
            // Canonical research defaults.
            NumberOfDrones = 5;
            NumberOfCompromisedDrones = 1;

            AttackStrength = 2.0;

            AggregationMethod =
                AggregationMethod.TrustAware;

            CommunicationReliability = 1.0;

            TotalRounds = 10;
            LocalEpochs = 100;
            LearningRate = 0.1;

            RandomSeed = 42;
        }
    }
}