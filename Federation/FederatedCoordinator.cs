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

        public LogisticModel GlobalModel { get; private set; }

        public int CurrentRound { get; private set; }

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

        public FederatedCoordinator()
        {
            GlobalModel = new LogisticModel();
            CurrentRound = 0;
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

                updates.Add(update);
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

            // For now, continue using ordinary
            // FedAvg aggregation.
            //
            // The combined trust scores are calculated
            // but do NOT yet influence aggregation.
            GlobalModel =
                TrustAwareAggregator.Aggregate(
                updates,
                LastCombinedTrustResults);

            CurrentRound++;

        }// end RunRound

    }// end class
}// end namespace