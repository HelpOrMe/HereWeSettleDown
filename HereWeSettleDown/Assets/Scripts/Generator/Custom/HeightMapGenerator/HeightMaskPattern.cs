using System.Collections.Generic;
using UnityEngine;

namespace Generator.Custom
{
    public class HeightMaskPattern : MonoBehaviour
    {
        public HeightMaskPatternType maskType;
        public NoiseMaskPatternSettings noiseSettings;
        public BorderMaskPatternSettings borderSettings;
        public AbsoluteMaskPatternSettings absoluteSettings;
        public PicksMaskPatternSettings picksSettings;

        public HeightMaskPattern[] appendMasksPattern;
        public HeightMaskPattern[] subtractMasksPattern;

        private Vector2Int[] crossPositions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        public int[,] GetMask(System.Random prng, float[,] heightMap)
        {
            int[,] mask;

            switch (maskType)
            {
                case (HeightMaskPatternType.Noise):
                    mask = GetNoiseMask(prng, heightMap); 
                    break;
                case (HeightMaskPatternType.Border):
                    mask = GetBorderMask(heightMap); 
                    break;
                case (HeightMaskPatternType.Picks):
                    mask = GetPicksMask(prng, heightMap); 
                    break;
                default:
                    mask = GetAbsoluteMask(heightMap);
                    break;
            }

            if (appendMasksPattern != null)
                mask = AppendMasks(mask, prng, heightMap);
            if (subtractMasksPattern != null)
                mask = SubtractMasks(mask, prng, heightMap);
            
            return mask;
        }

