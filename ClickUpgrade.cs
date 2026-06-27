using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hardGame
{
    internal class ClickUpgrade : AbstractUpgrade
    {
        public double ClickMultiplier { get; set; }
        public ClickUpgrade(string name, long baseCost, double multiplierPerLevel) : base(name, baseCost, multiplierPerLevel)
        {
            ClickMultiplier = 1;
        }

        public override void Upgrade()
        {
            Cost = (long)Math.Floor(BASE_COST * Math.Pow(1.25, Level)) + 1;
            //System.Diagnostics.Debug.WriteLine($"Upgraded {Name} to level {Level + 1}. New cost: {Cost}");
            Level++;
            this.ClickMultiplier += (int)MultiplierPerLevel;
        }
    }
}
