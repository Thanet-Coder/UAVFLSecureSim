using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAVFLSecureSim.World
{
    internal class FireMap
    {
        private readonly double[,] temperatures;

        public int Width { get; }
        public int Height { get; }

        public FireMap(int width, int height)
        {
            Width = width;
            Height = height;

            temperatures = new double[height, width];
        }

        public void SetTemperature(int x, int y, double temperature)
        {
            temperatures[y, x] = temperature;
        }

        public double GetTemperature(int x, int y)
        {
            return temperatures[y, x];
        }
    }
}
