using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunplay : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private SpriteRenderer _playerSpriteRenderer;
    private PlayerMovement _playerMovement;

    [SerializeField] private GameObject _shotgunHitbox;
    [SerializeField] private float _ShotgunHitboxSpawnOffset = 2.13f;

    [SerializeField] private GameObject _gun;
    private GunScript _gunScript;

    [SerializeField] private GameObject _thrownGun;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private bool _hasGun = false;
    private bool _touchingGun = false;
    [SerializeField] private float _groundPickupMovementLockTimer;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _playerMovement = gameObject.GetComponent<PlayerMovement>();

        _gunScript = _gun.GetComponent<GunScript>();

        _playerSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();
    }

    public void ChangeSpriteColor(Color color)
    {
        _playerSpriteRenderer.color = color;
    }

    /// <summary>
    /// Shoots via instantiating the shotgun hitbox and throwing the gun
    /// </summary>
    /// 
    private void Shoot()
    {
        _targetMovement.LockTargetPosition();

        Instantiate(_shotgunHitbox, _playerRigidbody2D.position + new Vector2(_ShotgunHitboxSpawnOffset * Mathf.Cos(_playerRigidbody2D.rotation * Mathf.Deg2Rad), _ShotgunHitboxSpawnOffset * Mathf.Sin(_playerRigidbody2D.rotation * Mathf.Deg2Rad)), transform.rotation);
        Instantiate(_thrownGun, _playerRigidbody2D.position, Quaternion.identity);

        _hasGun = false;

        _gunScript.SetInAir(true);

        ChangeSpriteColor(Color.grey);
    }

    /// <summary>
    /// picks up the gun
    /// </summary>
    private void PickupGunFromGround()
    {
        _gunScript.DisappearGun();
        StartCoroutine(GroundPickupFreeze());
    }

    public IEnumerator GroundPickupFreeze()
    {
        _playerMovement.SetMovementFrozen(true);
        _playerMovement.SetTurningFrozen(true);

        _targetMovement.MakeTargetTransparent();

        yield return new WaitForSeconds(_groundPickupMovementLockTimer);

        _playerMovement.SetMovementFrozen(false);
        _playerMovement.SetTurningFrozen(false);

        _targetMovement.MakeTargetOpaque();

        _hasGun = true;
        ChangeSpriteColor(Color.black);
    }

    private void PickupGunFromAir()
    {
        _hasGun = true;
        ChangeSpriteColor(Color.black);
    }

    public bool IsHasGun()
    {
        return _hasGun;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //if left click
        {
            if(_hasGun)
            {
                Shoot();
            }
        }

        if (Input.GetMouseButtonDown(1)) //if right click
        {
            if(_touchingGun) 
            {
                if (_gunScript.IsOnGround()) //if gun on ground
                {
                    PickupGunFromGround();
                }
                else //if gun in air
                {
                    PickupGunFromAir();
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Gun"))
        {
            _touchingGun = false;
        }
    }
}
