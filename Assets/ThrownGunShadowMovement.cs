using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGunShadowMovement : MonoBehaviour
{
    [SerializeField] private float _distanceFromEndPositionToDeleteShadow;

    private GameObject _target;

    private GameObject _thrownGun;
    private ThrownGunScript _thrownGunScript;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _distanceVector;

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Target");
        _endPosition = _target.transform.position;

        _thrownGun = GameObject.FindGameObjectWithTag("ThrownGun");
        _thrownGunScript = _thrownGun.GetComponent<ThrownGunScript>();

        _startPosition = gameObject.transform.position;

        _distanceVector = CreateDistanceVector(_startPosition, _endPosition);
    }

    private Vector3 CreateDistanceVector(Vector3 startPoint, Vector3 endPoint)
    {
        return new Vector3(endPoint.x - startPoint.x, endPoint.y - startPoint.y, 0);
    }

    private void MoveShadowToNewPosition()
    {
        gameObject.transform.position = _startPosition + new Vector3(_distanceVector.x * _thrownGunScript.GetProjectileTimer(), _distanceVector.y * _thrownGunScript.GetProjectileTimer(), 0);
    }

    private void Update()
    {
        MoveShadowToNewPosition();

        if(Vector3.Distance(gameObject.transform.position, _endPosition) <= _distanceFromEndPositionToDeleteShadow)
        {
            Destroy(gameObject);
        }
    }
}