        private int[,] AppendMasks(int[,] mask, System.Random prng, float[,] heightMap)
        {
            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            int[][,] appendMasks = new int[appendMasksPattern.Length][,];
            for (int i = 0; i < appendMasks.Length; i++)
                appendMasks[i] = appendMasksPattern[i].GetMask(prng, heightMap);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < appendMasks.Length; i ++)
                    {
                        if (appendMasks[i][x, y] == 1)
                            mask[x, y] = 1;
                    }
                }
            }

            return mask;
        }

        private int[,] SubtractMasks(int[,] mask, System.Random prng, float[,] heightMap)
        {
            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            int[][,] subtractMasks = new int[subtractMasksPattern.Length][,];
            for (int i = 0; i < subtractMasks.Length; i++)
                subtractMasks[i] = subtractMasksPattern[i].GetMask(prng, heightMap);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < subtractMasks.Length; i++)
                    {
                        if (subtractMasks[i][x, y] == 1)
                            mask[x, y] = 0;
                    }
                }
            }

            return mask;
        }

        private int[,] GetAbsoluteMask(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            int[,] mask = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (absoluteSettings.minWorldHeight < heightMap[x, y] &&
                        absoluteSettings.maxWorldHeight > heightMap[x, y])
                    {
                        mask[x, y] = 1;
                    }
                }
            }

            return mask;
        }

        private int[,] GetNoiseMask(System.Random prng, float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            int[,] mask = new int[width, height];

            float[,] biomeHeightMask = Noise.GenerateNoiseMap(prng, width, height, noiseSettings.maskSettings);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (noiseSettings.minMaskHeight < biomeHeightMask[x, y] && 
                        noiseSettings.maxMaskHeight > biomeHeightMask[x, y])
                    {
                        if (noiseSettings.minWorldHeight < heightMap[x, y] &&
                            noiseSettings.maxWorldHeight > heightMap[x, y])
                        {
                            mask[x, y] = 1;
                        }
                    }
                }
            }

            return mask;
        }

        private int[,] GetBorderMask(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            int[,] mask = new int[width, height];

            int xDist = Mathf.RoundToInt(width * borderSettings.xDistFromBorder);
            int yDist = Mathf.RoundToInt(height * borderSettings.yDistFromBorder);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if ((x < xDist || x > width - xDist) ||
                        (y < yDist || y > height - yDist))
                    {
                        if (borderSettings.minWorldHeight < heightMap[x, y] &&
                            borderSettings.maxWorldHeight > heightMap[x, y])
                        {
                            mask[x, y] = 1;
                        }
                    }
                }
            }

            return mask;
        }

        private int[,] GetPicksMask(System.Random prng, float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            List<Vector2Int> picks = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (picksSettings.minWorldHeight < heightMap[x, y] &&
                        picksSettings.maxWorldHeight > heightMap[x, y])
                    {
                        picks.Add(new Vector2Int(x, y));
                    }
                }
            }

            List<Vector2Int> selectedPicks = new List<Vector2Int>();
            for (int i = 0; i < Mathf.RoundToInt(picks.Count * picksSettings.detectPicksCount); i++)
            {
                int ind = prng.Next(0, picks.Count);
                selectedPicks.Add(picks[ind]);
                picks.RemoveAt(ind);
            }

            int[,] mask = new int[width, height];
            foreach (Vector2Int pick in selectedPicks)
            {
                mask = SelectPicks(heightMap, mask, pick.x, pick.y, picksSettings.maxIterations);
            }

            return mask;
        }

        private int[,] SelectPicks(float[,] heightMap, int[,] mask, int x, int y, int maxIterCount)
        {
            maxIterCount--;
            mask[x, y] = 1;

            if (maxIterCount > 0 && 
                heightMap[x, y] > picksSettings.minStopHeight && 
                heightMap[x, y] < picksSettings.maxStopHeight)
            {
                for (int i = 0; i < crossPositions.Length; i++)
                {
                    int nX = x + crossPositions[i].x;
                    int nY = y + crossPositions[i].y;

                    if (nX >= 0 && nX < heightMap.GetLength(0) &&
                        nY >= 0 && nY < heightMap.GetLength(1))
                    {
                        if (mask[nX, nY] == 0)
                            mask = SelectPicks(heightMap, mask, nX, nY, maxIterCount);
                    }
                }
            }
            else
            {
                // Draw borders with brush size
                int brushOffset = Mathf.RoundToInt(picksSettings.brushSize / 2f);
                for (int bX = -brushOffset; bX < brushOffset; bX++)
                {
                    for (int bY = -brushOffset; bY < brushOffset; bY++)
                    {
                        int px = x + bX;
                        int py = y + bY;
                        if (px >= 0 && px < heightMap.GetLength(0) &&
                            py >= 0 && py < heightMap.GetLength(1))
                        {
                            mask[px, py] = 1;
                        }
                    }
                }
            }
            return mask;
        }
    }

    public enum HeightMaskPatternType
    {
        Absolute,
        Noise,
        Border,
        Picks
    }

    [System.Serializable]
    public struct AbsoluteMaskPatternSettings
    {
        [Range(-1, 1)] public float minWorldHeight;
        [Range(-1, 1)] public float maxWorldHeight;
    }

    [System.Serializable]
    public struct NoiseMaskPatternSettings
    {
        [Range(-1, 1)] public float minWorldHeight;
        [Range(-1, 1)] public float maxWorldHeight;


        [Range(0, 1)] public float minMaskHeight;
        [Range(0, 1)] public float maxMaskHeight;

        public GenerationSettings maskSettings;
    }

    [System.Serializable]
    public struct BorderMaskPatternSettings
    {
        [Range(0, 1)] public float xDistFromBorder;
        [Range(0, 1)] public float yDistFromBorder;

        [Range(-1, 1)] public float minWorldHeight;
        [Range(-1, 1)] public float maxWorldHeight;
    }

    [System.Serializable]
    public struct PicksMaskPatternSettings
    {
        public int brushSize;

        public int maxIterations;
        [Range(0, 1)] public float detectPicksCount;

        [Range(0, 1)] public float minStopHeight;
        [Range(0, 1)] public float maxStopHeight;

        [Range(-1, 1)] public float maxWorldHeight;
        [Range(-1, 1)] public float minWorldHeight;
    }
}

