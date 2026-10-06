using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAVFLSecureSim.Drones
{
    internal class DroneObservation
    {
        public int X { get; }
        public int Y { get; }
        public double Temperature { get; }

        public DroneObservation(int x, int y, double temperature)
        {
            X = x;
            Y = y;
            Temperature = temperature;
        }
    }
}
