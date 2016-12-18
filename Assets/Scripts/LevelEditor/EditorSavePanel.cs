using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Prefabric.LevelEditor
{
    public class EditorSavePanel : MonoBehaviour
    {
        public Button SaveButton;
        public InputField LevelNameField;
        public Transform ButtonsParent;

        private readonly List<GameObject> _buttons = new List<GameObject>();
        private GameObject _fileButtonPrefab;
        private string _levelsPath;

        public void Init()
        {
            _levelsPath = Application.dataPath + "/Levels/"; // This'll change with build
            _fileButtonPrefab = PfResources.Load<GameObject>(PfResourceType.EditorFileButton);

            SaveButton.onClick.AddListener(() =>
            {
                var lvlPath = _levelsPath + LevelNameField.text + ".json";

                if (File.Exists(lvlPath))
                {
                    // TODO: "Are you sure" check?
                }

                if (!string.IsNullOrEmpty(LevelNameField.text))
                {
                    SetActive(false);
                    MessageBus.Publish(new EditorSaveLevelEvent() {Path = lvlPath });
                }
            });
        }

        public void SetActive(bool isActive)
        {
            foreach (var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();

            if (isActive)
            {
                foreach (var file in Directory.GetFiles(_levelsPath).Where(f => f.EndsWith("json")))
                {
                    var button = Instantiate(_fileButtonPrefab);
                    button.GetComponentInChildren<Text>().text = 
                        Path.GetFileNameWithoutExtension(file.Split('\\').Last()); // Show only file name

                    button.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        LevelNameField.text = button.GetComponentInChildren<Text>().text;
                    });

                    button.transform.SetParent(ButtonsParent);
                    _buttons.Add(button);
                }

            }
            gameObject.SetActive(isActive);
        }
    }
}
