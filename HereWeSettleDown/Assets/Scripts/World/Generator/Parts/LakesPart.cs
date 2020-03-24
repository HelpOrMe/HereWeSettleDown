using System.Collections.Generic;
using Helper.Random;
using Helper.Debugger;
using Settings;
using Settings.Generator;

namespace World.Generator
{
    public class LakesPart : GeneratorPart
    {
        private readonly List<Lake> lakes = new List<Lake>();
        private readonly LakeSettings lakeSettings = SettingsObject.GetObject<LakeSettings>();

        protected override void Run()
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
                    Region region = RegionsInfo.regions[Seed.Range(0, RegionsInfo.regions.Length)];
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
            LakesInfo.lakes = lakes.ToArray();
            LakesInfo.UpdateLakesMap();
        }
    }
}
