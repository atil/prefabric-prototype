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
        public override void Init(Guid id)
        {
            base.Init(id);

            // No enzones in the editor
            // I should probably centralize this game / editor distinction
            if (SceneManager.GetActiveScene().name == "LevelEditorScene")
            {
                return;
            }

            var endZonePrefab = PfResources.Load<GameObject>(PfResourceType.EndZone);

            for (int i = 0; i < 10; i++)
            {
                var endZoneGo = Instantiate(endZonePrefab, Position + Vector3.up * (i + 1), Quaternion.identity);

                if (i != 0)
                {
                    Destroy(endZoneGo.GetComponent<Collider>());
                }
            }

        }
    }
}
