using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UAVFLSecureSim.World;

namespace UAVFLSecureSim.Display
{
    internal static class MapRenderer
    {
        public static void Render(FireMap map)
        {
            Console.WriteLine();
            Console.WriteLine("UAV FEDERATED LEARNING - WILDFIRE SURVEY");
            Console.WriteLine();

            Console.Write("    ");

            for (int x = 0; x < map.Width; x++)
            {
                Console.Write($"{x,2} ");
            }

            Console.WriteLine();

            for (int y = 0; y < map.Height; y++)
            {
                Console.Write($"{y,2}  ");

                for (int x = 0; x < map.Width; x++)
                {
                    double temperature = map.GetTemperature(x, y);

                    char symbol = GetTemperatureSymbol(temperature);

                    Console.Write($" {symbol} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Legend:");
            Console.WriteLine(". = Normal");
            Console.WriteLine("H = High temperature");
            Console.WriteLine("F = Fire");
            Console.WriteLine("X = Extreme fire");
            Console.WriteLine();
        }

        private static char GetTemperatureSymbol(double temperature)
        {
            if (temperature >= 90)
                return 'X';

            if (temperature >= 60)
                return 'F';

            if (temperature >= 35)
                return 'H';

            return '.';
        }
    }
}
