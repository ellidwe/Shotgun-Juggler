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

    private bool _onGround = true;
    private bool _inAir = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerGunplay = _player.GetComponent<PlayerGunplay>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();

        _renderer = gameObject.GetComponent<Renderer>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
    }

    public bool IsOnGround()
    {
        return _onGround;
    }
    public bool IsInAir()
    {
        return _inAir;
    }

    public void SetOnGround(bool ground)
    {
        _onGround = ground;
        _inAir = !ground;
    }

    public void SetInAir(bool air)
    {
        _inAir = air;
        _onGround = !air;
    }

    public void PlaceGunOnGround()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        SetOnGround(true);

        _targetMovement.MakeTargetTransparent();
    }

    public void DisappearGun()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }

    public void EnableGunCollider()
    {
        _collider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerGunplay.IsHasGun())
        {
            DisappearGun();
        }
    }
}
