using CGS.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

namespace Arkademy
{
    public class StageBuilder : MonoBehaviour
    {
        [SerializeField] private Tile[] tilePrefabs;
        [SerializeField] private GameObject enterPrefab;
        public GameObject Enter => enter;
        [SerializeField] private GameObject enter;
        [SerializeField] private GameObject exitPrefab;
        public GameObject Exit => exit;
        [SerializeField] private GameObject exit;
        private readonly Dictionary<int, Tilemap> _tileMaps = new Dictionary<int, Tilemap>();
        public SquareGrid2D<int> Grid { get; private set; }
        [SerializeField] private Sys.StageData currData;

        private SquareGrid2D<int> GetGrid(Sys.StageData data)
        {
            if (data.mapData != null && data.size.x == data.mapData.GetLength(0) &&
                data.size.y == data.mapData.GetLength(1))
            {
                return new SquareGrid2D<int>(data.mapData,
                    Vector3.zero,
                    new Vector3(1, 1, 0));
            }

            var grid = new SquareGrid2D<int>(data.size.x, data.size.y,
                Vector3.zero,
                new Vector3(1, 1, 0));
            grid.Iterate((x, y) =>
            {
                if (grid.IsBoarder(x, y))
                {
                    grid[x, y] = 0;
                    return;
                }

                grid[x, y] = 1;
            });
            return grid;
        }

        public void Build(Sys.StageData data)
        {
            _tileMaps.Clear();
            Grid = GetGrid(data);
            data.mapData = Grid.Data;
            var layerMap = new Dictionary<int, Tuple<Tilemap, List<Vector3Int>, List<TileBase>>>();
            for (var i = 0; i < tilePrefabs.Length; i++)
            {
                var layerGo = new GameObject();
                layerGo.transform.SetParent(transform);
                layerGo.transform.localPosition = Vector3.zero;
                var map = layerGo.AddComponent<Tilemap>();
                var poses = new List<Vector3Int>();
                var tiles = new List<TileBase>();
                layerMap[i] = new Tuple<Tilemap, List<Vector3Int>, List<TileBase>>(map, poses, tiles);
            }


            Grid.Iterate((x, y) =>
            {
                var v = Grid[x, y];
                layerMap[v].Item2.Add(new Vector3Int(x, y, 0));
                layerMap[v].Item3.Add(Instantiate(tilePrefabs[v]));
            });

            foreach (var pair in layerMap)
            {
                pair.Value.Item1.SetTiles(pair.Value.Item2.ToArray(), pair.Value.Item3.ToArray());
                _tileMaps[pair.Key] = pair.Value.Item1;
                _tileMaps[pair.Key].transform.localPosition = Vector3.zero;
                var tilemapRenderer = _tileMaps[pair.Key].gameObject.AddComponent<TilemapRenderer>();
                tilemapRenderer.sortingOrder = -1;
                if (pair.Key == 0) //wall
                {
                    _tileMaps[pair.Key].gameObject.AddComponent<TilemapCollider2D>();
                    tilemapRenderer.sortingLayerName = "Front";
                    tilemapRenderer.sortingOrder = 0;
                    tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
                }
            }

            SetEnter(data.enter.x, data.enter.y);
            SetExit(data.exit.x, data.exit.y);

            currData = data;
        }

        public void SetGridDataAndTile(int x, int y, int value)
        {
            Grid[x, y] = value;
            currData.mapData = Grid.Data;
            foreach (var map in _tileMaps)
            {
                _tileMaps[map.Key].SetTile(new Vector3Int(x, y, 0),
                    map.Key == value ? Instantiate(tilePrefabs[value]) : null);
            }
        }

        public void SetEnter(int x, int y)
        {
            currData.enter = new Vector2Int(x, y);
            if (!enter) enter = Instantiate(enterPrefab, transform);
            enter.transform.position = Grid.GetPos(x, y);
        }

        public void SetExit(int x, int y)
        {
            currData.exit = new Vector2Int(x, y);
            if (!exit) exit = Instantiate(exitPrefab, transform);
            exit.transform.position = Grid.GetPos(x, y);
        }
    }
}