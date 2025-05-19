using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    private GameObject _player;
    private PlayerGunplay _playerGunplay;

    private GameObject _target;
    private TargetMovement _targetMovement;

    private Renderer _renderer;
    private BoxCollider2D _collider;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerGunplay = _player.GetComponent<PlayerGunplay>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();

        _renderer = gameObject.GetComponent<Renderer>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
    }

    public void PlaceGunOnGround()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        _targetMovement.MakeTargetTransparent();
    }

    public void DisableGun()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerGunplay.IsHasGun())
        {
            DisableGun();
        }
    }
}
