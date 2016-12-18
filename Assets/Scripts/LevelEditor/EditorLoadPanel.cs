using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric.LevelEditor
{
    public class EditorLoadPanel : MonoBehaviour
    {
        public Button LoadButton;
        public Transform ButtonsParent;
        public Text LevelNameText;

        private readonly List<GameObject> _buttons = new List<GameObject>();
        private GameObject _fileButtonPrefab;
        private string _levelsPath;

        public void Init()
        {
            _levelsPath = Application.dataPath + "/Levels/";
            _fileButtonPrefab = PfResources.Load<GameObject>(PfResourceType.EditorFileButton);

            LoadButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(LevelNameText.text))
                {
                    SetActive(false);
                    MessageBus.Publish(new EditorLoadLevelEvent() {Path = LevelNameText.text + ".json" });
                }
            });
        }

        public void SetActive(bool isActive)
        {
            foreach (var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            if (isActive)
            {
                foreach (var file in Directory.GetFiles(_levelsPath).Where(f => f.EndsWith("json")))
                {
                    var button = Instantiate(_fileButtonPrefab);
                    button.GetComponentInChildren<Text>().text =
                        Path.GetFileNameWithoutExtension(file.Split('\\').Last()); // Show only file name


                    button.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        LevelNameText.text = button.GetComponentInChildren<Text>().text;
                    });

                    button.transform.SetParent(ButtonsParent);
                    _buttons.Add(button);
                }

            }
            gameObject.SetActive(isActive);
        }
    }
}
