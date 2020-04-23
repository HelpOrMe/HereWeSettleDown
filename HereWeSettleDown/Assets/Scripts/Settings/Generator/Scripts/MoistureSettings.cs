using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/MoistureSettings")]
    public class MoistureSettings : SettingsObject
    {
        public bool SetMoistureFromOcean = true;
        public bool SetMoistureFromLakes = true;
        public bool SetMoistureFromRivers = true;
    }
}
