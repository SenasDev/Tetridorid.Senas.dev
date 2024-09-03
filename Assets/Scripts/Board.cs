using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Tilemap nextPieceTilemap;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private TetrominoData nextPieceData;
    public int linesClearedInMove;
    public ScoreManager scoreManager;
    public GameOverManager gameOverManager;

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SetNextPiece();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        
        TetrominoData data = nextPieceData;

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }

        SetNextPiece();
    }

    private void SetNextPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        nextPieceData = tetrominoes[random];

        if (nextPieceTilemap != null)
        {
            DisplayNextPiece(nextPieceData);
        }
    }

    private void DisplayNextPiece(TetrominoData data)
    {
        nextPieceTilemap.ClearAllTiles();

        Vector3Int position = new Vector3Int(0, 0, 0);
        foreach (Vector3Int cell in data.cells)
        {
            Vector3Int tilePosition = cell + position;
            nextPieceTilemap.SetTile(tilePosition, data.tile);
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();
        gameOverManager.GameOver();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

   public void ClearLines()
{
    RectInt bounds = Bounds;
    int row = bounds.yMin;

    linesClearedInMove = 0; 

    while (row < bounds.yMax)
    {
        if (IsLineFull(row)) {
            LineClear(row);
            linesClearedInMove++;
        } else {
            row++;
        }
    }

    if (scoreManager != null)
    {
        scoreManager.UpdateScore(linesClearedInMove);
    }
}
public void LineClear(int row)
{
    RectInt bounds = Bounds;

    for (int col = bounds.xMin; col < bounds.xMax; col++)
    {
        Vector3Int position = new Vector3Int(col, row, 0);
        tilemap.SetTile(position, null);
    }
    for (int r = row; r < bounds.yMax - 1; r++)
    {
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, r, 0);
            Vector3Int abovePosition = new Vector3Int(col, r + 1, 0);
            TileBase aboveTile = tilemap.GetTile(abovePosition);

            tilemap.SetTile(position, aboveTile);
            tilemap.SetTile(abovePosition, null);
        }
    }
}
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position)) {
                return false;
            }
        }
        return true;
    }
}

