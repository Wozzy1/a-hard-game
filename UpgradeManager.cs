using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace hardGame
{
    internal class UpgradeManager(Game game)
    {
        private Game game { get; init; } = game;
        public List<AbstractUpgrade> AllUpgrades = new List<AbstractUpgrade>();
        public List<AbstractUpgrade> UnlockedUpgrades = new List<AbstractUpgrade>();
        //public List<ClickUpgrade> ClickUpgrades = [];
        //public List<PassiveUpgrade> PassiveUpgrades = [];

        /// <summary>
        /// Initializes the list of all available upgrades in the game.
        /// Future work: Load upgrades from a JSON save file or database to save/load user progress.
        /// </summary>
        public void InitializeUpgrades()
        {
            this.AllUpgrades.Add(new ClickUpgrade(
                name: "Novice Player",
                baseCost: 10,
                multiplierPerLevel: 1.0
            ));
            //PassiveUpgrade pup1 = new PassiveUpgrade({Name = "Obessed Player", 50, 1.0, 10.0 });
            this.AllUpgrades.Add(new PassiveUpgrade(
                name: "Obessed Player",
                baseCost: 50,
                multiplierPerLevel: 1.0,
                secondsPerClick: 10.0
            ));

            this.UnlockedUpgrades.Add(this.AllUpgrades[0]);
        }

        public void AddPassiveUpgrade(PassiveUpgrade upgrade)
        {
            UnlockedUpgrades.Add(upgrade);
        }

        public void AddClickUpgrade(ClickUpgrade upgrade)
        {
            UnlockedUpgrades.Add(upgrade);
        }
    }
}
