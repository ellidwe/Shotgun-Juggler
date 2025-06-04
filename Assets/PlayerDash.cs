using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private Rigidbody2D playerRigidbody2d;
    private PlayerGunplay playerGunplay;
    private PlayerMovement playerMovement;

    private GameObject target;
    private TargetMovement targetMovement;

    [SerializeField] private float dashDistance;
    [SerializeField] private float dashDuration;

    private bool dashing = false;
    private float dashTimer;

    private bool postDashState = false;
    [SerializeField] private float postDashStateTimer;

    private Vector3 dashStartPoint;
    private Vector3 dashPath;
    private int dashStocks = 100; //test value

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        playerGunplay = gameObject.GetComponent<PlayerGunplay>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();

        target = GameObject.FindGameObjectWithTag("Target");
        targetMovement = target.GetComponent<TargetMovement>();
    }

    private void InitiateDash()
    {
        dashStocks -= 1;

        playerMovement.SetMovementFrozen(true);

        dashStartPoint = playerRigidbody2d.position;
        dashPath = new Vector3((playerMovement.GetLastDirectionMoved().x * dashDistance), (playerMovement.GetLastDirectionMoved().y * dashDistance), 0);

        //cancels ground pickup timer THIS WHOLE PART SHOULD BE REWRITTEN SMARTER AT SOME POINT
        if (playerGunplay.IsPickingGunUpFromGround())
        {
            playerGunplay.SetPickingUpGunFromGround(false);

            playerMovement.SetMovementFrozen(false);
            playerMovement.SetTurningFrozen(false);

            targetMovement.MakeTargetOpaque();

            playerGunplay.SetHasGun(true);
        }
        //playerSpriteRenderer.color = Color.blue; DELETE IF UNNECESSARY

        dashing = true;
        dashTimer = 0;
    }

    private void moveViaDash(Vector3 dashPath)
    {
        if (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;

            playerRigidbody2d.position = dashStartPoint + new Vector3(dashPath.x * (dashTimer / dashDuration), dashPath.y * (dashTimer / dashDuration), 0);
        }
        else
        {
            dashing = false;
            playerMovement.SetMovementFrozen(false);

            //playerSpriteRenderer.color = Color.gray; DELETE IF UNNEC

            StartCoroutine(RunPostDashState());
        }
    }

    private IEnumerator RunPostDashState()
    {
        postDashState = true;

        yield return new WaitForSeconds(postDashStateTimer);

        postDashState = false;
    }

    public bool IsDashing()
    {
        return dashing;
    }

    public bool IsPostDashState()
    {
        return postDashState;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dashStocks > 0 && !dashing)
        {
            InitiateDash();
        }

        if (dashing)
        {
            moveViaDash(dashPath);
        }
    }
}
