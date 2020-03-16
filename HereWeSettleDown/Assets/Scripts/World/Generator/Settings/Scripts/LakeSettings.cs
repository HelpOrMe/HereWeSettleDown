using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/LakeSettings")]
    public class LakeSettings : SettingsObject
    {
        public int MinLakeCount = 7;
        public int MaxLakeCount = 14;
    }
}
