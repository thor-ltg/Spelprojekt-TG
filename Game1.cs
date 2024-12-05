using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Spel_Projekt_Thor_Grimes
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D basic;
        Texture2D MMButton;
        List<Rectangle> Squares = new List<Rectangle>();
        List<Rectangle> Walls = new List<Rectangle>();
        List<Vector2> Velocity = new List<Vector2>();
        bool MainMenu = true;
        SpriteFont font;
        MouseState mouse;
        MouseState oldmouse;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basic = Content.Load<Texture2D>("basic");
            font = Content.Load<SpriteFont>("Font");
            MMButton = Content.Load<Texture2D>("button_rectangle_border");
            mouse = Mouse.GetState();
            oldmouse = Mouse.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            oldmouse = mouse;
            mouse = Mouse.GetState();
            if (MainMenu)
            {
                MMUpdate();
            }
            else
            {
                GameUpdate();
            }
            // TODO: Add your update logic here
            void MMUpdate()
            {
                if (true)
                {

                }
            }
            void GameUpdate()
            {

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (MainMenu)
            {
                spriteBatch.DrawString(font, "Placeholder", new Vector2(325, 100), Color.White);
                spriteBatch.Draw(MMButton, new Vector2(300, 200), Color.White);
                spriteBatch.DrawString(font, "PLAY!", new Vector2(365, 225), Color.DeepSkyBlue);
            }
            else
            {

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
