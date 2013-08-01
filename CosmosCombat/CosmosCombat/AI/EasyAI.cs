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
    class EasyAI:AI
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
                    float Score = 2*Vector2.Distance(planet.Position, myPlanet.Position)*(((float)planet.Forces[planet.Owner])/myPlanet.Forces[myPlanet.Owner]);
                    if (planet.Owner == PlayerType.Player)
                    {
                        Score *= 10;
                    }
                    if (Score < LowestScore)
                    {
                        Attack = planet;
                        LowestScore = Score;
                    }
                }

                if (Attack != null)
                {
                    if ((Attack.Forces[Attack.Owner]+1)/Manager.AIAttackBias < myPlanet.Forces[myPlanet.Owner]/2)
                    {
                        Manager.SendFleet(myPlanet.Forces[myPlanet.Owner] / 2, myPlanet, Attack);
                    }
                    return;
                }
            }
        }
    }
}
