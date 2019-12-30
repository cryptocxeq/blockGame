using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class StarFlyEffect : MonoBehaviour
{
    public float LifeTimeAfterFinishMove = .3f;
    private bool isMoving;
    private float _curTime;

    private void Update()
    {
        if (!isMoving)
        {
            _curTime -= Time.deltaTime;
            if (_curTime <= 0)
            {
                if (EffectPool.Pool.IsSpawned(transform))
                    EffectPool.Pool.Despawn(transform);
                else
                    Destroy(gameObject);
            }
        }
    }

    public void MoveToTargetPosition(Vector3 position)
    {
        isMoving = true;
        transform.DOMove(position, 5f).SetSpeedBased(true).OnComplete(() =>
        {
            ButtonRotateProgress.Instance.ChangeValue(1);
            isMoving = false;
        });
        _curTime = LifeTimeAfterFinishMove;
    }
}