﻿using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/HeightSettings")]
    public class HeightSettings : SettingsObject
    {
        public float heightValue = 10;
        public int heightOffset = 3;
        public int heightSmoothIterations = 2;
        public float heightSmoothCoef = 0.4f;
        public AnimationCurve heightCurve;
    }
}
