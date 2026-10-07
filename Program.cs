using UAVFLSecureSim.Display;
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.World;
using UAVFLSecureSim.Federation;
using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Trust;

namespace UAVFLSecureSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FireMap map = FixedFireMapGenerator.Create();

            Console.WriteLine("UAV-FL SecureSim");
            Console.WriteLine("================");
            Console.WriteLine();

            int numberOfDrones =
                ReadNumberOfDrones(map.Width);

            DroneSwarm swarm =
                new DroneSwarm(numberOfDrones, map);

            int numberOfCompromisedDrones =
                ReadNumberOfCompromisedDrones(numberOfDrones);

            swarm.CompromiseDrones(
                numberOfCompromisedDrones);

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

            Console.WriteLine();
            Console.WriteLine("Federated Learning");
            Console.WriteLine("------------------");

            FederatedCoordinator coordinator =
                new FederatedCoordinator();

            const int totalRounds = 10;
            const int localEpochs = 100;
            const double learningRate = 0.1;
            const double attackStrength = 2.0;

            Console.WriteLine(
                $"Attack strength: {attackStrength:F1}");

            Console.WriteLine();

            Console.WriteLine(
                $"Round {coordinator.CurrentRound,2}: " +
                $"{coordinator.GlobalModel}");

            for (int round = 0;
                 round < totalRounds;
                 round++)
            {
                // Delayed compromise experiment:
                // UAV-01 behaves honestly during rounds 1-5
                // and becomes compromised before round 6.
                if (round == 5)
                {
                    swarm.Drones[0].Compromise();

                    Console.WriteLine();
                    Console.WriteLine(
                        "*** UAV-01 COMPROMISED BEFORE ROUND 6 ***");
                    Console.WriteLine();
                }

                coordinator.RunRound(
                    swarm,
                    localEpochs,
                    learningRate,
                    attackStrength);

                if (coordinator.CurrentRound == 6)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Temporal Evidence at Compromise Transition");
                    Console.WriteLine(
                        "------------------------------------------");

                    foreach (TemporalConsistencyResult result
                        in coordinator.LastTemporalResults)
                    {
                        Console.WriteLine(
                            $"UAV-{result.DroneId:00} | " +
                            $"Similarity: {result.Similarity:F4} | " +
                            $"Temporal Trust: {result.TrustScore:F3}");
                    }

                    Console.WriteLine();
                }

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
                    $"Successful: {result.SuccessfulRounds}/" +
                    $"{result.ExpectedRounds} | " +
                    $"Reliability Trust: {result.TrustScore:F3}");
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


        private static int ReadNumberOfCompromisedDrones(
            int maximum)
        {
            while (true)
            {
                Console.Write(
                    $"Number of compromised UAVs (0-{maximum}): ");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) &&
                    number >= 0 &&
                    number <= maximum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Please enter a whole number between 0 and {maximum}.");

                Console.WriteLine();
            }
        }


        private static int ReadNumberOfDrones(int maximum)
        {
            while (true)
            {
                Console.Write(
                    $"Number of UAVs (1-{maximum}): ");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) &&
                    number >= 1 &&
                    number <= maximum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Please enter a whole number between 1 and {maximum}.");

                Console.WriteLine();
            }
        }

    } // end of class
} // end of namespace