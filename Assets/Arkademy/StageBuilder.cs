using CGS.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Arkademy
{
    public class StageBuilder : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;
        public SquareGrid2D<Vector2Int> Grid { get; private set; }

        public void Build(Sys.StageData data)
        {
            Grid = new SquareGrid2D<Vector2Int>(data.size.x, data.size.y, Vector3.zero, new Vector3(1, 1, 0));

            var tileCount = data.size.x * data.size.y;
            Debug.Log(tileCount);
            var layerGo = new GameObject();
            layerGo.transform.SetParent(transform);
            layerGo.transform.localPosition = Vector3.zero;
            var tileMap = layerGo.AddComponent<Tilemap>();
            var poses = new Vector3Int[tileCount];
            var tiles = new TileBase[tileCount];
            var c = 0;
            Grid.Iterate((x, y) =>
            {
                poses[c] = new Vector3Int(x, y, 0);
                tiles[c] = Instantiate(tilePrefab);
                c++;
            });
            tileMap.transform.localPosition = Vector3.zero;
            tileMap.SetTiles(poses, tiles);
            layerGo.gameObject.AddComponent<TilemapRenderer>();
        }
    }
}