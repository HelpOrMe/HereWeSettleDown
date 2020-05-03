using System.Collections.Generic;
using System.Linq;
using Helper.Debugging;
using Settings;
using Settings.Generator;
using UnityEngine;

namespace World.Generator
{
    public class MoisturePart : GeneratorPart
    {
        private readonly MoistureSettings moistureSettings = SerializedSettings.GetSettings<MoistureSettings>();


        protected override void Run()
        {
            Watcher.WatchRun(SetMoisture);
        }

        private void SetMoisture()
        {
            // Set ocean moisture
            if (moistureSettings.SetMoistureFromOcean)
            {
                foreach (Region region in RegionsInfo.regions)
                {
                    int ind = (RegionsInfo.MaxDistIndex - (int)region.type.DistIndexFromCoastline);
                    int moisture = Mathf.RoundToInt(ind * moistureSettings.OceanMoistureMultiplier);
                    region.type.Moisture = ind;
                }
            }
            
            // Set lakes moisture
            if (moistureSettings.SetMoistureFromLakes)
                SetMoistureAround(LakesInfo.lakes.ToList(), moistureSettings.LakesMoistureMultiplier);

            // Set river moisture
            if (moistureSettings.SetMoistureFromRivers)
            {
                foreach (River river in RiversInfo.rivers)
                {
                    List<Region> riverRegions = new List<Region>();
                    foreach (Vertex vertex in river.vertices)
                        riverRegions.AddRange(vertex.incidentRegions);
                    SetMoistureAround(riverRegions, moistureSettings.RiversMoistureMultiplier);
                }
            }

            // Set water moisture
            foreach (Region region in RegionsInfo.regions.Where(reg => reg.type.isWater))
                region.type.Moisture = RegionsInfo.MaxMoistureIndex;
        }

        private void SetMoistureAround(List<Region> regions, float multiplier = 1f)
        {
            int lowestMoisture = int.MaxValue;
            int moisture = RegionsInfo.MaxMoistureIndex;

            foreach (Region region in regions)
                if (region.type.Moisture == null || region.type.Moisture < moisture)
                    region.type.Moisture = moisture;

            var allRegions = new List<Region>(regions);
            var oldRegions = new List<Region>(regions);
            var newRegions = new List<Region>();

            while (oldRegions.Count > 0)
            {
                moisture--;
                moisture = Mathf.RoundToInt(moisture * multiplier);
                // Check for new lowest moisture
                if (moisture < lowestMoisture)
                    lowestMoisture = moisture;

                newRegions.Clear();
                foreach (Region region in oldRegions)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type.Moisture == null || nRegion.type.Moisture < moisture)
                        {
                            nRegion.type.Moisture = moisture;
                            newRegions.Add(nRegion);
                            allRegions.Add(nRegion);
                        }
                    }
                }
                oldRegions = new List<Region>(newRegions);
            }

            // Change lowest moisture to zero
            if (lowestMoisture < 0)
                allRegions.ForEach(reg => reg.type.Moisture -= lowestMoisture);
        }
    }
}
