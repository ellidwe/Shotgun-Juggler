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

    [SerializeField] private float _time360BonusLasts;
    [SerializeField] private float _timeToAllowNextDirectionIn360;

    private int _previousDirectionMovedFor360;
    private int _lastDirectionMovedFor360;
    private int _correctDirectionToMove = 0;
    private int _alternateDirectionToMove = 0;
    private int _correctDirectionChain;
    private float _nextCorrectInputTimer;
    private bool _isGoingCounterclockwise;
    
    private bool _isIn360Bonus;

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

        AssignLastDirectionMovedFor360();

        if (Mathf.Abs(_currentMovement.x) > 0 && Mathf.Abs(_currentMovement.y) > 0) //if moving diagonally
        {
            CorrectDiagonalMovement();
        }

        if (_currentMovement.x != 0 || _currentMovement.y != 0)
        {
            _lastDirectionMoved = _currentMovement;
        }
    }

    private void CorrectDiagonalMovement()
    {
        _currentMovement.x /= Mathf.Sqrt(2);
        _currentMovement.y /= Mathf.Sqrt(2);
    }

    private void AssignLastDirectionMovedFor360()
    {
        switch (_currentMovement.y)
        {
            case -1f:
                switch (_currentMovement.x)
                {
                    case -1f:
                        _lastDirectionMovedFor360 = 1;
                        break;

                    case 0f:
                        _lastDirectionMovedFor360 = 2;
                        break;

                    case 1f:
                        _lastDirectionMovedFor360 = 3;
                        break;
                }
                break;

            case 0f:
                switch (_currentMovement.x)
                {
                    case -1f:
                        _lastDirectionMovedFor360 = 4;
                        break;

                    case 0f:
                        _lastDirectionMovedFor360 = 5;
                        break;

                    case 1f:
                        _lastDirectionMovedFor360 = 6;
                        break;
                }
                break;

            case 1f:
                switch (_currentMovement.x)
                {
                    case -1f:
                        _lastDirectionMovedFor360 = 7;
                        break;

                    case 0f:
                        _lastDirectionMovedFor360 = 8;
                        break;

                    case 1f:
                        _lastDirectionMovedFor360 = 9;
                        break;
                }
                break;
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

    private void HandleMovement360()
    {
        if (_lastDirectionMovedFor360 == _correctDirectionToMove)
        {
            _nextCorrectInputTimer = 0;
            _correctDirectionChain++;
            //Debug.Log("correct direction chain: " + _correctDirectionChain);
        }
        else if (_lastDirectionMovedFor360 == _alternateDirectionToMove)
        {
            _alternateDirectionToMove = 0;

            _isGoingCounterclockwise = true;

            _nextCorrectInputTimer = 0;
            _correctDirectionChain++;
        }
        else if (_lastDirectionMovedFor360 == _previousDirectionMovedFor360)
        {
            _nextCorrectInputTimer += Time.deltaTime;
            
            if(_nextCorrectInputTimer >= _timeToAllowNextDirectionIn360)
            {
                Break360InputChain();
            }
        }
        else
        {
            Break360InputChain();
            //Debug.Log("wrong dir!");
        }

        if(_correctDirectionChain == 8)
        {
            StartCoroutine(Give360Bonus());

            Break360InputChain();
        }

        if(_correctDirectionChain == 0)
        {
            AssignFirstCorrectNextDirectionFor360();
        }
        else
        {
            AssignCorrectNextDirectionFor360();
        }

        _previousDirectionMovedFor360 = _lastDirectionMovedFor360;
    }

    private void Break360InputChain()
    {
        _nextCorrectInputTimer = 0;
        _correctDirectionChain = 0;

        _isGoingCounterclockwise = false;
    }

    private IEnumerator Give360Bonus()
    {
        _isIn360Bonus = true;

        _playerGunplay.SetDamageDealtByPlayer(_playerGunplay.GetDamageDealtByPlayer() * 2);

        yield return new WaitForSeconds(_time360BonusLasts);

        _isIn360Bonus = false;

        _playerGunplay.SetDamageDealtByPlayer(_playerGunplay.GetDamageDealtByPlayer() / 2);
    }

    public bool IsIn360Bonus()
    {
        return _isIn360Bonus;
    }

    public void SetIn360Bonus(bool isIn360Bonus)
    {
        _isIn360Bonus = isIn360Bonus;
    }

    private void AssignFirstCorrectNextDirectionFor360()
    {
        AssignCorrectNextDirectionFor360();

        switch(_correctDirectionToMove)
        {
            case 1:
                _alternateDirectionToMove = 3;
                break;

            case 2:
                _alternateDirectionToMove = 6;
                break;

            case 3:
                _alternateDirectionToMove = 9;
                break;

            case 4:
                _alternateDirectionToMove = 2;
                break;

            case 6:
                _alternateDirectionToMove = 8;
                break;

            case 7:
                _alternateDirectionToMove = 1;
                break;

            case 8:
                _alternateDirectionToMove = 4;
                break;

            case 9:
                _alternateDirectionToMove = 7;
                break;
        }
    }

    private void AssignCorrectNextDirectionFor360()
    {
        switch (_lastDirectionMovedFor360)
        {
            case 1:
                if(!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 4;
                }
                else
                {
                    _correctDirectionToMove = 2;
                }
                break;

            case 2:
                if(!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 1; 
                }
                else
                {
                    _correctDirectionToMove = 3;
                }
                break;

            case 3:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 2;
                }
                else
                {
                    _correctDirectionToMove = 6;
                }
                break;

            case 4:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 7;
                }
                else
                {
                    _correctDirectionToMove = 1;
                }
                break;

            case 5:
                _correctDirectionToMove = 0;
                break;

            case 6:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 3;
                }
                else
                {
                    _correctDirectionToMove = 9;
                }
                break;

            case 7:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 8;
                }
                else
                {
                    _correctDirectionToMove = 4;
                }
                break;

            case 8:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 9;
                }
                else
                {
                    _correctDirectionToMove = 7;
                }
                break;

            case 9:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 6;
                }
                else
                {
                    _correctDirectionToMove = 8;
                }
                break;
        }
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
        HandleMovement360();

        if (Input.GetKeyDown(KeyCode.Space) && _dashStocks > 0 && !_dashing)
        {
            InitiateDash();
        }

        if(_dashing)
        {
            moveViaDash(_dashPath);
            //Debug.Log("dashing");
        }
        else
        {
            if(_postDashState)
            {
                //Debug.Log("postDashState");
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
