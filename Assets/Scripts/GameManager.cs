using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    private Canvas _ui;

    [SerializeField]
    private float _timer = 150f;

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
        UpdateTimer();
        DisplayTimer();
        UpdateAmmo();
    }

    void UpdateTimer()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }
    }

    void DisplayTimer()
    {
        string min = ((int)_timer / 60).ToString();
        string sec = (_timer % 60).ToString("f2");
        if (_timer > 0f)
        {
            _ui.transform.Find("TimerText").GetComponent<Text>().text = min + ":" + sec;
        } else
        {
            _ui.transform.Find("TimerText").GetComponent<Text>().text = "";
            _spawnManager.StopSpawning();
        }
    }

    void UpdateAmmo()
    {
        string magAmmo = (PlayerManager.instance.player.transform.GetComponent<PlayerController>().GetAmmoCount(false).ToString());
        string reserveAmmo = (PlayerManager.instance.player.transform.GetComponent<PlayerController>().GetAmmoCount(true).ToString());
        _ui.transform.Find("AmmoText").GetComponent<Text>().text = magAmmo + "/" + reserveAmmo;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
