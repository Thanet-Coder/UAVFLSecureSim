using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Evaluation;
using UAVFLSecureSim.Learning;
using UAVFLSecureSim.Trust;

namespace UAVFLSecureSim.Federation
{
    internal class FederatedCoordinator
    {
        private readonly TemporalConsistencyEvaluator
            temporalConsistencyEvaluator = new();

        private readonly CommunicationReliabilityEvaluator
            communicationReliabilityEvaluator = new();

        private readonly Random random;

        public LogisticModel GlobalModel { get; private set; }

        public int CurrentRound { get; private set; }

        public AggregationMethod AggregationMethod
        { get; private set; }

        public IReadOnlyList<UpdateBehaviourResult>
            LastBehaviourResults
        { get; private set; }
                = Array.Empty<UpdateBehaviourResult>();

        public IReadOnlyList<ValidationImpactResult>
            LastValidationResults
        { get; private set; }
                = Array.Empty<ValidationImpactResult>();

        public IReadOnlyList<TemporalConsistencyResult>
            LastTemporalResults
        { get; private set; }
                = Array.Empty<TemporalConsistencyResult>();

        public IReadOnlyList<CommunicationReliabilityResult>
            LastCommunicationResults
        { get; private set; }
                = Array.Empty<CommunicationReliabilityResult>();

        public IReadOnlyList<CombinedTrustResult>
            LastCombinedTrustResults
        { get; private set; }
                = Array.Empty<CombinedTrustResult>();

        public FederatedCoordinator(
            AggregationMethod aggregationMethod =
                AggregationMethod.TrustAware,
            int randomSeed = 42)
        {
            GlobalModel = new LogisticModel();
            CurrentRound = 0;

            AggregationMethod = aggregationMethod;

            random = new Random(randomSeed);
        }

        public void RunRound(
            DroneSwarm swarm,
            int localEpochs,
            double learningRate,
            double attackStrength)
        {
            List<ModelUpdate> updates = new();

            // Each UAV trains locally and submits
            // only its model update.
            foreach (Drone drone in swarm.Drones)
            {
                ModelUpdate update =
                    drone.CreateModelUpdate(
                        GlobalModel,
                        localEpochs,
                        learningRate,
                        attackStrength);

                bool communicationSucceeded =
                    random.NextDouble() <
                    drone.CommunicationReliability;

                if (communicationSucceeded)
                {
                    updates.Add(update);
                }
            }

            // Evidence 1:
            // Measure how unusual each update is
            // compared with the rest of the swarm.
            LastBehaviourResults =
                UpdateBehaviourEvaluator.Evaluate(
                    updates,
                    GlobalModel);

            // Create independent validation data
            // for evaluating submitted models.
            IReadOnlyList<ValidationSample>
                validationSamples =
                    ValidationData.Create();

            // Evidence 2:
            // Measure how well each submitted model
            // performs on the validation data.
            LastValidationResults =
                ValidationImpactEvaluator.Evaluate(
                    updates,
                    validationSamples);

            // Evidence 3:
            // Compare each UAV's current update
            // direction with its previous update.
            LastTemporalResults =
                temporalConsistencyEvaluator.Evaluate(
                    updates,
                    GlobalModel);

            // Evidence 4:
            // Record whether each expected UAV
            // successfully supplied an update.
            LastCommunicationResults =
                communicationReliabilityEvaluator.Evaluate(
                    swarm.Drones,
                    updates);

            // Combine the four independent evidence
            // scores into one overall trust score.
            LastCombinedTrustResults =
                CombinedTrustEvaluator.Evaluate(
                    LastBehaviourResults,
                    LastValidationResults,
                    LastTemporalResults,
                    LastCommunicationResults);

            // Select the aggregation strategy for
            // this federated-learning experiment.
            if (AggregationMethod ==
                AggregationMethod.FedAvg)
            {
                GlobalModel =
                    FedAvgAggregator.Aggregate(
                        updates);
            }
            else
            {
                GlobalModel =
                    TrustAwareAggregator.Aggregate(
                        updates,
                        LastCombinedTrustResults);
            }

            CurrentRound++;

        } // end RunRound

    } // end class
} // end namespace