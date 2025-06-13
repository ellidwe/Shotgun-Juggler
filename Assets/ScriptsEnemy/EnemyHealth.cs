using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float _enemyHp;

    private GameObject player;
    private PlayerGunplay playerGunplay;
    private PlayerDash playerDash;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerGunplay = player.GetComponent<PlayerGunplay>();
        playerDash = player.GetComponent<PlayerDash>();
    }

    public void ReduceEnemyHP(float amountToReduceHPBy)
    {
        _enemyHp -= amountToReduceHPBy;
    }

    void KillEnemy()
    {
        Destroy(gameObject);

        playerDash.SetDashStocks(playerDash.GetDashStocks() + 1);
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
            ReduceEnemyHP(playerGunplay.GetDamageDealtByPlayer());
        }
    }
}
