using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGunOldDeleteLater : MonoBehaviour
{
    private GameObject _player;
    private PlayerMovement _playerMovement;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private GameObject _gun;
    private GunScript _gunScript;

    private Rigidbody2D _thrownGunRigidbody2d;

    [SerializeField] private float _forceMagnitude;
    [SerializeField] private float _xMultiplier;
    
    [SerializeField] private float _distanceFromTargetToDeleteObjAt;
    private float _yTargetToDeleteObjAt;

    private float _previousYPosition;

    private void InitializeProjectileMotion()
    {
        Vector2 lastDirectionMoved = _playerMovement.GetLastDirectionMoved();

        _yTargetToDeleteObjAt = _target.transform.position.y;

        _thrownGunRigidbody2d.AddForce(new Vector2(_forceMagnitude * lastDirectionMoved.x * _xMultiplier, _forceMagnitude));

        _previousYPosition = transform.position.y; //sets initial y position of thrownGun object
    }    

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerMovement = _player.GetComponent<PlayerMovement>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();

        _gun = GameObject.FindGameObjectWithTag("Gun");
        _gunScript = _gun.GetComponent<GunScript>();

        _thrownGunRigidbody2d = gameObject.GetComponent<Rigidbody2D>();

        InitializeProjectileMotion();
    }

    private void LandProjectile()
    {
        _gun.transform.position = gameObject.transform.position;
        _gunScript.PlaceGunOnGround();

        _targetMovement.MakeTargetTransparent();

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < _previousYPosition) //if moving downward
        {
            if(Mathf.Abs(_yTargetToDeleteObjAt - transform.position.y) <= _distanceFromTargetToDeleteObjAt || transform.position.y < _yTargetToDeleteObjAt) //if the distance between the current y and the target y is small enough or if the current y is below the target y
            {
                LandProjectile();
            }    
        }
        _previousYPosition = transform.position.y; //updates previous y pos
    }
}
