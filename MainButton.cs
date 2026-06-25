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
        public int lossMultiplier;
        public int winMultiplier;
        public int time;
        public Random random;
        public int losses = 0;
        public int wins = 0;


        public MouseState previousMouseState;
        public MainButton(Texture2D texture, Vector2 position, int lossMultiplier, int winMultiplier)
        {
            this.texture = texture;
            this.position = position;
            this.lossMultiplier = lossMultiplier;
            this.winMultiplier = winMultiplier;
            this.random = new Random();
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

                    // default winning probability is 0.0000000000000000000000000001079797
                    if (random.Next() * random.Next() * random.Next() == 1)
                    {
                        wins += winMultiplier;
                    }
                    else
                    {
                        losses += lossMultiplier;
                    }
                }
            }
            time++;
            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
