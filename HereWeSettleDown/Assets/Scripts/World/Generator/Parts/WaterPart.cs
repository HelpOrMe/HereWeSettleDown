using Helper.Debugging;
using Helper.Math;
using Helper.Random;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Generator.Nodes.HeightMap;

namespace World.Generator
{
    public class WaterPart : GeneratorPart
    {
        public HeightMapGenerationGraph waterMaskGraph;
        public static Region[] coastlineRegions;

        private float[,] waterMask;

        protected override void Run()
        {
            Watcher.WatchRun(GenerateWaterMask, SetGround, SetLakes, SetCoastline, SetRegionDistances);
        }

        private void GenerateWaterMask()
        {
            waterMaskGraph.SetGraphSettings(settings.worldWidth, settings.worldHeight, Seed.prng);
            Log.InfoSet("Water mask graph settings");
            waterMask = waterMaskGraph.GetMap(0);
        }

        private void SetLakes()
        {
            // Groups water into lakes and the ocean.
            // The lakes has type.isLake flag

            List<Region> waterRegions = RegionsInfo.regions.Where(region => region.type.isWater).ToList();
            List<List<Region>> allRegionGroups = new List<List<Region>>();

            while (waterRegions.Count > 0)
            {
                Region targetRegion = waterRegions[0];
                waterRegions.RemoveAt(0);

                List<Region> regionGroup = new List<Region>();
                List<Region> oldRegions = new List<Region>() { targetRegion };

                while (oldRegions.Count > 0)
                {
                    List<Region> newRegions = new List<Region>();

                    foreach (Region region in oldRegions)
                    {
                        foreach (Region regionNr in region.neighbours)
                        {
                            if (regionNr.type.isWater && !regionGroup.Contains(regionNr))
                            {
                                waterRegions.Remove(regionNr);
                                newRegions.Add(regionNr);
                                regionGroup.Add(regionNr);
                            }
                        }
                    }
                    oldRegions = newRegions;
                }

                allRegionGroups.Add(regionGroup);
            }
            Log.Info("Water grouped.");

            List<Region> lakes = new List<Region>();

            // Select all small water groups (lakes)
            int maxCount = allRegionGroups.Select(lst => lst.Count).Max();
            foreach (List<Region> regionGroup in allRegionGroups)
            {
                if (regionGroup.Count < maxCount)
                {
                    regionGroup.ForEach(region => region.type.MarkAsLake());
                    lakes.AddRange(regionGroup);
                }
            }

            LakesInfo.lakes = lakes.ToArray();
            LakesInfo.UpdateLakesMap();
        }

        private void SetGround()
        {
            // Set ground & water flags
            foreach (Region region in RegionsInfo.regions)
            {
                Vector2Int site = MathVert.ToVector2Int(region.site);
                if (waterMask[site.x, site.y] < 1)
                    region.type.MarkAsWater();
                else
                    region.type.MarkAsGround();
            }
        }

        private void SetCoastline()
        {
            List<Region> coastlineRegions = new List<Region>();
            foreach (Region region in RegionsInfo.regions)
            {
                foreach (Region regionNeighbour in region.neighbours)
                {
                    if (region.type.isWater && regionNeighbour.type.isGround)
                    {
                        region.type.MarkAsCoastline();
                        coastlineRegions.Add(region);
                        break;
                    }
                }
            }
            WaterPart.coastlineRegions = coastlineRegions.ToArray();
        }

        private void SetRegionDistances()
        {
            List<Region> regionsLayer = new List<Region>(coastlineRegions);

            int layerDist = 0;
            while (regionsLayer.Count > 0)
            {
                List<Region> regionsLayerClone = new List<Region>(regionsLayer);
                regionsLayer.Clear();

                foreach (Region region in regionsLayerClone)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type.DistIndexFromCoastline == null)
                        {
                            nRegion.type.DistIndexFromCoastline = layerDist;
                            regionsLayer.Add(nRegion);
                        }
                    }
                }
                layerDist++;
            }
        }
    }
}
