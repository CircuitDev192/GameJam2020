using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region Singleton

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

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
    private float _timer = 165f;

    [SerializeField]
    private GameObject _helicopter;
    private bool _heliStarted = false;
    private bool _playerDead = false;

    [SerializeField]
    private GameObject _endZombies;

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
                    _audioSource.volume = 0.3f;
                    _audioSource.Play();

                    KillPlayer();

                    StartCoroutine(RestartLevel());
                }
            } else
            {
                deathCamEndPos = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y + 10f, _mainCamera.transform.position.z - 15f);
                deathCamEndRot = _mainCamera.transform.position;
            }
        }
    }

    public void DestroyPlayer()
    {
        _ui.transform.Find("TimerText").gameObject.SetActive(false);
        _ui.transform.Find("AmmoText").gameObject.SetActive(false);
        Destroy(PlayerManager.instance.playerMovementScript.gameObject);
        _ui.transform.Find("HealthBar").gameObject.SetActive(false);
    }

    private void KillPlayer()
    {
        _ui.transform.Find("TimerText").gameObject.SetActive(false);
        _ui.transform.Find("DeadText").gameObject.SetActive(true);
        _ui.transform.Find("DeadText").GetComponent<Animator>().SetTrigger("PlayerDead");
        _ui.transform.Find("AmmoText").gameObject.SetActive(false);
        PlayerManager.instance.player.transform.GetComponent<PlayerController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonAIO>().enabled = false;
        _ui.transform.Find("HealthBar").gameObject.SetActive(false);
    }

    public void EndSequence()
    {
        _audioSource.clip = _music[2];
        _audioSource.volume = 0.25f;
        _audioSource.Play();
        _ui.transform.Find("Credits").GetComponent<Animator>().SetTrigger("StartCredits");
        StartCoroutine(BackToMenuCredits());
    }

    public void FadeToBlack()
    {
        _ui.transform.Find("FadeToBlack").GetComponent<Animator>().SetBool("Fade", true);
    }

    void UpdateTimer()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }

        if (_timer < _timer * 0.75f)
        {
            _spawnManager.SetMaxZombies(25);
        } else if (_timer < _timer * 0.50f)
        {
            _spawnManager.SetMaxZombies(30);
        } else if (_timer < _timer * 0.25f)
        {
            _spawnManager.SetMaxZombies(35);
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
            _ui.transform.Find("TimerText").GetComponent<Text>().text = "Exfil: " + min + ":" + sec;
        }
        else
        {
            _ui.transform.Find("TimerText").GetComponent<Text>().text = "Get to the LZ!";
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

    public void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void EnableEndZombies()
    {
        _endZombies.SetActive(true);
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(10f);
        FadeToBlack();
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator BackToMenuCredits()
    {
        yield return new WaitForSeconds(105f);
        EndGame();
    }
}
