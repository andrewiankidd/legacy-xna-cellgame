using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace A_Game
{


    public class chat : DrawableGameComponent
    {
        public static string[] chatarray;
        public static string chatname;
        public static string chattext;
        int page = 1;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        ContentManager _content;
        SpriteFont ArialFont, ArialSmallFont;
        KeyboardState newkbState, oldkbState;
        public static Texture2D chatoverlaytexture;

        public chat(Game game, ContentManager content, SpriteBatch spritebatch, GraphicsDeviceManager graphics) : base(game)
        {
            _content = content;
            _spriteBatch = spritebatch;
            _graphics = graphics;


        }

        protected override void LoadContent()
        {

        }
       
        public override void Update(GameTime gameTime)
        {
            if (Game1.gamestate == "chat")
            {
                newkbState = Keyboard.GetState();
                chatoverlaytexture = _content.Load<Texture2D>("chatoverlay");

                chatname = chatarray[0];
                chattext = chatarray[page];
                input();

                if (chattext == "--end--")
                {
                    page = 1;
                    npc.interactable = false;
                    Game1.gamestate = "play";
                }

                oldkbState = Keyboard.GetState();
                base.Update(gameTime);
            }
        }
        public void input()
        {
            if (KeypressTest(Keys.Enter))
            {
                page++;
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
