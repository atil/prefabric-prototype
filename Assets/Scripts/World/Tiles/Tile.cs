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

    public struct TileState
    {
        public Vector3 Position { get; private set; }
        public bool IsBent { get; private set; }

        public TileState(Vector3 pos, bool isBent) : this()
        {
            Position = pos;
            IsBent = isBent;
        }
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

        private const float BendMoveSpeed = 1.5f;
        private const float BendFadeSpeed = 1.5f;

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

        public virtual bool IsInteractable { get { return true; } }

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
        private BoxCollider _collider;

        private readonly Stack<TileState> _history = new Stack<TileState>();
        private Vector3 _initialPosition;

        public PfResourceType ResourceType { get; private set; }

        public virtual void Init(Guid id, PfResourceType resType)
        {
            Transform = transform;
            Id = id;
            _id = id.ToString();
            ResourceType = resType;
            _material = GetComponent<Renderer>().material;
            _normalColor = _material.color;
            VisualState = TileVisualState.Normal;
            _initialPosition = Position;
            _collider = GetComponent<BoxCollider>();
        }

        public void ExternalUpdate()
        {
        }

        private IEnumerator TweenCoroutine(Vector3 targetPosition)
        {
            var startPosition = Position;
            for (var f = 0f; f < 1f; f += PfTime.DeltaTime * BendMoveSpeed)
            {
                Position = Vector3.Lerp(startPosition, targetPosition, Curve.Instance.TileTween.Evaluate(f));
                yield return null;
            }

            Position = targetPosition;

            MessageBus.Publish(new TileTweenCompletedEvent() { Tile = this });
        }

        public void Bend(TileState nextState)
        {
            var initiallyActive = _history.Count == 0 || !_history.Peek().IsBent;
            _history.Push(nextState);

            var activeAfterBend = !nextState.IsBent;
            if (initiallyActive && !activeAfterBend)
            {
                FadeOut();
            }

            CoroutineStarter.StartCoroutine(TweenCoroutine(nextState.Position));
        }

        public void Unbend()
        {
            if (_history.Count == 0)
            {
                return;
            }

            var initiallyActive = !_history.Peek().IsBent;
            _history.Pop();

            Vector3 targetPosition;
            bool activeAfterBend;
            if (_history.Count != 0)
            {
                var targetState = _history.Peek();
                targetPosition = targetState.Position;
                activeAfterBend = !targetState.IsBent;
            }
            else // Empty history, return to initial state
            {
                targetPosition = _initialPosition;
                activeAfterBend = true;
            }

            if (!initiallyActive && activeAfterBend)
            {
                FadeIn();
            }

            CoroutineStarter.StartCoroutine(TweenCoroutine(targetPosition));
        }

        protected virtual void FadeOut()
        {
            var f = 1f;
            IDisposable fadeoutDisposable = null;
            fadeoutDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                _material.SetAlpha(Curve.Instance.TileTweenFade.Evaluate(f));
                f -= PfTime.DeltaTime * BendFadeSpeed;

                if (f < 0.001)
                {
                    _material.SetAlpha(0f);
                    fadeoutDisposable.Dispose();
                }
            });

            _collider.enabled = false;
        }

        protected virtual void FadeIn()
        {
            var f = 1f;
            IDisposable fadeoutDisposable = null;
            fadeoutDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                _material.SetAlpha(Curve.Instance.TileTweenFade.Evaluate(f));
                f += PfTime.DeltaTime * BendFadeSpeed;

                if (f > 0.999)
                {
                    _material.SetAlpha(1f);
                    fadeoutDisposable.Dispose();
                }
            });

            _collider.enabled = true;

        }
    }
}
