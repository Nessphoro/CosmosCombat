using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;


namespace MapEditor
{
    public class Planet
    {
        public Vector2 Position
        {
            get;
            set;
        }
        public float PlanetSize
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
        public PlayerType Owner
        {
            get;
            set;
        }
        public int Forces
        {
            get;
            set;
        }

        public int Growth
        {
            get;
            set;
        }
        public int GrowthCounter
        {
            get;
            set;
        }
    }
}
