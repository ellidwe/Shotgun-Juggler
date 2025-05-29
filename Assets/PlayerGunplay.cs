using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunplay : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private PlayerMovement _playerMovement;

    private SpriteRenderer _playerSpriteRenderer;
    private Renderer _missedJuggleRenderer;

    [SerializeField] private GameObject _shotgunHitbox;
    [SerializeField] private float _ShotgunHitboxSpawnOffset;

    [SerializeField] private GameObject _gun;
    private GunScript _gunScript;

    [SerializeField] private GameObject _thrownGun;

    private GameObject _target;
    private TargetMovement _targetMovement;

    [SerializeField] private float _groundPickupMovementLockTimer;
    [SerializeField] private float _timeToDisplayMissedJuggleIndicator;

    [SerializeField] private float _damageDealtByPlayer;

    private bool _pickingGunUpFromGround = false;
    private bool _hasGun = false;
    private bool _touchingGun = false;
    private bool _touchingTarget = false;
    private bool _airPickupPermitted;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _playerMovement = gameObject.GetComponent<PlayerMovement>();

        _playerSpriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        _missedJuggleRenderer = transform.GetChild(2).gameObject.GetComponent<Renderer>();
        _missedJuggleRenderer.enabled = false;

        _gunScript = _gun.GetComponent<GunScript>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();
    }

    public void ChangeSpriteColor(Color color)
    {
        _playerSpriteRenderer.color = color;
    }

    private void Shoot()
    {
        Vector2 shotgunHitboxSpawnPos = _playerRigidbody2D.position + new Vector2(_ShotgunHitboxSpawnOffset * Mathf.Cos(_playerRigidbody2D.rotation * Mathf.Deg2Rad), _ShotgunHitboxSpawnOffset * Mathf.Sin(_playerRigidbody2D.rotation * Mathf.Deg2Rad));
        Instantiate(_shotgunHitbox, shotgunHitboxSpawnPos, transform.rotation);
    }

    /// <summary>
    /// Shoots via instantiating the shotgun hitbox and throwing the gun
    /// </summary>
    /// 
    private void ShootAndThrowGun()
    {
        _targetMovement.LockTargetPosition();

        Shoot();

        Instantiate(_thrownGun, _playerRigidbody2D.position, Quaternion.identity);

        _hasGun = false;
        _airPickupPermitted = true;

        _gunScript.SetInAir(true);
    }

    /// <summary>
    /// picks up the gun
    /// </summary>
    private void PickupGunFromGround()
    {
        _gunScript.DisappearGun();
        StartCoroutine(GroundPickupFreeze());
    }

    private IEnumerator GroundPickupFreeze()
    {
        _pickingGunUpFromGround = true;

        _playerMovement.SetMovementFrozen(true);
        _playerMovement.SetTurningFrozen(true);

        _targetMovement.MakeTargetTransparent();

        yield return new WaitForSeconds(_groundPickupMovementLockTimer);

        if(_pickingGunUpFromGround)
        {
            _pickingGunUpFromGround = false;

            _playerMovement.SetMovementFrozen(false);
            _playerMovement.SetTurningFrozen(false);

            _targetMovement.MakeTargetOpaque();

            _hasGun = true;
        }
    }

    private void PickupGunFromAir()
    {
        _gunScript.SetGunPickupable(false);

        Destroy(GameObject.FindGameObjectWithTag("ThrownGun"));
        Destroy(GameObject.FindGameObjectWithTag("ThrownGunProjectileShadow"));

        _hasGun = true;
    }
    private void IndicateMissedJuggleAndDisallowJuggle()
    {
        _airPickupPermitted = false;
        StartCoroutine(ShowMissedJuggleIndicator());
    }
    private IEnumerator ShowMissedJuggleIndicator()
    {
        _missedJuggleRenderer.enabled = true;

        yield return new WaitForSeconds(_timeToDisplayMissedJuggleIndicator);

        _missedJuggleRenderer.enabled = false;
    }

    public bool IsHasGun()
    {
        return _hasGun;
    }

    public void SetHasGun(bool hasGun)
    {
        _hasGun = hasGun;
    }

    public bool IsPickingGunUpFromGround()
    {
        return _pickingGunUpFromGround;
    }

    public void SetPickingUpGunFromGround(bool pickingUpGunFromGround)
    {
        _pickingGunUpFromGround = pickingUpGunFromGround;
    }

    public float GetDamageDealtByPlayer()
    {
        return _damageDealtByPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_playerMovement.IsDashing())
        {
            if(_hasGun)
            {
                ChangeSpriteColor(Color.black);
            }
            else
            {
                ChangeSpriteColor(Color.gray);
            }
        }

        if (Input.GetMouseButtonDown(0)) //if left click
        {
            if (_hasGun)
            {
                if (_playerMovement.IsDashing() || _playerMovement.IsPostDashState())
                {
                    Shoot();
                }
                else
                {
                    ShootAndThrowGun();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) //if right click
        {
            if(_touchingGun && _gunScript.IsOnGround()) 
            {
                PickupGunFromGround();
            }
            else if(_gunScript.IsInAir())
            {
                if (_touchingTarget && _gunScript.IsGunPickupable() && _airPickupPermitted)
                {
                    PickupGunFromAir();
                }
                else
                {
                    IndicateMissedJuggleAndDisallowJuggle();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Gun"))
        {
            _touchingGun = true;
        }
        if (collision.tag.Equals("Target"))
        {
            _touchingTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Gun"))
        {
            _touchingGun = false;
        }
        if (collision.tag.Equals("Target"))
        {
            _touchingTarget = false;
        }
    }
}
