using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostBlock : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private GameBoard board;
    [SerializeField] private Block trackingBlock;

    public Tilemap Tilemap { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    private void Awake() 
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];    
    }

    private void LateUpdate() 
    {
        Clear();
        Copy();
        Drop();
        Set();    
    }

    private void Clear()
    {
        for(int i = 0; i < Cells.Length; ++i)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < Cells.Length; ++i)
        {
            Cells[i] = trackingBlock.Cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = trackingBlock.Position;

        int currentRow = position.y;
        int bottomRow = (-board.boardSize.y / 2) - 1;

        board.Clear(trackingBlock);

        for(int row = currentRow; row >= bottomRow; --row)
        {
            position.y = row;

            if(board.IsValidPosition(trackingBlock, position))
            {
                Position = position;
            }
            else
            {
                break;
            }
        }

        board.Set(trackingBlock);
    }

    private void Set()
    {
        if(!board.canPlay) { return; }

        for(int i = 0; i < Cells.Length; ++i)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, tile);
        }
    }
}
