using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Learning;
using UAVFLSecureSim.Trust;
using UAVFLSecureSim.Evaluation;


namespace UAVFLSecureSim.Federation
{
    internal class FederatedCoordinator
    {
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

            // Evaluate how unusual each UAV's model update is.
            LastBehaviourResults =
                UpdateBehaviourEvaluator.Evaluate(
                    updates,
                    GlobalModel);

            // Evaluate each submitted model against the
            // independent coordinator-side validation data.
            IReadOnlyList<ValidationSample> validationSamples =
                ValidationData.Create();

            LastValidationResults =
                ValidationImpactEvaluator.Evaluate(
                    updates,
                    validationSamples);

            // Standard FedAvg aggregation remains unchanged
            // during the evidence-gathering stages.
            GlobalModel =
                FedAvgAggregator.Aggregate(updates);

            CurrentRound++;
        }



    }// end of class
}// end of namespace