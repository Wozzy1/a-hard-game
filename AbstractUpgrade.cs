using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hardGame
{
    public abstract class AbstractUpgrade
    {
        public string Name { get; init; }
        public double BASE_COST { get; init; }
        public long Cost { get; set; }
        public double Level { get; set; }
        public double MultiplierPerLevel { get; init; }

        public AbstractUpgrade(string name, long baseCost, double multiplierPerLevel)
        {
            Name = name;
            BASE_COST = baseCost;
            Cost = baseCost;
            Level = 0;
            MultiplierPerLevel = multiplierPerLevel;
        }

        public abstract void Upgrade();
    }
}
