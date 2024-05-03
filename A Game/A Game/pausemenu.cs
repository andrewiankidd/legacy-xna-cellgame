﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GifAnimation;

namespace A_Game
{
   
    public class pausemenu : DrawableGameComponent
    {
        
        Texture2D pausemenutexture, selectiontexture;
        Vector2 selectionvector = new Vector2(345, 330);
        KeyboardState newkbState, oldkbState;
        ContentManager _content;
        SpriteBatch _spriteBatch;
        SpriteFont titlefont,menufont;
        float alphapulse = 100;
        int selected = 0;
        bool godown = true;

        public pausemenu(Game game, ContentManager content, SpriteBatch spriteBatch)
            : base(game)
        {
                _content = content;
                _spriteBatch = spriteBatch;
                pausemenutexture = this._content.Load<Texture2D>("mainmenutexture");
                selectiontexture = _content.Load<Texture2D>("selectiontexture");
                titlefont = _content.Load<SpriteFont>("assets/Fonts/titlefont");
                menufont = _content.Load<SpriteFont>("assets/Fonts/menufont");
        }



        public override void Draw(GameTime gameTime)
        {
            if (Game1.gamestate == "paused")
            {
                newkbState = Keyboard.GetState();
                input();


                if (alphapulse >= 1f) { godown = true; }
                if (alphapulse <= 0.5f) { godown = false; }

                if (godown)
                    alphapulse = alphapulse - 0.025f;

                if (!godown)
                    alphapulse = alphapulse + 0.025f;


                _spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointWrap,DepthStencilState.Default,RasterizerState.CullNone,Game1.effect, Game1.SpriteScale);
                _spriteBatch.Draw(pausemenutexture, new Vector2(0, 0), Color.White);
                _spriteBatch.Draw(selectiontexture, selectionvector, Color.White * alphapulse);
                _spriteBatch.DrawString(titlefont, "A GAME!", new Vector2(300, 20), Color.White * alphapulse);
                _spriteBatch.DrawString(menufont, "Unpause :D!", new Vector2(390, 330), Color.White);
                _spriteBatch.DrawString(menufont, "Save Your Game!", new Vector2(350, 380), Color.White);
                _spriteBatch.DrawString(menufont, "Quit Game D:!", new Vector2(370, 430), Color.White);
                _spriteBatch.DrawString(menufont, Game1.objective, new Vector2(250, 200), Color.White * alphapulse);


                _spriteBatch.End();
                oldkbState = Keyboard.GetState();
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
        private void input()
        {

            if (Game1.gamestate == "paused")
            {
                if (KeypressTest(Keys.W) || (KeypressTest(Keys.Up)))
                {
                    if (selected > 0)
                    {
                        selected--;
                    }
                }
                if (KeypressTest(Keys.S) || (KeypressTest(Keys.Down)))
                {
                    if (selected < 2)
                    {
                        selected++;
                    }
                }


                MouseState mouse = Mouse.GetState();

                if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 330 & mouse.Y < 380 || selected == 0)
                {
                    selectionvector = new Vector2(345, 330);
                    selected = 0;
                    if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 330 & mouse.Y < 380 & selected == 0 & mouse.LeftButton == ButtonState.Pressed || selected == 0 & KeypressTest(Keys.Enter))
                    {
                        Game1.gamestate="play";
                    }
                }

                if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 380 & mouse.Y < 430 || selected == 1)
                {
                    selectionvector = new Vector2(345, 380);
                    selected = 1;
                    if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 380 & mouse.Y < 430 & selected == 1 & mouse.LeftButton == ButtonState.Pressed || selected == 1 & KeypressTest(Keys.Enter))
                    {
                        savecontrol.gamesave();
                    }
                }

                if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 430 & mouse.Y < 480 || selected == 2)
                {
                    selectionvector = new Vector2(345, 430);
                    selected = 2;
                    if (mouse.X > 345 & mouse.X < 612 & mouse.Y > 430 & mouse.Y < 480 & selected == 2 & mouse.LeftButton == ButtonState.Pressed || selected == 2 & KeypressTest(Keys.Enter))
                    {
                        Game1.gamestate = "quit";
                    }

            }
        }   
    }
}
}

