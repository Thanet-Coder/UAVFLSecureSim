using System.Diagnostics;
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Federation;
using UAVFLSecureSim.Trust;
using UAVFLSecureSim.World;

namespace UAVFLSecureSim.Benchmark
{
    internal static class BenchmarkRunner
    {
        public static BenchmarkResult Run(
            BenchmarkScenario scenario,
            int totalRounds = 10,
            int localEpochs = 100,
            double learningRate = 0.1,
            int randomSeed = 42)
        {
            // -------------------------------------------------
            // Create Reproducible Simulation
            // -------------------------------------------------

            FireMap map =
                FixedFireMapGenerator.Create();

            DroneSwarm swarm =
                new DroneSwarm(
                    scenario.NumberOfDrones,
                    map);

            swarm.CompromiseDrones(
                scenario.NumberOfCompromisedDrones);

            foreach (Drone drone in swarm.Drones)
            {
                drone.SetCommunicationReliability(
                    scenario.CommunicationReliability);
            }

            swarm.Survey(map);

            FederatedCoordinator coordinator =
                new FederatedCoordinator(
                    scenario.AggregationMethod,
                    randomSeed);

            // -------------------------------------------------
            // Run Federated Learning Experiment
            // -------------------------------------------------

            Stopwatch stopwatch =
                Stopwatch.StartNew();

            for (int round = 0;
                 round < totalRounds;
                 round++)
            {
                coordinator.RunRound(
                    swarm,
                    localEpochs,
                    learningRate,
                    scenario.AttackStrength);
            }

            stopwatch.Stop();

            // -------------------------------------------------
            // Evaluate Final Global Model
            // -------------------------------------------------

            IReadOnlyList<ValidationSample>
                validationSamples =
                    ValidationData.Create();

            ModelEvaluationResult evaluation =
                ModelEvaluator.Evaluate(
                    coordinator.GlobalModel,
                    validationSamples);

            // -------------------------------------------------
            // Communication Statistics
            // -------------------------------------------------

            int successfulCommunications =
                coordinator.LastCommunicationResults
                    .Sum(result =>
                        result.SuccessfulRounds);

            int expectedCommunications =
                coordinator.LastCommunicationResults
                    .Sum(result =>
                        result.ExpectedRounds);

            // -------------------------------------------------
            // Return Benchmark Result
            // -------------------------------------------------

            return new BenchmarkResult(
                scenario.Name,
                evaluation.Accuracy,
                evaluation.Precision,
                evaluation.Recall,
                evaluation.F1Score,
                evaluation.LogLoss,
                successfulCommunications,
                expectedCommunications,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}