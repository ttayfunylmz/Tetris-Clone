using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public event Action OnGameOver;

    [SerializeField] private TetrominoData[] tetrominos;
    [SerializeField] private Vector3Int spawnPosition;

    public Block ActiveBlock { get; private set; } 
    public Tilemap Tilemap { get; private set; }
    public bool canPlay = true;

    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake() 
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        ActiveBlock = GetComponentInChildren<Block>();

        for(int i = 0; i < tetrominos.Length; ++i)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start() 
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if(!canPlay) { return; }

        int random = UnityEngine.Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        ActiveBlock.Initialize(this, spawnPosition, data);

        if(IsValidPosition(ActiveBlock, spawnPosition))
        {
            Set(ActiveBlock);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Tilemap.ClearAllTiles();
        DisableComponents();

        AudioManager.Instance.Play(Consts.Audio.GAME_OVER_SOUND);
        OnGameOver?.Invoke();
    }

    public void Set(Block block)
    {
        if(!canPlay) { return; }

        for(int i = 0; i < block.Cells.Length; ++i)
        {
            Vector3Int tilePosition = block.Cells[i] + block.Position;
            Tilemap.SetTile(tilePosition, block.TData.tile);
        }
    }

    public void Clear(Block block)
    {
        for(int i = 0; i < block.Cells.Length; ++i)
        {
            Vector3Int tilePosition = block.Cells[i] + block.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Block block, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < block.Cells.Length; ++i)
        {
            Vector3Int tilePosition = block.Cells[i] + position;

            if(!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if(Tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax)
        {
            if(IsLineFull(row))
            {
                ClearLine(row);
                AudioManager.Instance.Play(Consts.Audio.LINE_CLEAR_SOUND);
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; ++col)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if(!Tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void ClearLine(int row)
    {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; ++col)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            Tilemap.SetTile(position, null);
        }

        while(row < bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; ++col)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase aboveTile = Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                Tilemap.SetTile(position, aboveTile);
            }

            row++;
        }
    }

    private void DisableComponents()
    {
        canPlay = false;
        this.enabled = false;
        ActiveBlock.enabled = false;
    }

    public void EnableComponents()
    {
        canPlay = true;
        this.enabled = true;
        ActiveBlock.enabled = true;
    }
}
