﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UniRx;

namespace Prefabric
{
    public class TweenCompletedEvent : PfSceneEvent { }

    public class BendEvent : PfSceneEvent
    {
        public Tile Bender1 { get; set; }
        public Tile Bender2 { get; set; }
    }

    public class UnbendEvent : PfSceneEvent { }

    public class BendFailEvent : PfSceneEvent
    {
        public Tile Bender1 { get; set; }
        public Tile Bender2 { get; set; }
    }

    /// <summary>
    /// Handles everything related to tiles
    /// </summary>
    public class MapManager
    {
        private readonly List<AgentBase> _agents;
        private readonly PlayerAgent _player;
        private readonly List<Tile> _tiles;
        private readonly LevelLoader _levelLoader = new LevelLoader();
        
        private Tile _hoverTile;
        private Tile _firstSelectedTile;
        private readonly Stack<Tuple<Tile, Tile>> _bendHistory = new Stack<Tuple<Tile, Tile>>();
        private int _currentTweenerCount;
        private readonly Transform _bendGuide;

        public MapManager(string lvlName, List<AgentBase> agents)
        {
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = _levelLoader.LoadLevelAt(lvlName);

            var startTile = _tiles.Find(x => x is StartTile);
            _player.Position = startTile.Position + Vector3.up * 1.5f;

            _bendGuide = Object.Instantiate(PfResources.Load<GameObject>(PfResourceType.BendGuide)).transform;
            _bendGuide.gameObject.SetActive(false);

            MessageBus.OnEvent<TileSelectCommand>().Subscribe(ev => OnTileSelected(ev.Tile));
            MessageBus.OnEvent<TileHoverCommand>().Subscribe(ev => OnTileHover(ev.Tile));
            MessageBus.OnEvent<TileDeselectCommand>().Subscribe(ev => OnTileDeselect());
            MessageBus.OnEvent<UnbendCommand>().Subscribe(ev => Unbend());
            MessageBus.OnEvent<TileTweenCompletedEvent>().Subscribe(ev => OnTileCompletedTween(ev.Tile));
        }

        private void OnTileHover(Tile tile)
        {
            if (tile == _hoverTile) // Don't do anything if hovering over the same tile
            {
                return;
            }

            // Unselect old hover
            if (_hoverTile != null) // Will be null for the first assignment
            {
                _hoverTile.VisualState = TileVisualState.Normal;
            }

            // Reset inBtwn effect
            foreach (var t in _tiles)
            {
                t.VisualState = TileVisualState.Normal;
            }

            

            // If we are aiming at an axis-aligned tile, highlight the tiles in between them
            if (_firstSelectedTile != null
                && Tile.IsAxisAligned(_firstSelectedTile, tile))
            {
                // Set visual state of appropriate tiles to inbtwn
                foreach (var t in _tiles)
                {
                    if (Tile.IsInBetween(t, _firstSelectedTile, tile))
                    {
                        t.VisualState = TileVisualState.InBetweenHighlighted;
                    }
                }
            }

            if (!tile.IsInteractable) // Don't touch uninteractable tiles
            {
                return;
            }

            _hoverTile = tile;
            _hoverTile.VisualState = TileVisualState.Hovered;
        }

        private void OnTileSelected(Tile tile)
        {
            if (!tile.IsInteractable)
            {
                return;
            }

            // Selecting first tile
            if (_firstSelectedTile == null) 
            {
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileVisualState.Selected;
                _bendGuide.gameObject.SetActive(true);
                _bendGuide.position = _firstSelectedTile.Position;

                Sfx.Play(PfResourceType.SfxTileSelect1);
                return;
            }

            // Selecting two non-aligned tiles
            if (!Tile.IsAxisAligned(_firstSelectedTile, tile))
            {
                _firstSelectedTile.VisualState = TileVisualState.Normal;
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileVisualState.Selected;
                _bendGuide.gameObject.SetActive(true);
                _bendGuide.position = _firstSelectedTile.Position;

                Sfx.Play(PfResourceType.SfxTileSelect1);
                return;
            }

            _bendGuide.gameObject.SetActive(false);

            // Reset tile visuals before bending
            foreach (var t in _tiles)
            {
                t.VisualState = TileVisualState.Normal;
            }

            // Actual bending
            Bend(_firstSelectedTile, tile);

            _firstSelectedTile = null;
        }

        private void OnTileDeselect()
        {
            // Clear firstSelectedTile
            if (_firstSelectedTile != null)
            {
                _firstSelectedTile.VisualState = TileVisualState.Normal;
                _firstSelectedTile = null;
                _bendGuide.gameObject.SetActive(false);
            }
        }

