using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    private Vector2Int gridPosition;
    private Vector2Int gridMoveDirection;
    private Vector2Int lastMoveDirection;
    private float moveTimer;
    private float moveTimerMax = 1f;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<Vector2Int> snakeMovePositionList;
    private List<Vector2Int> snakeMoveDirectionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    public GameObject gameOverScreen;
    public bool gameOver;
    public AudioSource eatSound;
    private int playerScore;
    public Text scoreText;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    private void Awake()
    {
        moveTimer = 1f;
        gameOver = false;
        gridPosition = new Vector2Int(10, 10);
        gridMoveDirection = new Vector2Int(1, 0);
        lastMoveDirection = new Vector2Int(1, 0);
        snakeMovePositionList = new List<Vector2Int>();
        snakeMoveDirectionList = new List<Vector2Int>();
        snakeBodyPartList = new List<SnakeBodyPart>();
        playerScore = 0;
    }

    private void Update()
    {
        scoreText.text = playerScore.ToString();
        if (Input.GetKeyDown(KeyCode.W) && gridMoveDirection.y != -1)
        {
            gridMoveDirection.y = 1;
            gridMoveDirection.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.A) && gridMoveDirection.x != 1)
        {
            gridMoveDirection.x = -1;
            gridMoveDirection.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.S) && gridMoveDirection.y != 1)
        {
            gridMoveDirection.y = -1;
            gridMoveDirection.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.D) && gridMoveDirection.x != -1)
        {
            gridMoveDirection.x = 1;
            gridMoveDirection.y = 0;
        }
        
        gridMovementHandler();
        
    }

    private void gridMovementHandler()
    {
        gameOver = CheckHit();
        if (gameOver == false)
            {
                moveTimer = moveTimer + Time.deltaTime * 5;
                if (moveTimer >= moveTimerMax)
                {
                    snakeMovePositionList.Insert(0, gridPosition);
                    snakeMoveDirectionList.Insert(0, gridMoveDirection);
                    moveTimer = 0;
                    gridPosition += gridMoveDirection;

                    bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
                    if (snakeAteFood)
                    {
                        snakeBodySize++;
                        CreateSnakeBodyPart();
                    }
                    lastMoveDirection = snakeMoveDirectionList[snakeMoveDirectionList.Count - 1];

                    if (snakeMovePositionList.Count >= snakeBodySize + 1)
                    {
                        snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
                        snakeMoveDirectionList.RemoveAt(snakeMoveDirectionList.Count - 1);
                    }

                    if (snakeAteFood)
                    {
                        playerScore++;
                        
                        SwitchDir();
                        gridMoveDirection = snakeMoveDirectionList[0];
                        gridPosition.x = snakeMovePositionList[0].x + gridMoveDirection.x;
                        gridPosition.y = snakeMovePositionList[0].y + gridMoveDirection.y;
                       
                        if(moveTimerMax > 0f)
                        {
                            moveTimerMax = moveTimerMax - 0.025f;
                        }
                            
                    }


                    gameOver = CheckHit();
                    if (gameOver == false)
                    {
                        UpdateSnakeBodyParts();
                        transform.position = new Vector3(gridPosition.x, gridPosition.y);
                        
                    }
                    transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) - 90);



            }


        }
    }


    private bool CheckHit()
    {

            if (gridPosition.x > 19 || gridPosition.y > 19
                || gridPosition.x < 1 || gridPosition.y < 1)
            {
                gameOverScreen.SetActive(true);
                
                return true;
            }
            for (int i = 0; i < snakeMovePositionList.Count; i++)
            {
                if (gridPosition == snakeMovePositionList[i] )
                {
                    gameOverScreen.SetActive(true);
                    Debug.Log("DEAD!");
                    return true;
                }
            }
        return false;

    }
    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void SwitchDir() {

        List<Vector2Int> newList = new List<Vector2Int>();
        List<Vector2Int> newDir = new List<Vector2Int>();
        for (int i = 0; i < snakeMovePositionList.Count; i++)
        {
            newList.Add(snakeMovePositionList[snakeMovePositionList.Count - 1 - i]);
            newDir.Add(snakeMoveDirectionList[snakeMoveDirectionList.Count - 1 - i] * -1);
        }
        snakeMovePositionList = newList;
        snakeMoveDirectionList = newDir;
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            if (i < snakeBodyPartList.Count - 1)
                snakeBodyPartList[i].switchHeadToBody();

            snakeBodyPartList[i].SetDirection(snakeMoveDirectionList[i]);
            snakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
        }
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    public List<Vector2Int> GetPositionList()
    {
        return snakeMovePositionList;
    }

    private class SnakeBodyPart{

        private Vector2Int gridPosition;
        private Transform transform;
        GameObject snakeBodyGameObject;
        public SnakeBodyPart(int bodyIndex)
        {
            
            snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.eyesClosedSprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        
        }
        public void SetGridPosition(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
            transform.position = new Vector2(gridPosition.x, gridPosition.y);
        }

        public void switchHeadToBody()
        {
            this.snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
        }

        public void SetDirection(Vector2Int gridDir)
        {
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridDir) + 90);
        }

    }

    public static float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }
}

