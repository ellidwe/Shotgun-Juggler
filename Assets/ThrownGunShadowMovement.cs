using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGunShadowMovement : MonoBehaviour
{
    [SerializeField] private float _shadowMoveSpeed;
    [SerializeField] private float _distanceFromEndPositionToDeleteShadow;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private GameObject _player;

    private Vector3 _movementDirection;
    private Vector3 _endPosition;

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Target");
        _endPosition = _target.transform.position;
        _targetMovement = _target.GetComponent<TargetMovement>();

        _player = GameObject.FindGameObjectWithTag("Player");

        _movementDirection = new Vector3((_target.transform.position.x - _player.transform.position.x) / _targetMovement.GetTargetDistanceFromPlayer(), (_target.transform.position.y - _player.transform.position.y) / _targetMovement.GetTargetDistanceFromPlayer(), 0);        
    }

    private void MoveToNewPosition()
    {
        transform.position = transform.position + new Vector3(_movementDirection.x * _shadowMoveSpeed * Time.deltaTime, _movementDirection.y * _shadowMoveSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToNewPosition();

        if(Vector3.Distance(transform.position, _endPosition) <= _distanceFromEndPositionToDeleteShadow)
        {
            Destroy(gameObject);
        }
    }
}
