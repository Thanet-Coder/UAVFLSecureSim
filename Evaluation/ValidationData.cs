namespace UAVFLSecureSim.Evaluation
{
    internal static class ValidationData
    {
        public static IReadOnlyList<ValidationSample> Create()
        {
            double[] temperatures =
            {
                20.0,
                25.0,
                30.0,
                35.0,
                40.0,
                45.0,
                50.0,
                52.0,
                54.0,
                56.0,
                58.0,
                60.0,
                62.0,
                64.0,
                66.0,
                70.0,
                75.0,
                80.0,
                85.0,
                90.0,
                95.0
            };

            List<ValidationSample> samples = new();

            foreach (double temperature in temperatures)
            {
                int label =
                    temperature >= 60.0 ? 1 : 0;

                samples.Add(
                    new ValidationSample(
                        temperature,
                        label));
            }

            return samples;
        }
    }
}