using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CosmosCombat.AI
{
    public interface AI
    {
        void Update(MapState state,GameManger Manager);
    }
}
