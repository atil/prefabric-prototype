using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public enum TileState
    {
        None,
        Normal,
        Hovered,
        Selected,
    }

    public class Tile : MonoBehaviour
    {
        public static bool IsAxisAligned(Tile tile1, Tile tile2)
        {
            var v1 = tile1.Position;
            var v2 = tile2.Position;

            Vector3 result = new Vector3((int)v1.x ^ (int)v2.x, (int)v1.y ^ (int)v2.y, (int)v1.z ^ (int)v2.z);

            if (!Mathf.Approximately(result.x, 0f)) result.x = 1f;
            if (!Mathf.Approximately(result.y, 0f)) result.y = 1f;
            if (!Mathf.Approximately(result.z, 0f)) result.z = 1f;

            return Mathf.Approximately(result.sqrMagnitude, 1);
        }

        // These are to be removed when we move onto a custom shader
        // (which we'll do eventually)
        [SerializeField]
        private Color _normalColor;
        [SerializeField]
        private Color _hoverColor;
        [SerializeField]
        private Color _selectColor;

        [SerializeField] // To make it appear in the inspector
        private string _id;
        public Guid Id { get; private set; }

        // These shortcuts really come in handy
        public Transform Transform { get; private set; }
        public Vector3 Position { get { return Transform.position; } set { Transform.position = value; } }

        private TileState _state;
        public TileState VisualState
        {
            get { return _state; }
            set
            {
                switch (value)
                {
                    case TileState.None:
                        break;
                    case TileState.Normal:
                        _material.color = _normalColor;
                        break;
                    case TileState.Hovered:
                        _material.color = _hoverColor;
                        break;
                    case TileState.Selected:
                        _material.color = _selectColor;
                        break;
                    default:
                        break;
                }
                _state = value;
            }
        }


        // For now, I think going for shared material optimization is not needed
        // We won't have thousands of cubes, like we do in Fabric
        private Material _material;

        public virtual void Init(Guid id)
        {
            Transform = transform;
            Id = id;
            _material = GetComponent<Renderer>().material;
            _normalColor = _material.color;
            VisualState = TileState.Normal;
        }

        public void ExternalUpdate()
        {

        }


    }
}
