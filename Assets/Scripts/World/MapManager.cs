using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UniRx;

namespace Prefabric
{
    public class MapManager
    {
        private ControllerBase _controller;
        private List<AgentBase> _agents = new List<AgentBase>();
        private PlayerAgent _player;
        private List<Tile> _tiles;
        private LevelLoader _levelLoader = new LevelLoader();

        private Tile _hoverTile;
        private Tile _firstSelectedTile;

        private void Init(List<Tile> tiles, ControllerBase controller, List<AgentBase> agents)
        {
            _controller = controller;
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = tiles;

            var startTile = _tiles.Find(x => x is StartTile);
            _player.Position = startTile.Position + Vector3.up * 1.5f;

            MessageBus.OnEvent<TileSelectCommand>().Subscribe(ev => OnTileSelected(ev.Tile));
            MessageBus.OnEvent<TileHoverCommand>().Subscribe(ev => OnTileHover(ev.Tile));
        }

        public MapManager(string lvlPath, ControllerBase controller, List<AgentBase> agents)
        {
            Init(_levelLoader.LoadLevelAt(lvlPath), controller, agents);
        }

        public MapManager(int lvlNum, ControllerBase controller, List<AgentBase> agents)
        {
            Init(_levelLoader.LoadLevelAt(lvlNum), controller, agents);
        }

        private void OnTileHover(Tile tile)
        {
            if (tile == _hoverTile) // Don't do anything if hovering over the same tile
            {
                return;
            }

            // Unselect old hover
            if (_hoverTile != null // Will be null for the first assignment
                 && _hoverTile.VisualState != TileState.Selected) // Don't touch selected tile visuals
            {
                _hoverTile.VisualState = TileState.Normal;

            }

            _hoverTile = tile;

            if (_hoverTile.VisualState != TileState.Selected) // Don't touch selected tile visuals
            {
                _hoverTile.VisualState = TileState.Hovered;
            }
        }

        private void OnTileSelected(Tile tile)
        {
            // No tile selected present
            if (_firstSelectedTile == null) 
            {
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileState.Selected;
                return;
            }

            // Selecting two non-aligned tiles
            if (!Tile.IsAxisAligned(_firstSelectedTile, tile))
            {
                _firstSelectedTile.VisualState = TileState.Normal;
                _firstSelectedTile = tile;
                _firstSelectedTile.VisualState = TileState.Selected;
                return;
            }

            // Actual bending
            Bend(_firstSelectedTile, tile);

            _firstSelectedTile.VisualState = TileState.Normal;
            _firstSelectedTile = null;
        }

        private void Bend(Tile tile1, Tile tile2)
        {
            // Here be dragons
            var go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go1.transform.position = tile1.Position + Vector3.up;
            var go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go2.transform.position = tile2.Position + Vector3.up;
            GameObject.Destroy(go1, 1f);
            GameObject.Destroy(go2, 1f);

            Debug.Log("Bend!");

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