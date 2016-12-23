using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric.LevelEditor
{
    public class EditorTileSelector : MonoBehaviour
    {
        public RectTransform ButtonsParent;

        private ReadOnlyCollection<PfResourceType> _buttonResources;

        public void LoadTileButtons(PfResourceType[] buttonResources)
        {
            _buttonResources = new ReadOnlyCollection<PfResourceType>(buttonResources);

            var buttonPrefab = PfResources.Load<GameObject>(PfResourceType.EditorTileButton);
            foreach (var res in _buttonResources)
            {
                var buttonGo = Instantiate(buttonPrefab);
                buttonGo.transform.SetParent(ButtonsParent);
                buttonGo.GetComponent<EditorTileButton>().Init(res);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MessageBus.Publish(new EditorTileSelectedEvent() { SelectedTileResource = _buttonResources[0] });
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MessageBus.Publish(new EditorTileSelectedEvent() { SelectedTileResource = _buttonResources[1] });
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                MessageBus.Publish(new EditorTileSelectedEvent() { SelectedTileResource = _buttonResources[2] });
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                MessageBus.Publish(new EditorTileSelectedEvent() { SelectedTileResource = _buttonResources[3] });
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                MessageBus.Publish(new EditorTileSelectedEvent() { SelectedTileResource = _buttonResources[4] });
            }

        }

    }
}
