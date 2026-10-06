using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UAVFLSecureSim.World;

namespace UAVFLSecureSim.Drones
{
    internal class DroneSwarm
    {
        private readonly List<Drone> drones = new();

        public IReadOnlyList<Drone> Drones => drones;

        public DroneSwarm(int numberOfDrones, FireMap map)
        {
            if (numberOfDrones < 1)
            {
                throw new ArgumentException(
                    "The swarm must contain at least one UAV.");
            }

            if (numberOfDrones > map.Width)
            {
                throw new ArgumentException(
                    "The number of UAVs cannot exceed the map width.");
            }

            CreateDrones(numberOfDrones);
            AssignSurveyRegions(map);
        }

        public void CompromiseDrones(int numberOfCompromisedDrones)
        {
            if (numberOfCompromisedDrones < 0 ||
                numberOfCompromisedDrones > drones.Count)
            {
                throw new ArgumentException(
                    "The number of compromised UAVs must be between 0 " +
                    "and the total number of UAVs.");
            }

            for (int i = 0; i < numberOfCompromisedDrones; i++)
            {
                drones[i].Compromise();
            }
        }

        private void CreateDrones(int numberOfDrones)
        {
            for (int i = 1; i <= numberOfDrones; i++)
            {
                drones.Add(new Drone(i));
            }
        }

        private void AssignSurveyRegions(FireMap map)
        {
            int baseWidth = map.Width / drones.Count;
            int remainder = map.Width % drones.Count;

            int currentX = 0;

            for (int i = 0; i < drones.Count; i++)
            {
                int regionWidth = baseWidth;

                if (i < remainder)
                {
                    regionWidth++;
                }

                int startX = currentX;
                int endX = currentX + regionWidth - 1;

                drones[i].AssignSurveyRegion(startX, endX);

                currentX = endX + 1;
            }
        }

        public void Survey(FireMap map)
        {
            foreach (Drone drone in drones)
            {
                drone.Survey(map);
            }
        }
    }// end of class
}// end of namespace
