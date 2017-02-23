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
        InBetweenHighlighted,
        Bent,
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
        #region Alignment utils
        public static bool IsAxisAligned(Tile tile1, Tile tile2)
        {
            var v1 = tile1.Position;
            var v2 = tile2.Position;

            var result = new Vector3((int)v1.x ^ (int)v2.x, (int)v1.y ^ (int)v2.y, (int)v1.z ^ (int)v2.z);

            if (!Util.Approx(result.x, 0f)) result.x = 1f;
            if (!Util.Approx(result.y, 0f)) result.y = 1f;
            if (!Util.Approx(result.z, 0f)) result.z = 1f;

            return Util.Approx(result.sqrMagnitude, 1f);
        }

        public static Vector3 AlignedDir(Tile tile1, Tile tile2)
        {
            // This function is called too many times
            // Either eliminate those cases and/or optimize these Approx'es
            var p1 = tile1.Position;
            var p2 = tile2.Position;

            if (Util.Approx(p1.y, p2.y) 
                && Util.Approx(p1.z, p2.z))
            {
                return Vector3.right;
            }
            else if (Util.Approx(p1.x, p2.x) 
                && Util.Approx(p1.z, p2.z))
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.forward;
            }
        }

        public static bool IsInBetween(Tile tile, Tile bender1, Tile bender2)
        {
            // tile1 and tile2 are assumed to be aligned
            var alignedDir = AlignedDir(bender1, bender2);

            // Tell if tile's coord along alignedDir is in between of bender1's and bender2's
            return Vector3.Dot(tile.Position, alignedDir)
                .InBetween(Vector3.Dot(bender1.Position, alignedDir), Vector3.Dot(bender2.Position, alignedDir));
        }
        #endregion

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

        [SerializeField] // To make it appear in the inspector
        private TileVisualState _state;
        public TileVisualState VisualState
        {
            get { return _state; }
            set
            {
                if (IsBent) // Inactive tiles don't go into this
                {
                    return;
                }

                switch (value)
                {
                    case TileVisualState.None:
                        break;
                    case TileVisualState.Normal:
                        _material.SetRgb(_normalColor);
                        _state = value;
                        break;
                    case TileVisualState.Hovered:
                        if (_state == TileVisualState.Selected)
                        {
                            return;
                        }
                        _material.SetRgb(_hoverColor);
                        _state = value;
                        break;
                    case TileVisualState.Selected:
                        _material.SetRgb(_selectColor);
                        _state = value;
                        break;
                    case TileVisualState.InBetweenHighlighted:
                        if (_state == TileVisualState.Selected)
                        {
                            return;
                        }
                        _material.SetRgb(_selectColor);
                        _state = value;
                        break;
                    default:
                        break;
                }
            }
        }

        // For now, I think going for shared material optimization is not needed
        // We won't have thousands of cubes, like we do in Fabric
        private Material _material;

        private readonly Stack<TileState> _history = new Stack<TileState>();
        private Vector3 _initialPosition;

        public PfResourceType ResourceType { get; private set; }

        // This is called as soon as the tile is instantiated
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

            // Higher greytiles will look darker
            // This way, it's way easier to distinguish walls from the floor level
            // Note that this requires Position to be selected before Init() call
            if (ResourceType == PfResourceType.GreyTile)
            {
                var c = (1 - ((Position.y % 3) / 3)) * Color.white;
                _normalColor = c;
                _material.SetRgb(c);
            }
        }

        // This is a replacement for Unity's Update() function
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

            // Should be careful if anything in this lambda's body is dependent 
            // to any other thing updated in ExternalUpdate()
            // Because we don't know where UniRx plugs in EveryUpdate() call in the frame
            // (actually we can find out, but I'm too lazy)
            fadeoutDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                Transform.localScale = Vector3.one * f;
                _material.SetAlpha(Curve.Instance.TileTweenFade.Evaluate(f));

                f -= PfTime.DeltaTime * BendFadeSpeed;

                if (f < 0.001)
                {
                    _material.SetAlpha(0f);
                    gameObject.SetActive(false);
                    fadeoutDisposable.Dispose();
                }
            });
        }

        protected virtual void FadeIn()
        {
            gameObject.SetActive(true);

            var f = 0f;
            IDisposable fadeoutDisposable = null;
            fadeoutDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                var t = Curve.Instance.TileTweenFadeIn.Evaluate(f);
                Transform.localScale = Vector3.one * t;
                _material.SetAlpha(t);

                f += PfTime.DeltaTime * BendFadeSpeed;

                if (f > 0.999)
                {
                    _material.SetAlpha(1f);
                    fadeoutDisposable.Dispose();
                }
            });
        }
    }
}
