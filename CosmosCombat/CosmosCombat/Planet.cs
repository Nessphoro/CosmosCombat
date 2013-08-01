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
    public class Planet
    {

        public Vector2 Position
        {
            get;
            set;
        }
        public Texture2D Texture
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
        public bool InStateOfWar
        {
            get;
            set;
        }
        public Dictionary<PlayerType, int> Forces
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
        public int GrowthReset
        {
            get;
            set;
        }

        public int FightCounter
        {
            get;
            set;
        }

        public Planet()
        {
            Position = Vector2.Zero;
            ID = 0;
            Owner = PlayerType.Neutral;
            InStateOfWar = false;
            Forces = new Dictionary<PlayerType, int>();
            Growth = 0;
        }
    }
}
