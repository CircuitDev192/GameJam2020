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
    private AudioClip[] _music;
    private AudioSource _audioSource;

    private Camera _mainCamera;
    private Vector3 deathCamEndPos;
    private Vector3 deathCamEndRot;

    [SerializeField]
    private Canvas _ui;

    [SerializeField]
    private float _timer = 150f;

    [SerializeField]
    private GameObject _helicopter;
    private bool _heliStarted = false;

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
            _mainCamera = Camera.main;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogError("GameManager::Start() AudioSource is null");
            } else
            {
                _audioSource.clip = _music[0];
                _audioSource.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentScene == SCENES.GAME)
        {
            UpdateTimer();
            DisplayTimer();
            UpdateAmmo();
            UpdateHealth();
            if (PlayerManager.instance.player.transform.GetComponent<PlayerController>().GetHealth() <= 0f)
            {
                PlayerManager.instance.playerMovementScript.GetComponent<FirstPersonAIO>().enabled = false;
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, deathCamEndPos, Time.deltaTime);
                _mainCamera.transform.LookAt(deathCamEndRot);
                if (_audioSource.clip != _music[1])
                {
                    _audioSource.clip = _music[1];
                    _audioSource.Play();
                    StartCoroutine(RestartLevel());
                }
            } else
            {
                deathCamEndPos = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y + 10f, _mainCamera.transform.position.z - 15f);
                deathCamEndRot = _mainCamera.transform.position;
            }
        }
    }

    void UpdateTimer()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }
        if (_timer < 30f && !_heliStarted)
        {
            _heliStarted = true;
            _helicopter.transform.GetComponent<HelicopterController>().StartMovement();
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

    void UpdateHealth()
    {
        _ui.transform.Find("HealthBar").GetComponent<Slider>().value = PlayerManager.instance.player.transform.GetComponent<PlayerController>().GetHealth();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(17f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
