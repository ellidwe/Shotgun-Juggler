using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunHitbox : MonoBehaviour
{
    [SerializeField] float shotgunHitboxDurationBeforeDespawn;
    private float timer = 0;

    private SpriteRenderer spriteRenderer;

    private GameObject player;
    private PlayerGunplay playerGunplay;
    private PlayerDash playerDash;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerGunplay = player.GetComponent<PlayerGunplay>();
        playerDash = player.GetComponent<PlayerDash>();

        if(playerGunplay.GetDamageDealtByPlayer() == 2)
        {
            if(playerDash.IsDashing())
            {
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.yellow;
            }
        }
        else if (playerDash.IsDashing())
        {
            spriteRenderer.color = Color.blue;
        }
    }

    private void CheckForDespawn()
    {
        if (timer < shotgunHitboxDurationBeforeDespawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForDespawn();
    }
}
