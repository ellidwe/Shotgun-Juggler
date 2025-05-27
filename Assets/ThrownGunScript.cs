using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGunScript : MonoBehaviour
{
    [SerializeField] private float _projectileHeight;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _timeThresholdToAllowAirPickup;

    [SerializeField] private GameObject _thrownGunProjectileShadow;

    private SpriteRenderer _thrownGunSpriteRenderer;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private GameObject _gun;
    private GunScript _gunScript;

    private Vector3 _projectileStartPos;
    private Vector3 _projectileEndPos;

    private float _projectileTimer;

    //TEST/DEBUG CODE, creates timer that represents amount of time gun is catchable in air by player
    //private float _timeAirPickupable;

    private void Start()
    {
        _thrownGunSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();
        _projectileEndPos = _target.transform.position;

        _gun = GameObject.FindGameObjectWithTag("Gun");
        _gunScript = _gun.GetComponent<GunScript>();

        _projectileStartPos = gameObject.transform.position;

        // Setting _projectileTimer to 0 will begin the arc animation
        _projectileTimer = 0;

        //Instantiates shadow object that travels alongside thrown gun projectile
        Instantiate(_thrownGunProjectileShadow, transform.position, Quaternion.identity);

        //TEST/DEBUG CODE, sets initial value of _timeAirPickupable
        //_timeAirPickupable = 0;
    }

    //Directly copied from gamedevstackexchange (https://gamedev.stackexchange.com/questions/177374/emulating-3d-trajectory-in-top-down-2d-game) 
    //I have no clue why it works but it does and I'm scared to do anything with it at all so I'm leaving it confusing to read sorry
    //I think it returns the next position in the arc that the projectile should move to but idk how it does that (Math)
    Vector3 CalculateTrajectory(Vector3 firePos, Vector3 targetPos, float t)
    {
        Vector3 linearProgress = Vector3.Lerp(firePos, targetPos, t);
        float perspectiveOffset = Mathf.Sin(t * Mathf.PI) * _projectileHeight;

        Vector3 trajectoryPos = linearProgress + (Vector3.up * perspectiveOffset);
        return trajectoryPos;
    }

    public void MoveToNextPositionAndUpdateTimer()
    {
        Vector3 newProjectilePos = CalculateTrajectory(_projectileStartPos, _projectileEndPos, _projectileTimer);
        transform.position = newProjectilePos;

        _gun.transform.position = gameObject.transform.position;

        _projectileTimer += _projectileSpeed * Time.deltaTime;
    }

    private void AllowAirPickup()
    {

        _gunScript.SetGunPickupable(true);
        _thrownGunSpriteRenderer.color = Color.yellow;
    }

    private void LandProjectile()
    {
        //TEST/DEBUG CODE, prints _timeAirPickupable
        //Debug.Log(_timeAirPickupable);

        _gun.transform.position = gameObject.transform.position;
        _gunScript.PlaceGunOnGround();

        _targetMovement.MakeTargetTransparent();

        Destroy(gameObject);
    }

    public float GetProjectileTimer()
    {
        return _projectileTimer;
    }

    void Update()
    {
        if(_projectileTimer < 1)
        {
            if(_projectileTimer > _timeThresholdToAllowAirPickup)
            {
                //TEST/DEBUG CODE, increments timeCatchable while gun is catchable
                //_timeAirPickupable += Time.deltaTime;

                AllowAirPickup();
            }

            MoveToNextPositionAndUpdateTimer();
        }
        else
        {
            LandProjectile();
        }
    }
}
