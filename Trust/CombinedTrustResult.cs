namespace UAVFLSecureSim.Trust
{
    internal class CombinedTrustResult
    {
        public int DroneId { get; }

        public double BehaviourTrust { get; }
        public double ValidationTrust { get; }
        public double TemporalTrust { get; }
        public double CommunicationTrust { get; }

        public double OverallTrust { get; }

        public CombinedTrustResult(
            int droneId,
            double behaviourTrust,
            double validationTrust,
            double temporalTrust,
            double communicationTrust,
            double overallTrust)
        {
            DroneId = droneId;

            BehaviourTrust = behaviourTrust;
            ValidationTrust = validationTrust;
            TemporalTrust = temporalTrust;
            CommunicationTrust = communicationTrust;

            OverallTrust = overallTrust;
        }
    }
}