using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

namespace Prefabric
{
    public class TileTweenCompletedEvent : PfSceneEvent
    {
        public Tile Tile { get; set; }
    }

    public enum TileVisualState
    {
        None,
        Normal,
        Hovered,
        Selected,
    }

    public class TileState
    {
        public Vector3 Position { get; set; }
        public bool IsBent { get; set; } // Might not be needed
    }

    public class Tile : MonoBehaviour
    {
        public static bool IsAxisAligned(Tile tile1, Tile tile2)
        {
            var v1 = tile1.Position;
            var v2 = tile2.Position;

            var result = new Vector3((int)v1.x ^ (int)v2.x, (int)v1.y ^ (int)v2.y, (int)v1.z ^ (int)v2.z);

            if (!Mathf.Approximately(result.x, 0f)) result.x = 1f;
            if (!Mathf.Approximately(result.y, 0f)) result.y = 1f;
            if (!Mathf.Approximately(result.z, 0f)) result.z = 1f;

            return Mathf.Approximately(result.sqrMagnitude, 1f);
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

        public bool IsBent
        {
            get { return _history.Count > 0 && _history.Peek().IsBent; }
        }

        private TileVisualState _state;
        public TileVisualState VisualState
        {
            get { return _state; }
            set
            {
                switch (value)
                {
                    case TileVisualState.None:
                        break;
                    case TileVisualState.Normal:
                        _material.color = _normalColor;
                        break;
                    case TileVisualState.Hovered:
                        _material.color = _hoverColor;
                        break;
                    case TileVisualState.Selected:
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

        private readonly Stack<TileState> _history = new Stack<TileState>();
        private Vector3 _initialPosition;

        public virtual void Init(Guid id)
        {
            Transform = transform;
            Id = id;
            _material = GetComponent<Renderer>().material;
            _normalColor = _material.color;
            VisualState = TileVisualState.Normal;
            _initialPosition = Position;
        }

        public void ExternalUpdate()
        {
        }

        private IEnumerator TweenCoroutine(Vector3 targetPosition)
        {
            while (Vector3.Distance(targetPosition, Position) > 0.1)
            {
                Position = Vector3.Lerp(Position, targetPosition, Time.deltaTime * 5);
                yield return null;
            }
            Position = targetPosition;

            MessageBus.Publish(new TileTweenCompletedEvent() {Tile = this});
        }

        public void Bend(TileState nextState)
        {
            _history.Push(nextState);
            CoroutineStarter.StartCoroutine(TweenCoroutine(nextState.Position));
        }

        public void Unbend()
        {
            if (_history.Count == 0)
            {
                return;
            }

            _history.Pop();

            var targetPosition = _history.Count == 0 ? _initialPosition : _history.Peek().Position;
            CoroutineStarter.StartCoroutine(TweenCoroutine(targetPosition));
        }

        
    }
}
