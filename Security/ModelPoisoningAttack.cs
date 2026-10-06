using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Security
{
    internal static class ModelPoisoningAttack
    {
        public static ModelUpdate Poison(
            ModelUpdate honestUpdate,
            LogisticModel globalModel,
            double attackStrength)
        {
            if (attackStrength < 0)
            {
                throw new ArgumentException(
                    "Attack strength cannot be negative.");
            }

            double honestWeightDelta =
                honestUpdate.Weight - globalModel.Weight;

            double honestBiasDelta =
                honestUpdate.Bias - globalModel.Bias;

            double poisonedWeight =
                globalModel.Weight -
                (attackStrength * honestWeightDelta);

            double poisonedBias =
                globalModel.Bias -
                (attackStrength * honestBiasDelta);

            return new ModelUpdate(
                honestUpdate.DroneId,
                poisonedWeight,
                poisonedBias,
                honestUpdate.SampleCount);
        }
    }
}