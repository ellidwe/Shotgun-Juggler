using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashDuration;

    private Rigidbody2D _playerRigidbody2D;
    private SpriteRenderer _playerSpriteRenderer;
    private PlayerGunplay _playerGunplay;

    private GameObject _target;
    private TargetMovement _targetMovement;

    [SerializeField] private Camera _sceneCamera;

    private Vector2 _lastDirectionMoved;
    private Vector2 _currentMovement;
    private Vector2 _userMousePos;

    private bool _movementFrozen;
    private bool _turningFrozen;

    private bool _dashing = false;
    private float _dashTimer;

    private bool _postDashState = false;
    [SerializeField] private float _postDashStateTimer;
    
    private Vector3 _dashStartPoint;
    private Vector3 _dashPath;

    private int _dashStocks = 100; //test value

    private void Start()
    {
        _playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _playerSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _playerGunplay = gameObject.GetComponent<PlayerGunplay>();

        _target = GameObject.FindGameObjectWithTag("Target");
        _targetMovement = _target.GetComponent<TargetMovement>();
    }

    public void SetMovementFrozen(bool _movFrozen)
    {
        _movementFrozen = _movFrozen;
    }

    public void SetTurningFrozen(bool _turnFrozen)
    {
        _turningFrozen = _turnFrozen;
    }

    public Vector2 GetLastDirectionMoved()
    {
        return _lastDirectionMoved;
    }

    /// <summary>
    /// Gets input and adjusts movement instance variable, called in update to change player position
    /// based on current input, changes _lastDirMoved depending on what the last direction the player moved was
    /// </summary>
    /// 
    private void ChangeMovement()
    {
        //input
        _currentMovement.x = Input.GetAxisRaw("Horizontal");
        _currentMovement.y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(_currentMovement.x) > 0 && Mathf.Abs(_currentMovement.y) > 0)
        {
            _currentMovement.x /= Mathf.Sqrt(2);
            _currentMovement.y /= Mathf.Sqrt(2);
        }

        if (_currentMovement.x != 0 || _currentMovement.y != 0)
        {
            _lastDirectionMoved = _currentMovement;
        }
    }

    /// <summary>
    /// Moves the _playerRigidbody2d position to where it should be based on the current movement input this frame
    /// </summary>
    private void MoveViaDeterminedMovement()
    {
        if(!_movementFrozen)
        {
            _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + _currentMovement * _moveSpd * Time.fixedDeltaTime);
        }
    }

    private void AssignMousePosition()
    {
        _userMousePos = _sceneCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Uses mousePos to rotate PlayerChar game object
    /// </summary>
    private void RotateToMouse()
    {
        if(!_turningFrozen)
        {
            Vector2 dirToLook = _userMousePos - _playerRigidbody2D.position;
            float angle = Mathf.Atan2(dirToLook.y, dirToLook.x) * Mathf.Rad2Deg;

            _playerRigidbody2D.rotation = angle;
        }
    }

    private void InitiateDash()
    {
        _dashStocks -= 1;

        _movementFrozen = true;

        _dashStartPoint = _playerRigidbody2D.position;
        _dashPath = new Vector3(_lastDirectionMoved.x * _dashDistance, _lastDirectionMoved.y * _dashDistance, 0);

        //cancels ground pickup timer THIS WHOLE PART SHOULD BE REWRITTEN SMARTER AT SOME POINT
        if(_playerGunplay.IsPickingGunUpFromGround())
        {
            _playerGunplay.SetPickingUpGunFromGround(false);

            _movementFrozen = false;
            _turningFrozen = false;

            _targetMovement.MakeTargetOpaque();

            _playerGunplay.SetHasGun(true);
        }
        _playerSpriteRenderer.color = Color.blue;

        _dashing = true;
        _dashTimer = 0;
    }

    private void moveViaDash(Vector3 dashPath)
    {
        if(_dashTimer < _dashDuration)
        {
            _dashTimer += Time.deltaTime;

            _playerRigidbody2D.position = _dashStartPoint + new Vector3(dashPath.x * (_dashTimer / _dashDuration), dashPath.y * (_dashTimer / _dashDuration), 0);
        }
        else
        {
            _dashing = false;
            _movementFrozen = false;

            _playerSpriteRenderer.color = Color.gray;

            StartCoroutine(RunPostDashState());
        }
    }

    private IEnumerator RunPostDashState()
    {
        _postDashState = true;

        yield return new WaitForSeconds(_postDashStateTimer);

        _postDashState = false;
    }

    public bool IsDashing()
    {
        return _dashing;
    }

    public bool IsPostDashState()
    {
        return _postDashState;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _dashStocks > 0 && !_dashing)
        {
            InitiateDash();
        }

        if(_dashing)
        {
            moveViaDash(_dashPath);
            Debug.Log("dashing");
        }
        else
        {
            if(_postDashState)
            {
                Debug.Log("postDashState");
            }
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
