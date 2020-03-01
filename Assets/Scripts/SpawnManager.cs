using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{ 
    private bool _isSpawning = false;
    [SerializeField]
    private GameObject[] _spawnLocations;
    [SerializeField]
    private GameObject[] _zombies;
    [SerializeField]
    private int _maxZombies = 10;
    private int _currentZombies;

    public void StartSpawning()
    {
        _isSpawning = true;
        StartCoroutine(SpawnZombies());
    }

    public void ZombieDied()
    {
        _currentZombies--;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    IEnumerator SpawnZombies()
    {
        while (_isSpawning)
        {
            if (_currentZombies <= _maxZombies)
            {
                _currentZombies++;
                Vector3 spawnPoint;
                do
                {
                    spawnPoint = _spawnLocations[Random.Range(0, _spawnLocations.Length)].transform.position;
                } while (Vector3.Distance(PlayerManager.instance.player.transform.position, spawnPoint) < 60);

                Instantiate(_zombies[Random.Range(0, _zombies.Length)], spawnPoint, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
            yield return null;
        }
    }
}
