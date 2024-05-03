using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using GifAnimation;


namespace A_Game
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont ArialFont, ArialSmallFont;
        KeyboardState newkbState, oldkbState;
        Song song;

        public static Texture2D pausemenutexture, playerspritesheet, bgtexture, maptexture, splashlogo;
        public static Effect effect;
        public static string cd;
        ////various modes, sorted with any var's they may need

        //basic game settings
        public static string[] storyini;

        //main menu vars
        public static bool close=false;

        //gamecode
        public static string gamestate;
        float fadefloat=1f;
        public static string objective;
        int secs = 80;

        //hero
        public static int spriteX = 480, spriteY = 250;
        decimal step = 0;
        bool moving;
        public static Rectangle currentrect;
        public static string herofacing;
        int movespeed;

        //mapstuff
        public static int gbx = 0, gby = 0;
        bool panning;
        public static string[] mapdata;
        public static string curmap;
        float otherfloat = 0.2f;

        //scale
        public static Matrix SpriteScale;
        public static float charscale=2f;


        //lighting
        Texture2D lightmask, blackSquare, streetlight;
        RenderTarget2D mainScene;
        RenderTarget2D lightMask;
        Effect lightingEffect;
        public static Color darkness = Color.Black * 0.65f;
        public static List<string> torchPositions = new List<string>();

        const int LIGHTOFFSET = 115;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 1000;
            this.graphics.PreferredBackBufferHeight = 500;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<System.EventArgs>( Window_ClientSizeChanged );
            
        }
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            SpriteScale = Matrix.CreateScale(graphics.GraphicsDevice.Viewport.Width / 1000f, graphics.GraphicsDevice.Viewport.Height / 500f, 1);
            graphics.ApplyChanges();
        }
        protected override void Initialize()
        {
            //graphics
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteScale = Matrix.CreateScale(graphics.GraphicsDevice.Viewport.Width / 1000f, graphics.GraphicsDevice.Viewport.Height / 500f, 1);

            //components
            this.Components.Add(new mainmenu(this, Content, spriteBatch));
            this.Components.Add(new pausemenu(this, Content, spriteBatch));
            this.Components.Add(new savecontrol(this, Content, spriteBatch, graphics));
            this.Components.Add(new npc(this, Content, spriteBatch, graphics));
            this.Components.Add(new chat(this, Content, spriteBatch, graphics));

            //lights
            var pp = GraphicsDevice.PresentationParameters;
            mainScene = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lightMask = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            gamestate = "splash";
            base.Initialize();
        }
        protected override void LoadContent()
        {
            
            //lights
            blackSquare = Content.Load<Texture2D>("blacksquare");
            lightmask = Content.Load<Texture2D>("lightmask");
            lightingEffect = Content.Load<Effect>("lighting");
            streetlight = Content.Load<Texture2D>("Assets/other/streetlight");

            //textures
            pausemenutexture = this.Content.Load<Texture2D>("mainmenutexture");
            playerspritesheet = Texture2D.FromStream(GraphicsDevice, TitleContainer.OpenStream(cd + "Content/assets/sprites/playersprites.png"));
            splashlogo = Content.Load<Texture2D>("Assets/splash");
            
            //fonts
            ArialFont = this.Content.Load<SpriteFont>("Assets/Fonts/Arial");
            ArialSmallFont = this.Content.Load<SpriteFont>("Assets/Fonts/ArialSmallFont");

            //tunes
            //song = Content.Load<Song>("assets/audio/messagetoyou");
            //MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.4f;
 
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            newkbState = Keyboard.GetState();

            if (gamestate == "splash") { secs = secs - 1; }
            if (gamestate == "menu") { MediaPlayer.Resume(); }
            if (gamestate == "paused") { MediaPlayer.Resume(); }
            if (gamestate == "play") { MediaPlayer.Pause(); movement(); }
            if (gamestate == "chat") { }
            if (gamestate == "quit") { Exit(); }

            if (KeypressTest(Keys.F12))
            {
                screeny();
            }

            if (gamestate != "play")
            {
                Game1.darkness = Color.Black * 0;
            }
            if (gamestate == "play" && Game1.darkness.A == 0)
            {
                string str = Game1.mapdata[0].Substring(0, 2);
                float i = (Convert.ToInt32(str) * 0.01f);
                Game1.darkness = Color.Black * i;
            } 

            
            oldkbState = Keyboard.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            drawMain(gameTime);
            drawLightMask(gameTime);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, SpriteScale);
            lightingEffect.Parameters["lightMask"].SetValue(lightMask);
            lightingEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainScene, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private void drawLightMask(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(lightMask);
            GraphicsDevice.Clear(Color.White);
            string[] temp = null;

            // Create a Black Background
            spriteBatch.Begin();
            spriteBatch.Draw(blackSquare, new Vector2(0, 0), new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), darkness);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);


            foreach (var s in torchPositions)
            {
                temp = s.Split(',');
                spriteBatch.Draw(lightmask, new Vector2(Convert.ToInt32(temp[0]) + gbx, Convert.ToInt32(temp[1]) + gby), Color.White);   
            }
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }
        private void drawMain(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(mainScene);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            if (gamestate == "splash")
            {
                spriteBatch.Draw(splashlogo, new Vector2(0, 0), Color.White*fadefloat);
                if (gameTime.ElapsedGameTime.TotalSeconds > secs) { fadefloat = fadefloat - 0.01f; }
                Window.Title = fadefloat.ToString();
                if (fadefloat < 0f) { gamestate = "menu"; }
            }
            if (gamestate == "menu")
            {
                this.IsMouseVisible = true;
            }

            else if (gamestate == "paused")
            {
                this.IsMouseVisible = true;
            }
            else if (gamestate=="play")
            {
                spriteBatch.Draw(bgtexture, new Vector2(gbx, gby), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                foreach (string s in npc.npcinfo)
                {
                    string[] ssplit = s.Split(',');
                    int npcbase = (Convert.ToInt32(ssplit[1]) + gby + 35);
                    if (spriteY > (npcbase - 50)) { otherfloat = 0.2f; } else { otherfloat = 0.5f; }
                    spriteBatch.Draw(Texture2D.FromStream(GraphicsDevice, TitleContainer.OpenStream(Game1.cd + "content/assets/npcs/" + ssplit[2] + ".png")), new Vector2(Convert.ToInt32(ssplit[0]) + (gbx ), Convert.ToInt32(ssplit[1]) + (gby)), npc.direction, Color.White, 0, Vector2.Zero, Game1.charscale, SpriteEffects.None, otherfloat);
                }
                foreach (var s in torchPositions)
                {
                    string[] temp = s.Split(',');
                    int lampostbase = (Convert.ToInt32(temp[1]) + gby + streetlight.Height);
                    if (spriteY > (lampostbase - 60)) { otherfloat = 0.2f; } else { otherfloat = 0.5f; }
                    spriteBatch.Draw(streetlight, new Vector2(Convert.ToInt32(temp[0]) + (gbx + 77), Convert.ToInt32(temp[1]) + (gby)), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, otherfloat);
                }
                spriteBatch.Draw(playerspritesheet, new Vector2(spriteX, spriteY), currentrect, Color.White, 0, Vector2.Zero, charscale, SpriteEffects.None, 0.4f);
                spriteBatch.Draw(maptexture, new Vector2(gbx, gby), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }

            else if (gamestate == "chat")
            {
                spriteBatch.Draw(Texture2D.FromStream(graphics.GraphicsDevice, TitleContainer.OpenStream(Game1.cd + "content/assets/chat/" + chat.chatname + ".png")), new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(chat.chatoverlaytexture, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(ArialSmallFont, chat.chatname, new Vector2(40, 400), Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(ArialFont, chat.chattext, new Vector2(20, 420), Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            base.Draw(gameTime);
        }
        private bool KeypressTest(Keys theKey)
        {
            if (newkbState.IsKeyUp(theKey) && oldkbState.IsKeyDown(theKey))
            {

                return true;
            }
            return false;
        }
        private void screeny()
        {


            string filename = DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + "." + DateTime.Now.Second.ToString() + "." + DateTime.Now.DayOfWeek.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();

                int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
                int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

                //force a frame to be drawn (otherwise back buffer is empty) 
                Draw(new GameTime());

                //pull the picture from the buffer 
                int[] backBuffer = new int[w * h];
                GraphicsDevice.GetBackBufferData(backBuffer);

                //copy into a texture 
                Texture2D texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
                texture.SetData(backBuffer);
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/A Game/Screenshots/";
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                    //save to disk 
                    System.IO.Stream stream = System.IO.File.OpenWrite(folder+filename + ".jpg");

                    texture.SaveAsJpeg(stream, w, h);
                    stream.Dispose();

                    texture.Dispose();

            }

        
        public void movement()
        {
            //walking about


            if (KeypressTest(Keys.Escape) == true)
            {
                if (gamestate == "paused") { gamestate = "play"; } else { gamestate = "paused"; }
            }

            moving = true;
            if (newkbState.IsKeyDown(Keys.W) == true)
            {
                if (spriteY > gby && nextblocksafe("up") == true)
                {
                    if (spriteY <= 150) { gby++; spriteY = spriteY - Convert.ToInt32(0.5); } else { spriteY--; };
                }
                animatehero("up");
            }
            else if (newkbState.IsKeyDown(Keys.S) == true)
            {
                if (spriteY < gby + (bgtexture.Height - (currentrect.Height * Convert.ToInt32(charscale))) && nextblocksafe("down") == true)
                {
                    if (spriteY >= 320) { gby--; spriteY = spriteY + Convert.ToInt32(0.5); } else { spriteY++; };
                }
                animatehero("down");
            }
            else if (newkbState.IsKeyDown(Keys.A) == true)
            {
                if (spriteX > gbx && nextblocksafe("left") == true)
                {                   
                    if (spriteX <= 250) { gbx++; spriteX = spriteX - Convert.ToInt32(0.5); } else { spriteX--; };
                    //if (spriteX <= 250) { gbx=gbx+movespeed; spriteX = spriteX - Convert.ToInt32(0.5); } else { spriteX-=2; };
                }
                animatehero("left");
            }
            else if (newkbState.IsKeyDown(Keys.D) == true )
            {
                if (spriteX < gbx + (bgtexture.Width - (currentrect.Width * Convert.ToInt32(charscale))) && nextblocksafe("right") == true)
                {                    
                    if (spriteX >= 750) { gbx--; spriteX = spriteX + Convert.ToInt32(0.5); } else { spriteX++; };
                }
                animatehero("right");
            }

            else { moving = false; }

        }
        private void animatehero(string direction)
        {
            herofacing = direction;
            if (moving == true)
            {
                if (step >= 4) { step = 0; } else { step = step + 0.075m; }
            }

            if (direction == "up")
            {
                if (step < 4 && step > 3) { currentrect = new Rectangle(27, 3, 18, 30); }
                else if (step <= 3 && step >= 2) { currentrect = new Rectangle(3, 4, 18, 31); }
                else if (step < 2 && step >= 1) { currentrect = new Rectangle(27, 3, 18, 31); }
                else if (step < 1 && step >= 0) { currentrect = new Rectangle(51, 4, 18, 31); }

            }
            if (direction == "down")
            {

                if (step < 4 && step > 3) { currentrect = new Rectangle(27, 66, 18, 30); }
                else if (step <= 3 && step >= 2) { currentrect = new Rectangle(3, 67, 18, 31); }
                else if (step < 2 && step >= 1) { currentrect = new Rectangle(27, 66, 18, 31); }
                else if (step < 1 && step >= 0) { currentrect = new Rectangle(51, 67, 18, 31); }
            }
            if (direction == "left")
            {
                if (step < 4 && step > 3) { currentrect = new Rectangle(1, 98, 22, 31); }
                else if (step <= 3 && step >= 2) { currentrect = new Rectangle(26, 98, 22, 31); }
                else if (step < 2 && step >= 1) { currentrect = new Rectangle(1, 98, 22, 31); }
                else if (step < 1 && step >= 0) { currentrect = new Rectangle(50, 98, 22, 31); }
            }
            if (direction == "right")
            {
                if (step < 4 && step > 3) { currentrect = new Rectangle(0, 33, 22, 31); }
                else if (step <= 3 && step >= 2) { currentrect = new Rectangle(25, 33, 22, 31); }
                else if (step < 2 && step >= 1) { currentrect = new Rectangle(0, 33, 22, 31); }
                else if (step < 1 && step >= 0) { currentrect = new Rectangle(48, 33, 22, 31); }
            }

        }

        public bool nextblocksafe(string dir)
        {
            string[] temp = null;
    
            int calcx = spriteX - gbx;
            int calcy = spriteY - gby;

            Window.Title = " calcx " + calcx + " calcy " + calcy + " | gbx " + gbx + " gby " + gby + " | spriteX " + spriteX + " spriteY " + spriteY;

            int cellperm = 0;


            foreach (var s in torchPositions)
            {
                string[] lighttemp = s.Split(',');
                int streetlightbase = (Convert.ToInt32(lighttemp[1]) + streetlight.Height);
                int streetlightleft = (Convert.ToInt32(lighttemp[0]) + 40);
                int streetlightright = streetlightleft + 60;
                if (dir == "up")
                {

                    if (calcy - 1 < (streetlightbase - 50) && calcy > (streetlightbase - 90) && calcx < streetlightright && calcx > streetlightleft ) { cellperm = 1; } 
                }
                if (dir == "down")
                {

                    if (calcy < (streetlightbase - 50) && calcy + 1 > (streetlightbase - 90) && calcx < streetlightright && calcx > streetlightleft) { cellperm = 1; }
                }
                if (dir == "left")
                {

                    if (calcy < (streetlightbase - 50) && calcy > (streetlightbase - 90) && calcx -1 < streetlightright && calcx > streetlightleft) { cellperm = 1; }
                }
                if (dir == "right")
                {

                    if (calcy < (streetlightbase - 50) && calcy > (streetlightbase - 90) && calcx < streetlightright && calcx + 1 > streetlightleft) { cellperm = 1; }
                }

            }


            foreach (var s in npc.npcinfo)
            {
                string[] npctemp = s.Split(',');
                int npcbase = (Convert.ToInt32(npctemp[1])+35);
                int npcleft = (Convert.ToInt32(npctemp[0])-30);
                int npcright = npcleft + 65;
                if (dir == "up")
                {

                    if (calcy - 1 < (npcbase) && calcy > npcbase-50 && calcx > npcleft && calcx < npcright) 
                    { 
                        cellperm = 3;
                        npc.interactable = true; npc.npcname = npctemp[2]; 
                    }
                }
                if (dir == "down")
                {

                    if (calcy  < (npcbase) && calcy + 1 > npcbase - 50 && calcx > npcleft && calcx < npcright) 
                    {
                        cellperm = 3; npc.interactable = true; npc.npcname = npctemp[2]; 
                    }
                }
                if (dir == "left")
                {

                    if (calcy < (npcbase) && calcy > npcbase - 50 && calcx > npcleft && calcx - 1 < npcright)
                    {
                        cellperm = 3; npc.interactable = true; npc.npcname = npctemp[2]; 
                    }
                }
                if (dir == "right")
                {

                    if (calcy < (npcbase) && calcy > npcbase - 50 && calcx + 1 > npcleft && calcx < npcright)
                    {
                        cellperm = 3; npc.interactable = true; npc.npcname = npctemp[2]; 
                    }
                }

            }


                foreach (string row in mapdata)
                {
                    temp = row.Split(',');
                    //create rectangle from 4 co-ordinates
                    //order goes:
                    //permission, where the rectangle starts on x, x for the end, y for the start, y for the end
                    int xstart = Convert.ToInt32(temp[1]);
                    int xend = Convert.ToInt32(temp[2]);
                    int ystart = Convert.ToInt32(temp[3]);
                    int yend = Convert.ToInt32(temp[4]);

                    if (dir == "up")
                    {
                        if (calcy - 1 < (yend + 20) && calcy > (ystart) && calcx > (xstart ) && calcx < (xend))
                        {
                            cellperm = Convert.ToInt32(temp[0]); break;
                        }
                    }

                    if (dir == "down")
                    {
                        if (calcy < (yend + 20) && calcy + 1 > (ystart) && calcx > (xstart) && calcx < (xend))
                        {
                            cellperm = Convert.ToInt32(temp[0]);  break;
                        }
                    }

                    if (dir == "left")
                    {
                        if (calcy < (yend + 20) && calcy > (ystart) && calcx > (xstart) && calcx - 1 < (xend))
                        {
                            cellperm = Convert.ToInt32(temp[0]);  break;
                        }
                    }
                     if (dir == "right")
                    {
                        if (calcy < (yend + 20) && calcy > (ystart) && calcx + 1 > (xstart ) && calcx < (xend))
                        {
                            cellperm = Convert.ToInt32(temp[0]); break;
                        }
                    }



                }


            if (cellperm == 0) { return true; }
             if (cellperm == 1) { return false; }
             if (cellperm == 2) { warpmap(temp[5], Convert.ToInt32(temp[6]), Convert.ToInt32(temp[7])); return true; }
             if (cellperm == 3) { return false; }


            return false;
        }

        public void warpmap(string map, int x, int y)
        {
            //mapdate
            mapdata = System.IO.File.ReadAllLines(cd + "content/assets/maps/" + map + "/map.ini");
            foreach (string s in Game1.mapdata)
            {
                if (s.Substring(0, 1) == "3")
                {
                    npc.npcinfo.Add(s);

                }
            }
            //npcs
            npc.npcinfo.Clear();
            var npcini = System.IO.File.ReadAllLines(cd + "content/assets/maps/" + map + "/npcs.ini");
            npc.npcinfo = new List<string>(npcini);

            //lights
            torchPositions.Clear();
            var torchini = System.IO.File.ReadAllLines(cd + "content/assets/maps/" + map + "/lights.ini");
            torchPositions = new List<string>(torchini);
            string str = Game1.mapdata[0].Substring(0, 2);
            float i = (Convert.ToInt32(str) * 0.01f);
            Game1.darkness = Color.Black * i;
            //set and go
            bgtexture = Texture2D.FromStream(GraphicsDevice, TitleContainer.OpenStream(cd + "content/assets/maps/" + map + "/1.png"));
            maptexture = Texture2D.FromStream(GraphicsDevice, TitleContainer.OpenStream(cd + "content/assets/maps/" + map + "/2.png"));
            spriteX = x + gbx;
            spriteY = y + gby;
            curmap = map;
        }
    }
}
                