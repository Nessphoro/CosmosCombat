using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CosmosCombat
{
    public enum PlayerType
    {
        Player,AI,Neutral
    }
    public class MapState
    {
        public List<Planet> Planets
        {
            get;
            set;
        }
        public List<Fleet> Fleets
        {
            get;
            set;
        }

        public List<Planet> GetMyPlanets(PlayerType type)
        {
            List<Planet> ret = new List<Planet>();
            foreach (Planet p in Planets)
            {
                if (p.Owner == type)
                {
                    ret.Add(p);
                }
            }
            return ret;
        }
        public List<Planet> GetEnemyPlanets(PlayerType type)
        {
            List<Planet> ret = new List<Planet>();
            foreach (Planet p in Planets)
            {
                if (p.Owner != type)
                {
                    ret.Add(p);
                }
            }
            return ret;
        }

        public List<Planet> PlayerPlanets
        {
            get
            {
                List<Planet> ret=new List<Planet>();
                foreach (Planet p in Planets)
                {
                    if (p.Owner == PlayerType.Player)
                    {
                        ret.Add(p);
                    }
                }
                return ret;
            }
        }
        public List<Planet> AIPlanets
        {
            get
            {
                List<Planet> ret = new List<Planet>();
                foreach (Planet p in Planets)
                {
                    if (p.Owner == PlayerType.AI)
                    {
                        ret.Add(p);
                    }
                }
                return ret;
            }
        }
        public List<Planet> NeutralPlanets
        {
            get
            {
                List<Planet> ret = new List<Planet>();
                foreach (Planet p in Planets)
                {
                    if (p.Owner == PlayerType.Neutral)
                    {
                        ret.Add(p);
                    }
                }
                return ret;
            }
        }
    }
    public class GameManger
    {
        public MapState State
        {
            get;
            set;
        }
        Random rnd=new Random();
        public float PlayerAttackBias = 1.5f;
        public float AIAttackBias = 0.5f;

        public GameManger()
        {
            State = new MapState();
            State.Fleets = new List<Fleet>();
            State.Planets = new List<Planet>();
        }

        public void SendFleet(int Ammount, Planet Origin, Planet Destination)
        {
            if (Origin.ID == Destination.ID)
                return;
            if (Origin.Forces[Origin.Owner] < Ammount)
                return;
            Origin.Forces[Origin.Owner] -= Ammount;

            Fleet fleet = new Fleet();
            fleet.Owner = Origin.Owner;
            //Put the ship on the edge
            Vector2 Normalized = Destination.Position - Origin.Position;
            Normalized.Normalize();
            fleet.Rotation = (float)Math.Atan2(Normalized.Y, Normalized.X) + MathHelper.ToRadians(90);
            Normalized *= Origin.PlanetSize * 64;

            fleet.Position = Origin.Position+Normalized;
            fleet.Origin = Origin;
            fleet.Destination = Destination;
            fleet.Count = Ammount;

            int ExtraShipCount = 0;
            if (fleet.Count-10 >= 10)
                ExtraShipCount = (fleet.Count-10) / 10;
            fleet.Positions = new Vector2[ExtraShipCount];
            Normalized.Normalize();

            for (int i = 0; i < ExtraShipCount; i++)
            {
                float Angle = MathHelper.ToRadians((float)(rnd.Next(360) + rnd.NextDouble()));
                float Distance=8+(float)Math.Sqrt(rnd.NextDouble())*((1+(float)Math.Sqrt(ExtraShipCount)/2)*8);
                fleet.Positions[i] =  new Vector2((float)Math.Cos(Angle)*Distance,(float)Math.Sin(Angle)*Distance);

                foreach (Vector2 p in fleet.Positions)
                {
                    if (p == null||p==fleet.Positions[i])
                        continue;

                    if(Vector2.Distance(p,fleet.Positions[i])<16)
                    {
                        Angle = MathHelper.ToRadians((float)(rnd.Next(360) + rnd.NextDouble()));
                        Distance = 8 + (float)Math.Sqrt(rnd.NextDouble()) * ((1 + (float)Math.Sqrt(ExtraShipCount)/2) * 8);
                        fleet.Positions[i] = new Vector2((float)Math.Cos(Angle) * Distance, (float)Math.Sin(Angle) * Distance);
                    }
                }
            }

            State.Fleets.Add(fleet);
        }
        public void SendFleet(int Ammount, int OriginID, int DestinationID)
        {
            Planet Origin=null, Destination=null;
            foreach (Planet planet in State.Planets)
            {
                if (planet.ID == OriginID)
                    Origin = planet;
                else if (planet.ID == DestinationID)
                    Destination = planet;
            }
            if (Origin == null || Destination == null)
                return;
            SendFleet(Ammount, Origin, Destination);
        }
        public bool GameEnd()
        {
            bool ret = false;
            PlayerType looser=PlayerType.Neutral;
            if (State.PlayerPlanets.Count == 0)
            {
                looser = PlayerType.Player;
                ret = true;
            }
            if (State.AIPlanets.Count == 0)
            {
                looser = PlayerType.AI;
                ret = true;
            }
            if (looser!=PlayerType.Neutral)
            {
                foreach (Planet p in State.Planets)
                {
                    if (p.Forces.ContainsKey(looser))
                        ret = false;
                }
            }
            if (looser != PlayerType.Neutral&&ret==true)
            {
                foreach (Fleet f in State.Fleets)
                {
                    if (f.Owner == looser)
                        ret = false;
                }
            }

            return ret;
        }
        public PlayerType GetLooser()
        {
            bool ret = false;
            PlayerType looser = PlayerType.Neutral;
            if (State.PlayerPlanets.Count == 0)
            {
                looser = PlayerType.Player;
                ret = true;
            }
            if (State.AIPlanets.Count == 0)
            {
                looser = PlayerType.AI;
                ret = true;
            }
            if (looser != PlayerType.Neutral)
            {
                foreach (Planet p in State.Planets)
                {
                    if (p.Forces.ContainsKey(looser))
                        ret = false;
                }
            }
            if (looser != PlayerType.Neutral && ret == true)
            {
                foreach (Fleet f in State.Fleets)
                {
                    if (f.Owner == looser)
                        ret = false;
                }
            }

            return looser;
        }
        public void Update()
        {
            
            //Process planets
            foreach (Planet planet in State.Planets)
            {
                //Process growth
                if (planet.Owner != PlayerType.Neutral && !planet.InStateOfWar)
                {
                    planet.GrowthCounter--;
                    if (planet.GrowthCounter <= 0)
                    {
                        planet.Forces[planet.Owner] += planet.Growth;
                        planet.GrowthCounter = planet.GrowthReset;
                    }
                }

                //Check for battle
                if (planet.Forces.Count >= 2)
                {
                    
                    planet.InStateOfWar = true;
                    //War is on
                    planet.FightCounter--;
                    if (planet.FightCounter <= 0)
                    {
                        planet.FightCounter = 10;

                        float PlayerMultiplier=((float)planet.Forces[PlayerType.Player]) /150;
                        float AIMultiplier = ((float)planet.Forces[PlayerType.AI])/150;

                        int PlayerForce = planet.Forces[PlayerType.Player] -1- (int)(1 * AIMultiplier);
                        if (rnd.NextDouble() < 0.2)
                            PlayerForce--;
                        int AIForce = planet.Forces[PlayerType.AI] -1 - (int)(1 * PlayerMultiplier);
                        if (rnd.NextDouble() < 0.2)
                            AIForce--;

                        planet.Forces[PlayerType.Player] = PlayerForce;
                        planet.Forces[PlayerType.AI] = AIForce;
                        if (PlayerForce <= 0 && AIForce <= 0)
                        {
                            //Both forces are at zero, break the tie by giving the owner the win
                            planet.InStateOfWar = false;
                            planet.Forces.Remove(planet.Owner == PlayerType.Player ? PlayerType.AI : PlayerType.Player);
                        }
                        else if (PlayerForce > 0 && AIForce > 0)
                        {
                            //War is still on, just to make sure for the case below
                        }
                        else
                        {
                            //One is a winner
                            planet.InStateOfWar = false;
                            planet.Owner = PlayerForce <= 0 ? PlayerType.AI : PlayerType.Player;
                            planet.Forces.Remove(planet.Owner == PlayerType.Player ? PlayerType.AI : PlayerType.Player);
                        }
                    }

                    
                }
            }

            List<Fleet> DeadFleets = new List<Fleet>();
            foreach (Fleet fleet in State.Fleets)
            {
                if (Vector2.Distance(fleet.Position, fleet.Destination.Position) < fleet.Destination.PlanetSize*64)
                {
                    //We have arrived at a planet
                    if (fleet.Destination.Owner == PlayerType.Neutral && fleet.Destination.Forces[PlayerType.Neutral] < fleet.Count)
                    {
                        //If the owner is neutral capture/attack is instantanious; otherwise process in the next turn
                        fleet.Destination.Owner = fleet.Owner;
                        fleet.Destination.Forces.Add(fleet.Owner, fleet.Count - fleet.Destination.Forces[PlayerType.Neutral]);
                        fleet.Destination.Forces.Remove(PlayerType.Neutral);
                    }
                    else if (fleet.Destination.Owner == PlayerType.Neutral)
                    {
                        fleet.Destination.Forces[PlayerType.Neutral] -= fleet.Count;
                    }
                    else
                    {
                        //We need to go in the state of war (if we're attacking)
                        if (fleet.Destination.Owner != fleet.Owner)
                      //fleet.Destination.InStateOfWar = true;
                            
                        {
                            int AfterFight = fleet.Destination.Forces[fleet.Destination.Owner] - (int)(fleet.Count*(fleet.Owner==PlayerType.Player?PlayerAttackBias:AIAttackBias));
                            if (AfterFight < 0)
                            {

                                fleet.Destination.Forces.Add(fleet.Owner, (int)(fleet.Count*(fleet.Owner==PlayerType.Player?PlayerAttackBias:AIAttackBias)) - fleet.Destination.Forces[fleet.Destination.Owner]);
                                fleet.Destination.Forces.Remove(fleet.Destination.Owner);
                                fleet.Destination.Owner = fleet.Owner;
                            }
                            else
                            {
                                fleet.Destination.Forces[fleet.Destination.Owner] = AfterFight;
                            }
                        }
                        else
                        {
                            fleet.Destination.Forces[fleet.Owner] += fleet.Count;
                        }
                             
                        
                        //if (!fleet.Destination.Forces.ContainsKey(fleet.Owner))
                        //    fleet.Destination.Forces.Add(fleet.Owner, fleet.Count);
                        //else
                        //{
                        //    fleet.Destination.Forces[fleet.Owner] += fleet.Count;
                        //}
                        
                    }
                    DeadFleets.Add(fleet);
                }
                else
                {
                    //Enroute
                    Vector2 DirectionVector=fleet.Destination.Position - fleet.Position;
                    DirectionVector.Normalize();
                    fleet.Position +=DirectionVector*1.15f;
                }
            }
            foreach (Fleet dead in DeadFleets)
            {
                State.Fleets.Remove(dead);
            }
        }
    }
}
