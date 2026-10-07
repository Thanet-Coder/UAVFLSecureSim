using UAVFLSecureSim.Federation;

namespace UAVFLSecureSim.Benchmark
{
    internal static class BenchmarkScenarioFactory
    {
        public static List<BenchmarkScenario> Create()
        {
            return new List<BenchmarkScenario>
            {
                new BenchmarkScenario(
                    "A - Honest FedAvg",
                    5,
                    0,
                    2.0,
                    AggregationMethod.FedAvg,
                    1.0),

                new BenchmarkScenario(
                    "B - Honest Trust-Aware",
                    5,
                    0,
                    2.0,
                    AggregationMethod.TrustAware,
                    1.0),

                new BenchmarkScenario(
                    "C - 20% Malicious FedAvg",
                    5,
                    1,
                    2.0,
                    AggregationMethod.FedAvg,
                    1.0),

                new BenchmarkScenario(
                    "D - 20% Malicious Trust-Aware",
                    5,
                    1,
                    2.0,
                    AggregationMethod.TrustAware,
                    1.0),

                new BenchmarkScenario(
                    "E - 40% Malicious FedAvg",
                    5,
                    2,
                    2.0,
                    AggregationMethod.FedAvg,
                    1.0),

                new BenchmarkScenario(
                    "F - 40% Malicious Trust-Aware",
                    5,
                    2,
                    2.0,
                    AggregationMethod.TrustAware,
                    1.0),

                new BenchmarkScenario(
                    "G - 20% Malicious Trust-Aware + Intermittent Communication",
                    5,
                    1,
                    2.0,
                    AggregationMethod.TrustAware,
                    0.8)
            };
        }
    }
}