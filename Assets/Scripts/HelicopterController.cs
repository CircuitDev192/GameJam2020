using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [SerializeField]
    private GameObject _door, _tail, _rotor;
    [SerializeField]
    private bool _doorOpen = false;
    private bool _startPath = false;
    private bool _endPath = false;
    private Vector3 _doorOpenPos;
    private Vector3 _doorClosePos;
    [SerializeField]
    private float _speed = 15f;
    private int _lastWaypoint = 0;
    [SerializeField]
    private GameObject[] _waypoints;
    [SerializeField]
    private GameObject[] _endWaypoints;
    private int _lastEndWaypoint = 0;
    [SerializeField]
    private GameObject _playerExit;
    private bool _endSequenceStarted = false;
    private Camera _camera;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _helicopterSound;
    private Animator _audioAnim;


    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _doorOpenPos = new Vector3(_door.transform.localPosition.x, _door.transform.localPosition.y, _door.transform.localPosition.z + 2.81f);
        _doorClosePos = new Vector3(_door.transform.localPosition.x, _door.transform.localPosition.y, _door.transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(20, 0, 0);
        _audioSource = GetComponent<AudioSource>();
        _audioAnim = GetComponent<Animator>();
        if (_audioSource == null) Debug.LogError("HelicopterController::Start() Audio Source is null");
        if (_audioAnim == null) Debug.LogError("HelicopterController::Start() Animator is null");
    }

    // Update is called once per frame
    void Update()
    {
        if (_doorOpen)
        {
            _door.transform.localPosition = Vector3.MoveTowards(_door.transform.localPosition, _doorOpenPos, Time.deltaTime);
        } else
        {
            _door.transform.localPosition = Vector3.MoveTowards(_door.transform.localPosition, _doorClosePos, Time.deltaTime);
        }
        _rotor.transform.localRotation *= Quaternion.AngleAxis(1000 * Time.deltaTime, Vector3.down);
        _tail.transform.localRotation *= Quaternion.AngleAxis(2500 * Time.deltaTime, Vector3.right);

        if (_startPath)
        {
            transform.position = Vector3.MoveTowards(transform.position, _waypoints[_lastWaypoint].transform.position, _speed * Time.deltaTime);
            if (transform.position == _waypoints[_lastWaypoint].transform.position)
            {
                if (transform.position != _waypoints[_waypoints.Length - 1].transform.position)
                {
                    _lastWaypoint++;
                }
            }
            if (transform.position == _waypoints[_waypoints.Length - 2].transform.position)
            {
                StartCoroutine(LevelHelicopter());
            }
            if (transform.position == _waypoints[_waypoints.Length - 1].transform.position)
            {
                StartOpenDoor();
            }
        }
        else if (_endPath)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endWaypoints[_lastEndWaypoint].transform.position, _speed * Time.deltaTime);
            if (transform.position == _endWaypoints[_lastEndWaypoint].transform.position)
            {
                if (transform.position != _endWaypoints[_endWaypoints.Length - 1].transform.position)
                { 
                    _lastEndWaypoint++;
                    if (transform.position != _endWaypoints[0].transform.position)
                    {
                        StartCoroutine(TurnHelicopter());
                        if (!_endSequenceStarted)
                        {
                            _endSequenceStarted = true;
                            GameManager.instance.EndSequence();
                        }
                    }
                }
            }
            if (transform.position == _endWaypoints[_endWaypoints.Length - 2].transform.position)
            {
                StartCloseDoor();
            }
        }

        if (_camera.transform.IsChildOf(this.transform) && _startPath)
        {
            _startPath = false;
            _audioAnim.SetTrigger("HeliSoundTrigger");
            StartCoroutine(GetPlayerIntoHeli());
        }
    }

    public void StartMovement()
    {
        _startPath = true;
        _audioSource.clip = _helicopterSound;
        _audioSource.Play();
        Debug.Log("Playing Heli Sound");
    }

    private void StartOpenDoor()
    {
        _doorOpen = true;
        _playerExit.SetActive(true);
    }

    private void StartCloseDoor()
    {
        _doorOpen = false;
        _playerExit.SetActive(false);
        GameManager.instance.FadeToBlack();
    }

    IEnumerator LevelHelicopter()
    {
        _speed = 5f;
        while (transform.localRotation != Quaternion.Euler(0, 0, 0))
        {
            transform.localRotation *= Quaternion.AngleAxis(10 * Time.deltaTime, Vector3.left);
            yield return null;
        }
    }

    IEnumerator TurnHelicopter()
    {
        Vector3 targetPos = _endWaypoints[_lastEndWaypoint].transform.position;
        targetPos.y = this.transform.position.y - 20;
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - this.transform.position);
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator GetPlayerIntoHeli()
    {
        bool _startEndPath = false;
        GameManager.instance.EnableEndZombies();
        Vector3 _cameraEndPos = new Vector3(-0.44f, 2.98f, -0.45f);
        Quaternion _cameraEndRot = Quaternion.Euler(25.1f, -131.59f, 3.75f);
        while (_camera.transform.localRotation != _cameraEndRot || _camera.transform.localPosition != _cameraEndPos)
        {
            _camera.transform.localRotation = Quaternion.Lerp(_camera.transform.localRotation, _cameraEndRot, Time.deltaTime);
            _camera.transform.localPosition = Vector3.MoveTowards(_camera.transform.localPosition, _cameraEndPos, 3 * Time.deltaTime);
            if (_camera.transform.localPosition == _cameraEndPos)
            {
                if (!_startEndPath)
                {
                    _startEndPath = true;
                    StartCoroutine(StartEndPath());
                }
            }
            if (RenderSettings.fogEndDistance < 300f)
            {
                RenderSettings.fogEndDistance += 50 * Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator StartEndPath()
    {
        yield return new WaitForSeconds(1.5f);
        _endPath = true;
    }
}
