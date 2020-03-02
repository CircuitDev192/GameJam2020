using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExit : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.EndGame();
        }
    }
}
