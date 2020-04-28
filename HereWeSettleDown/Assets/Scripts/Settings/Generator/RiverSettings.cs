using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/RiverSettings")]
    public class RiverSettings : SettingsObject
    {
        public int MinRiverCount = 7;
        public int MaxRiverCount = 14;
    }
}
