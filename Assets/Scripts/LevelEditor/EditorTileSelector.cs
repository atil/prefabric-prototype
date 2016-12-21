using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric.LevelEditor
{
    public class EditorTileSelector : MonoBehaviour
    {
        public RectTransform ButtonsParent;

        public void LoadTileButtons(PfResourceType[] buttonResources)
        {
            var buttonPrefab = PfResources.Load<GameObject>(PfResourceType.EditorTileButton);

            foreach (var res in buttonResources)
            {
                var buttonGo = Instantiate(buttonPrefab);
                buttonGo.transform.SetParent(ButtonsParent);
                buttonGo.GetComponent<EditorTileButton>().Init(res);
            }
        }

    }
}
