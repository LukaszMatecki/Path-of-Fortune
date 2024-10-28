using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 11;
    public int columns = 11;
    public float tileSize = 1.0f;

    public GameObject tilePrefab3D;
    public GameObject characterModel;

    private void Start()
    {
        GenerateGrid();
        PlaceCharacterOnStartTile();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tile = Instantiate(tilePrefab3D, pos, Quaternion.identity);
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                tile.name = $"Tile_{x}_{y}";
            }
        }
    }
    
    public bool CanPlaceTile(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / tileSize);
        int z = Mathf.FloorToInt(position.z / tileSize);

        return x >= 0 && x < columns && z >= 0 && z < rows;
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize) * (int)tileSize;
        int z = Mathf.RoundToInt(position.z / tileSize) * (int)tileSize;
        return new Vector3(x, 0, z);
    }

    void PlaceCharacterOnStartTile()
    {
        if (characterModel != null)
        {
            int middleColumn = columns / 2;
            int bottomRow = 0;

            float startX = middleColumn * tileSize; 
            float startZ = bottomRow * tileSize;

            Vector3 startPosition = new Vector3(startX, characterModel.transform.localScale.y, startZ);
            characterModel.transform.position = startPosition;
        }
    }
}
