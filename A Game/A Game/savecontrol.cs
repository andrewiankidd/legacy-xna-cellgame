using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace A_Game
{

    public class savecontrol : DrawableGameComponent
    {
        static GraphicsDeviceManager _graphics;
        ContentManager _content;
        SpriteBatch _spriteBatch;
        static string cd;
        static bool oktoload=false;

        public savecontrol(Game game, ContentManager content,  SpriteBatch spriteBatch, GraphicsDeviceManager graphics) : base(game)
        {
            _content = content;
             cd = _content.RootDirectory.ToString() + "/";
             _graphics = graphics;
             _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
        }

        static public void newgame()
        {
            string file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/A Game/save.sav";
            if (System.IO.File.Exists(file)) { System.IO.File.Delete(file); }


            string folder = cd + "/assets/";

            string[] savedata = System.IO.File.ReadAllLines(folder + "save.sav");
            //playername = savedata[1];
            //get xy
            string[] pos = savedata[3].Split(',');
            //set x
            Game1.spriteX = Convert.ToInt32(pos[0]);
            Game1.gbx = Convert.ToInt32(pos[2]);
            //set y
            Game1.spriteY = Convert.ToInt32(pos[1]);
            Game1.gby = Convert.ToInt32(pos[3]);
            //get direction/frame
            string[] rect = savedata[5].Split(',');
            //set direction/frame
            Game1.currentrect = new Rectangle(Convert.ToInt32(rect[0]), Convert.ToInt32(rect[1]), Convert.ToInt32(rect[2]), Convert.ToInt32(rect[3]));
            //get map
            Game1.curmap = savedata[7];
            //set map
            Game1.mapdata = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/map.ini");

            npc.npcinfo.Clear();
            var npcini = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/npcs.ini");
            npc.npcinfo = new List<string>(npcini);

            Game1.torchPositions.Clear();
            var torchini = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/lights.ini");
            Game1.torchPositions = new List<string>(torchini);

            Game1.bgtexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + savedata[7] + "/1.png"));
            Game1.maptexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + savedata[7] + "/2.png"));
            Game1.objective = savedata[9];

            chat.chatarray = System.IO.File.ReadAllLines(cd + "/assets/story/introduction.txt");
            Game1.gamestate="chat";
        }


        static public void loadgamesave()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/A Game/";

            string[] savedata = System.IO.File.ReadAllLines(folder + "save.sav");
            //playername = savedata[1];
            //get xy
            string[] pos = savedata[3].Split(',');
            //set x
            Game1.spriteX = Convert.ToInt32(pos[0]); 
            Game1.gbx = Convert.ToInt32(pos[2]);
            //set y
            Game1.spriteY = Convert.ToInt32(pos[1]); 
            Game1.gby = Convert.ToInt32(pos[3]);
            //get direction/frame
            string[] rect = savedata[5].Split(',');
            //set direction/frame
            Game1.currentrect = new Rectangle(Convert.ToInt32(rect[0]), Convert.ToInt32(rect[1]), Convert.ToInt32(rect[2]), Convert.ToInt32(rect[3]));
            //get map
            Game1.curmap = savedata[7];
            //set map

            Game1.mapdata = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/map.ini");

             npc.npcinfo.Clear();
            var npcini = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/npcs.ini");
            npc.npcinfo = new List<string>(npcini);

            Game1.torchPositions.Clear();
            var torchini = System.IO.File.ReadAllLines(cd + "/assets/maps/" + savedata[7] + "/lights.ini");
            Game1.torchPositions = new List<string>(torchini);

            string str = Game1.mapdata[0].Substring(0,2);
            float i = (Convert.ToInt32(str) * 0.01f);
            Game1.darkness = Color.Black * i;

            Game1.bgtexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + savedata[7] + "/1.png"));
            Game1.maptexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + savedata[7] + "/2.png"));
            Game1.objective = savedata[9];
            Game1.gamestate = "play"; 
            oktoload = false;
        }
        public override void Draw(GameTime gameTime)
        {
            if (oktoload == true)
            {
               
                Game1.bgtexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + Game1.curmap + "/1.png"));
                Game1.maptexture = Texture2D.FromStream(_graphics.GraphicsDevice, TitleContainer.OpenStream(cd + "/assets/maps/" + Game1.curmap + "/2.png"));
                Game1.gamestate = "play";
 
            }
            base.Draw(gameTime);
        }
        static public void gamesave()
        {
            string[] savedata = new string[10];
            savedata[0] = "[playername]";
            savedata[1] = "Andy";
            savedata[2] = "[playerpos]";
            savedata[3] = Game1.spriteX + "," + Game1.spriteY + "," + Game1.gbx + "," + Game1.gby;
            savedata[4] = "[playerrect]";
            savedata[5] = Game1.currentrect.X + "," + Game1.currentrect.Y + "," + Game1.currentrect.Width + "," + Game1.currentrect.Height;
            savedata[6] = "[map]";
            savedata[7] = Game1.curmap;
            savedata[8] = "[chapter]";
            savedata[9] = Game1.objective;

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/A Game/";
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            System.IO.File.WriteAllLines(folder + "save.sav", savedata);
            Game1.gamestate = "play";

        }
    }
}
