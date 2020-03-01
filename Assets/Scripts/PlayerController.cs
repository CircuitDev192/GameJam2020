using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //THIS FILE IS NOT FOR PLAYER MOVEMENT. 
    //THAT IS MANAGED BY FIRST PERSON AIO

    private Animator _animator;
    private Camera _mainCamera;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _gunSounds;
    [SerializeField]
    private GameObject _chestJoint;
    private bool _fullAuto = false;
    private float _fireRateAuto = 0.11f;
    private float _nextTimeToFireAuto;
    private float _fireRate = 0.4f;
    private float _nextTimeToFire;
    private bool _canReload = false;
    private bool _isSprinting = false;
    private float _weaponDamage = 15f;
    private bool _isReloading = false;
    private float _maxHealth = 100f;
    private float _currentHealth;
    private int _ammoCount = 30;
    private int _ammoReserve = 90;
    [SerializeField]
    private ParticleSystem _muzzleFlash;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("PlayerController::Start() Audio Source is null");
        }
        InitAnimator();
        _mainCamera = Camera.main;
        _currentHealth = _maxHealth;
    }

    void InitAnimator()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("PlayerController::Start() Animator is null");
        }
        _animator.SetFloat("Head_Horizontal_f", -0.6f);
        _animator.SetInteger("Animation_int", 0);
        _animator.SetFloat("Head_Vertical_f", 0.0f);
        _animator.SetFloat("Body_Horizontal_f", 0.6f);
        _animator.SetFloat("Body_Vertical_f", 0.0f);
        _animator.SetInteger("WeaponType_int", 2);
    }

    // Update is called once per frame
    void Update()
    {
        LockPosToCamera();

        if (!_isSprinting)
        {
            if (!_isReloading && _ammoCount > 0)
            {
                if (!_fullAuto)
                {
                    if (Input.GetMouseButtonDown(0) && Time.time >= _nextTimeToFire)
                    {
                        _animator.SetBool("Shoot_b", true);
                        _animator.SetBool("FullAuto_b", false);
                        _nextTimeToFire = Time.time + _fireRate;
                        _weaponDamage = 26f;
                        Shoot();
                        Debug.Log("You Shot");
                        StartCoroutine(EndShoot());
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        _animator.SetBool("Shoot_b", true);
                        _animator.SetBool("FullAuto_b", true);
                        if (Time.time >= _nextTimeToFireAuto)
                        {
                            _weaponDamage = 15f;
                            Shoot();
                            _nextTimeToFireAuto = Time.time + _fireRateAuto;
                            Debug.Log("You Shot");
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        _audioSource.PlayOneShot(_gunSounds[4], 1f);
                    }
                    else
                    {
                        StartCoroutine(EndShoot());
                    }
                }
            } else if (_ammoCount <= 0)
            {
                _animator.SetBool("Shoot_b", false);
                _animator.SetBool("FullAuto_b", false);
            }
        }
               

        //IDLE, WALK, RUN
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _animator.SetFloat("Speed_f", 1f);
                _isSprinting = true;
            }
            else
            {
                _animator.SetFloat("Speed_f", 0.49f);
                _isSprinting = false;
            }
        } else
        {
            _animator.SetFloat("Speed_f", 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetBool("Jump_b", true);
            StartCoroutine(EndJump());
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!_fullAuto)
            {
                _fullAuto = true;
            } else
            {
                _fullAuto = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && _canReload && _ammoReserve > 0 && !_isSprinting)
        {
            _ammoReserve += _ammoCount;
            if (_ammoReserve > 30)
            {
                _ammoCount = 30;
                _ammoReserve -= 30;
            } else
            {
                _ammoCount = _ammoReserve;
                _ammoReserve = 0;
            }
            _canReload = false;
            _isReloading = true;
            _animator.SetBool("Reload_b", true);
            _audioSource.PlayOneShot(_gunSounds[5], 1f);
            StartCoroutine(EndReload());
        }

    }

    private void Shoot()
    {
        _ammoCount--;
        if (!_muzzleFlash.isPlaying)
        {
            _muzzleFlash.Play();
        }

        if (!_fullAuto)
        {
            _audioSource.PlayOneShot(_gunSounds[0], 1f); //SingleShot
        }
        else
        {
            _audioSource.PlayOneShot(_gunSounds[Random.Range(1, 4)], 1f);
        }

        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit))
        {
            Debug.Log("Hit: " + hit.transform.name);    

            Zombie zombie = hit.transform.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(_weaponDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0f)
        {
            _animator.SetBool("Death_b", true);
        }
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public void AddHealth(float health)
    {
        _currentHealth += health;
        if (_currentHealth > 100f)
        {
            _currentHealth = 100f;
        }
    }

    public int GetAmmoCount(bool reserve)
    {
        if (reserve)
        {
            return _ammoReserve;
        } else
        {
            return _ammoCount;
        }
    }

    public void AddAmmo(int ammo)
    {
        _ammoReserve += ammo;
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Jump_b", false);
    }
    IEnumerator EndReload()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Reload_b", false);
        yield return new WaitForSeconds(1.9f);
        _isReloading = false;
    }

    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Shoot_b", false);
        _canReload = true;
    }

    private void LockPosToCamera()
    {
        this.transform.position = new Vector3(this.transform.parent.position.x - 0.07f, this.transform.parent.position.y - 1.029999f, this.transform.parent.position.z);
    }
}
