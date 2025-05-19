using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunplay : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;

    [SerializeField] private GameObject _shotgunHitbox;
    [SerializeField] private float _ShotgunHitboxSpawnOffset = 2.13f;

    [SerializeField] private GameObject _thrownGun;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private SpriteRenderer _playerSpriteRenderer;

    private bool _hasGun = false;
    private bool _touchingGun = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();

        _playerSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
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

        ChangeSpriteColor(Color.grey);
    }

    /// <summary>
    /// picks up the gun
    /// </summary>
    private void PickupGun()
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
                PickupGun();
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
