using System.Collections.Generic;
using UnityEngine;
using World.Generator.Helper;

namespace World.Generator.HeightMap
{
    public class HeightMaskPattern : MonoBehaviour
    {
        public HeightMaskPatternType maskType;

        public NoiseMaskPatternSettings noiseSettings;
        public BorderMaskPatternSettings borderSettings;
        public PicksMaskPatternSettings picksSettings;
        public AbsoluteMaskPatternSettings absoluteSettings;

        public HeightMaskPattern[] appendMasksPatterns;
        public HeightMaskPattern[] subtractMasksPatterns;

        private readonly float[,] generatedMask;

        private readonly Vector2Int[] crossPositions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        public float[,] GetExistsMask(System.Random prng, float[,] heightMap)
        {
            return generatedMask == null ? GetMask(prng, heightMap) : generatedMask;
        }

        public float[,] GetMask(System.Random prng, float[,] heightMap)
        {
            if (prng == null || heightMap == null)
                return null;

            float[,] generatedMask;
            switch (maskType)
            {
                case (HeightMaskPatternType.Noise):
                    generatedMask = GetNoiseMask(prng, heightMap); 
                    break;
                case (HeightMaskPatternType.Border):
                    generatedMask = GetBorderMask(heightMap); 
                    break;
                case (HeightMaskPatternType.Picks):
                    generatedMask = GetPicksMask(prng, heightMap); 
                    break;
                default:
                    generatedMask = GetAbsoluteMask(heightMap);
                    break;
            }

            if (generatedMask != null)
            {
                if (appendMasksPatterns != null)
                {
                    foreach (HeightMaskPattern secMaskPattern in appendMasksPatterns)
                    {
                        float[,] secMask = secMaskPattern.GetExistsMask(prng, heightMap);
                        if (secMask != null)
                            generatedMask = AppendMasks(generatedMask, secMask);
                    }
                }

                if (subtractMasksPatterns != null)
                {
                    foreach (HeightMaskPattern secMaskPattern in subtractMasksPatterns)
                    {
                        float[,] secMask = secMaskPattern.GetExistsMask(prng, heightMap);
                        if (secMask != null)
                            generatedMask = SubtractMasks(generatedMask, secMask);
                    }
                }
            }
            
            return generatedMask;
        }

        public float EvaluteHeight(int x, int y, float value)
        {
            if (generatedMask != null)
            {
                if (x >= 0 && x < generatedMask.GetLength(0) &&
                    y >= 0 && y < generatedMask.GetLength(1))
                {
                    return (generatedMask[x, y] + value) / 2;
                }
            }

            return value;
        }

        private float[,] GetNoiseMask(System.Random prng, float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float[,] mask = new float[width, height];

            float[,] biomeHeightMask = Noise.GenerateNoiseMap(prng, width, height, noiseSettings.noiseSettings);
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
                            mask[x, y] = GetFadeValue(heightMap[x, y], noiseSettings);
                        }
                    }
                }
            }

            return mask;
        }

        private float[,] GetBorderMask(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float[,] mask = new float[width, height];

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
                            mask[x, y] = GetFadeValue(heightMap[x, y], borderSettings);
                        }
                    }
                }
            }

            return mask;
        }

        private float[,] GetPicksMask(System.Random prng, float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            List<Vector2Int> picks = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (picksSettings.minFindPicksHeight < heightMap[x, y] &&
                        picksSettings.maxFindPicksHeight > heightMap[x, y])
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

            float[,] mask = new float[width, height];
            Debug.Log(selectedPicks.Count);
            foreach (Vector2Int pick in selectedPicks)
            {
                mask = SelectPicks(heightMap, mask, pick.x, pick.y, picksSettings.maxIterations);
            }

            return mask;
        }

        private float[,] SelectPicks(float[,] heightMap, float[,] mask, int x, int y, int maxIterCount)
        {
            maxIterCount--;
            mask[x, y] = GetFadeValue(heightMap[x, y], picksSettings);

            if (maxIterCount > 0 && 
                heightMap[x, y] > picksSettings.minWorldHeight && 
                heightMap[x, y] < picksSettings.maxWorldHeight)
            {
                for (int i = 0; i < crossPositions.Length; i++)
                {
                    int nX = x + crossPositions[i].x;
                    int nY = y + crossPositions[i].y;

                    if (nX >= 0 && nX < heightMap.GetLength(0) &&
                        nY >= 0 && nY < heightMap.GetLength(1))
                    {
                        if (mask[nX, nY] == 0)
                        {
                            mask = SelectPicks(heightMap, mask, nX, nY, maxIterCount);
                        }
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
                            mask[px, py] = GetFadeValue(heightMap[x, y], picksSettings);
                        }
                    }
                }
            }
            return mask;
        }

        private float[,] GetAbsoluteMask(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float[,] mask = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (absoluteSettings.minWorldHeight < heightMap[x, y] &&
                        absoluteSettings.maxWorldHeight > heightMap[x, y])
                    {
                        mask[x, y] = GetFadeValue(heightMap[x, y], absoluteSettings);
                    }
                }
            }

            return mask;
        }

        private float GetFadeValue(float value, MaskPatternSettings settings)
        {
            return (value - settings.minWorldHeight) / (settings.maxWorldHeight - settings.minWorldHeight);
        }

        public static float[,] AppendMasks(float[,] mainMask, float[,] secMask)
        {
            if (!EqualMasksSize(mainMask, secMask))
                return mainMask;

            int width = mainMask.GetLength(0);
            int height = mainMask.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mainMask[x, y] = (mainMask[x, y] + secMask[x, y]) / 2;
                }
            }

            return mainMask;
        }

        public static float[,] SubtractMasks(float[,] mainMask, float[,] secMask)
        {
            if (!EqualMasksSize(mainMask, secMask))
                return mainMask;

            int width = mainMask.GetLength(0);
            int height = mainMask.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mainMask[x, y] = Mathf.Abs(mainMask[x, y] - secMask[x, y]) / 2;
                }
            }

            return mainMask;
        }

        public static bool EqualMasksSize(float[,] firstMask, float[,] secondMask)
        {
            return (firstMask.GetLength(0) == secondMask.GetLength(0) &&
                    firstMask.GetLength(1) == secondMask.GetLength(1));
        }
    }

    public enum HeightMaskPatternType
    {
        Noise,
        Border,
        Picks,
        Absolute
    }

    public class MaskPatternSettings
    {
        [Range(-1, 1)] public float minWorldHeight = 0;
        [Range(-1, 1)] public float maxWorldHeight = 1;
    }

    [System.Serializable]
    public class NoiseMaskPatternSettings : MaskPatternSettings
    {
        [Range(0, 1)] public float minMaskHeight = 0;
        [Range(0, 1)] public float maxMaskHeight = 1;

        public NoiseSettings noiseSettings;
    }

    [System.Serializable]
    public class BorderMaskPatternSettings : MaskPatternSettings
    {
        [Range(0, 1)] public float xDistFromBorder = 0.25f;
        [Range(0, 1)] public float yDistFromBorder = 0.25f;
    }

    [System.Serializable]
    public class PicksMaskPatternSettings : MaskPatternSettings
    {
        public int brushSize = 2;
        public int maxIterations = 5000;
        [Range(0, 1)] public float detectPicksCount = 0.5f;

        [Range(0, 1)] public float minFindPicksHeight = 0.9f;
        [Range(0, 1)] public float maxFindPicksHeight = 1;
    }

    [System.Serializable]
    public class AbsoluteMaskPatternSettings : MaskPatternSettings
    {

    }
}
