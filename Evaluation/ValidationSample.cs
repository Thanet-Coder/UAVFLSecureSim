namespace UAVFLSecureSim.Evaluation
{
    internal class ValidationSample
    {
        public double Temperature { get; }
        public int Label { get; }

        public ValidationSample(
            double temperature,
            int label)
        {
            Temperature = temperature;
            Label = label;
        }
    }
}
