using System;
using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/BiomesSettings")]
    public class BiomesSettings : SettingsObject
    {
        public string[] biomesTypes = new string[1];
        public int moistureLevels = 10;
        public int heightLevels = 10;
        
        public SelectedBiomesArray selectedBiomes = new SelectedBiomesArray(10, 10);

        [Serializable]
        public class SelectedBiomesArray
        {
            [SerializeField] public int[] array;
            [SerializeField] public int width;
            [SerializeField] public int height;

            public SelectedBiomesArray(int width, int height)
            {
                if (width * height < 0)
                    return;

                this.width = width;
                this.height = height;
                array = new int[width * height];
            }

            public int this[int x, int y]
            {
                get => array[width * y + x];
                set => array[width * y + x] = value;
            }

            public static implicit operator int[,](SelectedBiomesArray array)
            {
                int[,] array2d = new int[array.width, array.height];
                for (int x = 0; x < array.width; x++)
                {
                    for (int y = 0; y < array.height; y++)
                    {
                        array2d[x, y] = array[x, y];
                    }
                }
                return array2d;
            }
        }
    }
}
