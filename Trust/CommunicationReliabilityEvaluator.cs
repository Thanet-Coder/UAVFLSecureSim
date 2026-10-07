using UAVFLSecureSim.Drones;
using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Trust
{
    internal class CommunicationReliabilityEvaluator
    {
        private readonly Dictionary<int, int>
            expectedRounds = new();

        private readonly Dictionary<int, int>
            successfulRounds = new();

        public IReadOnlyList<CommunicationReliabilityResult> Evaluate(
            IReadOnlyList<Drone> drones,
            IReadOnlyList<ModelUpdate> receivedUpdates)
        {
            List<CommunicationReliabilityResult> results = new();

            HashSet<int> receivedDroneIds =
                receivedUpdates
                    .Select(update => update.DroneId)
                    .ToHashSet();

            foreach (Drone drone in drones)
            {
                if (!expectedRounds.ContainsKey(drone.Id))
                {
                    expectedRounds[drone.Id] = 0;
                    successfulRounds[drone.Id] = 0;
                }

                expectedRounds[drone.Id]++;

                if (receivedDroneIds.Contains(drone.Id))
                {
                    successfulRounds[drone.Id]++;
                }

                double trustScore =
                    expectedRounds[drone.Id] == 0
                        ? 0.0
                        : (double)successfulRounds[drone.Id] /
                          expectedRounds[drone.Id];

                results.Add(
                    new CommunicationReliabilityResult(
                        drone.Id,
                        expectedRounds[drone.Id],
                        successfulRounds[drone.Id],
                        trustScore));
            }

            return results;
        }
    }
}