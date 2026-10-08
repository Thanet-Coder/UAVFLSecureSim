
using UAVFLSecureSim.Benchmark;
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Federation;
using UAVFLSecureSim.Trust;
using UAVFLSecureSim.World;

namespace UAVFLSecureSim.Detection
{
    internal static class DetectionBenchmarkRunner
    {
        public static IReadOnlyList<DroneDetectionResult> Run(
            BenchmarkScenario scenario,
            double detectionThreshold = 0.50,
            int totalRounds = 10,
            int localEpochs = 100,
            double learningRate = 0.1,
            int randomSeed = 42,
            bool showTrustComponents = false)
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

            List<DroneDetectionResult> detectionResults =
                new();

            // -------------------------------------------------
            // Run Federated Learning and Collect Detection Data
            // -------------------------------------------------

            for (int round = 0;
                 round < totalRounds;
                 round++)
            {
                coordinator.RunRound(
                    swarm,
                    localEpochs,
                    learningRate,
                    scenario.AttackStrength);

                if (showTrustComponents && round == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Round 1 Trust Components");
                    Console.WriteLine("------------------------");

                    Console.WriteLine(
                        $"{"UAV",-8} " +
                        $"{"Behaviour",10} " +
                        $"{"Validation",11} " +
                        $"{"Temporal",10} " +
                        $"{"Comms",8} " +
                        $"{"Overall",9}");

                    foreach (CombinedTrustResult trust
                        in coordinator.LastCombinedTrustResults)
                    {
                        Console.WriteLine(
                            $"{"UAV-" + trust.DroneId.ToString("00"),-8} " +
                            $"{trust.BehaviourTrust,10:F4} " +
                            $"{trust.ValidationTrust,11:F4} " +
                            $"{trust.TemporalTrust,10:F4} " +
                            $"{trust.CommunicationTrust,8:F4} " +
                            $"{trust.OverallTrust,9:F4}");
                    }
                }



                IReadOnlyList<DroneDetectionResult>
                    roundResults =
                        DroneDetectionEvaluator.Evaluate(
                            coordinator.LastCombinedTrustResults,
                            swarm.Drones,
                            detectionThreshold);

                detectionResults.AddRange(roundResults);
            }

            return detectionResults;
        }
    }
}
