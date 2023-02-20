using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;
    
    

    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();

    }
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
    private void SpawnFood()
    {
        bool inList;
        do
        {
            inList = false;
            List<Vector2Int> temp = snake.GetPositionList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i] == foodGridPosition)
                {
                    inList = true;
                }
            }
            foodGridPosition = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        } while (snake.GetGridPosition() == foodGridPosition || inList == true);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if(snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            return true;
        }
        else
        {
            return false;
        }
    }


}
