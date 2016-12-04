using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UniRx;

namespace Prefabric
{
    public class TweenCompletedEvent : PfEvent { }

    public class MapManager
    {
        private List<AgentBase> _agents = new List<AgentBase>();
        private PlayerAgent _player;
        private List<Tile> _tiles;
        private readonly LevelLoader _levelLoader = new LevelLoader();

        private Tile _hoverTile;
        private Tile _firstSelectedTile;

        private int _currentTweenerCount;

        private void Init(List<Tile> tiles, List<AgentBase> agents)
        {
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = tiles;

            var startTile = _tiles.Find(x => x is StartTile);
            _player.Position = startTile.Position + Vector3.up * 1.5f;

            MessageBus.OnEvent<TileSelectCommand>().Subscribe(ev => OnTileSelected(ev.Tile));
            MessageBus.OnEvent<TileHoverCommand>().Subscribe(ev => OnTileHover(ev.Tile));
            MessageBus.OnEvent<UnbendCommand>().Subscribe(ev => Unbend());
            MessageBus.OnEvent<TileTweenCompletedEvent>().Subscribe(ev => OnTileCompletedTween(ev.Tile));

        }

        public MapManager(string lvlPath, List<AgentBase> agents)
        {
            Init(_levelLoader.LoadLevelAt(lvlPath), agents);
        }

        public MapManager(int lvlNum, List<AgentBase> agents)
        {
            Init(_levelLoader.LoadLevelAt(lvlNum), agents);
        }

        private void OnTileHover(Tile tile)
        {
            if (tile == _hoverTile) // Don't do anything if hovering over the same tile
            {
                return;
            }

            // Unselect old hover
            if (_hoverTile != null // Will be null for the first assignment
                 && _hoverTile.VisualState != TileVisualState.Selected) // Don't touch selected tile visuals
            {
                _hoverTile.VisualState = TileVisualState.Normal;
            }

            _hoverTile = tile;

            if (_hoverTile.VisualState != TileVisualState.Selected) // Don't touch selected tile visuals
            {
                _hoverTile.VisualState = TileVisualState.Hovered;
            }
        }

        private void OnTileSelected(Tile tile)
        {
            // No tile selected present
            if (_firstSelectedTile == null) 
            {
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileVisualState.Selected;
                return;
            }

            // Selecting two non-aligned tiles
            if (!Tile.IsAxisAligned(_firstSelectedTile, tile))
            {
                _firstSelectedTile.VisualState = TileVisualState.Normal;
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileVisualState.Selected;
                return;
            }

            // Actual bending
            Bend(_firstSelectedTile, tile);

            _firstSelectedTile.VisualState = TileVisualState.Normal;
            _firstSelectedTile = null;
        }

        private void Bend(Tile tile1, Tile tile2)
        {
            Vector3 alignedDir;
            if (Mathf.Approximately(tile1.Position.y, tile2.Position.y) && Mathf.Approximately(tile1.Position.z, tile2.Position.z))
            {
                alignedDir = Vector3.right;
            }
            else if (Mathf.Approximately(tile1.Position.x, tile2.Position.x) && Mathf.Approximately(tile1.Position.z, tile2.Position.z))
            {
                alignedDir = Vector3.up;
            }
            else
            {
                alignedDir = Vector3.forward;
            }

            var proj1 = Vector3.Dot(tile1.Position, alignedDir);
            var proj2 = Vector3.Dot(tile2.Position, alignedDir);

            var playerProj = Vector3.Dot(_player.Position, alignedDir);
            if (playerProj > proj1 && playerProj < proj2)
            {
                return;
            }

            var moveDistance = Vector3.Distance(tile1.Position, tile2.Position) / 2 - 0.5f;
            foreach (var tile in _tiles)
            {
                var tileProj = Vector3.Dot(tile.Position, alignedDir);
                var targetPos = Vector3.zero;
                var isBent = false;
                if (tileProj >= proj1 && tileProj >= proj2)
                {
                    targetPos = alignedDir * -moveDistance + tile.Position;
                }
                else if (tileProj > proj1 && tileProj < proj2)
                {
                    targetPos = tile.Position + Vector3.down * Random.Range(50f, 150f) * Mathf.Sign(Random.Range(-1f, 1f));
                    isBent = true;
                }
                else if (tileProj <= proj1 && tileProj <= proj2)
                {
                    targetPos = alignedDir * moveDistance + tile.Position;
                }

                tile.Bend(new TileState()
                {
                    Position = targetPos,
                    IsBent = isBent
                });
                _currentTweenerCount++;
            }

            var dist1 = Vector3.Distance(_player.Position, tile1.Position);
            var dist2 = Vector3.Distance(_player.Position, tile2.Position);
            _player.Transform.parent = dist1 < dist2 ? tile1.Transform : tile2.Transform;
        }

        private void Unbend()
        {
            foreach (var tile in _tiles)
            {
                tile.Unbend();
                _currentTweenerCount++;
            }
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