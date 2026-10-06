using UAVFLSecureSim.Display;
using UAVFLSecureSim.World;

namespace UAVFLSecureSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FireMap map = FixedFireMapGenerator.Create();

            MapRenderer.Render(map);
        }
    }
}
