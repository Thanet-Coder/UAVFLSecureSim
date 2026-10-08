using UAVFLSecureSim.Display;
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.World;
using UAVFLSecureSim.Federation;
using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Trust;
using UAVFLSecureSim.Configuration;
using UAVFLSecureSim.Benchmark;

namespace UAVFLSecureSim
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("UAV-FL SecureSim");
            Console.WriteLine("================");
            Console.WriteLine();
            Console.WriteLine("1. Interactive Simulation");
            Console.WriteLine("2. Benchmark Suite");
            Console.WriteLine();

            int mode =
                ReadApplicationMode();

            Console.WriteLine();

            if (mode == 2)
            {
                RunBenchmarkSuite();
                return;
            }

            // -------------------------------------------------
            // Experiment Configuration
            // -------------------------------------------------

            ExperimentConfiguration configuration =
                new ExperimentConfiguration();

            FireMap map = FixedFireMapGenerator.Create();

            configuration.NumberOfDrones =
                ReadNumberOfDrones(map.Width);

            DroneSwarm swarm =
                new DroneSwarm(
                    configuration.NumberOfDrones,
                    map);

            configuration.NumberOfCompromisedDrones =
                ReadNumberOfCompromisedDrones(
                    configuration.NumberOfDrones);

            swarm.CompromiseDrones(
                configuration.NumberOfCompromisedDrones);

            configuration.AggregationMethod =
                ReadAggregationMethod();

            configuration.AttackStrength =
                ReadAttackStrength();

            configuration.CommunicationReliability =
                ReadCommunicationReliability();

            foreach (Drone drone in swarm.Drones)
            {
                drone.SetCommunicationReliability(
                    configuration.CommunicationReliability);
            }

            // -------------------------------------------------
            // Display Experiment Configuration
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("Experiment Configuration");
            Console.WriteLine("------------------------");

            Console.WriteLine(
                $"UAVs:              " +
                $"{configuration.NumberOfDrones}");

            Console.WriteLine(
                $"Compromised UAVs:  " +
                $"{configuration.NumberOfCompromisedDrones}");

            Console.WriteLine(
                $"Aggregation:       " +
                $"{configuration.AggregationMethod}");

            Console.WriteLine(
                $"Attack strength:   " +
                $"{configuration.AttackStrength:F1}");

            Console.WriteLine(
                $"Communication:     " +
                $"{configuration.CommunicationReliability:P0}");

            Console.WriteLine(
                $"FL rounds:         " +
                $"{configuration.TotalRounds}");

            Console.WriteLine(
                $"Local epochs:      " +
                $"{configuration.LocalEpochs}");

            Console.WriteLine(
                $"Learning rate:     " +
                $"{configuration.LearningRate:F1}");

            Console.WriteLine(
                $"Random seed:       " +
                $"{configuration.RandomSeed}");

            Console.WriteLine();

            // -------------------------------------------------
            // UAV Survey
            // -------------------------------------------------

            swarm.Survey(map);

            MapRenderer.Render(map);

            Console.WriteLine("UAV Survey Regions");
            Console.WriteLine("------------------");

            foreach (Drone drone in swarm.Drones)
            {
                Console.WriteLine(
                    $"{drone.Name} | " +
                    $"Region: {drone.StartX}-{drone.EndX} | " +
                    $"Samples: {drone.ObservationCount,3} | " +
                    $"Mean: {drone.AverageTemperature,6:F1} C | " +
                    $"Min: {drone.MinimumTemperature,5:F1} C | " +
                    $"Max: {drone.MaximumTemperature,5:F1} C");
            }

            // -------------------------------------------------
            // Simulation Security Ground Truth
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Simulation Security Ground Truth");
            Console.WriteLine(
                "--------------------------------");

            foreach (Drone drone in swarm.Drones)
            {
                string status =
                    drone.IsCompromised
                        ? "COMPROMISED"
                        : "HONEST";

                Console.WriteLine(
                    $"{drone.Name}: {status}");
            }

            // -------------------------------------------------
            // Federated Learning
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("Federated Learning");
            Console.WriteLine("------------------");

            FederatedCoordinator coordinator =
                new FederatedCoordinator(
                    configuration.AggregationMethod,
                    configuration.RandomSeed);

            Console.WriteLine(
                $"Aggregation: " +
                $"{coordinator.AggregationMethod}");

            Console.WriteLine(
                $"Attack strength: " +
                $"{configuration.AttackStrength:F1}");

            Console.WriteLine();

            Console.WriteLine(
                $"Round {coordinator.CurrentRound,2}: " +
                $"{coordinator.GlobalModel}");

            for (int round = 0;
                 round < configuration.TotalRounds;
                 round++)
            {
                coordinator.RunRound(
                    swarm,
                    configuration.LocalEpochs,
                    configuration.LearningRate,
                    configuration.AttackStrength);

                Console.WriteLine(
                    $"Round {coordinator.CurrentRound,2}: " +
                    $"{coordinator.GlobalModel}");
            }

            // -------------------------------------------------
            // Update Behaviour Evidence
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Update Behaviour Evidence");
            Console.WriteLine(
                "-------------------------");

            foreach (UpdateBehaviourResult result
                in coordinator.LastBehaviourResults)
            {
                Console.WriteLine(
                    $"UAV-{result.DroneId:00} | " +
                    $"Distance: {result.Distance:F4} | " +
                    $"Behaviour Trust: {result.TrustScore:F3}");
            }

            // -------------------------------------------------
            // Validation Impact Evidence
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Validation Impact Evidence");
            Console.WriteLine(
                "--------------------------");

            foreach (ValidationImpactResult result
                in coordinator.LastValidationResults)
            {
                Console.WriteLine(
                    $"UAV-{result.DroneId:00} | " +
                    $"Log Loss: {result.LogLoss:F4} | " +
                    $"Validation Trust: {result.TrustScore:F3}");
            }

            // -------------------------------------------------
            // Temporal Consistency Evidence
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Temporal Consistency Evidence");
            Console.WriteLine(
                "-----------------------------");

            foreach (TemporalConsistencyResult result
                in coordinator.LastTemporalResults)
            {
                Console.WriteLine(
                    $"UAV-{result.DroneId:00} | " +
                    $"Similarity: {result.Similarity:F4} | " +
                    $"Temporal Trust: {result.TrustScore:F3}");
            }

            // -------------------------------------------------
            // Communication Reliability Evidence
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Communication Reliability Evidence");
            Console.WriteLine(
                "----------------------------------");

            foreach (CommunicationReliabilityResult result
                in coordinator.LastCommunicationResults)
            {
                Console.WriteLine(
                    $"UAV-{result.DroneId:00} | " +
                    $"Successful: " +
                    $"{result.SuccessfulRounds}/" +
                    $"{result.ExpectedRounds} | " +
                    $"Reliability Trust: " +
                    $"{result.TrustScore:F3}");
            }

            // -------------------------------------------------
            // Combined Multi-Evidence Trust Scores
            // -------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "Combined Multi-Evidence Trust");
            Console.WriteLine(
                "-----------------------------");

            foreach (CombinedTrustResult result
                in coordinator.LastCombinedTrustResults)
            {
                Console.WriteLine(
                    $"UAV-{result.DroneId:00} | " +
                    $"B: {result.BehaviourTrust:F3} | " +
                    $"V: {result.ValidationTrust:F3} | " +
                    $"T: {result.TemporalTrust:F3} | " +
                    $"C: {result.CommunicationTrust:F3} | " +
                    $"Overall: {result.OverallTrust:F3}");
            }

            // -------------------------------------------------
            // Global Model Evaluation
            // -------------------------------------------------

            IReadOnlyList<ValidationSample>
                validationSamples =
                    ValidationData.Create();

            ModelEvaluationResult evaluation =
                ModelEvaluator.Evaluate(
                    coordinator.GlobalModel,
                    validationSamples);

            Console.WriteLine();
            Console.WriteLine(
                "Global Model Evaluation");
            Console.WriteLine(
                "-----------------------");

            Console.WriteLine(
                $"True Positives:  " +
                $"{evaluation.TruePositives}");

            Console.WriteLine(
                $"True Negatives:  " +
                $"{evaluation.TrueNegatives}");

            Console.WriteLine(
                $"False Positives: " +
                $"{evaluation.FalsePositives}");

            Console.WriteLine(
                $"False Negatives: " +
                $"{evaluation.FalseNegatives}");

            Console.WriteLine();

            Console.WriteLine(
                $"Accuracy:  {evaluation.Accuracy:P1}");

            Console.WriteLine(
                $"Precision: {evaluation.Precision:P1}");

            Console.WriteLine(
                $"Recall:    {evaluation.Recall:P1}");

            Console.WriteLine(
                $"F1 Score:  {evaluation.F1Score:P1}");

            Console.WriteLine(
                $"Log Loss:  {evaluation.LogLoss:F4}");

        } // end of Main method

        private static int ReadApplicationMode()
{
    while (true)
    {
        Console.Write(
            "Select mode [default 1]: ");

        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) ||
            input == "1")
        {
            return 1;
        }

        if (input == "2")
        {
            return 2;
        }

        Console.WriteLine();
        Console.WriteLine(
            "Please enter 1 or 2.");
        Console.WriteLine();
    }
}






        private static void RunBenchmarkSuite()
        {
            List<BenchmarkScenario> benchmarkScenarios =
                BenchmarkScenarioFactory.Create();

            List<BenchmarkResult> benchmarkResults =
                new List<BenchmarkResult>();

            int[] randomSeeds = { 42, 101, 202, 303, 404 };

            // -------------------------------------------------
            // Model Quality Benchmarks
            // -------------------------------------------------

            Console.WriteLine("UAV-FL SecureSim Benchmark Suite");
            Console.WriteLine("================================");
            Console.WriteLine();

            Console.WriteLine(
                $"{"Scenario",-55} " +
                $"{"Seed",5} " +
                $"{"Acc",7} " +
                $"{"Prec",7} " +
                $"{"Recall",7} " +
                $"{"F1",7} " +
                $"{"Loss",8} " +
                $"{"Comms",9} " +
                $"{"Time ms",10}");

            Console.WriteLine(new string('-', 124));

            foreach (BenchmarkScenario scenario in benchmarkScenarios)
            {
                foreach (int randomSeed in randomSeeds)
                {
                    BenchmarkResult result =
                        BenchmarkRunner.Run(
                            scenario,
                            randomSeed: randomSeed);

                    benchmarkResults.Add(result);

                    Console.WriteLine(
                        $"{result.ScenarioName,-55} " +
                        $"{result.RandomSeed,5} " +
                        $"{result.Accuracy,7:P1} " +
                        $"{result.Precision,7:P1} " +
                        $"{result.Recall,7:P1} " +
                        $"{result.F1Score,7:P1} " +
                        $"{result.LogLoss,8:F4} " +
                        $"{result.SuccessfulCommunications + "/" +
                           result.ExpectedCommunications,9} " +
                        $"{result.ExecutionTimeMilliseconds,10:F2}");
                }
            }

            Console.WriteLine();

            string resultsPath =
                Path.Combine(
                    "results",
                    "benchmark_results.csv");

            BenchmarkCsvExporter.Export(
                benchmarkResults,
                resultsPath);

            Console.WriteLine(
                $"Model results saved to: " +
                $"{Path.GetFullPath(resultsPath)}");

            // -------------------------------------------------
            // Performance Timing Benchmarks
            // -------------------------------------------------

            const int warmupRuns = 3;
            const int timedRepetitions = 10;
            const int timingSeed = 42;

            List<(
                BenchmarkScenario Scenario,
                BenchmarkTimingResult Timing)> timingResults =
                    new List<(
                        BenchmarkScenario Scenario,
                        BenchmarkTimingResult Timing)>();

            Console.WriteLine();
            Console.WriteLine("Benchmark Timing Results");
            Console.WriteLine("------------------------");

            Console.WriteLine(
                $"{"Scenario",-55} " +
                $"{"Mean ms",10} " +
                $"{"Std Dev",10} " +
                $"{"Min ms",10} " +
                $"{"Max ms",10}");

            Console.WriteLine(new string('-', 101));

            foreach (BenchmarkScenario scenario in benchmarkScenarios)
            {
                BenchmarkTimingResult timing =
                    BenchmarkTimingRunner.Run(
                        scenario,
                        randomSeed: timingSeed,
                        warmupRuns: warmupRuns,
                        timedRepetitions: timedRepetitions);

                timingResults.Add((scenario, timing));

                Console.WriteLine(
                    $"{scenario.Name,-55} " +
                    $"{timing.MeanMilliseconds,10:F3} " +
                    $"{timing.StandardDeviationMilliseconds,10:F3} " +
                    $"{timing.MinimumMilliseconds,10:F3} " +
                    $"{timing.MaximumMilliseconds,10:F3}");
            }

            Console.WriteLine();

            string timingPath =
                Path.Combine(
                    "results",
                    "benchmark_timing.csv");

            BenchmarkTimingCsvExporter.Export(
                timingResults,
                timingPath,
                warmupRuns);

            Console.WriteLine(
                $"Timing results saved to: " +
                $"{Path.GetFullPath(timingPath)}");

            Console.WriteLine();
            Console.WriteLine(
                $"Benchmark suite complete. " +
                $"{benchmarkResults.Count} model-quality runs " +
                $"and {timingResults.Count} timing scenarios.");
        }// end of RunBenchmarkSuite method






        private static double ReadCommunicationReliability()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Communication Conditions");
                Console.WriteLine("------------------------");
                Console.WriteLine("1. Reliable (100%)");
                Console.WriteLine("2. Moderate  (80%)");
                Console.WriteLine("3. Poor      (60%)");
                Console.WriteLine("4. No Communication (0%)");
                Console.Write(
                    "Select communication conditions [default 1]: ");

                string? input = Console.ReadLine();

                // Pressing Enter selects the research default.
                if (string.IsNullOrWhiteSpace(input) ||
                    input == "1")
                {
                    return 1.0;
                }

                if (input == "2")
                {
                    return 0.8;
                }

                if (input == "3")
                {
                    return 0.6;
                }

                if (input == "4")
                {
                    return 0.0;
                }

                Console.WriteLine();
                Console.WriteLine(
                    "Please enter 1, 2, 3 or 4.");
            }
        }// end of ReadCommunicationReliability method


        private static double ReadAttackStrength()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Model Poisoning Attack");
                Console.WriteLine("----------------------");
                Console.WriteLine("1. Low      (1.0)");
                Console.WriteLine("2. Moderate (2.0)");
                Console.WriteLine("3. High     (5.0)");
                Console.Write(
                    "Select attack strength [default 2]: ");

                string? input = Console.ReadLine();

                // Pressing Enter selects the research default.
                if (string.IsNullOrWhiteSpace(input) ||
                    input == "2")
                {
                    return 2.0;
                }

                if (input == "1")
                {
                    return 1.0;
                }

                if (input == "3")
                {
                    return 5.0;
                }

                Console.WriteLine();
                Console.WriteLine(
                    "Please enter 1, 2 or 3.");
            }
        }

        private static AggregationMethod
    ReadAggregationMethod()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Aggregation Method");
                Console.WriteLine("------------------");
                Console.WriteLine("1. Standard FedAvg");
                Console.WriteLine("2. Trust-Aware");
                Console.Write(
                    "Select aggregation method [default 2]: ");

                string? input = Console.ReadLine();

                // Pressing Enter selects the research default.
                if (string.IsNullOrWhiteSpace(input) ||
                    input == "2")
                {
                    return AggregationMethod.TrustAware;
                }

                if (input == "1")
                {
                    return AggregationMethod.FedAvg;
                }

                Console.WriteLine();
                Console.WriteLine(
                    "Please enter 1 or 2.");
            }
        }


        private static int ReadNumberOfCompromisedDrones(
            int maximum)
        {
            while (true)
            {
                Console.Write(
                    $"Number of compromised UAVs " +
                    $"(0-{maximum}): ");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) &&
                    number >= 0 &&
                    number <= maximum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Please enter a whole number " +
                    $"between 0 and {maximum}.");

                Console.WriteLine();
            }
        }


        private static int ReadNumberOfDrones(
            int maximum)
        {
            while (true)
            {
                Console.Write(
                    $"Number of UAVs " +
                    $"(1-{maximum}): ");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) &&
                    number >= 1 &&
                    number <= maximum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Please enter a whole number " +
                    $"between 1 and {maximum}.");

                Console.WriteLine();
            }
        }

    } // end of class
} // end of namespace