using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Prefabric
{
    public enum PfResourceType
    {
        WhiteTile,
    }

	public static class PfResources
	{
        private static List<string> _levelPaths = new List<string>();
        private static Dictionary<PfResourceType, string> _resourcePaths = new Dictionary<PfResourceType, string>();

		static PfResources()
        {
            var json = JSON.Parse(Resources.Load<TextAsset>("levelPaths").text);
            foreach (JSONNode pathString in json["paths"].AsArray)
            {
                _levelPaths.Add(pathString.Value);
            }

            _resourcePaths.Add(PfResourceType.WhiteTile, "Prefabs/Tiles/WhiteTile");
        }

        public static string LevelStringOf(int index)
        {
            var textAsset = Resources.Load<TextAsset>(_levelPaths[index]);
            if (textAsset == null)
            {
                Debug.LogError("No level found for index : " + 0);
                return string.Empty; 
            }
            return textAsset.text;
        }

        public static T Load<T>(PfResourceType resType) where T : Object
        {
            if (!_resourcePaths.ContainsKey(resType))
            {
                Debug.LogError("Resource doesn't exist : " + resType);
                return null;
            }

            return Resources.Load<T>(_resourcePaths[resType]);
        }
	}
}