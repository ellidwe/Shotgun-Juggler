using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] float _enemyMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private Vector3 GetDistanceVector()
    {
        return new Vector3(_player.transform.position.x - gameObject.transform.position.x, _player.transform.position.y - gameObject.transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(GetDistanceVector().x * _enemyMoveSpeed * Time.deltaTime, GetDistanceVector().y * _enemyMoveSpeed * Time.deltaTime, 0);
    }
}
