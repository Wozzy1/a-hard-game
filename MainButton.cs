using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hardGame
{

    public class MainButton
    {
        public Texture2D texture;
        public Vector2 position;
        public int time;
        public Random random;
        public long lifetimeLosses = 0;
        public long losses = 0;
        public long lifetimeWins = 0;
        public long wins = 0;
        public readonly List<AbstractUpgrade> upgrades;

        // default winning probability is 0.0000000000000000000000000001079797
        private double WIN_RATE = 0.0000000000000000000000000001079797; // 1 in 9,264,100,000,000,000,000,000,000,000

        public MouseState previousMouseState;
        public MainButton(Texture2D texture, Vector2 position, List<AbstractUpgrade> upgrades)
        {
            this.texture = texture;
            this.position = position;
            this.random = new Random();
            this.upgrades = upgrades;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState != mouseState)
            {
                Rectangle buttonRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
                if (buttonRectangle.Contains(mouseState.Position))
                {
                    System.Diagnostics.Debug.WriteLine("Button clicked at " + time);

                    SimulatePlays(((ClickUpgrade)upgrades[0]).clickMultiplier, WIN_RATE);

                }
            }
            time++;
            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Using central limit theorem to simulate wins and losses. 
        /// Didn't think high school stats would be useful lol
        /// </summary>
        /// <param name="totalPlays"></param>
        /// <param name="wr"></param>
        private void SimulatePlays(double totalPlays, double wr)
        {
            double mean = totalPlays * wr;
            double stdDev = Math.Sqrt(totalPlays * wr * (1 - wr));

            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            double simulatedWins = mean + stdDev * randStdNormal;

            long finalWins = (long)Math.Max(0, Math.Min(totalPlays, Math.Round(simulatedWins)));
            long finalLosses = (long)(totalPlays - finalWins);

            wins += finalWins;
            lifetimeWins += finalWins;
            losses += finalLosses;
            lifetimeLosses += finalLosses;
        }
    }
}
