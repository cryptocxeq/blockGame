using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BgItemIngame : MonoBehaviour
{
    public bool HasItem;
    public BlockItem CurBlock, getBlockItem;

    public Transform CurBlockItemTransform;

    //public SpriteRenderer Render;
    public SpriteRenderer Preview;
    public bool HasStar;

    private Color _deactiveColor;
    private Color _activeColor = new Color(1, 1, 1, .2f);
    [SerializeField] private GameObject _star;

    private void Awake()
    {
        _deactiveColor = new Color(1, 1, 1, 0);
    }

    private void OnEnable()
    {
        //ResetObj();
    }

    public void ResetObj()
    {
        HasItem = false;
        CurBlock = null;
        _star.SetActive(false);
    }

    public void AddBlockItem(BlockItem blockItem)
    {
        if (blockItem == CurBlock)
            return;
        Preview.sprite = blockItem.GetComponent<SpriteRenderer>().sprite;
        CurBlock = blockItem;
    }

    public void RemoveBlockItem(BlockItem blockItem)
    {
        Preview.sprite = null;
        CurBlock = null;
    }

    public void ApplyNewBlock(BlockItem blockItem)
    {
        blockItem.SetNoStar();
        HasItem = true;
        CurBlock = blockItem;

        Preview.sprite = null;
        Preview.DOKill();
        _star.SetActive(false);
        HasStar = false;
    }

    public void ApplyOldBlock(BlockItem blockItem,int sprite,int valueStar)
    {
        blockItem.SetNoStar();
        HasItem = true;
        CurBlock = blockItem;
        int index = 0;
        bool star;
        for(int i = 0; i < GameController.Instance.BlockItemSprites.Count; i++)
        {
            if (sprite.ToString() == GameController.Instance.BlockItemSprites[i].name)
            {
                index = i;
            }
        }
        blockItem.SetDefaulSprite(GameController.Instance.BlockItemSprites[index]);
        Preview.sprite = null;
        Preview.DOKill();
        if (valueStar == 1)
        {
            star = true;
        }
        else
        {
            star = false;
        }
        _star.SetActive(star);
        HasStar = star;
    }

    public void ApplyBlock(BlockItem blockItem)
    {
        HasItem = true;
        CurBlock = blockItem;

        int valueStar = 0;
        if (blockItem.HasStar)
        {
            valueStar = 1;
        }
        else
        {
            valueStar = 0;
        }

        SaveGame.instance.saveBlock(gameObject.name, int.Parse(blockItem.GetComponent<BlockItem>().DefaultSprite.name), valueStar);

        Preview.sprite = null;
        Preview.DOKill();
        _star.SetActive(blockItem.HasStar);
        HasStar = blockItem.HasStar;
    }

    public void restart()
    {
        CurBlock = null;
        HasStar = false;
        HasItem = false;
        _star.SetActive(false);
        //Debug.Log(gameObject.transform.childCount);
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        PlayerPrefs.DeleteKey("background_" + gameObject.name);
        SaveGame.instance.deleteAll();
    }

    public void ResetBgItem()
    {
        HasStar = false;
        HasItem = false;
        _star.SetActive(false);
        PlayerPrefs.DeleteKey("background_" + gameObject.name);
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    if (Application.isPlaying)
    //    {
    //        if (HasItem)
    //        {
    //            if (CurBlock == null)
    //            {
    //                Gizmos.color = Color.red;
    //                Gizmos.DrawSphere(transform.position, .3f);
    //            }
    //            else
    //            {
    //                Gizmos.color = Color.blue;
    //                Gizmos.DrawSphere(transform.position, .3f);
    //            }
    //        }
    //    }
    //}
#endif
}