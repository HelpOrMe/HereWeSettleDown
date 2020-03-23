using System.Collections.Generic;
using Helper.Debugger;

namespace World.Generator
{
    public class MoisturePart : GeneratorPart
    {
        protected override void Run()
        {
            Watcher.WatchRun(SetMoisture);
        }

        private void SetMoisture()
        {
            foreach (Region region in RegionsInfo.regions)
                region.type.Moisture = RegionsInfo.MaxDistIndex - region.type.DistIndexFromCoastline;

            foreach (Lake lake in LakesPart.lakes)
            {
                List<Region> lakeRegions = new List<Region>();
                foreach (Vertex vertex in lake.vertices)
                    lakeRegions.AddRange(vertex.incidentRegions);
                SetLakeMoisture(lakeRegions);
            }
        }

        private void SetLakeMoisture(List<Region> regionsLayer)
        {
            int moisture = RegionsInfo.MaxMoistureIndex;

            foreach (Region region in regionsLayer)
                if (region.type.Moisture < moisture)
                    region.type.Moisture = moisture;

            while (regionsLayer.Count > 0)
            {
                moisture--;

                List<Region> oldRegionsLayer = new List<Region>(regionsLayer);
                regionsLayer.Clear();
                foreach (Region region in oldRegionsLayer)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type == null || nRegion.type.Moisture < moisture)
                        {
                            nRegion.type.Moisture = moisture;
                            regionsLayer.Add(nRegion);
                        }
                    }
                }
            }
        }
    }
}
