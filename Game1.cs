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
        Texture2D Button;
        SpriteFont MMFont;
        SpriteFont Font;
        List<Rectangle> Squares = new List<Rectangle>();
        List<Rectangle> Walls = new List<Rectangle>();
        List<Vector2> Velocity = new List<Vector2>();
        int gamestate = 0; // 0 = main menu, 1 = level selector, 2 = in game, 3 = level creator (if i have time)
        bool debug = false;
        bool rel = false;
        Vector2 mousepos;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle rectangle;
        MouseState mouse;
        MouseState oldmouse;
        float globalhoverdarken = 1; // global used for sprites, local used for rectangles
        List<float> localhoverdarken = new List<float>();
        List<Rectangle> LSRectangle = new List<Rectangle>();
        Dictionary<string, Vector2> LSText = new Dictionary<string, Vector2>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            LSRectangle.Add(new Rectangle(25, 400, 90, 35));
            LSText.Add("Back", new Vector2(55, 412));
            localhoverdarken.Add(1);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basic = Content.Load<Texture2D>("basic");
            MMFont = Content.Load<SpriteFont>("Font");
            Font = Content.Load<SpriteFont>("smallFont");
            Button = Content.Load<Texture2D>("button_rectangle_border");
            mouse = Mouse.GetState();
            oldmouse = Mouse.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            oldmouse = mouse;
            mouse = Mouse.GetState();
            if (gamestate == 0)
            {
                MMUpdate();
            }
            else if (gamestate == 1)
            {
                LevelSelectorUpdate();
            }
            else if (gamestate == 2)
            {
                GameUpdate();
            }
            if (debug) // debug mode allows you to create rectangles and get their information so it can be imported into the game
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
            void MMUpdate()
            {
                if (new Rectangle(300, 200, 190, 65).Contains(mouse.Position)) // make new rectangle which is the hitbox of the play button, if mouse is within the rectangle then darken the play button
                {
                    globalhoverdarken = 0.8f;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        gamestate++;
                    }
                }
                else // if it doesn't contain then set back to 1 (full brightness)
                {
                    globalhoverdarken = 1;
                }
            }
            void LevelSelectorUpdate()
            {
                for (int i = 0; i < LSRectangle.Count(); i++)
                {
                    if (LSRectangle[i].Contains(mouse.Position))
                    {
                        localhoverdarken[i] = 0.8f;
                        if (mouse.LeftButton == ButtonState.Pressed )
                        {
                            var text = LSText.ElementAt(i).Key;
                            if (text.Contains("B"))
                            {
                                gamestate--;
                            }
                            else if (text.Contains("1"))
                            {

                            }
                        }
                    }
                    else
                    {
                        localhoverdarken[i] = 1;
                    }
                }
            }
            void GameUpdate()
            {

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue*0.5F);

            spriteBatch.Begin();
            if (gamestate == 0)
            {
                spriteBatch.DrawString(MMFont, "Placeholder", new Vector2(325, 100), Color.White);
                spriteBatch.Draw(Button, new Vector2(300, 200), Color.White * globalhoverdarken);
                spriteBatch.DrawString(MMFont, "PLAY!", new Vector2(365, 225), Color.DeepSkyBlue * globalhoverdarken);
            }
            else if (gamestate == 1)
            {
                for (int i = 0; i < LSRectangle.Count(); i++)
                {
                    var element = LSText.ElementAt(i);
                    spriteBatch.Draw(Button, LSRectangle[i], Color.White * localhoverdarken[i]);
                    spriteBatch.DrawString(Font, element.Key, element.Value, Color.DeepSkyBlue * localhoverdarken[i]);
                }
            }
            foreach (var item in rectangles)
            {
                spriteBatch.Draw(basic, item, Color.White);
            }
            if (rectangles.Count() != 0)
            {
                spriteBatch.DrawString(MMFont, gamestate.ToString(), new Vector2(400, 10), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
