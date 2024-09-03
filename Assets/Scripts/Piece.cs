using System.Runtime.CompilerServices;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public int rotationIndex { get; private set; }
    public int Nlevels = 20;

    public float stepDelay = 1.0f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private bool swipeUpDetected;
    private float touchMoveTime;
    private float touchStartTime;
    private float touchMoveDelay = 0.2f;
    private float maxTapDuration = 0.2f; 
    private float maxSwipeDistance = 10f;
    private float minSwipeDistance = 50f; 
    private float swipeThreshold = 300.0f;


    private ScoreManager scoreManager;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        scoreManager = FindObjectOfType<ScoreManager>(); 
        this.Data = data;
        this.board = board;
        this.Position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0.1f;

        if (Cells == null)
        {
            Cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.cells[i];
        }
        UpdateStepDelay(scoreManager.Level);
    }

    private void Update()
    {
        board.Clear(this);

        lockTime += Time.deltaTime;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    touchStartTime = Time.time;
                    swipeUpDetected = false;
                    
                    
                    break;

                case TouchPhase.Moved:
                    touchEndPosition = touch.position;
                    Vector2 swipeDelta = touchEndPosition - touchStartPosition;

                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) && Mathf.Abs(swipeDelta.x) > minSwipeDistance)
                    {
                        if (swipeDelta.x > 0)
                        {
                            Move(Vector2Int.right);
                            touchStartPosition = touch.position;
                            touchMoveTime = Time.time + touchMoveDelay;
                        }
                        else if (swipeDelta.x < 0)
                        {
                            Move(Vector2Int.left);
                            touchStartPosition = touch.position;
                            touchMoveTime = Time.time + touchMoveDelay;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    touchEndPosition = touch.position;
                    
                    swipeDelta = touchEndPosition - touchStartPosition;

                    if (swipeDelta.y > Mathf.Abs(swipeDelta.x) && swipeDelta.y > swipeThreshold)
                    {
                        swipeUpDetected = true;
                    }

                    if (swipeUpDetected)
                    {
                         Debug.Log("swipe up OKK" );
                        HardDrop();
                    }
                    
                    else if (Time.time - touchStartTime <= maxTapDuration && swipeDelta.magnitude < maxSwipeDistance)
                    {
                        Rotate(-1);
                    }
                    break;
            }
        }

        if (Time.time > stepTime)
        {
            Step();
        }
        board.Set(this);
    }

     public void UpdateStepDelay(int level)
    {
        float decrement = 0.99f / Nlevels;
        stepDelay = Mathf.Max(0.01f, 1f - (level * decrement)); 
        lockDelay = Mathf.Max(0.01f, 1f - (level * decrement)); 

        stepTime = Time.time + stepDelay; // 
    }

    public void HardDropButton()
    {
        HardDrop();
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

      private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            Position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }
        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = global::Data.RotationMatrix;

        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];

            int x, y;

            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < Data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = Data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, Data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
