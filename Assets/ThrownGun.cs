using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGun : MonoBehaviour
{
    [SerializeField] private float _projectileHeight = 10f;
    [SerializeField] private float _projectileSpeed = 5f;

    [SerializeField] private GameObject _thrownGunProjectileShadow;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private GameObject _gun;
    private GunScript _gunScript;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _projectileTimer = 1;

    private void Start()
    {
        _gun = GameObject.FindGameObjectWithTag("Gun");
        _gunScript = _gun.GetComponent<GunScript>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();
        _endPos = _target.transform.position;

        Instantiate(_thrownGunProjectileShadow, transform.position, Quaternion.identity);

        FireProjectile(gameObject.transform.position);
    }

    void FireProjectile(Vector3 firePoint)
    {
        _startPos = firePoint;
        
        // Setting the projectileTimer to 0 will begin the fire animation
        _projectileTimer = 0;
    }

    Vector3 CalculateTrajectory(Vector3 firePos, Vector3 targetPos, float t)
    {
        Vector3 linearProgress = Vector3.Lerp(firePos, targetPos, t);
        float perspectiveOffset = Mathf.Sin(t * Mathf.PI) * _projectileHeight;

        Vector3 trajectoryPos = linearProgress + (Vector3.up * perspectiveOffset);
        return trajectoryPos;
    }

    public void MoveToNextPositionAndUpdateTimer()
    {
        Vector3 newProjectilePos = CalculateTrajectory(_startPos, _endPos, _projectileTimer);
        transform.position = newProjectilePos;

        _projectileTimer += _projectileSpeed * Time.deltaTime;
    }

    private void LandProjectile()
    {
        _gun.transform.position = gameObject.transform.position;
        _gunScript.PlaceGunOnGround();

        _targetMovement.MakeTargetTransparent();

        Destroy(gameObject);
    }

    void Update()
    {
        if(_projectileTimer < 1)
        {
            MoveToNextPositionAndUpdateTimer();
        }
        else
        {
            LandProjectile();
        }
    }
}
