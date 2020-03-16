using System.Collections.Generic;
using Helper.Random;
using Helper.Debugger;
using Settings;

namespace World.Generator
{
    public class LakesPart : GeneratorPart
    {
        public static List<Lake> lakes = new List<Lake>();
        private readonly LakeSettings lakeSettings = SettingsObject.GetObject<LakeSettings>();

        public override void Run()
        {
            Watcher.WatchRun(SetLakes);
        }

        private void SetLakes()
        {
            int rndLakedCount = Seed.Range(lakeSettings.MinLakeCount, lakeSettings.MaxLakeCount);
            for (int i = 0; i < rndLakedCount; i++)
            {
                while (true)
                {
                    Region region = RegionsPart.regions[Seed.Range(0, RegionsPart.regions.Length)];
                    if (region.type.isGround && region.type.DistIndexFromCoastline > 2)
                    {
                        Vertex vertex = region.vertices[Seed.Range(0, region.vertices.Length)];
                        Lake lake = new Lake(vertex);
                        lake.Set();
                        lakes.Add(lake);
                        break;
                    }
                }
            }
        }
    }
}
