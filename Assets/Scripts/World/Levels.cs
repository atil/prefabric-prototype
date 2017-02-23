using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Prefabric
{
    public static class Levels
    {
        public static readonly ReadOnlyCollection<string> Paths;

        static Levels()
        {
            var a = new List<string>()
            {
                "singleBend.json",
                "alignmentChain4.json",
                "bendingWalls.json",
                "inBetween.json",
            };
            Paths = new ReadOnlyCollection<string>(a);
        }
    }
}
