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
    /// Represents in what state a button is
    /// </summary>
    enum GameButtonState { Hovering, Normal, Click }
    /// <summary>
    /// Class for basic button support
    /// </summary>
    class Button
    {
        /*Self explanatory*/
        protected Vector2 _position;
        protected Vector2 _origin;
        protected Texture2D _normal;
        protected Texture2D _click;
        protected GameButtonState _buttonState;
        protected float _scale;

        public Button(Texture2D Normal,Texture2D Click, Vector2 Position)
        {
            _position = Position;
            _normal = Normal;
            _click = Click;
            _buttonState = GameButtonState.Normal;
            _origin = new Vector2(_normal.Width / 2, _normal.Height / 2);
            _scale = 1f;
            Draw = true;
        }
        public Texture2D Normal
        {
            get { return _normal; }
            set { _normal = value; _origin = new Vector2(_normal.Width / 2, _normal.Height / 2); }
        }
        public Texture2D Click
        {
            get { return _click; }
            set { _click = value; }
        }
        /// <summary>
        /// Get button position
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// Position offset
        /// </summary>
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        /// <summary>
        /// Get a texture that is needed to be drawn in current state
        /// </summary>
        public Texture2D Texture 
        {
            get 
            {
                switch (_buttonState)
                {
                    case GameButtonState.Normal:
                        return _normal;
                        
                    case GameButtonState.Hovering:
                        return null;

                    case GameButtonState.Click:
                        return _click;
                    default:
                        return _normal;
                }
            } 
        }
        /// <summary>
        /// Intersection rectangle, offseted by position and origin
        /// </summary>
        public Rectangle Rectangle
        { 
            get 
            {
                Rectangle ret = _normal.Bounds;
                ret.Width=(int)(ret.Width*_scale);
                ret.Height = (int)(ret.Height * _scale);
                ret.X = (int)_position.X - (int)(_origin.X*_scale);
                ret.Y = (int)_position.Y - (int)(_origin.Y*_scale);
                return ret;
            } 
        }
        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }
        /// <summary>
        /// State 
        /// </summary>
        public GameButtonState ButtonState 
        { 
            get { return _buttonState; } 
            set { _buttonState = value; }
        }
        /// <summary>
        /// Value that identifies the button
        /// </summary>
        public int Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Should this button be rendered
        /// </summary>
        public bool Draw
        {
            get;
            set;
        }
        public bool DrawOverlay
        {
            get;
            set;
        }
        /// <summary>
        /// Action to be called on click
        /// </summary>
        public EventHandler OnClick;
    }
}
