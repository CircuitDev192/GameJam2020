using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private bool _isAmmo;
    [SerializeField]
    private GameObject _pointA, _pointB;
    private bool _goingToA;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position == _pointA.transform.position)
        {
            _goingToA = false;
        } else if (this.transform.position == _pointB.transform.position)
        {
            _goingToA = true;
        }

        if (_goingToA)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _pointA.transform.position, 0.5f * Time.deltaTime);
        } else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _pointB.transform.position, 0.5f * Time.deltaTime);
        }

        this.transform.Rotate(0, 20f * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (_isAmmo)
            {
                PlayerManager.instance.player.GetComponent<PlayerController>().AddAmmo(30);
            } else
            {
                PlayerManager.instance.player.GetComponent<PlayerController>().AddHealth(25f);
            }
            Destroy(gameObject);
        }
    }
}
