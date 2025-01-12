using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_MongoDB.MongoDB.Entities
{
    public class Rat
    {
        public Guid Id { get; set; }
        public int Health { get; set; }
        public string? Name { get; set; }
        public int AttackPower { get; set; }
        public int DefenseStrength { get; set; }
        public string? CurrentLocation { get; set; }
    }
}
