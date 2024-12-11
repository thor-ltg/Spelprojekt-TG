using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


namespace Spel_Projekt_Thor_Grimes
{
    public class Game1 : Game
    {
        SoundEffect hit;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D basic;
        Texture2D Button;
        SpriteFont MMFont;
        SpriteFont Font;
        List<Rectangle> Walls = new List<Rectangle>();
        int gamestate = 0; // 0 = main menu, 1 = level selector, 2 = in game, 3 = level creator (if i have time)
        bool debug = false;
        bool rel = false;
        bool m = true;
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
        Dictionary<int, Color> playercolor = new Dictionary<int, Color>(); // int = playernr, color = color
        Dictionary<int, Rectangle> win = new Dictionary<int, Rectangle>(); // int = level rec = pos
        Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>> moving = new Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>>(); // first int = levelnr, rectangle = startpos 2nd int = length units/frame vector2 = end pos
        List<Vector2> start = new List<Vector2>();
        Vector2 pan;
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
            playercolor[0] = Color.Red;
            playercolor[1] = Color.Blue;
            playercolor[2] = Color.Yellow;
            playercolor[3] = Color.Green;
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
            Dictionary<Rectangle, Dictionary<int, Vector2>> m = new Dictionary<Rectangle, Dictionary<int, Vector2>>();
            Dictionary<int, Vector2> t = new Dictionary<int, Vector2>();
            l.Add(new Rectangle(0, 0, 10, 430));
            l.Add(new Rectangle(0, 0, 500, 10));
            l.Add(new Rectangle(500, 0, 10, 440));
            l.Add(new Rectangle(0, 430, 500, 10));
            l.Add(new Rectangle(50, 300, 5, 130));
            l.Add(new Rectangle(150, 300, 5, 130));
            l.Add(new Rectangle(250, 300, 5, 130));
            l.Add(new Rectangle(350, 300, 5, 130));
            l.Add(new Rectangle(450, 300, 5, 130));
            l.Add(new Rectangle(0, 300, 50, 5));
            l.Add(new Rectangle(450, 300, 50, 5));
            l.Add(new Rectangle(215, 0, 5, 50));
            l.Add(new Rectangle(285, 0, 5, 50));
            levellistrectangles.Add(1, l);
            win[1] = new Rectangle(215, 0, 70, 50);
            t[1] = new Vector2(200, 100);
            m[new Rectangle(100, 200, 25, 25)] = t;
            moving[1] = m;
            start.Add(new Vector2(0, 50));
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
            hit = Content.Load<SoundEffect>("hit"); // || https://pixabay.com/sound-effects/plastic-hit-3-34297/ by freesound_community, sound was cropped to 0.25 seconds
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
            void LevelSelectorUpdate() // || LEVEL SELECTION METHOD
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
            void GameUpdate() // || GAME UPDATE METHOD
            {
                for (int i = 0; i < 4; i++) // || VELOCITY
                {
                    player[i].X += (int)velo[i].X;
                    player[i].Y += (int)velo[i].Y;
                }
                for (int i = 0; i < moving[level].Count(); i++) // || MOVING PART LOGIC
                {
                    var elements = moving[level].ElementAt(i);
                    var intv2a = elements.Value;
                    var intv2 = intv2a.ElementAt(0);

                    Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>> temp = new Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>>();
                    Dictionary<Rectangle, Dictionary<int, Vector2>> temp2 = new Dictionary<Rectangle, Dictionary<int, Vector2>>();
                    Dictionary<int, Vector2> temp3 = new Dictionary<int, Vector2>();
                    temp3[intv2.Key] = intv2.Value;
                    Rectangle r;
                    if (m)
                    {
                        r = new Rectangle(elements.Key.X + intv2.Key, elements.Key.Y, elements.Key.Width, elements.Key.Height);
                    }
                    else
                    {
                        r = new Rectangle(elements.Key.X - intv2.Key, elements.Key.Y, elements.Key.Width, elements.Key.Height);
                    }
                    temp2[r] = temp3;
                    temp[level] = temp2;
                    if (r.X > intv2.Value.X || r.X < start[level - 1].X)
                    {
                        m = !m;
                    }
                    moving = temp;
                }
                for (int i = 0; i < 4; i++) // || COLLISION DETECTION
                {
                    Rectangle playerax = new Rectangle((int)(player[i].X + velo[i].X), player[i].Y, player[i].Width, player[i].Height); // player after x
                    Rectangle playeray = new Rectangle(player[i].X, (int)(player[i].Y + velo[i].Y), player[i].Width, player[i].Height); // player after y
                    Rectangle playerav = new Rectangle((int)(player[i].X + velo[i].X), (int)(player[i].Y + velo[i].Y), player[i].Width, player[i].Height); // player after velocity
                    List<Rectangle> currec = levellistrectangles[level];
                    for (int j = 0; j < currec.Count(); j++)
                    {
                        if (playerav.Intersects(currec[j]))
                        {
                            if (playerax.Intersects(currec[j]))
                            {
                                velo[i].X *= -1;
                                hit.Play();
                            }
                            else if (playeray.Intersects(currec[j]))
                            {
                                velo[i].Y *= -1;
                                hit.Play();
                            }
                            else
                            {
                                velo[i] *= -1;
                                hit.Play();
                            }
                            break;
                        }
                    }
                    for (int j = 0; j < 4; j++) // || PLAYER ON PLAYER COLLISION
                    {
                        Rectangle player2av = new Rectangle((int)(player[j].X + velo[j].X), (int)(player[j].Y + velo[j].Y), player[j].Width, player[j].Height);
                        if (i == j)
                        {
                            continue;
                        }
                        if (playerav.Intersects(player2av))
                        {
                            if (playerax.Intersects(player2av))
                            {
                                velo[i].X *= -1;
                                velo[j].X *= -1;
                                hit.Play();
                                continue;
                            }
                            if (playeray.Intersects(player2av))
                            {
                                velo[i].Y *= -1;
                                velo[j].Y *= -1;
                                hit.Play();
                                continue;
                            }
                            if (!(playerax.Intersects(player2av)) &! playeray.Intersects(player2av))
                            {
                                velo[i] *= -1;
                                hit.Play();
                            }
                        }
                    }
                    for (int j = 0; j < moving[level].Count; j++) // || PLAYER ON MOVING COLLISION
                    {
                        var elements = moving[level].ElementAt(j);
                        if (playerav.Intersects(elements.Key))
                        {
                            if (playerax.Intersects(elements.Key))
                            {
                                velo[i].X *= -1;
                                hit.Play();
                            }
                            else if (playeray.Intersects(elements.Key))
                            {
                                velo[i].Y *= -1;
                                hit.Play();
                            }
                            else
                            {
                                velo[i] *= -1;
                                hit.Play();
                            }
                        }
                    }
                    if (win[level].Contains(player[i]))
                    {
                        gamestate = 1;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    pan.Y++;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    pan.Y--;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    pan.X--;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    pan.X++;
                }
            }
            void InitializeLevel() // || INITALIZE LEVEL METHOD
            {
                for (int i = 0; i < startingpos[level].Count(); i++) 
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
                Rectangle pwin = new Rectangle((int)(win[level].X + pan.X), (int)(win[level].Y+pan.Y), win[level].Width, win[level].Height);
                spriteBatch.Draw(basic, pwin, Color.Gold);
                for (int i = 0; i < moving[level].Count(); i++)
                {
                    var elements = moving[level].ElementAt(i);
                    Rectangle pmoving = new Rectangle((int)(elements.Key.X + pan.X), (int)(elements.Key.Y + pan.Y), elements.Key.Width, elements.Key.Height);
                    spriteBatch.Draw(basic, pmoving, Color.White);
                }
                for (int i = 0; i < 4; i++)
                {
                    Rectangle pplayer = new Rectangle((int)(player[i].X + pan.X), (int)(player[i].Y + pan.Y), player[i].Width, player[i].Height);
                    spriteBatch.Draw(basic, pplayer, playercolor[i]);
                }
                List<Rectangle> currec = levellistrectangles[level];
                for (int i = 0; i < currec.Count(); i++)
                {
                    Rectangle pcurrec = new Rectangle((int)(currec[i].X + pan.X), (int)(currec[i].Y + pan.Y), currec[i].Width, currec[i].Height);
                    spriteBatch.Draw(basic, pcurrec, Color.White);
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
