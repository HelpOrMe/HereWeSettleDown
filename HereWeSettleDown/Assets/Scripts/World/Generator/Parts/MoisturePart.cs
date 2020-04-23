using System.Collections.Generic;
using System.Linq;
using Helper.Debugger;
using Settings;
using Settings.Generator;

namespace World.Generator
{
    public class MoisturePart : GeneratorPart
    {
        private readonly MoistureSettings moistureSettings = SettingsObject.GetObject<MoistureSettings>();


        protected override void Run()
        {
            Watcher.WatchRun(SetMoisture);
        }

        private void SetMoisture()
        {
            RegionsInfo.UpdateMoistureIndex(RegionsInfo.MaxDistIndex);

            // Set ocean moisture
            foreach (Region region in RegionsInfo.regions)
                if (moistureSettings.SetMoistureFromOcean || region.type.DistIndexFromCoastline <= 0)
                    region.type.Moisture = RegionsInfo.MaxDistIndex - region.type.DistIndexFromCoastline;
            
            // Set lakes moisture
            if (moistureSettings.SetMoistureFromLakes)
                SetMoistureAround(LakesInfo.lakes.ToList());

            // Set river moisture
            if (moistureSettings.SetMoistureFromRivers)
            {
                foreach (River river in RiversInfo.rivers)
                {
                    List<Region> riverRegions = new List<Region>();
                    foreach (Vertex vertex in river.vertices)
                        riverRegions.AddRange(vertex.incidentRegions);
                    SetMoistureAround(riverRegions);
                }
            }
        }

        private void SetMoistureAround(List<Region> regionsLayer)
        {
            int moisture = RegionsInfo.MaxMoistureIndex;

            foreach (Region region in regionsLayer)
                if (region.type.Moisture == null || region.type.Moisture < moisture)
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
                        if (nRegion.type.Moisture == null || nRegion.type.Moisture < moisture)
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
