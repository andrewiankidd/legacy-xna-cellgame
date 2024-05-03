using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using GifAnimation;

namespace A_Game
{

    public class npc : DrawableGameComponent
    {
        ContentManager _content;
        SpriteBatch _spriteBatch;
        GraphicsDeviceManager _graphics;
        KeyboardState newkbState, oldkbState;
        public static Rectangle[] directions = new Rectangle[4];
        public static Rectangle direction;
        public static List<string> npcinfo = new List<string>();
        public static bool interactable;
        public static string npcname;

        public npc(Game game, ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
            : base(game)
        {
            _content = content;
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            directions[0] = new Rectangle(26, 3, 20, 30);
            directions[1] = new Rectangle(27, 66, 18, 30); 
            directions[2] = new Rectangle(1, 98, 22, 31);
            directions[3] = new Rectangle(50, 34, 22, 31);
            direction = directions[2];


        }
        public override void Update(GameTime gameTime)
        {
            if (Game1.gamestate == "play")
            {
                newkbState = Keyboard.GetState();
                if (interactable && KeypressTest(Keys.Enter))
                {
                    
                    if (Game1.herofacing=="down"){direction = directions[0];}
                    if (Game1.herofacing == "up") { direction = directions[1]; }
                    if (Game1.herofacing == "right") { direction = directions[2]; }
                    if (Game1.herofacing == "left") { direction = directions[3]; }
                    
                    chat.chatarray = System.IO.File.ReadAllLines(Game1.cd + "content/assets/npcs/" + npcname + ".txt");
                    Game1.gamestate = "chat";
                }
                oldkbState = Keyboard.GetState();
            }
            base.Update(gameTime);
        }
                    
        public override void Draw(GameTime gameTime)
        {
            if (Game1.gamestate == "play")
            {

                base.Draw(gameTime);
            }
        }
        private bool KeypressTest(Keys theKey)
        {
            if (newkbState.IsKeyUp(theKey) && oldkbState.IsKeyDown(theKey))
            {

                return true;
            }
            return false;
        }
    }
}

