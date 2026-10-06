using UAVFLSecureSim.World;

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
    }
}