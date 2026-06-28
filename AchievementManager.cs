using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace hardGame
{
    internal class AchievementManager
    {
        private readonly HashSet<LossMilestone> _lossMilestones =
        [
            new("10 Losses", "You hit 10 Lifetime Losses!", 10),
            new("50 Losses", "You hit 50 Lifetime Losses!", 50),
            new("500 Losses", "You hit 500 Lifetime Losses!", 500),
            new("1000 Losses", "You hit 1000 Lifetime Losses!", 1000)
        ];

        public Game game;

        public HashSet<string> Milestones { get; } = new HashSet<string>();
        public HashSet<double> LossMilestones = new HashSet<double>();
        public AchievementManager(Game game)
        {
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
        }

        public bool CheckLossMilestone(double milestone)
        {
            return LossMilestones.Contains(milestone);
        }

        public List<LossMilestone> CheckLifetimeLosses(double lifetimeLosses)
        {
            var newlyUnlocked = new List<LossMilestone>();

            foreach (var milestone in _lossMilestones)
            {
                // If they meet the threshold and haven't unlocked it yet
                if (lifetimeLosses >= milestone.Value && Milestones.Add(milestone.Key))
                {
                    LossMilestones.Add(milestone.Value);

                    newlyUnlocked.Add(milestone);
                    Debug.WriteLine($"Added milestone {milestone.Key}");
                }
            }

            return newlyUnlocked;
        }
    }
}
