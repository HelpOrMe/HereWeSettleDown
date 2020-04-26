using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helper.Debugging;
using Helper.Math;
using Helper.Random;
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
            Watcher.WatchRun(GenerateWaterMask, SetRegionTypes, SetCoastline, SetRegionDistances);
        }

        private void GenerateWaterMask()
        {
            waterMaskGraph.SetGraphSettings(settings.worldWidth, settings.worldHeight, Seed.prng);
            waterMask = waterMaskGraph.GetMap(0);
        }

        private void SetRegionTypes()
        {
            SetGround();
            SetLakes();
        }

        private void SetLakes()
        {
            // todo: Translate

            // Этот метод групперует регионы воды и разделяет их на озера и океан.
            // Озера имеют флаг isLake и isWater, когда же океан имеет только isWater

            // Берем все регионы воды в список
            var waterRegions = RegionsInfo.regions.Where(region => region.type.isWater).ToList();

            // Список всех озер и океана
            var allRegionGroups = new List<List<Region>>();
            
            // Объединяем соединенные регионы воды в списки
            while (waterRegions.Count > 0)
            {
                // Берем первый регион воды из списка
                Region targetRegion = waterRegions[0];
                waterRegions.RemoveAt(0);

                var regionGroup = new List<Region>();
                var oldRegions = new List<Region>() { targetRegion };

                // Ищем соединенные регионы воды
                while (oldRegions.Count > 0)
                {
                    var newRegions = new List<Region>();

                    // Перебираем все последние регионы
                    foreach (Region region in oldRegions)
                    {
                        // Берем все соседние регионы воды
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

                // Добавляем соединенные регионы в общий список
                allRegionGroups.Add(regionGroup);
            }

            List<Region> lakes = new List<Region>();

            // Добавляем все группы регионов воды в список озер кроме океана (самой большой группы)
            int maxCount = allRegionGroups.Select(lst => lst.Count).Max();
            foreach (var regionGroup in allRegionGroups)
            {
                if (regionGroup.Count < maxCount)
                {
                    lakes.AddRange(regionGroup);
                }
            }
            
            LakesInfo.lakes = lakes.ToArray();
            LakesInfo.UpdateLakesMap();
        }

        private void SetGround()
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
