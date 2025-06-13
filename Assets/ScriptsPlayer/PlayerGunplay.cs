using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunplay : MonoBehaviour
{
    private Rigidbody2D playerRigidbody2D;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private Trick360 trick360;


    private SpriteRenderer playerSpriteRenderer;
    private Renderer missedJuggleRenderer;

    [SerializeField] private GameObject shotgunHitbox;
    [SerializeField] private float shotgunHitboxSpawnOffset;

    [SerializeField] private GameObject gun;
    private GunScript gunScript;

    [SerializeField] private GameObject thrownGun;

    private GameObject target;
    private TargetMovement targetMovement;

    [SerializeField] private float groundPickupMovementLockTimer;
    [SerializeField] private float timeToDisplayMissedJuggleIndicator;
    [SerializeField] private float timeAirPickupBonusLasts;

    [SerializeField] private float damageDealtByPlayer;

    private bool pickingGunUpFromGround = false;
    private bool hasGun = false;
    private bool touchingGun = false;
    private bool touchingTarget = false;
    private bool airPickupPermitted;

    private bool isInAirPickupBonus;
    

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerDash = gameObject.GetComponent<PlayerDash>();
        trick360 = gameObject.GetComponent<Trick360>();

        playerSpriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        missedJuggleRenderer = transform.GetChild(2).gameObject.GetComponent<Renderer>();
        missedJuggleRenderer.enabled = false;

        gunScript = gun.GetComponent<GunScript>();

        target = GameObject.FindGameObjectWithTag("Target");
        targetMovement = target.GetComponent<TargetMovement>();
    }

    public void ChangeSpriteColor(Color color)
    {
        playerSpriteRenderer.color = color;
    }

    private void Shoot()
    {
        trick360.SetIn360Bonus(false);
        isInAirPickupBonus = false;

        Vector2 shotgunHitboxSpawnPos = playerRigidbody2D.position + new Vector2(shotgunHitboxSpawnOffset * Mathf.Cos(playerRigidbody2D.rotation * Mathf.Deg2Rad), shotgunHitboxSpawnOffset * Mathf.Sin(playerRigidbody2D.rotation * Mathf.Deg2Rad));

        Instantiate(shotgunHitbox, shotgunHitboxSpawnPos, transform.rotation);
    }

    /// <summary>
    /// Shoots via instantiating the shotgun hitbox and throwing the gun
    /// </summary>
    /// 
    private void ShootAndThrowGun()
    {
        targetMovement.LockTargetPosition();

        Shoot();

        Instantiate(thrownGun, playerRigidbody2D.position, Quaternion.identity);

        hasGun = false;
        airPickupPermitted = true;

        gunScript.SetInAir(true);
    }

    /// <summary>
    /// picks up the gun
    /// </summary>
    private void PickupGunFromGround()
    {
        gunScript.DisappearGun();
        StartCoroutine(GroundPickupFreeze());
    }

    private IEnumerator GroundPickupFreeze()
    {
        pickingGunUpFromGround = true;

        playerMovement.SetMovementFrozen(true);
        playerMovement.SetTurningFrozen(true);

        targetMovement.MakeTargetTransparent();

        yield return new WaitForSeconds(groundPickupMovementLockTimer);

        if(pickingGunUpFromGround)
        {
            pickingGunUpFromGround = false;

            playerMovement.SetMovementFrozen(false);
            playerMovement.SetTurningFrozen(false);

            targetMovement.MakeTargetOpaque();

            hasGun = true;
        }
    }

    private void PickupGunFromAir()
    {
        gunScript.SetGunPickupable(false);

        Destroy(GameObject.FindGameObjectWithTag("ThrownGun"));
        Destroy(GameObject.FindGameObjectWithTag("ThrownGunProjectileShadow"));

        hasGun = true;

        StartCoroutine(AirPickupDamageBonus());
    }

    private IEnumerator AirPickupDamageBonus()
    {
        isInAirPickupBonus = true;

        damageDealtByPlayer *= 2;

        yield return new WaitForSeconds(timeAirPickupBonusLasts);

        isInAirPickupBonus = false;

        damageDealtByPlayer /= 2;
    }

    private void IndicateMissedJuggleAndDisallowJuggle()
    {
        airPickupPermitted = false;
        StartCoroutine(ShowMissedJuggleIndicator());
    }

    private IEnumerator ShowMissedJuggleIndicator()
    {
        missedJuggleRenderer.enabled = true;

        yield return new WaitForSeconds(timeToDisplayMissedJuggleIndicator);

        missedJuggleRenderer.enabled = false;
    }

    public bool IsHasGun()
    {
        return hasGun;
    }

    public void SetHasGun(bool hasGun)
    {
        this.hasGun = hasGun;
    }

    public bool IsPickingGunUpFromGround()
    {
        return pickingGunUpFromGround;
    }

    public void SetPickingUpGunFromGround(bool pickingUpGunFromGround)
    {
        pickingGunUpFromGround = pickingUpGunFromGround;
    }

    public float GetDamageDealtByPlayer()
    {
        return damageDealtByPlayer;
    }

    public void SetDamageDealtByPlayer(float newDmg)
    {
        damageDealtByPlayer = newDmg;
    }

    private void ManageSpriteColor()
    {
        if(playerDash.IsDashing())
        {
            ChangeSpriteColor(Color.blue);
        }
        else if(trick360.IsIn360Bonus() || isInAirPickupBonus)
        {
            ChangeSpriteColor(Color.yellow);
        }
        else if(hasGun)
        {
            ChangeSpriteColor(Color.black);
        }
        else
        {
            ChangeSpriteColor(Color.gray);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        ManageSpriteColor();

        if (Input.GetMouseButtonDown(0)) //if left click
        {
            if (hasGun)
            {
                if (playerDash.IsDashing() || playerDash.IsPostDashState())
                {
                    Shoot();
                }
                else
                {
                    ShootAndThrowGun();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) //if right click
        {
            if(touchingGun && gunScript.IsOnGround()) 
            {
                PickupGunFromGround();
            }
            else if(gunScript.IsInAir())
            {
                if (touchingTarget && gunScript.IsGunPickupable() && airPickupPermitted)
                {
                    PickupGunFromAir();
                }
                else
                {
                    IndicateMissedJuggleAndDisallowJuggle();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Gun"))
        {
            touchingGun = true;
        }
        if (collision.tag.Equals("Target"))
        {
            touchingTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Gun"))
        {
            touchingGun = false;
        }
        if (collision.tag.Equals("Target"))
        {
            touchingTarget = false;
        }
    }
}
