using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class PageBase : MonoBehaviour
{
    protected Animator _ani;

    public virtual void Awake()
    {
        _ani = GetComponent<Animator>();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (_ani!=null)
        {
            _ani.Play("Show", 0,0);
        }
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}