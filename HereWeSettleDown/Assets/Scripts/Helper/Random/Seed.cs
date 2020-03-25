using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Helper.Random
{
    public static class Seed
    {
        public static int seed
        {
            get => _seed == null ? 0 : (int)_seed;
            set => _seed = value;
        }
        private static int? _seed;

        private static readonly Dictionary<int, System.Random> threadsPRNGs = new Dictionary<int, System.Random>();
        public static System.Random prng
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (!threadsPRNGs.ContainsKey(threadId))
                {
                    threadsPRNGs.Add(threadId, GetNewPrng());
                }

                return threadsPRNGs[threadId];
            }
        }

        public static float Random(int dec = 2)
        {
            int maxValue = (int)Mathf.Pow(10, dec);
            int minValue = -1 * maxValue;

            return prng.Next(minValue, maxValue) / (float)maxValue;
        }

        public static int RandomSign()
        {
            return prng.Next(0, 1) == 0 ? -1 : 1;
        }

        public static int Range(int min, int max)
        {
            return prng.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            string minStr = min.ToString();
            string maxStr = max.ToString();

            int dec = Mathf.Max(
                minStr.Length - minStr.IndexOf('.') - 1,
                maxStr.Length - maxStr.IndexOf('.') - 1);

            int decValue = (int)Mathf.Pow(10, dec);

            return prng.Next((int)(min * decValue), (int)(max * decValue)) / (float)decValue;
        }

        public static System.Random GetNewPrng()
        {
            return new System.Random(seed);
        }
    }
}
