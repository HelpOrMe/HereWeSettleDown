using UnityEngine;
using Settings;

namespace World.Generator
{
    [CreateAssetMenu(menuName = "Settings/LakeSettings")]
    public class LakeSettings : SettingsObject
    {
        public int MinLakeCount = 7;
        public int MaxLakeCount = 14;
    }
}
