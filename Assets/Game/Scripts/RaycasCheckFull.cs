using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class RaycasCheckFull : MonoBehaviour
{
    public Vector3 Direct;
    public float Leng = 20;
    public LayerMask LayerCheck;

    public List<BgItemIngame> Items;

    public void Init()
    {
        var hits = Physics2D.RaycastAll(transform.position, Direct, Leng, LayerCheck);
        foreach (var hit in hits)
        {
            Items.Add(hit.collider.GetComponent<BgItemIngame>());
        }
    }

    public bool IsFull()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (!Items[i].HasItem)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsFull2()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].CurBlock == null)
            {
                return false;
            }
        }

        return true;
    }

    public void ClearGround(Vector3 centerPosition)
    {
        foreach (var item in Items)
        {
            if (item.HasItem && item.CurBlock != null)
            {
                if (item.HasStar)
                {
                    GameController.Instance.CreateStarEffect(item.transform.position);
                }

                item.HasItem = false;
                item.CurBlock.ClearBlockWhenClearRow(Vector2.Distance(centerPosition, item.transform.position) / 10f);
                item.CurBlock = null;
                item.ResetBgItem();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (GameController.Instance != null && GameController.Instance.IsTestMode)
            {
                Gizmos.DrawRay(transform.position, Direct * Leng);
            }
        }
    }
#endif
}