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

        public TrustCombinationMethod TrustCombinationMethod
        { get; private set; }

        public IReadOnlyList<UpdateBehaviourResult>
            LastBehaviourResults
        { get; private set; }
                = Array.Empty<UpdateBehaviourResult>();

        public IReadOnlyList<ValidationImpactResult>
            LastValidationResults
        { get; private set; }
                = Array.Empty<ValidationImpactResult>();

        public IReadOnlyList<GlobalRelativeValidationResult>
            LastGlobalRelativeValidationResults
        { get; private set; }
        = Array.Empty<GlobalRelativeValidationResult>();

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
            int randomSeed = 42,
            TrustCombinationMethod trustCombinationMethod =
                TrustCombinationMethod.OriginalWeighted)
        {
            GlobalModel = new LogisticModel();
            CurrentRound = 0;

            AggregationMethod = aggregationMethod;
            TrustCombinationMethod = trustCombinationMethod;
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


            // -------------------------------------------------
            // Handle a round with no received model updates
            // -------------------------------------------------

            if (updates.Count == 0)
            {
                // Record the communication failures even
                // though no model updates were received.
                LastCommunicationResults =
                    communicationReliabilityEvaluator.Evaluate(
                        swarm.Drones,
                        updates);

                // No submitted updates means there is no
                // new behaviour, validation or temporal
                // evidence to evaluate.
                LastBehaviourResults =
                    Array.Empty<UpdateBehaviourResult>();

                LastValidationResults =
                    Array.Empty<ValidationImpactResult>();

                LastGlobalRelativeValidationResults =
                    Array.Empty<GlobalRelativeValidationResult>();

                LastTemporalResults =
                    Array.Empty<TemporalConsistencyResult>();

                LastCombinedTrustResults =
                    Array.Empty<CombinedTrustResult>();

                // Keep the existing global model unchanged.
                // The unsuccessful round still counts.
                CurrentRound++;

                return;
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

            // Experimental diagnostic:
            // Compare each submitted model against the
            // current global model before aggregation.
            LastGlobalRelativeValidationResults =
                GlobalRelativeValidationEvaluator.Evaluate(
                    updates,
                    GlobalModel,
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

            // Select the trust-combination mechanism.
            // OriginalWeighted preserves the baseline.
            // ValidationConditionedTemporal is experimental.

            if (TrustCombinationMethod ==
                TrustCombinationMethod.OriginalWeighted)
            {
                LastCombinedTrustResults =
                    CombinedTrustEvaluator.Evaluate(
                        LastBehaviourResults,
                        LastValidationResults,
                        LastTemporalResults,
                        LastCommunicationResults);
            }
            else
            {
                LastCombinedTrustResults =
                    ValidationConditionedTrustEvaluator.Evaluate(
                        LastBehaviourResults,
                        LastValidationResults,
                        LastTemporalResults,
                        LastCommunicationResults);
            }

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