using UAVFLSecureSim.Display;
using UAVFLSecureSim.Drones;
using UAVFLSecureSim.World;

namespace UAVFLSecureSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FireMap map = FixedFireMapGenerator.Create();

            Console.WriteLine("UAV-FL SecureSim");
            Console.WriteLine("================");
            Console.WriteLine();

            int numberOfDrones = ReadNumberOfDrones(map.Width);

            DroneSwarm swarm = new DroneSwarm(numberOfDrones, map);
            swarm.Survey(map);

            MapRenderer.Render(map);

            Console.WriteLine("UAV Survey Regions");
            Console.WriteLine("------------------");

            foreach (Drone drone in swarm.Drones)
            {
                Console.WriteLine(
                    $"{drone.Name} | " +
                    $"Region: {drone.StartX}-{drone.EndX} | " +
                    $"Samples: {drone.ObservationCount,3} | " +
                    $"Mean: {drone.AverageTemperature,6:F1} C | " +
                    $"Min: {drone.MinimumTemperature,5:F1} C | " +
                    $"Max: {drone.MaximumTemperature,5:F1} C");
            }
        }

        private static int ReadNumberOfDrones(int maximum)
        {
            while (true)
            {
                Console.Write($"Number of UAVs (1-{maximum}): ");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) &&
                    number >= 1 &&
                    number <= maximum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Please enter a whole number between 1 and {maximum}.");
                Console.WriteLine();
            }
        }
    }
}