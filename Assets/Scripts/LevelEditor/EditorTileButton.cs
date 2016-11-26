using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
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

        private Button _button;

        public void Init(PfResourceType tileResourceType)
        {
            _button = GetComponent<Button>();

            Text.text = tileResourceType.ToString();
            _button.onClick.AddListener(() =>
            {
                MessageBus.Publish(new EditorTileSelectedEvent()
                {
                    SelectedTileResource = tileResourceType
                });
            });

            MessageBus.OnEvent<EditorCameraStateChangedEvent>().Subscribe(ev =>
            {
                _button.interactable = !ev.IsActive;
            });

            _button.interactable = false;
        }
    }
}
