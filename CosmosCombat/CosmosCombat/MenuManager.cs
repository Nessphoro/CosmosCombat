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

namespace CosmosCombat
{
    /// <summary>
    /// Class for managing menues 
    /// </summary>
    class MenuManager
    {
        /// <summary>
        /// Stores all the buttons
        /// </summary>
        protected List<Button> _buttons;
        protected bool Clicking = false;
        /// <summary>
        /// Stores all the buttons
        /// </summary>
        public List<Button> Buttons { get { return _buttons; } }
        public Texture2D Overlay;
        public Vector2 OverlayOrigin;
        public MenuManager()
        {
            _buttons = new List<Button>();
        }
        /// <summary>
        /// Update button states, and process all clicks
        /// </summary>
        public void Update(Rectangle clickRect,bool released)
        {
            Clicking = false;
            foreach (Button b in _buttons)
            {
                if (!b.Draw)
                    continue;
                b.ButtonState = GameButtonState.Normal;
                if (clickRect.Intersects(b.Rectangle)&&!Clicking)
                {
                    Clicking = true;
                    b.ButtonState = GameButtonState.Click;
                    if (b.OnClick != null&&released)
                    {
                        b.OnClick(b, EventArgs.Empty);
                    }
                }

            }
            
        }
        /// <summary>
        /// Draw buttons onto the screen
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button button in _buttons)
            {
                if (!button.Draw)
                    continue;
                spriteBatch.Draw(button.Texture, button.Position, null, Color.White, 0f, button.Origin, button.Scale, SpriteEffects.None, 0f);
                /*if (button.DrawOverlay)
                    spriteBatch.Draw(Overlay, button.Position, null, Color.White, 0f, OverlayOrigin, button.Scale, SpriteEffects.None, 0f);*/
            }
        }
    }
}
