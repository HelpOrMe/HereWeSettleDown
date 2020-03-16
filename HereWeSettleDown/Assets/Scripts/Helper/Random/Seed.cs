using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Helper.Threading;

namespace Helper.Random
{
    public static class Seed
    {
        public static int seed
        {
            get => _seed != null ? (int)_seed : SetPRSeed();
            set => UpdateSeed(value);
        }
        private static int? _seed;

        private static Dictionary<Thread, System.Random> subThreadPrng = new Dictionary<Thread, System.Random>();
        public static System.Random prng
        {
            get
            {
                // Give another prng to sub threads
                if (!MainThreadInvoker.CheckForMainThread())
                {
                    if (!subThreadPrng.ContainsKey(Thread.CurrentThread))
                        subThreadPrng.Add(Thread.CurrentThread, new System.Random(seed));
                    return subThreadPrng[Thread.CurrentThread];
                }

                if (_prng == null)
                    SetPRSeed();
                return _prng;
            }
        }
        private static System.Random _prng;

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

        private static void UpdateSeed(int seed)
        {
            _prng = new System.Random(seed);
            _seed = seed;
        }

        private static int SetPRSeed()
        {
            UpdateSeed(new System.Random().Next(int.MinValue, int.MaxValue));
            return (int)_seed;
        }
    }
}
