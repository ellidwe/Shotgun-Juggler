using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpd = 5f;

    private Rigidbody2D _playerRigidbody2D;

    [SerializeField] private Camera _sceneCamera;

    private Vector2 _lastDirectionMoved;
    private Vector2 _currentMovement;
    private Vector2 _userMousePos;

    private bool _movementFrozen;
    private bool _turningFrozen;

    private void Start()
    {
        _playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void SetMovementFrozen(bool _movFrozen)
    {
        _movementFrozen = _movFrozen;
    }

    public void SetTurningFrozen(bool _turnFrozen)
    {
        _turningFrozen = _turnFrozen;
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

    public Vector2 GetLastDirectionMoved()
    {
        return _lastDirectionMoved;
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

    // Update is called once per frame
    void Update()
    {
        ChangeMovement();
        AssignMousePosition();
    }

    private void FixedUpdate()
    {
        MoveViaDeterminedMovement();
        RotateToMouse();
    }
}
