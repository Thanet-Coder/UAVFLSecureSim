namespace UAVFLSecureSim.Learning
{
    internal class LogisticModel
    {
        public double Weight { get; set; }
        public double Bias { get; set; }

        public LogisticModel(double weight = 0.0, double bias = 0.0)
        {
            Weight = weight;
            Bias = bias;
        }

        public double PredictProbability(double temperature)
        {
            double x = NormaliseTemperature(temperature);
            double z = (Weight * x) + Bias;

            return Sigmoid(z);
        }

        public bool PredictFire(double temperature)
        {
            return PredictProbability(temperature) >= 0.5;
        }

        public LogisticModel Copy()
        {
            return new LogisticModel(Weight, Bias);
        }

        public static double NormaliseTemperature(double temperature)
        {
            return (temperature - 50.0) / 25.0;
        }

        private static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }

        public override string ToString()
        {
            return $"Weight={Weight:F4}, Bias={Bias:F4}";
        }
    }
}