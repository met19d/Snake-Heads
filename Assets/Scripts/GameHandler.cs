using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField]private Snake snake;
    private LevelGrid levelGrid;

    void Start()
    {
        
        Debug.Log("GameHandler.Start");

        levelGrid = new LevelGrid(19, 19);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);


    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void score()
    {

    }
}
