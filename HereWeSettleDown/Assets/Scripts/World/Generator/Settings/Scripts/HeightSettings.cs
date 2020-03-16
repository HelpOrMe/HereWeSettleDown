using UnityEngine;
using Settings;

namespace World.Generator
{
    [CreateAssetMenu(menuName = "Settings/HeightSettings")]
    public class HeightSettings : SettingsObject
    {
        public int heightOffset = 3;
        public int heightSmoothIterations = 2;
        public float heightSmoothCoef = 0.4f;
    }
}
