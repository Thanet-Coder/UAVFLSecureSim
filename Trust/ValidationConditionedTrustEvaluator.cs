namespace UAVFLSecureSim.Trust
{
    internal static class ValidationConditionedTrustEvaluator
    {
        private const double BehaviourWeight = 0.25;
        private const double ValidationWeight = 0.40;
        private const double TemporalWeight = 0.25;
        private const double CommunicationWeight = 0.10;

        public static IReadOnlyList<CombinedTrustResult> Evaluate(
            IReadOnlyList<UpdateBehaviourResult> behaviourResults,
            IReadOnlyList<ValidationImpactResult> validationResults,
            IReadOnlyList<TemporalConsistencyResult> temporalResults,
            IReadOnlyList<CommunicationReliabilityResult> communicationResults)
        {
            List<CombinedTrustResult> results = new();

            foreach (UpdateBehaviourResult behaviour
                in behaviourResults)
            {
                ValidationImpactResult? validation =
                    validationResults.FirstOrDefault(
                        result =>
                            result.DroneId == behaviour.DroneId);

                TemporalConsistencyResult? temporal =
                    temporalResults.FirstOrDefault(
                        result =>
                            result.DroneId == behaviour.DroneId);

                CommunicationReliabilityResult? communication =
                    communicationResults.FirstOrDefault(
                        result =>
                            result.DroneId == behaviour.DroneId);

                if (validation == null ||
                    temporal == null ||
                    communication == null)
                {
                    continue;
                }

                // Experimental mechanism:
                // Temporal consistency is conditioned
                // on validation performance.
                double adjustedTemporalTrust =
                    temporal.TrustScore *
                    validation.TrustScore;

                double overallTrust =
                    (behaviour.TrustScore * BehaviourWeight) +
                    (validation.TrustScore * ValidationWeight) +
                    (adjustedTemporalTrust * TemporalWeight) +
                    (communication.TrustScore *
                        CommunicationWeight);

                results.Add(
                    new CombinedTrustResult(
                        behaviour.DroneId,
                        behaviour.TrustScore,
                        validation.TrustScore,
                        adjustedTemporalTrust,
                        communication.TrustScore,
                        overallTrust));
            }

            return results;
        }
    }
}