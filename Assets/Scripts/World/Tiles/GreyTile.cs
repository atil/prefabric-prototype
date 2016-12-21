using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prefabric
{
    public class GreyTile : Tile
    {
        public override bool IsInteractable
        {
            get
            {
                return false;
            }
        }
    }
}
