using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpd;

    private Rigidbody2D playerRigidbody2D;
    private SpriteRenderer playerSpriteRenderer;
    private PlayerGunplay playerGunplay;
    private PlayerDash playerDash;

    private GameObject target;
    private TargetMovement targetMovement;

    [SerializeField] private Camera _sceneCamera;

    private Vector2 lastDirectionMoved;
    private int lastDirectionMovedNumpad;
    private Vector2 currentMovement;
    private Vector2 userMousePos;

    private bool movementFrozen;
    private bool turningFrozen;

    private void Start()
    {
        playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        playerSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        playerGunplay = gameObject.GetComponent<PlayerGunplay>();
        playerDash = gameObject.GetComponent<PlayerDash>();

        target = GameObject.FindGameObjectWithTag("Target");
        targetMovement = target.GetComponent<TargetMovement>();
    }

    public void SetMovementFrozen(bool _movFrozen)
    {
        movementFrozen = _movFrozen;
    }

    public void SetTurningFrozen(bool _turnFrozen)
    {
        turningFrozen = _turnFrozen;
    }

    public Vector2 GetLastDirectionMoved()
    {
        return lastDirectionMoved;
    }

    /// <summary>
    /// Gets input and adjusts movement instance variable, called in update to change player position
    /// based on current input, changes _lastDirMoved depending on what the last direction the player moved was
    /// </summary>
    /// 
    private void ChangeMovement()
    {
        //input
        currentMovement.x = Input.GetAxisRaw("Horizontal");
        currentMovement.y = Input.GetAxisRaw("Vertical");

        AssignLastDirectionMovedNumpad();

        if (Mathf.Abs(currentMovement.x) > 0 && Mathf.Abs(currentMovement.y) > 0) //if moving diagonally
        {
            CorrectDiagonalMovement();
        }

        if (currentMovement.x != 0 || currentMovement.y != 0)
        {
            lastDirectionMoved = currentMovement;
        }
    }

    /// <summary>
    /// Divides diagonal movement x and y by sqrt2 so the magnitude of the diagonal movement vector is equal to the magnitude of the horizontal/vertical movement vectors.
    /// </summary>
    private void CorrectDiagonalMovement()
    {
        currentMovement.x /= Mathf.Sqrt(2);
        currentMovement.y /= Mathf.Sqrt(2);
    }

    /// <summary>
    /// Stores the value of the last direction moved in by the player, in numpad notation, in the lastDirectionMovedNumpad variable.
    /// </summary>
    private void AssignLastDirectionMovedNumpad()
    {
        switch (currentMovement.y)
        {
            case -1f:
                switch (currentMovement.x)
                {
                    case -1f:
                        lastDirectionMovedNumpad = 1;
                        break;

                    case 0f:
                        lastDirectionMovedNumpad = 2;
                        break;

                    case 1f:
                        lastDirectionMovedNumpad = 3;
                        break;
                }
                break;

            case 0f:
                switch (currentMovement.x)
                {
                    case -1f:
                        lastDirectionMovedNumpad = 4;
                        break;

                    case 0f:
                        lastDirectionMovedNumpad = 5;
                        break;

                    case 1f:
                        lastDirectionMovedNumpad = 6;
                        break;
                }
                break;

            case 1f:
                switch (currentMovement.x)
                {
                    case -1f:
                        lastDirectionMovedNumpad = 7;
                        break;

                    case 0f:
                        lastDirectionMovedNumpad = 8;
                        break;

                    case 1f:
                        lastDirectionMovedNumpad = 9;
                        break;
                }
                break;
        }
    }

    public int GetLastDirectionMovedNumpad()
    {
        return lastDirectionMovedNumpad;
    }

    /// <summary>
    /// Moves the _playerRigidbody2d position to where it should be based on the current movement input this frame.
    /// </summary>
    private void MoveViaDeterminedMovement()
    {
        if(!movementFrozen)
        {
            playerRigidbody2D.MovePosition(playerRigidbody2D.position + currentMovement * moveSpd * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Gets the position of the cursor
    /// </summary>
    private void AssignMousePosition()
    {
        userMousePos = _sceneCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Uses mousePos to rotate PlayerChar game object.
    /// </summary>
    private void RotateToMouse()
    {
        if(!turningFrozen)
        {
            Vector2 dirToLook = userMousePos - playerRigidbody2D.position;
            float angle = Mathf.Atan2(dirToLook.y, dirToLook.x) * Mathf.Rad2Deg;

            playerRigidbody2D.rotation = angle;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if(!playerDash.IsDashing())
        {
            ChangeMovement();
        }
        AssignMousePosition();
    }

    private void FixedUpdate()
    {
        MoveViaDeterminedMovement();
        RotateToMouse();
    }
}
