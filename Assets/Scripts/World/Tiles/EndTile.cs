using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prefabric
{
    public class EndTile : Tile
    {
        public override void Init(Guid id, PfResourceType resType)
        {
            base.Init(id, resType);

            // No endzones in the editor
            // I should probably centralize this game / editor distinction
            if (SceneManager.GetActiveScene().name == "LevelEditorScene")
            {
                return;
            }
        }

        protected override void FadeIn()
        {
            base.FadeIn();
            foreach (Transform t in Transform)
            {
                t.gameObject.SetActive(true);
            }
        }

        protected override void FadeOut()
        {
            base.FadeOut();
            foreach (Transform t in Transform)
            {
                t.gameObject.SetActive(false);
            }

        }
    }
}
