using Helper.Debugger;
using Helper.Math;
using Helper.Random;
using System.Collections.Generic;
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
            Watcher.WatchRun(GenerateWaterMask, SetWater, SetCoastline, SetRegionDistances);
        }

        private void GenerateWaterMask()
        {
            waterMask = waterMaskGraph.GetMap(settings.worldWidth, settings.worldHeight, Seed.prng);
        }

        private void SetWater()
        {
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
