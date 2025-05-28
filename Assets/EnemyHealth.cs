using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float _enemyHp;

    private GameObject _player;
    private PlayerGunplay _playerGunplay;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerGunplay = _player.GetComponent<PlayerGunplay>();
    }

    public void ReduceEnemyHP(float amountToReduceHPBy)
    {
        _enemyHp -= amountToReduceHPBy;
    }

    void KillEnemy()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_enemyHp <= 0)
        {
            KillEnemy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ShotgunHitbox")
        {
            ReduceEnemyHP(_playerGunplay.GetDamageDealtByPlayer());
        }
    }
}
