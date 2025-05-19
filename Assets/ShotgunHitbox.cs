using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunHitbox : MonoBehaviour
{
    [SerializeField] float _shotgunHitboxDurationBeforeDespawn;
    private float _timer = 0;

    private void IncreaseTimer()
    {
        _timer += Time.deltaTime;
    }

    private void CheckForDespawn()
    {
        if (_timer < _shotgunHitboxDurationBeforeDespawn)
        {
            IncreaseTimer();
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
