using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private float stepDelay = 1f;
    [SerializeField] private float lockDelay = 0.5f;

    public GameBoard Board { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public TetrominoData TData { get; private set; }
    public int RotationIndex { get; private set; }

    private float stepTime;
    private float lockTime;

    public void Initialize(GameBoard board, Vector3Int position, TetrominoData data)
    {
        Board = board;
        Position = position;
        TData = data;
        RotationIndex = 0;

        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        Cells ??= new Vector3Int[data.Cells.Length];

        for(int i = 0; i < data.Cells.Length; ++i)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    private void Update() 
    {
        Board.Clear(this);

        lockTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            HandleRotation(-1);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            HandleRotation(+1);
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            HandleMovement(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            HandleMovement(Vector2Int.right);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            HandleMovement(Vector2Int.down);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            HandleHardDrop();
        }

        if(Time.time >= stepTime)
        {
            Step();
        }

        Board.Set(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        HandleMovement(Vector2Int.down);

        if(lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
        AudioManager.Instance.Play(Consts.Audio.LOCK_SOUND);
    }

    private bool HandleMovement(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool isValid = Board.IsValidPosition(this, newPosition);

        if(isValid)
        {
            Position = newPosition;
            lockTime = 0f;
        }

        return isValid;
    }

    private void HandleRotation(int direction)
    {
        int originalRotation = RotationIndex;
        RotationIndex = ExtensionMethods.Wrap(RotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if(!TestWallkicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }

        AudioManager.Instance.Play(Consts.Audio.ROTATE_SOUND);
    }

    private void ApplyRotationMatrix(int direction)
    {
        for(int i = 0; i < Cells.Length; ++i)
        {
            Vector3 cell = Cells[i];

            int x, y;

            switch(TData.tetromino)
            {
                case Tetromino.Letter_I:
                case Tetromino.Letter_O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallkicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for(int i = 0; i < TData.WallKicks.GetLength(1); ++i)
        {
            Vector2Int translation = TData.WallKicks[wallKickIndex, i];

            if(HandleMovement(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if(rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return ExtensionMethods.Wrap(wallKickIndex, 0, TData.WallKicks.GetLength(0));
    }

    private void HandleHardDrop()
    {
        while(HandleMovement(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }
}
