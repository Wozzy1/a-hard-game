using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hardGame
{
    internal class PassiveUpgrade : AbstractUpgrade
    {
        public double SecondsPerClick { get; set; }
        public double AccumulatedTime { get; set; } = 0;
        public PassiveUpgrade(string name, long baseCost, double multiplierPerLevel, double secondsPerClick) : base(name, baseCost, multiplierPerLevel)
        {
            this.SecondsPerClick = secondsPerClick;
        }

        public override void Upgrade()
        {
            Cost = (long)Math.Floor(BASE_COST * Math.Pow(1.25, Level)) + 1;
        }
        /// <summary>
        /// Tracks time since last passive click. For reach PassiveUpgrade, this dictacts if a click should happen in this Update.
        /// </summary>
        /// <param name="elapsedTimeMS"></param>
        /// <returns> Returns true if a click event should be triggered, otherwise false.</returns>
        public bool Update(double elapsedTimeMS)
        {
            AccumulatedTime += elapsedTimeMS;

            // Convert SecondsPerClick to milliseconds for comparison since in the future some might be < 1 sec
            if (AccumulatedTime >= SecondsPerClick * 1000)
            {

                // Reset the accumulated time
                AccumulatedTime = 0;
                return true;
            }
            return false;
        }
    }
}
