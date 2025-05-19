using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [SerializeField] private float _targetDistanceFromPlayer;

    private GameObject _player;
    private PlayerMovement _playerMovement;
    private PlayerGunplay _playerGunplay;

    private SpriteRenderer _targetSpriteRenderer;
    private bool _lockTargetPosition = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _playerGunplay = _player.GetComponent<PlayerGunplay>();

        _targetSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public float GetTargetDistanceFromPlayer()
    {
        return _targetDistanceFromPlayer;
    }

    public void LockTargetPosition()
    {
        _lockTargetPosition = true;
    }

    public void UnlockTargetPosition()
    {
        _lockTargetPosition = false;
    }

    public void MakeTargetTransparent()
    {
        UnlockTargetPosition();

        Color transparent = _targetSpriteRenderer.color;
        transparent.a = 0f;
        _targetSpriteRenderer.color = transparent;
    }

    public void MakeTargetOpaque()
    {
        Color opaque = _targetSpriteRenderer.color;
        opaque.a = 1f;
        _targetSpriteRenderer.color = opaque;
    }

    private void MoveToWhereTargetShouldBe()
    {
        Vector2 playerLastDirection = _playerMovement.GetLastDirectionMoved();
        transform.position = new Vector3(_player.transform.position.x + (playerLastDirection.x * _targetDistanceFromPlayer), _player.transform.position.y + (playerLastDirection.y * _targetDistanceFromPlayer), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerGunplay.IsHasGun())
        {
            _lockTargetPosition = false;

            MakeTargetOpaque();

            MoveToWhereTargetShouldBe();
        }
        else if(_lockTargetPosition) //if gun in air
        {
            MakeTargetOpaque();
        }
        else //if gun on ground
        {
            MakeTargetTransparent();
        }
    }
}
