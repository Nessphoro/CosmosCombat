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
    class HardAI : AI
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
                    float Score = Vector2.Distance(planet.Position, myPlanet.Position) / 5 - ((((float)planet.Forces[planet.Owner]) / myPlanet.Forces[myPlanet.Owner]) * 10) - planet.Growth * 20 + planet.Forces[planet.Owner]*2;
                    if (Score < LowestScore)
                    {
                        Attack = planet;
                        LowestScore = Score;
                    }
                }

                if (Attack != null)
                {
                    if ((Attack.Forces[Attack.Owner] + 1)/Manager.AIAttackBias < myPlanet.Forces[myPlanet.Owner])
                    {
                        Manager.SendFleet((int)((Attack.Forces[Attack.Owner] + 1) / Manager.AIAttackBias), myPlanet, Attack);
                    }
                }
            }
        }
    }
}
