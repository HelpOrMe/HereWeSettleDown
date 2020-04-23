namespace World.Generator
{
    public class RegionType
    {
        public readonly Region parent;

        public bool isWater { get; private set; }
        public bool isGround { get; private set; }
        public bool isCoastline { get; private set; }
        public bool isMountain { get; private set; }

        public int? DistIndexFromCoastline
        {
            get => distIndexFromCoastline;
            set
            {
                RegionsInfo.UpdateDistIndex((int)value);
                distIndexFromCoastline = value;
            }
        }
        private int? distIndexFromCoastline;

        public int? Moisture
        {
            get => moisture;
            set
            {
                RegionsInfo.UpdateMoistureIndex((int)value);
                moisture = value;
            }
        }
        private int? moisture;

        public string biomeType;

        public RegionType(Region region)
        {
            parent = region;
        }

        public void MarkAsWater()
        {
            isWater = true;
            isGround = false;
            DistIndexFromCoastline = -1;
        }

        public void MarkAsGround()
        {
            isGround = true;
            isWater = false;
        }

        public void MarkAsCoastline()
        {
            MarkAsWater();
            isCoastline = true;
            DistIndexFromCoastline = 0;
        }

        public void MarkAsMountain()
        {
            MarkAsGround();
            isMountain = true;
        }
    }
}
