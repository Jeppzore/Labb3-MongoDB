using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_MongoDB.MongoDB
{
    public class Players
    {
        public Guid Id { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string ?Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int VisionRange { get; set; }
        public int AttackPower { get; set; }
        public int DefenseStrength { get; set; }
        public string ?CurrentLocation { get; set; }
        public DateTime LastSaveTime { get; set; }

    }
}
