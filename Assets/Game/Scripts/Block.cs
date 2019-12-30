using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int DefaultAngleZ;

    public SpriteRenderer ChildRender;
    public Transform BlocksContain;

    public bool IsRotate;
    public Transform Container;
    [SerializeField] private LayerMask _layerCheck;
    private RaycastHit2D _hit;

    public List<BlockItem> Items = new List<BlockItem>();

    public bool IsPlacable;

    public bool CanUse;


    //public Transform ItemsContainer;
    public GameObject RotateIcon;
    public bool IsHard;

    private void Start()
    {
        //ItemsContainer = transform.Find("Items");
    }

    [ContextMenu("Editor Init")]
    private void EditorInit()
    {
        BlocksContain = transform.Find("Items");
    }

    private void OnEnable()
    {
        IsPlacable = false;
        IsRotate = false;
        RotateIcon.gameObject.SetActive(false);
    }

    public void SetSprite(Sprite sprite)
    {
        foreach (var item in Items)
        {
            item.SetDefaulSprite(sprite);
        }
    }

    public void SetClearSprite(Sprite sprite)
    {
        foreach (var item in Items)
        {
            item.SetClearSprite(sprite);
        }
    }

    public void ResetToDefaultSprite()
    {
        foreach (var item in Items)
        {
            item.ResetToDefaultSprite();
        }
    }

    public void InitItem()
    {
        Items.Clear();
        foreach (Transform child in BlocksContain)
        {
            if (child.gameObject.activeSelf)
                Items.Add(child.gameObject.GetComponent<BlockItem>());
        }
    }

    public void InitItemPrefab()
    {
        Items.Clear();
        foreach (Transform child in BlocksContain)
        {
            if (child.gameObject.activeSelf)
                Items.Add(child.gameObject.GetComponent<BlockItem>());
        }
    }

    private Collider2D _curBgIngame;

    public void UpdateBlock()
    {
        if (Application.isPlaying && transform.localScale.x >= GameController.Instance.BigScale.x)
        {
            int count = 0;
            int i = 0;
            foreach (BlockItem item in Items)
            {
                _hit = Physics2D.Raycast(item.transform.position, Vector3.back, 10, _layerCheck);

                if (i == 0)
                {
                    if (_curBgIngame != _hit.collider)
                    {
                        GameController.Instance.ResetListClear();
                    }

                    _curBgIngame = _hit.collider;
                    i++;
                }

                if (_hit.collider != null && !_hit.collider.GetComponent<BgItemIngame>().HasItem)
                {
                    count++;
                }
            }

            if (count == Items.Count)
            {
                foreach (BlockItem item in Items)
                {
                    _hit = Physics2D.Raycast(item.transform.position, Vector3.back, 10, _layerCheck);
                    if (_hit.collider != null)
                    {
                        item.AddToGround(_hit.collider.GetComponent<BgItemIngame>());
                    }
                }

                IsPlacable = true;
            }
            else
            {
                GameController.Instance.ResetListClear();

                foreach (BlockItem item in Items)
                {
                    item.RemoveFromGround();
                }

                IsPlacable = false;
            }
        }
    }

    public bool CheckAbleUse(Vector3 position)
    {
    
        // Dich về vị trí của item đầu tiên của block
        position -= (Items[0].transform.position - transform.position) / transform.localScale.x;
        position.z = -1;
       // Debug.Log("Items[0].transform.position " + Items[0].transform.position);
      //  Debug.Log("transform.position " + transform.position);
      //  Debug.Log("transform.localScale.x " + transform.localScale.x);
      //  Debug.Log("position " + position);
        foreach (BlockItem item in Items)
        {
             // Debug.Log("item " + item.ToString());
             if (item != null)
            {
                _hit = Physics2D.Raycast(position + (item.transform.position - transform.position) / transform.localScale.x * GameController.Instance.BigScale.x, Vector3.back, 10,_layerCheck);
            }
           
            if (_hit.collider == null || _hit.collider.GetComponent<BgItemIngame>().HasItem)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator CheckAbleUse2(Vector3 position)
    {
        // Dich về vị trí của item đầu tiên của block
        position -= Items[0].transform.localPosition;
        position.z = -1;

        var temm = GameObject.Find("Flag");
        List<GameObject> objs = new List<GameObject>();

        foreach (BlockItem item in Items)
        {
            _hit = Physics2D.Raycast(position + item.transform.localPosition, Vector3.back, 10, _layerCheck);
            if (_hit.collider == null || _hit.collider.GetComponent<BgItemIngame>().HasItem)
            {
                foreach (var o in objs)
                    Destroy(o);
                yield break;
            }

            GameObject obj = Instantiate(temm);
            obj.transform.position = position + item.transform.localPosition;
            objs.Add(obj);
        }

        Debug.Log("Ok");

        yield return new WaitForSeconds(.3f);
        foreach (var o in objs)
        {
            Destroy(o);
        }
    }

    public void ApplyBlock()
    {
        // Apply block
        foreach (var item in Items)
        {
            item.ApplyBlock();
        }

        BlockPool.Pool.Despawn(transform);
        transform.SetParent(BlockPool.Pool.transform);
        SoundController.Instance.PlaySoundEffect(SoundController.Instance.PutItem);
    }


    public void ActiveBlock()
    {
        if (IsRotate)
            RotateIcon.SetActive(false);

        transform.DOKill();
        transform.DOScale(GameController.Instance.BigScale.x, .3f);
    }

    public void MoveToContainer()
    {
        transform.DOKill();
        transform.DOScale(GameController.Instance.SmallScale, .3f);
        transform.DOMove(Container.transform.position, .3f);
        if (IsRotate)
            RotateIcon.SetActive(true);
    }

    [ContextMenu("Check")]
    public void EditorCheckCanUse()
    {
        StartCoroutine(CheckCanUseEditor());
    }

    IEnumerator CheckCanUseEditor()
    {
        foreach (var bgItemIngame in GameController.Instance.BoardItems)
        {
            yield return CheckAbleUse2(bgItemIngame.transform.position);
            yield return new WaitForSeconds(2);
        }
    }

    public void SetCanUseState()
    {
        CanUse = true;
        foreach (var blockItem in Items)
        {
            blockItem.Render.color = Color.white;
        }
    }

    private static Color CannotUseStateColor = new Color(1, 1, 1, .3f);

    public void SetCannotUseState()
    {
        CanUse = false;
        foreach (var blockItem in Items)
        {
            blockItem.Render.color = CannotUseStateColor;
        }
    }

    public void RotateBlock()
    {
        Vector3 angle = new Vector3();
        DOVirtual.Float(BlocksContain.eulerAngles.z, BlocksContain.eulerAngles.z + 90, .1f, value =>
        {
            angle.z = value;
            BlocksContain.eulerAngles = angle;
        }).OnComplete(() =>
        {
            foreach (var item in Items)
            {
                item.transform.eulerAngles = Vector3.zero;
            }

            // Check can use
            if (GameController.Instance.CheckAbleBlock(this))
            {
                SetCanUseState();
            }
            else
            {
                SetCannotUseState();
            }

            if ((int) BlocksContain.eulerAngles.z == 0)
            {
                // Revert rotate
                GameController.Instance.NumRotate++;
                IsRotate = false;
                RotateIcon.SetActive(false);
            }
        });
        IsRotate = true;
        SoundController.Instance.PlaySoundEffect(SoundController.Instance.PickItem);
    }


#if UNITY_EDITOR
    public GameObject ItemPrefab;
    public Transform _curTrans;

    public Transform IsHasObjectAtLocalPostion(Vector3 targetLocalPosition)
    {
        foreach (Transform child in BlocksContain)
        {
            if (Vector2.Distance(targetLocalPosition, child.localPosition) < .1f)
                return child;
        }

        return null;
    }

    public void Recenter()
    {
        Vector3 center = new Vector3();
        foreach (Transform child in transform)
        {
            center += child.localPosition;
        }

        center /= transform.childCount;

        foreach (Transform child in transform)
        {
            child.localPosition -= center;
        }
    }

    public void ResetBlock()
    {
        // Remove blocks
        while (transform.childCount > 1)
        {
            DestroyImmediate(transform.GetChild(1).gameObject);
        }

        _curTrans = transform.GetChild(0);
    }

    public void InitBlock(BlockInitDirect direct)
    {
        float width = 0.6675f;
        float height = .77f;

        ItemPrefab = transform.GetChild(0).gameObject;

        if (_curTrans == null)
            _curTrans = ItemPrefab.transform;

        Vector3 targetPos;
        switch (direct)
        {
            case BlockInitDirect.Up:
                targetPos = _curTrans.position + new Vector3(0, height);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;
            case BlockInitDirect.Down:
                targetPos = _curTrans.position - new Vector3(0, height);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;

            case BlockInitDirect.UpLeft:
                targetPos = _curTrans.position + new Vector3(-width, height / 2);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;
            case BlockInitDirect.DownLeft:
                targetPos = _curTrans.position + new Vector3(-width, -height / 2);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;
            case BlockInitDirect.UpRight:
                targetPos = _curTrans.position + new Vector3(width, height / 2);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;
            case BlockInitDirect.DownRight:
                targetPos = _curTrans.position + new Vector3(width, -height / 2);
                if (!CheckExists(targetPos))
                {
                    GameObject obj = Instantiate(ItemPrefab, transform);
                    obj.transform.position = targetPos;
                    _curTrans = obj.transform;
                }

                break;
        }
    }

    private bool CheckExists(Vector3 targetPos)
    {
        foreach (Transform child in transform)
        {
            if (Vector2.Distance(targetPos, child.position) <= .1f)
            {
                _curTrans = child;
                return true;
            }
        }

        return false;
    }

    //private void OnDrawGizmos()
    //{
    //    if (_curTrans != null)
    //    {
    //        Gizmos.DrawWireSphere(_curTrans.position, .3f);
    //    }
    //}
#endif
}

public enum BlockInitDirect
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
}