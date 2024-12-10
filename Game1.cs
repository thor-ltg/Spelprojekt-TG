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
        List<Rectangle> Walls = new List<Rectangle>();
        int gamestate = 0; // 0 = main menu, 1 = level selector, 2 = in game, 3 = level creator (if i have time)
        bool debug = true;
        bool rel = false;
        Vector2 mousepos;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle rectangle;
        MouseState mouse;
        MouseState oldmouse;
        Rectangle[] player = new Rectangle[4];
        Vector2[] velo = new Vector2[4];
        float globalhoverdarken = 1; // global used for sprites, local used for rectangles
        List<float> localhoverdarken = new List<float>();
        List<Rectangle> LSRectangle = new List<Rectangle>();
        Dictionary<string, Vector2> LSText = new Dictionary<string, Vector2>();
        int level;
        Dictionary<int, Dictionary<int, Vector2>> startingpos = new Dictionary<int, Dictionary<int, Vector2>>(); // first int = level, 2nd int = player nr vector2 is pos
        Dictionary<int, List<Rectangle>> levellistrectangles = new Dictionary<int, List<Rectangle>>(); // int = level, rectangle = rectangle

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            for (int i = 0; i < 2; i++)
            {
                localhoverdarken.Add(1);
            }
            LSRectangle.Add(new Rectangle(25, 400, 90, 35));
            LSText.Add("Back", new Vector2(55, 412));
            LSRectangle.Add(new Rectangle(25, 100, 100, 35));
            LSText.Add("Level 1", new Vector2(52, 112));
            Dictionary<int, Vector2> startpos = new Dictionary<int, Vector2>();
            startpos.Add(0, new Vector2(100, 400));
            startpos.Add(1, new Vector2(200, 400));
            startpos.Add(2, new Vector2(300, 400));
            startpos.Add(3, new Vector2(400, 400));
            startingpos.Add(1, startpos);
            List<Rectangle> l = new List<Rectangle>();
            Rectangle r = new Rectangle(0, 0, 100, 500);
            l.Add(r);
            r = new Rectangle(0, 0, 800, 100);
            l.Add(r);
            levellistrectangles.Add(1, l);

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
                            if (text.Contains("Back"))
                            {
                                gamestate--;
                            }
                            else if (text.Contains("Level"))
                            {
                                string[] strings = text.Split(' ');
                                level = int.Parse(strings[1]);
                                InitializeLevel();
                                gamestate++;
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
                for (int i = 0; i < 4; i++)
                {
                    player[i].X += (int)velo[i].X;
                    player[i].Y += (int)velo[i].Y;
                }
                for (int i = 0; i < 4; i++)
                {
                    List<Rectangle> currec = levellistrectangles[level];
                    for (int j = 0; j < currec.Count(); j++)
                    {
                        if (player[i].Intersects(currec[j]))
                        {
                            Rectangle playerbx = new Rectangle((int)(player[i].X + velo[i].X), player[i].Y, player[i].Width, player[i].Height); // player before x
                            Rectangle playerby = new Rectangle(player[i].X, (int)(player[i].Y + velo[i].Y), player[i].Width, player[i].Height); // player before y
                            if (playerby.Intersects(currec[j]))
                            {
                                velo[i].Y *= -1;
                            }
                            else if (playerbx.Intersects(currec[j]))
                            {
                                velo[i].X *= -1;
                            }
                            else
                            {
                                velo[i] *= -1;
                            }
                        }
                    }
                }
            }
            void InitializeLevel()
            {
                for (int i = 0; i < startingpos[level].Count(); i++) // TODO fix level int so it represents level, didn't work when i had [level]
                {
                    var pos = startingpos[level].ElementAt(i).Value;
                    Rectangle playerrec = new Rectangle((int)pos.X, (int)pos.Y, 15, 15);
                    player[i] = playerrec;
                }
                for (int i = 0; i < 4; i++)
                {
                    Random rng = new Random();
                    if (rng.Next(2) == 1)
                    {
                        velo[i].X = 1;
                    }
                    else
                    {
                        velo[i].X = -1;
                    }
                    if (rng.Next(2) == 1)
                    {
                        velo[i].Y = 1;
                    }
                    else
                    {
                        velo[i].Y = -1;
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue*0.5F);

            spriteBatch.Begin();
            if (gamestate == 0)
            {
                spriteBatch.DrawString(MMFont, "Placeholder", new Vector2(330, 95), Color.White);
                spriteBatch.Draw(Button, new Vector2(300, 200), Color.White * globalhoverdarken);
                spriteBatch.DrawString(MMFont, "PLAY!", new Vector2(370, 220), Color.DeepSkyBlue * globalhoverdarken);
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
            else if (gamestate == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(basic, player[i], Color.White);
                }
                List<Rectangle> currec = levellistrectangles[level];
                for (int i = 0; i < currec.Count(); i++)
                {
                    spriteBatch.Draw(basic, currec[i], Color.White);
                }
            }
            foreach (var item in rectangles)
            {
                spriteBatch.Draw(basic, item, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
