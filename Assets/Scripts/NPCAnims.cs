using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnims : MonoBehaviour
{
    [SerializeField]
    private int _animToPlay;
    [SerializeField]
    private bool _crouch;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("NPCAnims:Start() Animator is null");
        }
        else
        {
            _animator.SetInteger("Animation_int", _animToPlay);
        }
        if (_crouch)
        {
            _animator.SetBool("Crouch_b", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
