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
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
        Dictionary<int, List<bool>> m = new Dictionary<int, List<bool>>(); 
        int time = 0;
        MouseState mouse;
        MouseState oldmouse;
        Vector2 oldmousepos;
        Rectangle[] player = new Rectangle[4];
        Vector2[] velo = new Vector2[4];
        float globalhoverdarken = 1; // global used for sprites, local used for rectanglesA
        List<float> localhoverdarken = new List<float>();
        List<Rectangle> LSRectangle = new List<Rectangle>();
        Dictionary<string, Vector2> LSText = new Dictionary<string, Vector2>();
        int level;
        List<Rectangle> l = new List<Rectangle>();
        Dictionary<int, Dictionary<int, Vector2>> startingpos = new Dictionary<int, Dictionary<int, Vector2>>(); // first int = level, 2nd int = player nr vector2 is pos
        Dictionary<int, List<Rectangle>> levellistrectangles = new Dictionary<int, List<Rectangle>>(); // int = level, rectangle = rectangle
        Dictionary<int, Color> playercolor = new Dictionary<int, Color>(); // int = playernr, color = color
        Dictionary<int, Rectangle> win = new Dictionary<int, Rectangle>(); // int = level rec = pos
        Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>> moving = new Dictionary<int, Dictionary<Rectangle, Dictionary<int, Vector2>>>(); // first int = levelnr, rectangle = startpos 2nd int = length units/frame vector2 = end pos
        List<Vector2> start = new List<Vector2>();
        Vector2 pan;
        Dictionary<int, Dictionary<Rectangle, Rectangle>> portal = new Dictionary<int, Dictionary<Rectangle, Rectangle>>(); // int = levelnr, 1st rectangle = dest 1, 2nd = dest 2 (one way 1->2)
        Dictionary<int, List<Rectangle>> trails = new Dictionary<int, List<Rectangle>>(); // int = playernr, list<rect> = trail pos and size
        float zoom = 1;
        bool[] dead = new bool[4];
        bool[] winner = new bool[4];
        Rectangle LCSwall = new Rectangle(100, 420, 25, 25); // LCS = level creator select
        Rectangle LCSredspawn = new Rectangle(150, 420, 25, 25);
        Rectangle LCSbluespawn = new Rectangle(200, 420, 25, 25);
        Rectangle LCSyellowspawn = new Rectangle(250, 420, 25, 25);
        Rectangle LCSgreenspawn = new Rectangle(300, 420, 25, 25);
        bool selectw;
        bool selectr;
        bool selectb;
        bool selecty;
        bool selectg;
        bool clicked;
        List<Rectangle> LCSwallspos = new List<Rectangle>();
        Rectangle LCSwallghost;
        Vector2[] playerspawn = new Vector2[4];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            for (int i = 0; i < 4; i++)
            {
                localhoverdarken.Add(1);
            }
            for (int i = 1; i <= 2; i++)
            {
                List<Rectangle> empty = new List<Rectangle>();
                empty.Add(new Rectangle());
                levellistrectangles.Add(i, empty);
            }
            for (int i = 0; i < 4; i++)
            {
                List<Rectangle> empty = new List<Rectangle>();
                empty.Add(new Rectangle());
                trails.Add(i, empty);
                for (int j = 0; j < 15; j++)
                {
                    trails[i].Add(new Rectangle());
                }
            }
            playercolor[0] = Color.Red;
            playercolor[1] = Color.Blue;
            playercolor[2] = Color.Yellow;
            playercolor[3] = Color.Green;
            List<bool> boollist = new List<bool>();
            LSRectangle.Add(new Rectangle(25, 400, 90, 35)); // LEVEL SELECT START
            LSText.Add("Back", new Vector2(55, 412));
            LSRectangle.Add(new Rectangle(25, 100, 100, 35));
            LSText.Add("Level 1", new Vector2(52, 112));
            LSRectangle.Add(new Rectangle(175, 100, 100, 35));
            LSText.Add("Level 2", new Vector2(202, 112));
            LSRectangle.Add(new Rectangle(175, 400, 100, 35));
            LSText.Add("Lev. Creator", new Vector2(180, 412)); // LEVEL SELECT END
            Dictionary<int, Vector2> startpos = new Dictionary<int, Vector2>();
            Dictionary<Rectangle, Rectangle> portalpair = new Dictionary<Rectangle, Rectangle>();
            startpos.Add(0, new Vector2(100, 400)); // LEVEL 1 START
            startpos.Add(1, new Vector2(200, 400));
            startpos.Add(2, new Vector2(300, 400));
            startpos.Add(3, new Vector2(400, 400));
            startingpos.Add(1, startpos.ToDictionary());
            Dictionary<Rectangle, Dictionary<int, Vector2>> mo = new Dictionary<Rectangle, Dictionary<int, Vector2>>();
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
            levellistrectangles[1] = l.ToList();
            win[1] = new Rectangle(215, 0, 70, 50);
            t[1] = new Vector2(200, 100);
            mo[new Rectangle(100, 200, 25, 25)] = t;
            moving[1] = mo;
            boollist.Add(true);
            m[1] = boollist.ToList();
            boollist.Clear();
            start.Add(new Vector2(0, 50)); // LEVEL 1 END
            l.Clear(); // LEVEL 2 START
            startpos.Clear();
            l.Add(new Rectangle(0, 0, 10, 430));
            l.Add(new Rectangle(0, 0, 800, 10));
            l.Add(new Rectangle(790, 0, 10, 440));
            l.Add(new Rectangle(0, 430, 800, 10));
            l.Add(new Rectangle(5, 425, 50, 5));
            l.Add(new Rectangle(5, 375, 50, 5));
            l.Add(new Rectangle(5, 325, 50, 5));
            l.Add(new Rectangle(5, 275, 50, 5));
            l.Add(new Rectangle(5, 225, 800, 5));
            l.Add(new Rectangle(750, 340, 50, 10));
            l.Add(new Rectangle(750, 400, 50, 10));
            l.Add(new Rectangle(740, 395, 10, 15));
            l.Add(new Rectangle(740, 340, 10, 15));
            l.Add(new Rectangle(590, 175, 10, 50));
            l.Add(new Rectangle(650, 175, 10, 50));
            l.Add(new Rectangle(645, 165, 15, 10));
            l.Add(new Rectangle(590, 165, 15, 10));
            l.Add(new Rectangle(355, 0, 10, 50));
            l.Add(new Rectangle(435, 0, 10, 50));
            levellistrectangles[2] = l.ToList();
            startpos.Add(0, new Vector2(25, 400));
            startpos.Add(1, new Vector2(25, 350));
            startpos.Add(2, new Vector2(25, 300));
            startpos.Add(3, new Vector2(25, 250));
            startingpos[2] = startpos.ToDictionary();
            win[2] = new Rectangle(365, 0, 70, 50);
            Rectangle portal1 = new Rectangle(750, 350, 50, 50);
            Rectangle portal2 = new Rectangle(600, 175, 50, 50);
            portalpair[portal1] = portal2;
            portal[2] = portalpair;
            l.Clear();
            boollist.Clear(); // LEVEL 2 END
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
            time++;
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
            else if (gamestate == 3)
            {
                LevelCreator();
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
                            else if (text.Contains("Lev. Creator"))
                            {
                                gamestate = 3;
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
                for (int i = 0; i < 4; i++) // || TRAIL HANDLER
                {
                    for (int j = 0; j < 15; j++)
                    {
                        Vector2 offset = new Vector2();
                        if (velo[i].X == -1)
                        {
                            offset.X = 1;
                        }
                        if (velo[i].Y == -1)
                        {
                            offset.Y = 1;
                        }
                        if (j == time % 15)
                        {
                            trails[i][j] = player[i];
                        }
                        else
                        {
                            trails[i][j] = new Rectangle(trails[i][j].X + (int)offset.X, trails[i][j].Y + (int)offset.Y, trails[i][j].Width - 1, trails[i][j].Height - 1);
                        }
                    }
                }
                for (int i = 0; i < 4; i++) // || VELOCITY
                {
                    if (!dead[i] && !winner[i])
                    {
                        player[i].X += (int)velo[i].X;
                        player[i].Y += (int)velo[i].Y;
                    }
                }
                if (moving.ContainsKey(level))
                {
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
                        if (m[level][i])
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
                            m[level][i] = !m[level][i];
                        }
                        moving = temp;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Rectangle playerax = new Rectangle((int)(player[i].X + velo[i].X), player[i].Y, player[i].Width, player[i].Height); // player after x
                        Rectangle playeray = new Rectangle(player[i].X, (int)(player[i].Y + velo[i].Y), player[i].Width, player[i].Height); // player after y
                        Rectangle playerav = new Rectangle((int)(player[i].X + velo[i].X), (int)(player[i].Y + velo[i].Y), player[i].Width, player[i].Height); // player after velocity
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
                            if (elements.Key.Contains(player[i]))
                            {
                                dead[i] = true;
                            }
                        }
                    }
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
                    if (portal.ContainsKey(level))
                    {
                        for (int j = 0; j < portal[level].Count(); j++)
                        {
                            Rectangle portal1 = portal[level].ElementAt(j).Key;
                            Rectangle portal2 = portal[level].ElementAt(j).Value;
                            if (portal1.Contains(player[i]))
                            {
                                player[i].X = portal2.X + portal2.Width / 2;
                                player[i].Y = portal2.Y + portal2.Height / 2;
                            }
                        }
                    }
                    if (win[level].Contains(player[i]))
                    {
                        winner[i] = true;
                        for (int j = 0; j < 4; j++)
                        {
                            int stop = 0;
                            if (winner[i] || dead[i])
                            {
                                stop++;
                            }
                            if (stop == 4)
                            {
                                gamestate = 1;
                            }
                        }
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    pan.Y += 10 * (1 / zoom);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    pan.Y -= 10 * (1 / zoom);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    pan.X -= 10 * (1 / zoom);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    pan.X += 10 * (1 / zoom);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.O) && zoom > 0.4)
                {
                    zoom /= 1.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.I) && zoom < 10)
                {
                    zoom *= 1.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    pan.X = 0;
                    pan.Y = 0;
                    zoom = 1;
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
            void LevelCreator()
            {
                if (LCSwall.Contains(mouse.Position))
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        selectw = true;
                        time = 0;
                    }
                }
                if (selectw)
                {
                    if (clicked)
                    {
                        if (oldmousepos.X > mouse.X)
                        {
                            LCSwallghost.X = (int)oldmousepos.X;
                        }
                        else
                        {
                            LCSwallghost.X = mouse.X;
                        }
                        if (oldmouse.Y > mouse.Y)
                        {
                            LCSwallghost.Y = (int)oldmousepos.Y;
                        }
                        else
                        {
                            LCSwallghost.Y = mouse.Y;
                        }
                        LCSwallghost.Width = Math.Abs((int)oldmousepos.X - mouse.X);
                        LCSwallghost.Height = Math.Abs((int)oldmousepos.Y - mouse.Y);
                    }
                    else
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released && time != 0)
                        {
                            LCSwallghost.X = mouse.X;
                            LCSwallghost.Y = mouse.Y;
                            oldmousepos.X = mouse.X;
                            oldmousepos.Y = mouse.Y;
                            clicked = true;
                        }
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
                if (portal.ContainsKey(level))
                {
                    for (int i = 0; i < portal[level].Count(); i++)
                    {
                        var elements = portal.ElementAt(i);
                        Rectangle portal1 = elements.Value.ElementAt(i).Key;
                        Rectangle portal2 = elements.Value.ElementAt(i).Value;
                        spriteBatch.Draw(basic, new Rectangle((int)((portal1.X + pan.X) * zoom), (int)((portal1.Y + pan.Y) * zoom), (int)(portal1.Height * zoom), (int)(portal1.Width * zoom)), Color.Firebrick);
                        spriteBatch.Draw(basic, new Rectangle((int)((portal2.X + pan.X) * zoom), (int)((portal2.Y + pan.Y) * zoom), (int)(portal2.Height * zoom), (int)(portal2.Width * zoom)), Color.Firebrick);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    Rectangle pplayer = new Rectangle((int)((player[i].X + pan.X) * zoom), (int)((player[i].Y + pan.Y) * zoom), (int)(player[i].Width * zoom), (int)(player[i].Height * zoom));
                    if (!dead[i])
                    {
                        spriteBatch.Draw(basic, pplayer, playercolor[i]);
                        for (int j = 0; j < 15; j++)
                        {
                            int w = 15;
                            Rectangle ptrail = new Rectangle((int)((trails[i][j].X + pan.X) * zoom), (int)((trails[i][j].Y + pan.Y) * zoom), (int)(trails[i][j].Width * zoom), (int)((trails[i][j].Height) * zoom));
                            spriteBatch.Draw(basic, ptrail, playercolor[i] * (0.01F * w));
                            w--;
                        }
                    }
                }
                Rectangle pwin = new Rectangle((int)((win[level].X + pan.X)*zoom), (int)((win[level].Y+pan.Y)*zoom), (int)(win[level].Width*zoom), (int)(win[level].Height*zoom));
                spriteBatch.Draw(basic, pwin, Color.Gold);
                if (moving.ContainsKey(level))
                {
                    for (int i = 0; i < moving[level].Count(); i++)
                    {
                        var elements = moving[level].ElementAt(i);
                        Rectangle pmoving = new Rectangle((int)((elements.Key.X + pan.X)*zoom), (int)((elements.Key.Y + pan.Y)*zoom), (int)(elements.Key.Width*zoom), (int)(elements.Key.Height*zoom));
                        spriteBatch.Draw(basic, pmoving, Color.White);
                    }
                }
                List<Rectangle> currec = levellistrectangles[level];
                for (int i = 0; i < currec.Count(); i++)
                {
                    Rectangle pcurrec = new Rectangle((int)((currec[i].X + pan.X)*zoom), (int)((currec[i].Y + pan.Y)*zoom), (int)(currec[i].Width*zoom), (int)((currec[i].Height)*zoom));
                    spriteBatch.Draw(basic, pcurrec, Color.White);
                }
            }
            else if (gamestate == 3)
            {
                spriteBatch.Draw(basic, new Rectangle(0, 400, 800, 80), Color.DarkGray);
                spriteBatch.Draw(basic, LCSwall, Color.White);
                spriteBatch.Draw(basic, LCSredspawn, Color.Red);
                spriteBatch.Draw(basic, LCSbluespawn, Color.Blue);
                spriteBatch.Draw(basic, LCSyellowspawn, Color.Yellow);
                spriteBatch.Draw(basic, LCSgreenspawn, Color.Green);
                spriteBatch.Draw(basic, LCSwallghost, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