        private void Bend(Tile tile1, Tile tile2)
        {
            // No bending while a tween is in progress
            if (_currentTweenerCount > 0)
            {
                return;
            }

            // The direction on which these tiles are aligned
            var alignedDir = Tile.AlignedDir(tile1, tile2);

            var proj1 = Vector3.Dot(tile1.Position, alignedDir);
            var proj2 = Vector3.Dot(tile2.Position, alignedDir);

            // Player shouldn't stand in between bending tiles
            // (That's gonna be a thing to explain, speaking from experience)
            var playerProj = Vector3.Dot(_player.Position, alignedDir);
            if (playerProj.InBetween(proj1, proj2))
            {
                MessageBus.Publish(new BendFailEvent()
                {
                    Bender1 = tile1,
                    Bender2 = tile2,
                });

                Sfx.Play(PfResourceType.SfxBendFail);

                return;
            }


            // If there's any black tile in between, fail bend
            foreach (var blackTile in _tiles.Where(t => t is BlackTile))
            {
                var blackTileProj = Vector3.Dot(blackTile.Position, alignedDir);
                if (blackTileProj.InBetween(proj1, proj2))
                {
                    MessageBus.Publish(new BendFailEvent()
                    {
                        Bender1 = tile1,
                        Bender2 = tile2,
                    });

                    return;
                }
            }

            // The distance which the tiles are going to be displaced
            // Subtracting 0.5 to snap to grid
            var moveDistance = Vector3.Distance(tile1.Position, tile2.Position) / 2 - 0.5f;
            foreach (var tile in _tiles)
            {
                var tileProj = Vector3.Dot(tile.Position, alignedDir);
                Vector3 targetPos;
                var isBent = false;

                // These 3 cases correspond the 3 zones which are emerged from 
                // dissecting the space with 2 planes
                if (tileProj >= proj1 && tileProj >= proj2) // Upper
                {
                    // Canceled for now
                    //// If this tile is behind a black tile, it will block the movement
                    //var maxProj = Mathf.Max(proj1, proj2);
                    //var tileHits = Physics.RaycastAll(new Ray(tile.Position, -alignedDir), Mathf.Abs(tileProj - maxProj),
                    //    1 << Layer.Tile);
                    //var isBehindBlackTile = tileHits.Any(th => th.transform.GetComponent<Tile>() is BlackTile)
                    //    || tile is BlackTile;
                    //var blackTileCoeff = isBehindBlackTile ? 0f : 1f;

                    targetPos = alignedDir * -1 * moveDistance + tile.Position;
                }
                else if (tileProj <= proj1 && tileProj <= proj2) // Lower
                {
                    // Canceled for now
                    //// If this tile is behind a black tile, it will block the movement
                    //var minProj = Mathf.Min(proj1, proj2);
                    //var tileHits = Physics.RaycastAll(new Ray(tile.Position, alignedDir), Mathf.Abs(minProj - tileProj),
                    //    1 << Layer.Tile);
                    //var isBehindBlackTile = tileHits.Any(th => th.transform.GetComponent<Tile>() is BlackTile)
                    //    || tile is BlackTile;
                    //var blackTileCoeff = isBehindBlackTile ? 0f : 1f;

                    targetPos = alignedDir * moveDistance + tile.Position;
                }
                else // In between -- these are going to be flown away
                {
                    // If doing a vertical bend, add a little randomness to flyAwayDir. Looks better
                    var flyAwayDir = Vector3.down;
                    if (alignedDir == Vector3.up)
                    {
                        flyAwayDir = Random.value > 0.5 ? Vector3.forward : Vector3.right;
                    }

                    // Fly away
                    targetPos = tile.Position 
                        + flyAwayDir 
                            * Random.Range(5f, 15f)
                            * Mathf.Sign(Random.Range(-1f, 1f)); // Randomly, up or down
                    isBent = true;
                }

                tile.Bend(new TileState(targetPos, isBent));
            }

            _currentTweenerCount = _tiles.Count;

            MessageBus.Publish(new BendEvent()
            {
                Bender1 = tile1,
                Bender2 = tile2,
            });

            _bendHistory.Push(new Tuple<Tile, Tile>(tile1, tile2));

            Sfx.Play(PfResourceType.SfxTileSelect2);

        }

        private void Unbend()
        {
            if (_currentTweenerCount > 0 // No unbending while another tweening is active
                || _bendHistory.Count == 0) // ..or nothing to unbend
            {
                return;
            }

            _bendHistory.Pop();

            _currentTweenerCount = _tiles.Count;
            foreach (var tile in _tiles)
            {
                tile.Unbend();
            }

            Sfx.Play(PfResourceType.SfxUnbend);

            MessageBus.Publish(new UnbendEvent());
        }

        private void OnTileCompletedTween(Tile tile)
        {
            _currentTweenerCount--;
            if (_currentTweenerCount == 0)
            {
                _player.Transform.parent = null;
                MessageBus.Publish(new TweenCompletedEvent());
            }
        }

        public void Update()
        {
            foreach (var tile in _tiles)
            {
                tile.ExternalUpdate();
            }
        }
        
    }
}