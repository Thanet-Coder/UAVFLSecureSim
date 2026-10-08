namespace UAVFLSecureSim.Trust
{
    internal class GlobalRelativeValidationResult
    {
        public int DroneId { get; }

        public double GlobalLogLoss { get; }

        public double SubmittedLogLoss { get; }

        public double LogLossDifference { get; }

        public GlobalRelativeValidationResult(
            int droneId,
            double globalLogLoss,
            double submittedLogLoss)
        {
            DroneId = droneId;

            GlobalLogLoss = globalLogLoss;

            SubmittedLogLoss = submittedLogLoss;

            LogLossDifference =
                submittedLogLoss - globalLogLoss;
        }
    }
}