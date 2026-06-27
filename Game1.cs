using Gum.Forms;
using Gum.Forms.Controls;
using Gum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Gum.Wireframe;
using Gum.GueDeriving;
using System.Diagnostics;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using System.Linq;

namespace hardGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MainButton mainButton;
        private const int WIDTH = 800;
        private const int HEIGHT = 600;

        //private Panel shopPanel;
        private ScrollViewer shopScrollViewer;
        private bool sKeyDown;
        private const int SHOP_PANEL_WIDTH = (int)(WIDTH * 0.4);
        private UpgradeManager um;
        private AchievementManager pm;

        private bool firstShopOpened = false;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            um = new UpgradeManager(this);
            pm = new AchievementManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            InitializeGum();
            InitializeUI();
            um.InitializeUpgrades();
            //InitializeUpgrades();
        }


        private void InitializeUI()
        {
            GumService.Default.Root.Children.Clear();
            CreateShopPanel();
        }

        private void InitializeUpgrades()
        {
            um.AddClickUpgrade( new ClickUpgrade(
                name: "Novice Player",
                baseCost: 10,
                multiplierPerLevel: 1.0
            ));
            //PassiveUpgrade pup1 = new PassiveUpgrade({Name = "Obessed Player", 50, 1.0, 10.0 });
            um.AddPassiveUpgrade( new PassiveUpgrade(
                name: "Obessed Player",
                baseCost: 50,
                multiplierPerLevel: 1.0,
                secondsPerClick: 10.0
            ));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D texture = Content.Load<Texture2D>("mainButton");
            mainButton = new MainButton(texture, new Vector2(WIDTH / 2 - texture.Width / 2, HEIGHT / 2 - texture.Height / 2), um);
            
            // TESTS
            //mainButton.losses += 2000000000000;
            //for (int i = 0; i < 100; i++)
            //{
            //    Upgrades[0].Upgrade();
            //}
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mainButton.Update(gameTime);
            GumService.Default.Update(gameTime);
            pm.Update(gameTime);
            base.Update(gameTime);

            // hide shop features for brand new players
            if (mainButton.lifetimeLosses < 10) return;
            //if (pm.CheckLossMilestone(10)) return;

            // when player reaches 50 losses, make the second upgrade available in the shop
            foreach (var milestone in pm.CheckLifetimeLosses(mainButton.lifetimeLosses))
            {
                if (milestone.Key == "50 Losses")
                {
                    um.AddPassiveUpgrade((PassiveUpgrade)um.AllUpgrades[1]);
                    Debug.WriteLine("Unlocked Passive Upgrade: Obessed Player");
                    RefreshShopRows();
                }
            }
            
            //if (mainButton.lifetimeLosses < 10) return;
            //if (mainButton.lifetimeLosses >= 50 && mainButton.losses > 50)
            //{
            //    um.AddPassiveUpgrade((PassiveUpgrade)um.AllUpgrades[1]);
            //}


            // Add toggling of the shop because what if i want to enjoy clicking the button forever
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                sKeyDown = true;
                firstShopOpened = true;
                RefreshShopRows();
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.S) && sKeyDown)
            { 
                shopScrollViewer.IsVisible = !shopScrollViewer.IsVisible;
                Debug.WriteLine($"Shop panel visibility: {shopScrollViewer.IsVisible}");
                Debug.WriteLine("Count of unlocked upgrades" + um.UnlockedUpgrades.Count);
                sKeyDown = false;
            }

            // Adjusting the position of the button based on if shop is open or not
            if (shopScrollViewer.IsVisible)
            {
                mainButton.position = new Vector2((((WIDTH + SHOP_PANEL_WIDTH) / 2) - mainButton.texture.Width / 2), HEIGHT / 2 - mainButton.texture.Height / 2);
            }
            else
            {
                mainButton.position = new Vector2(WIDTH / 2 - mainButton.texture.Width / 2, HEIGHT / 2 - mainButton.texture.Height / 2);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                mainButton.texture,
                mainButton.position,
                Color.White
            );

            _spriteBatch.DrawString(
                Content.Load<SpriteFont>("ComicSans"),
                $"Losses: {mainButton.losses} Wins: {mainButton.wins}",
                new Vector2(10, 10),
                Color.White
            );

            if (mainButton.lifetimeLosses == 10 && !firstShopOpened)
            {
                _spriteBatch.DrawString(
                    Content.Load<SpriteFont>("ComicSans"),
                    $"Congrats! Press S to open Sixer. \nHire new help to gamble more!!",
                    new Vector2(WIDTH / 4, HEIGHT * 0.25f),
                    Color.White
                );
            }

            _spriteBatch.End();

            GumService.Default.Draw();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Example init function from the docs
        /// </summary>
        protected void InitializeGum()
        {
            // Initialize the Gum service. The second parameter specifies
            // the version of the default visuals to use. V3 is the latest
            // version.
            GumService.Default.Initialize(this, DefaultVisualsVersion.V3);

            // Tell the Gum service which content manager to use. We will tell it to
            // use the global content manager from our Core.
            GumService.Default.ContentLoader.XnaContentManager = Content;

            // Register keyboard input for UI control.
            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

            // Register gamepad input for Ui control.
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

            // Customize the tab reverse UI navigation to also trigger when the keyboard
            // Up arrow key is pushed.
            FrameworkElement.TabReverseKeyCombos.Add(
               new KeyCombo() { PushedKey = (Gum.Forms.Input.Keys)Keys.Up });

            // Customize the tab UI navigation to also trigger when the keyboard
            // Down arrow key is pushed.
            FrameworkElement.TabKeyCombos.Add(
               new KeyCombo() { PushedKey = (Gum.Forms.Input.Keys)Keys.Down });

            // The assets created for the UI were done so at 1/4th the size to keep the size of the
            // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
            // the game's resolution then tell gum to zoom in by a factor of 4.
            GumService.Default.CanvasWidth = WIDTH;
            GumService.Default.CanvasHeight = HEIGHT;
            GumService.Default.Renderer.Camera.Zoom = 1f;
        }

        /// <summary>
        /// Creates a side panel that is rendered on the main game screen.
        /// Was not able to get gradients to work despite following docs to a T.
        /// </summary>
        /// <param name="upgrades"></param>
        private void CreateShopPanel()
        {
            shopScrollViewer = new ScrollViewer();
            shopScrollViewer.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            shopScrollViewer.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            shopScrollViewer.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            shopScrollViewer.Width = (int)(WIDTH * 0.4);
            shopScrollViewer.Height = (int)(HEIGHT * 0.8);
            shopScrollViewer.IsVisible = false;
            shopScrollViewer.AddToRoot();

            shopScrollViewer.InnerPanel.StackSpacing = 5; // 5px gap between your rows
            shopScrollViewer.MouseWheelScrollSpeed = 40;   // Adjust scroll sensitivity

            var background = new RectangleRuntime();
            background.Dock(Dock.Fill);
            background.FillColor = new Color(225, 173, 109);
            //background.UseGradient = true;
            background.IsFilled = true;
            //background.Color2 = Color.Black;
            background.StrokeWidth = 0;
            shopScrollViewer.AddChild(background);


            var title = new TextRuntime();
            title.Text = "Shop [S]";
            title.FontSize = 16;
            title.Dock(Dock.Top);
            title.HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            shopScrollViewer.AddChild(title);

            //int startingY = 40;
            //int rowSpacing = 60; // 55px height + 5px margin

            //for (int i = 0; i < um.AllUpgrades.Count; i++)
            //{
            //    var row = CreateLobbyRowItem(um.AllUpgrades[i]);
            //    if (um.UnlockedUpgrades.Contains(um.AllUpgrades[i]))
            //    {
            //        row.Y = startingY + (i * rowSpacing); // Stacks them vertically
            //        shopScrollViewer.AddChild(row);
            //    }
            //}
            RefreshShopRows();
        }
        public void RefreshShopRows()
        {
            // Clear out only the upgrade rows from the scroll viewer's InnerPanel
            // (If title/background are children of the main scrollViewer, they won't be deleted here)
            shopScrollViewer.InnerPanel.Children.Clear();

            foreach (var upgrade in um.AllUpgrades)
            {
                // Only generate and add rows for upgrades the player has unlocked
                if (um.UnlockedUpgrades.Contains(upgrade))
                {
                    var row = CreateLobbyRowItem(upgrade);

                    // Notice: No manual Y coordinate positioning is needed!
                    // Gum's ScrollViewer stacks everything automatically.
                    shopScrollViewer.AddChild(row);
                }
            }
        }

        /// <summary>
        /// graphic design is not my passion
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        private Panel CreateLobbyRowItem(AbstractUpgrade upgrade)
        {
            // 1. Main Container Row Panel
            var rowContainer = new Panel();
            rowContainer.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            rowContainer.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            rowContainer.Width = 290;  // Leave a safe 15px margin on both sides of a 320px panel
            rowContainer.Height = 55;

            // Position it safely below the title
            rowContainer.XUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
            rowContainer.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            rowContainer.X = 0; // Centers it perfectly within the shop panel
            //rowContainer.Y = 40; // Pushes it down below the "Shop" title text

            // Background shape
            var rowBackground = new RectangleRuntime();
            rowBackground.Dock(Dock.Fill);
            rowBackground.IsFilled = true;
            rowBackground.FillColor = Color.White;
            rowBackground.StrokeColor = Color.Black;
            rowBackground.StrokeWidth = 2;
            rowContainer.AddChild(rowBackground);

            // --- LEFT SIDE: Enlist Button ---
            var enlistButton = new Button();
            enlistButton.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            enlistButton.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            enlistButton.Width = 85;
            enlistButton.Height = 28;

            // Anchor to Left Center of the row
            enlistButton.XUnits = Gum.Converters.GeneralUnitType.PixelsFromSmall;
            enlistButton.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
            enlistButton.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;
            enlistButton.X = 12; // Margin from left edge
            enlistButton.Y = 0;  // Centered vertically

            enlistButton.Text = $"Hire ${(long)upgrade.Cost}";

            var btnBorder = new RectangleRuntime();
            btnBorder.Dock(Dock.Fill);
            btnBorder.IsFilled = false;
            btnBorder.StrokeColor = Color.Black;
            btnBorder.StrokeWidth = 2;
            enlistButton.AddChild(btnBorder);

            rowContainer.AddChild(enlistButton);

            // --- CENTER: Text Details Stack ---
            var detailsStack = new Panel();
            detailsStack.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            detailsStack.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            detailsStack.Width = 110;
            detailsStack.Height = 36;

            // Anchor to Middle Center of the row
            detailsStack.XUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
            detailsStack.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
            detailsStack.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            detailsStack.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;
            detailsStack.X = 0;
            detailsStack.Y = 0;

            var titleText = new TextRuntime();
            titleText.Text = upgrade.Name;
            //titleText.FontSize = 8;
            titleText.FontScale = 0.75f;
            titleText.Color = Color.Black;
            titleText.HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            //titleText.Dock(Dock.Top);
            titleText.Anchor(Anchor.Top);
            detailsStack.AddChild(titleText);

            var subText = new TextRuntime();
            subText.Text = $"{(int)upgrade.Level} playing";
            //subText.FontSize = 6;
            subText.FontScale = 0.6f;
            subText.Color = Color.DarkGray;
            subText.HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            subText.Anchor(Anchor.Center);
            detailsStack.AddChild(subText);

            rowContainer.AddChild(detailsStack);

            // --- RIGHT SIDE: Placeholder Image ---
            var imagePlaceholder = new RectangleRuntime();
            imagePlaceholder.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            imagePlaceholder.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;
            imagePlaceholder.Width = 45;
            imagePlaceholder.Height = 26;

            // Anchor to Right Center of the row
            imagePlaceholder.XUnits = Gum.Converters.GeneralUnitType.PixelsFromLarge;
            imagePlaceholder.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
            imagePlaceholder.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Right;
            imagePlaceholder.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;
            imagePlaceholder.X = -12; // Inward from right side
            imagePlaceholder.Y = 0;   // Centered vertically
            imagePlaceholder.IsFilled = true;
            imagePlaceholder.FillColor = new Color(180, 120, 80);

            enlistButton.Click += (sender, e) =>
            {
                if (mainButton.losses >= upgrade.Cost)
                {
                    mainButton.losses -= (int)upgrade.Cost;

                    // Execute the interface logic
                    upgrade.Upgrade();

                    // Increment your global multiplier or handle business logic
                    //mainButton.clickMultiplier += (int)upgrade.MultiplierPerLevel;

                    // Update UI Elements text immediately on click
                    enlistButton.Text = $"Hire: ${(int)upgrade.Cost}";
                    subText.Text = $"Level: {upgrade.Level}";

                    Debug.WriteLine($"{upgrade.Name} upgraded to Level {upgrade.Level}. New cost: {upgrade.Cost}. Remaining losses: {mainButton.losses}");
                }
                else
                {
                    Debug.WriteLine("Not enough losses to upgrade.");
                }
            };

            rowContainer.AddChild(imagePlaceholder);

            return rowContainer;
        }

    }
}
