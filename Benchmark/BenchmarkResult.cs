namespace UAVFLSecureSim.Benchmark
{
    internal class BenchmarkResult
    {
        public string ScenarioName { get; }

        public double Accuracy { get; }
        public double Precision { get; }
        public double Recall { get; }
        public double F1Score { get; }
        public double LogLoss { get; }
        public int RandomSeed { get; }

        public int SuccessfulCommunications { get; }
        public int ExpectedCommunications { get; }

        public double ExecutionTimeMilliseconds { get; }

        public BenchmarkResult(
            string scenarioName,
            int randomSeed,
            double accuracy,
            double precision,
            double recall,
            double f1Score,
            double logLoss,
            int successfulCommunications,
            int expectedCommunications,
            double executionTimeMilliseconds)
        {
            ScenarioName = scenarioName;
            RandomSeed = randomSeed;

            Accuracy = accuracy;
            Precision = precision;
            Recall = recall;
            F1Score = f1Score;
            LogLoss = logLoss;

            SuccessfulCommunications =
                successfulCommunications;

            ExpectedCommunications =
                expectedCommunications;

            ExecutionTimeMilliseconds =
                executionTimeMilliseconds;
        }// end of constructor
    }// end of class
}// end of namespace