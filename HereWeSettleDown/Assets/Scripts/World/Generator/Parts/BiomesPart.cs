﻿using UnityEngine;
using Helper.Debugger;
using Settings;
using Settings.Generator;

namespace World.Generator
{
    public class BiomesPart : GeneratorPart
    {
        BiomesSettings biomesSettings = SettingsObject.GetObject<BiomesSettings>();

        protected override void Run()
        {
            Watcher.WatchRun(SetBiomes);
        }

        private void SetBiomes()
        {
            string[,] convertedBiomes = ConvertBiomes();
            foreach (Region region in RegionsInfo.regions)
            {
                float deltaMoisture = (int)region.type.Moisture / (float)RegionsInfo.MaxMoistureIndex;
                float deltaHeight = (int)region.type.DistIndexFromCoastline / (float)RegionsInfo.MaxDistIndex;

                int tableMoistureLevel = Mathf.RoundToInt(Mathf.Lerp(0, biomesSettings.moistureLevels - 1, deltaMoisture));
                int tableHeightLevel = Mathf.RoundToInt(Mathf.Lerp(0, biomesSettings.heightLevels - 1, deltaHeight));

                region.type.biomeType = convertedBiomes[tableMoistureLevel, tableHeightLevel];
            }
        }

        private string[,] ConvertBiomes()
        {
            int[,] selectedBiomes = biomesSettings.selectedBiomes;
            
            int width = selectedBiomes.GetLength(0);
            int height = selectedBiomes.GetLength(1);
            
            string[,] convertedBiomes = new string[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    convertedBiomes[x, y] = biomesSettings.biomesTypes[selectedBiomes[x, y]];
                }
            }

            return convertedBiomes;
        }
    }
}
