using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerMovement : MonoBehaviour
{
    [SerializeField] private float _enemySpawnerMoveSpeed;
    [SerializeField] private float _distanceToChangeTargetPoint;

    private int _nextPoint = 1;

    private float[,] _travelToList = new float[8, 2]
    {
        {0f, 4f},
        {5f, 3f},
        {10f, 0f},
        {5f, -3f},
        {0f, -4f},
        {-5f, -3f},
        {-10, 0},
        {-5f, 3f},
    };  
        
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(_travelToList[_nextPoint, 0], _travelToList[_nextPoint, 1], 0), _enemySpawnerMoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, new Vector3(_travelToList[_nextPoint, 0], _travelToList[_nextPoint, 1], 0)) <= _distanceToChangeTargetPoint)
        {
            if (_nextPoint == _travelToList.GetLength(0) - 1)
            {
                _nextPoint = 0;
            }
            else
            {
                _nextPoint++;
            }
        }
    }
}
