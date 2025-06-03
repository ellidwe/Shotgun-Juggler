using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _playerStartingHealth;

    private float _playerCurrentHealth;

    private PlayerMovement _playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        _playerCurrentHealth = _playerStartingHealth;

        _playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    private void GameOver()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerCurrentHealth <= 0)
        {
            GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Enemy"))
        {
            if(!_playerMovement.IsDashing())
            {
                _playerCurrentHealth -= 1; //change later to make it reduce playerhealth by enemy damage unless decide to make one hit kill
            }
        }
    }
}
