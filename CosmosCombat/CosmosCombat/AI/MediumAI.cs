using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace CosmosCombat.AI
{
    class MediumAI:AI
    {
        public void Update(MapState state, GameManger Manager)
        {
            List<Fleet> fleets = state.Fleets;
            foreach (Planet myPlanet in state.GetMyPlanets(PlayerType.AI))
            {
                Planet Attack = null;
                float LowestScore = float.PositiveInfinity;

                foreach (Planet planet in state.GetEnemyPlanets(PlayerType.AI))
                {

                    bool Flying = false;
                    foreach (Fleet f in fleets)
                    {
                        if (f.Owner == PlayerType.AI)
                        {
                            if (f.Destination.ID == planet.ID)
                            {
                                Flying = true;
                                break;
                            }
                        }
                    }

                    if (Flying)
                        continue;

                    //float Score = Vector2.DistanceSquared(planet.Position, myPlanet.Position) * (((float)planet.Forces[planet.Owner]) / myPlanet.Forces[myPlanet.Owner])/planet.Growth;
                    float Score = Vector2.Distance(planet.Position, myPlanet.Position) / 5 - planet.PlanetSize * 80 + planet.Forces[planet.Owner] * 2;
                    if (Score < LowestScore)
                    {
                        Attack = planet;
                        LowestScore = Score;
                    }
                }

                if (Attack != null)
                {
                    if ((Attack.Forces[Attack.Owner] + 1)/Manager.AIAttackBias < myPlanet.Forces[myPlanet.Owner] / 1.25f)
                    {
                        Manager.SendFleet((int)(myPlanet.Forces[myPlanet.Owner] / 1.25f), myPlanet, Attack);
                    }
                }
            }
        }
    }
}
