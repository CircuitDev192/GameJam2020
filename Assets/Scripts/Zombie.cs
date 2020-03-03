using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    private float _health = 50f;
    private float _attackRate = 1.5f;
    private float _attackTimer;
    private bool _isDead = false;

    [SerializeField]
    private GameObject[] _bodyParts;
    private Animator _animator;
    private Collider _collider;

    private float _lookRadius = 125f;

    private Transform _target;
    private NavMeshAgent _agent;
    private GameObject _spawnManager;
    [SerializeField]
    private GameObject[] _pickups;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Zombie::Start() Animator is null");
        }
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("Zombie::Start() NavMeshAgent is null");
        }
        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            Debug.LogError("Zombie::Start() Collider is null");
        }

        _spawnManager = GameObject.FindWithTag("SpawnManager");
        if (_spawnManager == null)
        {
            Debug.LogError("Zombie::Start() Spawn Manager is null");
        }

        _agent.speed = 4.5f;
        _animator.SetBool("Walking_b", false);
        _attackTimer = _attackRate;
        _target = PlayerManager.instance.player.transform;
    }

    private void Update()
    {
        if (!_isDead && _target != null)
        {
            float distance = Vector3.Distance(this.transform.position, _target.transform.position);
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (distance <= _lookRadius)
            {
                _agent.SetDestination(_target.transform.position);
                _animator.SetBool("Walking_b", true);

                if (distance <= _agent.stoppingDistance)
                {
                    if (_attackTimer <= 0)
                    {
                        PlayerManager.instance.player.GetComponent<PlayerController>().TakeDamage(8f);
                        _attackTimer = _attackRate;
                    }
                    FaceTarget();
                    if (PlayerManager.instance.player.GetComponent<PlayerController>().GetHealth() <= 0f)
                    {
                        _animator.SetBool("PlayerDead_b", true);
                        GetComponent<Collider>().enabled = false;
                    }
                }

            }
            else
            {
                _animator.SetBool("Walking_b", false);
                _agent.SetDestination(this.transform.position);
            }
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0f)
        {
            _animator.enabled = false;
            _collider.enabled = false;
            _agent.enabled = false;
            for (int i = 0; i < _bodyParts.Length; i++)
            {
                _bodyParts[i].GetComponent<Collider>().enabled = true;
                _bodyParts[i].GetComponent<Rigidbody>().useGravity = true;
                _bodyParts[i].GetComponent<Rigidbody>().isKinematic = false;
                _bodyParts[i].GetComponent<Rigidbody>().drag = 1.5f;
            }
            _isDead = true;
            _spawnManager.GetComponent<SpawnManager>().ZombieDied();
            if (Random.Range(0f, 100f) < 33.3f)
            {
                Vector3 pickupPos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                Instantiate(_pickups[Random.Range(0, _pickups.Length)], pickupPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
            Destroy(this.gameObject, 20f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }
}
