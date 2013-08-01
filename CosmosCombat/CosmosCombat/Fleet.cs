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
    public class Fleet
    {
        public Vector2 Position
        {
            get;
            set;
        }
        public PlayerType Owner
        {
            get;
            set;
        }
        public Planet Origin
        {
            get;
            set;
        }
        public Planet Destination
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public float Rotation
        {
            get;
            set;
        }

        public Vector2[] Positions
        {
            get;
            set;
        }
    }
}
