using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAVFLSecureSim.World
{
    internal static class FixedFireMapGenerator
    {
        public static FireMap Create()
        {
            FireMap map = new FireMap(15, 10);

            // Start with a normal ambient temperature.
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    map.SetTemperature(x, y, 22.0);
                }
            }

            // Hot area surrounding the fire.
            SetArea(map, 5, 2, 9, 6, 42.0);

            // Main fire.
            SetArea(map, 6, 3, 8, 5, 75.0);

            // Very hot centre.
            map.SetTemperature(7, 4, 95.0);

            return map;
        }

        private static void SetArea(
            FireMap map,
            int startX,
            int startY,
            int endX,
            int endY,
            double temperature)
        {
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    map.SetTemperature(x, y, temperature);
                }
            }
        }
    }
}