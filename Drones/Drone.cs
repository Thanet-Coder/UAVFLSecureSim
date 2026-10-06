using UAVFLSecureSim.World;
using UAVFLSecureSim.Learning;

namespace UAVFLSecureSim.Drones
{
    internal class Drone
    {
        private readonly List<DroneObservation> observations = new();

        public int Id { get; }
        public string Name => $"UAV-{Id:00}";

        public int StartX { get; private set; }
        public int EndX { get; private set; }

        public int ObservationCount => observations.Count;
        public double AverageTemperature =>
    observations.Count == 0
        ? 0.0
        : observations.Average(o => o.Temperature);

        public double MinimumTemperature =>
            observations.Count == 0
                ? 0.0
                : observations.Min(o => o.Temperature);

        public double MaximumTemperature =>
            observations.Count == 0
                ? 0.0
                : observations.Max(o => o.Temperature);

        public Drone(int id)
        {
            Id = id;
        }

        public void AssignSurveyRegion(int startX, int endX)
        {
            StartX = startX;
            EndX = endX;
        }

        public void Survey(FireMap map)
        {
            observations.Clear();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = StartX; x <= EndX; x++)
                {
                    double temperature = map.GetTemperature(x, y);

                    observations.Add(
                        new DroneObservation(x, y, temperature));
                }
            }
        }

        public override string ToString()
        {
            return $"{Name}: columns {StartX}-{EndX}";
        }

        public LogisticModel TrainLocalModel(
            LogisticModel globalModel,
            int epochs,
            double learningRate)
        {
            LogisticModel localModel = globalModel.Copy();

            List<(double Temperature, int Label)> trainingData = new();

            foreach (DroneObservation observation in observations)
            {
                int label = observation.Temperature >= 60.0 ? 1 : 0;

                trainingData.Add(
                    (observation.Temperature, label));
            }

            ModelTrainer.Train(
                localModel,
                trainingData,
                epochs,
                learningRate);

            return localModel;
        }


        public ModelUpdate CreateModelUpdate(
            LogisticModel globalModel,
            int epochs,
            double learningRate)
        {
            LogisticModel localModel =
                TrainLocalModel(
                    globalModel,
                    epochs,
                    learningRate);

            return new ModelUpdate(
                Id,
                localModel.Weight,
                localModel.Bias,
                ObservationCount);
        }


    }// end of class
}// end of namespace