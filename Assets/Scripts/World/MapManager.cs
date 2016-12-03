using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UniRx;

namespace Prefabric
{
    public class MapManager
    {
        private List<AgentBase> _agents = new List<AgentBase>();
        private PlayerAgent _player;
        private List<Tile> _tiles;
        private readonly LevelLoader _levelLoader = new LevelLoader();

        private Tile _hoverTile;
        private Tile _firstSelectedTile;

        private void Init(List<Tile> tiles, List<AgentBase> agents)
        {
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = tiles;

            var startTile = _tiles.Find(x => x is StartTile);
            _player.Position = startTile.Position + Vector3.up * 1.5f;

            MessageBus.OnEvent<TileSelectCommand>().Subscribe(ev => OnTileSelected(ev.Tile));
            MessageBus.OnEvent<TileHoverCommand>().Subscribe(ev => OnTileHover(ev.Tile));

            MessageBus.OnEvent<UnbendCommand>().Subscribe(ev =>
            {
                Unbend();
            });
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
            // Here be dragons
            Debug.Log("Bend!");
        }

        private void Unbend()
        {
            Debug.Log("Unbend!");
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