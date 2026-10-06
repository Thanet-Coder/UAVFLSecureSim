namespace UAVFLSecureSim.Learning
{
    internal class ModelUpdate
    {
        public int DroneId { get; }
        public double Weight { get; }
        public double Bias { get; }
        public int SampleCount { get; }

        public ModelUpdate(
            int droneId,
            double weight,
            double bias,
            int sampleCount)
        {
            DroneId = droneId;
            Weight = weight;
            Bias = bias;
            SampleCount = sampleCount;
        }


    }// end of class
}// end of namespace