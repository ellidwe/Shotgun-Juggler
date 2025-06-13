using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick360 : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerGunplay playerGunplay;

    [SerializeField] private float _time360BonusLasts;
    [SerializeField] private float _timeToAllowNextDirectionIn360;

    private int _previousDirectionMovedFor360;

    private int _correctDirectionToMove = 0;
    private int _alternateDirectionToMove = 0;

    private int _correctDirectionChain;

    private float _nextCorrectInputTimer;

    private bool _isGoingCounterclockwise;

    private bool _isIn360Bonus;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerGunplay = gameObject.GetComponent<PlayerGunplay>();
    }

    private void HandleMovement360()
    {
        if (playerMovement.GetLastDirectionMovedNumpad() == _correctDirectionToMove)
        {
            _nextCorrectInputTimer = 0;
            _correctDirectionChain++;
            //Debug.Log("correct direction chain: " + _correctDirectionChain);
        }
        else if (playerMovement.GetLastDirectionMovedNumpad() == _alternateDirectionToMove)
        {
            _alternateDirectionToMove = 0;

            _isGoingCounterclockwise = true;

            _nextCorrectInputTimer = 0;
            _correctDirectionChain++;
        }
        else if (playerMovement.GetLastDirectionMovedNumpad() == _previousDirectionMovedFor360)
        {
            _nextCorrectInputTimer += Time.deltaTime;

            if (_nextCorrectInputTimer >= _timeToAllowNextDirectionIn360)
            {
                Break360InputChain();
            }
        }
        else
        {
            Break360InputChain();
            //Debug.Log("wrong dir!");
        }

        if (_correctDirectionChain == 8)
        {
            StartCoroutine(Give360Bonus());

            Break360InputChain();
        }

        if (_correctDirectionChain == 0)
        {
            AssignFirstCorrectNextDirectionFor360();
        }
        else
        {
            AssignCorrectNextDirectionFor360();
        }

        _previousDirectionMovedFor360 = playerMovement.GetLastDirectionMovedNumpad();
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

        playerGunplay.SetDamageDealtByPlayer(playerGunplay.GetDamageDealtByPlayer() * 2);

        yield return new WaitForSeconds(_time360BonusLasts);

        _isIn360Bonus = false;

        playerGunplay.SetDamageDealtByPlayer(playerGunplay.GetDamageDealtByPlayer() / 2);
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

        switch (_correctDirectionToMove)
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
        switch (playerMovement.GetLastDirectionMovedNumpad())
        {
            case 1:
                if (!_isGoingCounterclockwise)
                {
                    _correctDirectionToMove = 4;
                }
                else
                {
                    _correctDirectionToMove = 2;
                }
                break;

            case 2:
                if (!_isGoingCounterclockwise)
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

    // Update is called once per frame
    void Update()
    {
        HandleMovement360();
    }
}
