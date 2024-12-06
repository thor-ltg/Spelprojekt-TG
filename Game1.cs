using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
        bool debug = true;
        bool rel = false;
        Vector2 mousepos;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle rectangle;
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
                GameUpdate();
            }
            else
            {
                MMUpdate();
            }
            if (debug)
            {
                if (mouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed)
                {
                    if (rel)
                    {
                        if (mouse.Position.X < mousepos.X)
                        {
                            if (mouse.Position.Y < mousepos.Y)
                            {
                                rectangle = new Rectangle(mouse.Position.X, mouse.Position.Y, (int)Math.Abs(mouse.Position.X - mousepos.X), (int)Math.Abs(mouse.Position.Y - mousepos.Y));
                            }
                            else
                            {
                                rectangle = new Rectangle(mouse.Position.X, (int)mousepos.Y, (int)Math.Abs(mouse.Position.X - mousepos.X), (int)Math.Abs(mouse.Position.Y - mousepos.Y));
                            }
                        }
                        else
                        {
                            if (mouse.Position.Y < mousepos.Y)
                            {
                                rectangle = new Rectangle((int)mousepos.X, mouse.Position.Y, (int)Math.Abs(mouse.Position.X - mousepos.X), (int)Math.Abs(mouse.Position.Y - mousepos.Y));
                            }
                            else
                            {
                                rectangle = new Rectangle((int)mousepos.X, (int)mousepos.Y, (int)Math.Abs(mouse.Position.X - mousepos.X), (int)Math.Abs(mouse.Position.Y - mousepos.Y));
                            }
                        }
                        rectangles.Add(rectangle);
                    }
                    else
                    {
                        mousepos = mouse.Position.ToVector2();
                    }
                    rel = !rel;
                }
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
            foreach (var item in rectangles)
            {
                spriteBatch.Draw(basic, item, Color.White);
            }
            if (rectangles.Count() != 0)
            {
                spriteBatch.DrawString(font, rectangles.Last().ToString(), new Vector2(400, 10), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
