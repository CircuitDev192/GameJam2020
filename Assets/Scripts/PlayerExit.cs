using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExit : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _camera.transform.SetParent(this.transform.parent.transform, true);
            Destroy(_camera.transform.GetChild(0).gameObject);
            GameManager.instance.DestroyPlayer();
        }
    }
}
