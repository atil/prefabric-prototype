using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric.LevelEditor
{
    public class EditorTileSelectedEvent : PfEvent
    {
        public PfResourceType SelectedTileResource { get; set; }
    }

    public class EditorTileButton : MonoBehaviour
    {
        public Text Text;

        public void Init(PfResourceType tileResourceType)
        {
            Text.text = tileResourceType.ToString();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                MessageBus.Publish(new EditorTileSelectedEvent()
                {
                    SelectedTileResource = tileResourceType
                });
            });
        }
    }
}
