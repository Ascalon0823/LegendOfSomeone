using CGS.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Arkademy
{
    public class StageBuilder : MonoBehaviour
    {
        [SerializeField] private Tile[] tilePrefabs;
        [SerializeField] private Tilemap map;
        public SquareGrid2D<int> Grid { get; private set; }

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
            Grid = GetGrid(data);
            data.mapData = Grid.Data;
            var tileCount = data.size.x * data.size.y;
            Debug.Log(tileCount);
            var layerGo = new GameObject();
            layerGo.transform.SetParent(transform);
            layerGo.transform.localPosition = Vector3.zero;
            map = layerGo.AddComponent<Tilemap>();
            var poses = new Vector3Int[tileCount];
            var tiles = new TileBase[tileCount];
            var c = 0;
            Grid.Iterate((x, y) =>
            {
                poses[c] = new Vector3Int(x, y, 0);
                tiles[c] = Instantiate(tilePrefabs[Grid[x, y]]);
                c++;
            });
            map.transform.localPosition = Vector3.zero;
            map.SetTiles(poses, tiles);
            layerGo.gameObject.AddComponent<TilemapRenderer>();
        }

        public void SetGridDataAndTile(int x, int y, int value)
        {
            Grid[x, y] = value;
            map.SetTile(new Vector3Int(x, y, 0), Instantiate(tilePrefabs[value]));
        }
    }
}