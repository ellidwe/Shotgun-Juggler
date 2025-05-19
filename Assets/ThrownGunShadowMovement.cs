using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGunShadowMovement : MonoBehaviour
{
    [SerializeField] private float _shadowMoveSpeed;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private Vector3 _movementDirection;

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();

        _movementDirection = new Vector3(_target.transform.position.x / _targetMovement.GetTargetDistanceFromPlayer(), _target.transform.position.x / _targetMovement.GetTargetDistanceFromPlayer(), 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(_movementDirection.x * _shadowMoveSpeed * Time.deltaTime, _movementDirection.y * _shadowMoveSpeed * Time.deltaTime);
    }
}
