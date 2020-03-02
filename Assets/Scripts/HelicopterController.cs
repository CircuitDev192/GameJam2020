﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [SerializeField]
    private GameObject _door, _tail, _rotor;
    [SerializeField]
    private bool _doorOpen = false;
    private bool _startPath = false;
    private Vector3 _doorOpenPos;
    private Vector3 _doorClosePos;
    [SerializeField]
    private float _speed = 15f;
    private int _lastWaypoint = 0;
    [SerializeField]
    private GameObject[] _waypoints;


    // Start is called before the first frame update
    void Start()
    {
        _doorOpenPos = new Vector3(_door.transform.localPosition.x, _door.transform.localPosition.y, _door.transform.localPosition.z + 2.81f);
        _doorClosePos = new Vector3(_door.transform.localPosition.x, _door.transform.localPosition.y, _door.transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(20, 0, 0);
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
                _lastWaypoint++;
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
    }

    public void StartMovement()
    {
        _startPath = true;
    }

    private void StartOpenDoor()
    {
        _doorOpen = true;
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
}