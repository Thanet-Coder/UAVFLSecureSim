using UAVFLSecureSim.Federation;

namespace UAVFLSecureSim.Benchmark
{
    internal class BenchmarkScenario
    {
        public string Name { get; }
        public int NumberOfDrones { get; }
        public int NumberOfCompromisedDrones { get; }
        public double AttackStrength { get; }
        public AggregationMethod AggregationMethod { get; }
        public double CommunicationReliability { get; }

        public BenchmarkScenario(
            string name,
            int numberOfDrones,
            int numberOfCompromisedDrones,
            double attackStrength,
            AggregationMethod aggregationMethod,
            double communicationReliability)
        {
            Name = name;
            NumberOfDrones = numberOfDrones;
            NumberOfCompromisedDrones =
                numberOfCompromisedDrones;
            AttackStrength = attackStrength;
            AggregationMethod = aggregationMethod;
            CommunicationReliability =
                communicationReliability;
        }
    }
}