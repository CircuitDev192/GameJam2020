using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum SCENES
    {
        NULL,
        MENU,
        GAME
    }

    [SerializeField]
    private SpawnManager _spawnManager;

    private SCENES _currentScene;
    // Start is called before the first frame update
    void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                _currentScene = SCENES.MENU;
                break;
            case 1:
                _currentScene = SCENES.GAME;
                break;
            default:
                _currentScene = SCENES.NULL;
                break;
        }

        if (_currentScene == SCENES.GAME)
        {
            _spawnManager.StartSpawning();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
