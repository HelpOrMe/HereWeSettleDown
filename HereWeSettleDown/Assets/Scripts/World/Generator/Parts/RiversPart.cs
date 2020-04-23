using Helper.Debugger;
using Helper.Random;
using Settings;
using Settings.Generator;
using System.Collections.Generic;

namespace World.Generator
{
    public class RiversPart : GeneratorPart
    {
        private readonly List<River> rivers = new List<River>();
        private readonly RiverSettings riverSettings = SettingsObject.GetObject<RiverSettings>();

        protected override void Run()
        {
            Watcher.WatchRun(SetRivers);
        }

        private void SetRivers()
        {
            int rndRiverCount = Seed.Range(riverSettings.MinRiverCount, riverSettings.MaxRiverCount);
            for (int i = 0; i < rndRiverCount; i++)
            {
                while (true)
                {
                    Region region = RegionsInfo.regions[Seed.Range(0, RegionsInfo.regions.Length)];
                    if (region.type.isGround && region.type.DistIndexFromCoastline > 2)
                    {
                        Vertex vertex = region.vertices[Seed.Range(0, region.vertices.Length)];
                        River river = new River(vertex);
                        river.Set();
                        rivers.Add(river);
                        break;
                    }
                }
            }
            RiversInfo.rivers = rivers.ToArray();
            RiversInfo.UpdateRiversMap();
        }
    }
}
