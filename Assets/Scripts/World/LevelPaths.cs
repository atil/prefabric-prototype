using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace Prefabric
{
    public static class LevelPaths
    {
        public static ReadOnlyCollection<string> Paths;

        static LevelPaths()
        {
            var tmpList = new List<string>();
            var json = JSON.Parse(PfResources.LoadStringAt("levelPaths.json"));
            foreach (JSONNode pathString in json["paths"].AsArray)
            {
                tmpList.Add(pathString.Value);
            }
            Paths = new ReadOnlyCollection<string>(tmpList);
            
        }
    }
}
