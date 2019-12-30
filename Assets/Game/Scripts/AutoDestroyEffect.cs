using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public float LiveTime;
    private float _startTime;
    private void OnEnable()
    {
        _startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - _startTime >= LiveTime)
        {
            if (EffectPool.Pool.IsSpawned(transform))
            {
                EffectPool.Pool.Despawn(transform);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
