using System.Collections.Generic;
using Helper.Debugger;

namespace World.Generator
{
    public class WetPart : GeneratorPart
    {
        protected override void Run()
        {
            Watcher.WatchRun(SetWet);
        }

        private void SetWet()
        {
            foreach (Region region in RegionsPart.regions)
            {
                region.type.Wet = RegionType.MaxDistIndex - region.type.DistIndexFromCoastline;
            }

            foreach (Lake lake in LakesPart.lakes)
            {
                List<Region> lakeRegions = new List<Region>();
                foreach (Vertex vertex in lake.vertices)
                    lakeRegions.AddRange(vertex.incidentRegions);
                SetRecWet(lakeRegions);
            }
        }

        private void SetRecWet(List<Region> regionsLayer)
        {
            int wet = RegionType.MaxDistIndex - 1;

            foreach (Region region in regionsLayer)
                if (region.type == null || region.type.Wet < wet)
                    region.type.Wet = wet;

            while (regionsLayer.Count > 0)
            {
                wet--;

                List<Region> oldRegionsLayer = new List<Region>(regionsLayer);
                regionsLayer.Clear();
                foreach (Region region in oldRegionsLayer)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type == null || nRegion.type.Wet < wet)
                        {
                            nRegion.type.Wet = wet;
                            regionsLayer.Add(nRegion);
                        }
                    }
                }
            }
        }
    }
}
