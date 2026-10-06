using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Federation
{
    internal class FederatedCoordinator
    {
        public LogisticModel GlobalModel { get; private set; }

        public int CurrentRound { get; private set; }

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

            GlobalModel =
                FedAvgAggregator.Aggregate(updates);

            CurrentRound++;
        }



    }// end of class
}// end of namespace