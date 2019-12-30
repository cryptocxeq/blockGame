using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    JSONNode data;
    private TextAsset datas;
    public ParticleSystem LeafEffect;
    public bool IsTestMode = true,IsContinue;
    public static GameController Instance;

    Sprite sprite;

    [Header("Prefabs")] [SerializeField] private GameObject _bgItemIngamePrefab;

    [SerializeField] [Header("Container")] private Transform _bgItemInGameContainer;
    private Vector3 _bgItemInGameContainerDefaultPosition;

    public int BoardSize = 10;

    public GameObject BlockPrefab;
    public GameObject ItemBlockPrefab;
    public List<GameObject> BlocksPrefabs;
    public List<GameObject> HardBlockPrefabs;
    public List<Sprite> BlockItemSprites;
    public GameObject RaycasCheckFullPrefab;
    public List<RaycasCheckFull> RaycasCheckFulls;

    public List<Transform> _listSpawnPos;

    [SerializeField] private LayerMask _setPositionLayer;

    private Vector3 _mousePosition;

    private Block _curBlock;
    public BlockItem getBlockItem;
    public float CountTime;
    public List<Sprite> Decos;

    public List<BgItemIngame> BoardItems = new List<BgItemIngame>();
    public Transform[][] BoardItemMaxtrix = new Transform[8][];

    private Vector3 _moveOffset = new Vector3(0, 2);
    private bool IsLock;
    [SerializeField] private GameObject _breakEffect;
    [SerializeField] private GameObject _scoreEffect;
    [SerializeField] private GameObject _bonusEffect;
    [SerializeField] private GameObject _tapTheBlockToRotate;
    [SerializeField] private GameObject _theHiddenBlockCanBeRotated;
    [SerializeField] private Text _txtNumberRotate;
    [SerializeField] private Animator _getMoreRotateAnim;
    [SerializeField] private GameObject _btnGetMoveRotate;
    [SerializeField] private GameObject _starFlyEffect;
    [SerializeField] private Transform _rotateButtonTransform;
    public Vector3 BigScale = new Vector3(.84f, .84f);
    public Vector3 SmallScale = new Vector3(.3f, .3f);

    public bool IsPlaying;
    private Vector3 _mouseDownPosition;

    public static Action OnScoreChanged;
    public static Action OnBestScoreChanged;
    private static int _score;


    public static int Score
    {
        get { return _score; }
        set
        {
            _score = value;

            if (OnScoreChanged != null)
                OnScoreChanged();
        }
    }

    public static int BestScore
    {
        get { return PlayerPrefs.GetInt("best_score"); }
        set
        {
            PlayerPrefs.SetInt("best_score", value);
            if (OnBestScoreChanged != null)
                OnBestScoreChanged();
        }
    }

    public int CountBlock
    {
        get { return PlayerPrefs.GetInt("count_block", 0); }
        set
        {
            PlayerPrefs.SetInt("count_block", value);
        }
    }

    public int NumRotate
    {
        get { return PlayerPrefs.GetInt("num_rotate", 10); }
        set
        {
            PlayerPrefs.SetInt("num_rotate", value);
            _txtNumberRotate.text = NumRotate.ToString();
        }
    }


    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 300;
        _bgItemInGameContainerDefaultPosition = _bgItemInGameContainer.position;
    }

    void Start()
    {
        //PlayerPrefs.SetInt("firstLogin", 0);
        IsContinue=false;
        _score = PlayerPrefs.GetInt("Score");

        _txtNumberRotate.text = NumRotate.ToString();

        InitBgBoard();
        _bgItemInGameContainer.gameObject.SetActive(false);

        //StartGame();
        InitBlocksPrefab();
    }

    void InitBlocksPrefab()
    {
        foreach (var blocksPrefab in BlocksPrefabs)
            blocksPrefab.GetComponent<Block>().InitItemPrefab();
    }

    void Update()
    {
        if (!IsPlaying)
            return;

        CountTime += Time.deltaTime;

        if (IsLock)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_tapTheBlockToRotate.activeSelf)
                HideTabTheBlockToRotate();

            if (_theHiddenBlockCanBeRotated.activeSelf)
                _theHiddenBlockCanBeRotated.SetActive(false);

            _mouseDownPosition = GetMousePosition();
        }
        else if (Input.GetMouseButton(0))
        {
            if (_curBlock == null)
            {
                //Debug.Log("_mouseDownPosition" + _mouseDownPosition);
                float leng = Vector2.Distance(_mouseDownPosition, GetMousePosition());

                if (leng >= .05f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector3.zero, 10, _setPositionLayer);

                    if (hit.collider != null)
                    {
                        if (hit.collider.transform.childCount > 0)
                        {
                            // get block
                            _curBlock = hit.collider.transform.GetChild(0).GetComponent<Block>();
                            if (_curBlock.CanUse)
                            {

                                _curBlock.ActiveBlock();
                                SoundController.Instance.PlaySoundEffect(SoundController.Instance.PickItem);
                            }
                            else
                                _curBlock = null;
                        }
                    }
                }
            }

            if (_curBlock != null)
            {
                _mousePosition = GetMousePosition();
                _curBlock.transform.position = _mousePosition + _moveOffset;

                _curBlock.UpdateBlock();

                // Display can clear
                DisplayCanClear();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_curBlock != null)
            {
                if (_curBlock.IsPlacable && _curBlock.transform.localScale.x >= BigScale.x)
                {
                    _curBlock.ApplyBlock();
                    CountBlock++;

                    //Debug.Log("CountBlock " + CountBlock);
                    //ShakeCam2();
                    IsLock = true;
                    DOVirtual.DelayedCall(.3f, () =>
                    {
                        // Check full
                        CheckFull();

                        if (CheckEmptySet())
                        {
                            SaveGame.instance.deleteList();
                            // New new three blocks
                            CreateThreeBlock();
                        }
                        else
                        {
                            // Check gameover
                            if (CheckGamever())
                            {
                                SaveGame.instance.deleteList();
                                // Show gameover
                                Debug.Log("GAMEOVER");
                                _theHiddenBlockCanBeRotated.SetActive(false);
                                ShowGameover();
                            }
                        }

                        IsLock = false;
                    });
                }
                else
                {
                    _curBlock.MoveToContainer();
                }

                _curBlock = null;
            }
            else
            {

                float leng = Vector2.Distance(_mouseDownPosition, GetMousePosition());
                if (leng < .5f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector3.zero, 10, _setPositionLayer);
                    if (hit.collider != null)
                    {
                        if (hit.collider.transform.childCount > 0)
                        {
                            // get block
                            var block = hit.collider.transform.GetChild(0).GetComponent<Block>();
                            if (!block.IsRotate)
                            {
                                if (NumRotate > 0 && block.Items.Count > 1)
                                {
                                    block.IsRotate = true;
                                    block.RotateIcon.SetActive(true);
                                    NumRotate--;
                                    block.RotateBlock();

                                    // Check numrotate
                                    if (NumRotate <= 0)
                                    {
                                        _btnGetMoveRotate.SetActive(true);
                                        _getMoreRotateAnim.Play("Start");
                                    }
                                }
                                else
                                {
                                    _getMoreRotateAnim.Play("Show", 0, 0);
                                }
                            }
                            else
                            {
                                block.RotateBlock();
                            }
                        }
                    }
                }
            }
        }
    }

    private void ShowGameover()
    {
        IsPlaying = false;
        // Gray effect
        foreach (var item in BoardItems)
        {
            if (item.CurBlock != null)
            {
                item.CurBlock.ShowGameoverEffect();
            }
        }

        SoundController.Instance.PlaySoundEffect(SoundController.Instance.Gameover);
        DOVirtual.DelayedCall(2f, () => { UiManager.Instance.Popup.ShowGameoverPopup(); });
    }

    List<RaycasCheckFull> lst = new List<RaycasCheckFull>();

    public void ResetListClear()
    {
        foreach (var full in lst)
        {
            foreach (var item in full.Items)
            {
                item.CurBlock.ResetToDefaultSprite();
            }
        }
    }

    void DisplayCanClear()
    {
        lst.Clear();
        foreach (var item in RaycasCheckFulls)
        {
            if (item.IsFull2())
                lst.Add(item);
        }

        foreach (var full in lst)
        {
            foreach (var item in full.Items)
            {
                if (item.CurBlock != null)
                {
                    item.CurBlock.SetClearSprite(_curBlock.Items[0].GetComponent<SpriteRenderer>().sprite);
                }
            }
        }
    }

    private void CheckFull()
    {
        List<RaycasCheckFull> lst = new List<RaycasCheckFull>();
        foreach (var item in RaycasCheckFulls)
        {
            if (item.IsFull())
            {
                lst.Add(item);
            }
        }

        if (lst.Count > 0)
        {
            int index = lst.Count;
            if (index > 4)
                index = 4;

            int count = 0;
            Vector3 pos = Vector3.zero;
            foreach (var full in lst)
            {
                foreach (var item in full.Items)
                {
                    count++;
                    pos += item.transform.position;
                }
            }

            pos /= count;
            // Spawn effect tai vi tri 0,0
            SpawnBonusEffect(Vector3.up * 1.5f, index);
            ShakeCam();

            foreach (var full in lst)
            {
                full.ClearGround(pos);
            }

            SoundController.Instance.PlayEatSound(index);
            BonusScore(index, pos);

            LeafEffect.Emit(30);
        }
    }

    private void BonusScore(int index, Vector3 pos)
    {
        // Bonus score
        int score = 0;
        if (index == 2)
        {
            score = 50;
        }
        else if (index == 3)
        {
            score = 100;
        }
        else if (index == 4)
        {
            score = 150;
        }
        else if (index == 5)
        {
            score = 200;
        }

        if (score != 0)
        {
            Score += score;
            SaveGame.instance.saveScore(Score);
            SpawnScoreEffect(pos + Vector3.up * .75f, "+" + score);
        }
    }

    private Vector3 _tempMousePosition;

    private Vector3 GetMousePosition()
    {
        if (Input.touchCount > 0)
        {
            _tempMousePosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            // Debug.Log("_tempMousePosition1" + _tempMousePosition);
        }
        else
        {

            _tempMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("_tempMousePosition2" + _tempMousePosition);
        }
        _tempMousePosition.z = -1;
        return _tempMousePosition;
    }

    /// <summary>
    /// Start new game
    /// </summary>
    public void StartGame()
    {

        CountTime = 0;
        HardPoint = 0;
        CountBlock = 0;


        _theHiddenBlockCanBeRotated.SetActive(false);
        _bgItemInGameContainer.position = _bgItemInGameContainerDefaultPosition;
        API.Instance.ShowBanner();

        SoundController.Instance.PlaySoundEffect(SoundController.Instance.StartLevel);

        if (NumRotate <= 0)
        {
            _btnGetMoveRotate.SetActive(true);
            _getMoreRotateAnim.Play("Start", 0, 0);
        }
        else
        {
            _btnGetMoveRotate.SetActive(false);
        }

        ShowTabTheBlockToRotate();
        IsPlaying = true;
        Score = 0;
        IsLock = false;
        _bgItemInGameContainer.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("firstLogin", 0) == 0)
        {
            StartCreate();
        }
        else
        {
            CreateThreeBlock();
        }

        RunBgItemEffect();
    }

    private void RunBgItemEffect()
    {
        Anim1();
    }

    private void Anim4()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                var temp = BoardItemMaxtrix[i][j];
                temp.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                temp.GetComponent<SpriteRenderer>().DOFade(1, .3f).SetDelay((i + j) / 20f);
            }
        }
    }

    private void Anim3()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                var temp = BoardItemMaxtrix[i][j];
                temp.DOMove(temp.position + new Vector3(-8, 8), .3f).SetDelay((i + j) / 20f).From();
            }
        }
    }

    private void Anim1()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                var temp = BoardItemMaxtrix[i][j];
                temp.localScale = Vector3.zero;
                temp.DOScale(1, .3f).SetDelay((i + j) / 20f);
            }
        }
    }

    private void Anim2()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                var temp = BoardItemMaxtrix[i][j];
                temp.localScale = Vector3.zero;
                temp.DOScale(1, .3f).SetDelay(Random.Range(0, .5f));
            }
        }
    }

    /// <summary>
    /// Stop game
    /// Clear all pools
    /// </summary>
    public void StopGame()
    {
        BlockPool.Pool.DespawnAll();
        _bgItemInGameContainer.gameObject.SetActive(false);
    }

    public bool CheckEmptySet()
    {
        foreach (var item in _listSpawnPos)
        {
            if (item.childCount > 0)
                return false;
        }

        return true;
    }

    public bool CheckGamever()
    {
        //        bool check = true;
        //        foreach (var item in _listSpawnPos)
        //        {
        //            if (item.childCount > 0)
        //            {
        //                var block = item.GetChild(0).GetComponent<Block>();
        //                if (CheckAbleBlock(block))
        //                {
        //                    check = false;
        //                    block.SetCanUseState();
        //                }
        //                else
        //                {
        //                    block.SetCannotUseState();
        //
        //                    _theHiddenBlockCanBeRotated.SetActive(true);
        //                }
        //            }
        //        }
        //
        //        return check;

        bool check = true;
        foreach (var item in _listSpawnPos)
        {
            if (item.childCount > 0)
            {
                var block = item.GetChild(0).GetComponent<Block>();
                if (CheckAbleBlock(block, 2) == 1)
                {
                    check = false;
                    block.SetCanUseState();
                }
                else if (CheckAbleBlock(block, 2) == 2)
                {
                    check = false;
                    // Show warning rotate
                    _theHiddenBlockCanBeRotated.SetActive(true);
                    block.SetCannotUseState();
                }
                else
                {
                    block.SetCannotUseState();
                    _theHiddenBlockCanBeRotated.SetActive(true);
                }
            }
        }

        return check;
    }

    public bool CheckAbleBlock(Block block)
    {
        foreach (var ground in BoardItems)
        {
            if (block.CheckAbleUse(ground.transform.position))
                return true;
        }

        return false;
    }

    private Vector3 _curentAngleOfBlock;
    private Vector3 _tempAngle = new Vector3();

    public List<Vector3> ListAngle;

    /// <summary>
    /// 0: là sai, 1: là đúng, 2: là đúng nhưng phải xoay block 
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public int CheckAbleBlock(Block block, int checkDeep)
    {
        ListAngle.Clear();
        int check = 0;
        foreach (var ground in BoardItems)
        {
            if (block.CheckAbleUse(ground.transform.position))
            {
                if (checkDeep == 2)
                {
                    return 1;
                }

                check = 1;
                ListAngle.Add(Vector3.zero);
                break;
            }
        }

        if (checkDeep > 0)
        {
            foreach (var ground in BoardItems)
            {
                _curentAngleOfBlock = block.BlocksContain.eulerAngles;
                for (int i = 0; i < 360; i += 90)
                {
                    _tempAngle.z = i;
                    block.BlocksContain.eulerAngles = _tempAngle;
                    if (block.CheckAbleUse(ground.transform.position))
                    {
                        ListAngle.Add(_tempAngle);
                        check = 1;
                        if (checkDeep == 2)
                        {
                            block.BlocksContain.eulerAngles = _curentAngleOfBlock;
                            return 2;
                        }
                    }
                }

                block.BlocksContain.eulerAngles = _curentAngleOfBlock;
            }
        }

        return check;

        //        foreach (var ground in BoardItems)
        //        {
        //            if (block.CheckAbleUse(ground.transform.position))
        //                return 1;
        //        }
        //
        //        if (checkDeep)
        //        {
        //            foreach (var ground in BoardItems)
        //            {
        //                _curentAngleOfBlock = block.BlocksContain.eulerAngles;
        //                for (int i = 0; i < 360; i += 60)
        //                {
        //                    _tempAngle.z = i;
        //                    block.BlocksContain.eulerAngles = _tempAngle;
        //                    if (block.CheckAbleUse(ground.transform.position))
        //                    {
        //                        block.BlocksContain.eulerAngles = _curentAngleOfBlock;
        //                        return 2;
        //                    }
        //                }
        //
        //                block.BlocksContain.eulerAngles = _curentAngleOfBlock;
        //            }
        //        }
        //
        //        return 0;
    }

    [ContextMenu("UpdateCenterOfAllBlock")]
    public void UpdateCenterOfAllBlock()
    {
        foreach (var item in BlocksPrefabs)
        {
            Vector3 center = new Vector3();

            foreach (Transform child in item.transform)
            {
                child.localPosition = child.localPosition;
                center += child.localPosition;
            }

            center /= item.transform.childCount;

            // remove offset
            foreach (Transform child in item.transform)
            {
                child.localPosition -= center;
            }
        }
    }

    public int HardPoint = 0;
    public List<GameObject> ListCreated;

    private bool CheckCanAddToList(GameObject obj)
    {
        foreach (var item in ListCreated)
        {
            if (item == obj && item.GetComponent<Block>().IsHard)
            {
                return false;
            }
        }

        int count = 0;
        foreach (var item in ListCreated)
        {
            if (item.GetComponent<Block>().IsHard)
            {
                count++;
            }
        }

        return count < 3;
    }

    void StartCreate()
    {

        int random, index;
        Transform randomObjTransform;
        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
            {
                Transform blockTransform = BlockPool.Pool.Spawn(BlockPrefab);
                blockTransform.localScale = Vector3.one;

                random = Random.Range(0, BlockItemSprites.Count);
                sprite = BlockItemSprites[random];
                randomObjTransform = BlocksPrefabs[7].transform;
                ListCreated.Add(randomObjTransform.gameObject);

                var block = blockTransform.GetComponent<Block>();
                block.BlocksContain.eulerAngles = Vector3.zero;
                foreach (Transform child in randomObjTransform.GetComponent<Block>().BlocksContain)
                {
                    Transform itemTransform = BlockPool.Pool.Spawn(ItemBlockPrefab, block.BlocksContain);
                    itemTransform.localPosition = child.localPosition;
                    itemTransform.localScale = Vector3.one * 1.38f;
                    itemTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Decos[Random.Range(0, Decos.Count)];
                    itemTransform.GetComponent<BlockItem>().SetNoStar();
                }

                blockTransform.localScale = SmallScale;

                blockTransform.SetParent(_listSpawnPos[i]);

                block.Container = _listSpawnPos[i];
                //            foreach (var item in block.Items)
                //            {
                //                item.transform.localPosition *= 1.05f;
                //            }

                block.InitItem();
                block.SetSprite(sprite);


                if (ListAngle.Count > 0)
                {
                    block.BlocksContain.eulerAngles = ListAngle[Random.Range(0, ListAngle.Count)];
                    while (CheckAbleBlock(block, 2) == 0)
                    {
                        block.BlocksContain.eulerAngles += Vector3.forward * 60;
                    }

                    block.DefaultAngleZ = (int)block.BlocksContain.eulerAngles.z;
                    foreach (var item in block.Items)
                    {
                        item.transform.eulerAngles = Vector3.zero;
                    }
                }

                blockTransform.DOKill();
                blockTransform.position = _listSpawnPos[i].position + Vector3.down * 4;
                //Debug.Log("blockTransform.position1" + blockTransform.position);
                blockTransform.DOMove(_listSpawnPos[i].position, .3f).SetEase(Ease.OutBack);
                //Debug.Log("blockTransform.position2" + blockTransform.position);
                if (Random.Range(0, 100) < 50)
                {
                    // set block has star
                    int blockItemIndex = Random.Range(0, block.Items.Count);
                    block.Items[blockItemIndex].SetHasStar();
                }
            }
        }

        CheckGamever();
        SaveGame.instance.firstLogin();
    }

    void CreateThreeBlock()
    {
        //ListCreated.Clear();

        HardPoint = 0;



        Debug.Log(CountBlock);
        if (CountBlock > 250)
        {
            HardPoint = 90;
        }
        else if (CountBlock > 200)
        {
            HardPoint = 80;
        }
        else if (CountBlock > 150)
        {
            HardPoint = 50;
        }
        else if (CountBlock > 100)
        {
            HardPoint = 30;
        }
        else if (CountBlock > 80)
        {
            HardPoint = 30;
        }
        else if (CountBlock > 60)
        {
            HardPoint = 20;
        }
        else if (CountBlock > 50)
        {
            HardPoint = 10;
        }
        else if (CountBlock > 30)
        {
            HardPoint = 5;
        }
        Debug.Log(HardPoint);

        int random, index;
        Transform randomObjTransform;
        for (int i = 0; i < 3; i++)
        {
            // Random sprite

            Transform blockTransform = BlockPool.Pool.Spawn(BlockPrefab);
            blockTransform.localScale = Vector3.one;

            // Random block
            // Check block

            if (PlayerPrefs.GetString("block") == "true" && PlayerPrefs.GetString("all") == "true" && IsContinue==false)
            {
                IsContinue = true;
                Debug.Log(PlayerPrefs.HasKey("indexSet_" + i));

                Score = PlayerPrefs.GetInt("Score");

                index = PlayerPrefs.GetInt("indexSet_" + i);

                sprite = BlockItemSprites[index];
                if (i == 2)
                {
                    if (PlayerPrefs.HasKey("type_" + 1 + "indexSet_" + i))
                    {
                        index = PlayerPrefs.GetInt("type_" + 1 + "indexSet_" + i);
                    }
                    else
                    {
                        index = PlayerPrefs.GetInt("type_" + 0 + "indexSet_" + i);
                    }
                }
                else
                {

                    index = PlayerPrefs.GetInt("type_" + 0 + "indexSet_" + i);
                }
                randomObjTransform = BlocksPrefabs[i].transform;
                for (int x = 0; x < BlocksPrefabs.Count; x++)
                {
                    if (index == int.Parse(BlocksPrefabs[x].name.Substring(5)))
                    {
                        randomObjTransform = BlocksPrefabs[x].transform;
                    }
                }


            }
            else
            {
                random = Random.Range(0, BlockItemSprites.Count);
                sprite = BlockItemSprites[random];

                SaveGame.instance.saveSprite(i, random);

                random = Random.RandomRange(0, BlocksPrefabs.Count);
                randomObjTransform = BlocksPrefabs[random].transform;
                SaveGame.instance.saveListCreated(0, i, int.Parse(BlocksPrefabs[random].name.Substring(5)));
                if (i == 2)
                {
                    if (Random.Range(0, 101) < HardPoint)
                    {
                        // Random block
                        RandomList1(ref HardBlockPrefabs);
                        for (int j = 0; j < HardBlockPrefabs.Count; j++)
                        {
                            randomObjTransform = HardBlockPrefabs[j].transform;
                            if (CheckAbleBlock(randomObjTransform.GetComponent<Block>(), 1) != 0
                                && CheckCanAddToList(randomObjTransform.gameObject))
                            {
                                int indexBlock = int.Parse(randomObjTransform.gameObject.name.Substring(5));

                                SaveGame.instance.saveListCreated(1, i, indexBlock);
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Random block
                        RandomList1(ref BlocksPrefabs);
                        for (int j = 0; j < BlocksPrefabs.Count; j++)
                        {
                            randomObjTransform = BlocksPrefabs[j].transform;
                            if (CheckAbleBlock(randomObjTransform.GetComponent<Block>(), 1) != 0
                                && CheckCanAddToList(randomObjTransform.gameObject))
                            {
                                int indexBlock = int.Parse(randomObjTransform.gameObject.name.Substring(5));

                                SaveGame.instance.saveListCreated(0, i, indexBlock);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Random block
                    RandomList1(ref BlocksPrefabs);
                    for (int j = 0; j < BlocksPrefabs.Count; j++)
                    {
                        randomObjTransform = BlocksPrefabs[j].transform;
                        if (CheckAbleBlock(randomObjTransform.GetComponent<Block>(), 1) != 0
                            && CheckCanAddToList(randomObjTransform.gameObject))
                        {
                            int indexBlock = int.Parse(randomObjTransform.gameObject.name.Substring(5));

                            SaveGame.instance.saveListCreated(0, i, indexBlock);
                            break;
                        }
                    }
                }
            }


            ListCreated.Add(randomObjTransform.gameObject);

            var block = blockTransform.GetComponent<Block>();
            block.BlocksContain.eulerAngles = Vector3.zero;
            foreach (Transform child in randomObjTransform.GetComponent<Block>().BlocksContain)
            {
                Transform itemTransform = BlockPool.Pool.Spawn(ItemBlockPrefab, block.BlocksContain);
                itemTransform.localPosition = child.localPosition;
                itemTransform.localScale = Vector3.one * 1.38f;
                itemTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Decos[Random.Range(0, Decos.Count)];
                itemTransform.GetComponent<BlockItem>().SetNoStar();
            }

            blockTransform.localScale = SmallScale;

            blockTransform.SetParent(_listSpawnPos[i]);

            block.Container = _listSpawnPos[i];
            //            foreach (var item in block.Items)
            //            {
            //                item.transform.localPosition *= 1.05f;
            //            }

            block.InitItem();
            block.SetSprite(sprite);


            if (ListAngle.Count > 0)
            {
                block.BlocksContain.eulerAngles = ListAngle[Random.Range(0, ListAngle.Count)];
                while (CheckAbleBlock(block, 2) == 0)
                {
                    block.BlocksContain.eulerAngles += Vector3.forward * 60;
                }

                block.DefaultAngleZ = (int)block.BlocksContain.eulerAngles.z;
                foreach (var item in block.Items)
                {
                    item.transform.eulerAngles = Vector3.zero;
                }
            }

            blockTransform.DOKill();
            blockTransform.position = _listSpawnPos[i].position + Vector3.down * 4;
            //Debug.Log("blockTransform.position1" + blockTransform.position);
            blockTransform.DOMove(_listSpawnPos[i].position, .3f).SetEase(Ease.OutBack);
            //Debug.Log("blockTransform.position2" + blockTransform.position);
            if (Random.Range(0, 100) < 50)
            {
                // set block has star
                int blockItemIndex = Random.Range(0, block.Items.Count);
                block.Items[blockItemIndex].SetHasStar();
            }


            //if (i == 2 || i == 1)
            //{
            //    if (_listSpawnPos[i].childCount >= 2)
            //    {
            //        Destroy(_listSpawnPos[i].GetChild(0).gameObject);
            //    }
            //}
        }

        CheckGamever();
        if (CheckGamever())
        {
            SaveGame.instance.deleteList();
            // Show gameover
            Debug.Log("GAMEOVER");
            _theHiddenBlockCanBeRotated.SetActive(false);
            ShowGameover();
        }
    }

    public void RandomList1(ref List<GameObject> lst)
    {
        GameObject temp;
        int random;
        for (int i = 0; i < lst.Count; i++)
        {
            temp = lst[i];
            random = Random.RandomRange(0, lst.Count);
            lst[i] = lst[random];
            lst[random] = temp;
        }
    }

    private void InitBgBoard()
    {
        BoardItems.Clear();
        _bgItemIngamePrefab.gameObject.SetActive(true);
        float itemSize = 84f / 100f;

        _bgItemIngamePrefab.GetComponent<BoxCollider2D>().size = new Vector2(.84f, .84f);

        Transform boardItemTrans;
        Vector2 tempPosition = new Vector2();
        Vector2 offset = new Vector2(itemSize * BoardSize, -itemSize * BoardSize) / 2 -
                         new Vector2(itemSize / 2f, -itemSize / 2f);

        datas = Resources.Load<TextAsset>("FirstLogin");

        string[] tdata = datas.text.Split('\n');

        for (int i = 0; i < BoardSize; i++)
        {
            BoardItemMaxtrix[i] = new Transform[BoardSize];
            for (int j = 0; j < BoardSize; j++)
            {
                boardItemTrans = BoardItemPool.Pool.Spawn(_bgItemIngamePrefab, _bgItemInGameContainer);
                boardItemTrans.name = i.ToString() + "_" + j.ToString();


                tempPosition.x = j * itemSize;
                tempPosition.y = -i * itemSize;
                //Debug.Log("tempPosition " + tempPosition);
                boardItemTrans.localPosition = tempPosition - offset;
                //Debug.Log(boardItemTrans.position + "boardItemTrans.position");
                if (i == 0)
                {
                    CreateRaycastFull(boardItemTrans.position, Vector3.down);
                }

                if (j == 0)
                {
                    CreateRaycastFull(boardItemTrans.position, Vector3.right);
                }

                BoardItemMaxtrix[i][j] = boardItemTrans;
                BoardItems.Add(boardItemTrans.GetComponent<BgItemIngame>());
                boardItemTrans.transform.localScale = Vector3.one;
                if (PlayerPrefs.GetInt("firstLogin", 0) == 0)
                {
                    data = JSONArray.Parse(tdata[i]);
                    JSONNode farr = data["a"];
                    for (int x = 0; x < farr.Count; x++)
                    {
                        if (j == farr[x])
                        {
                            GameObject BlockItem = Instantiate(ItemBlockPrefab, boardItemTrans);

                            BlockItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Decos[Random.Range(0, Decos.Count)];
                            BlockItem.GetComponent<BlockItem>().SetDefaulSprite(BlockItemSprites[0]);
                            //BlockItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = BlockItemSprites[0];
                            //Transform itemTransform = BlockPool.Pool.Spawn(ItemBlockPrefab, block.BlocksContain);

                            boardItemTrans.GetComponent<BgItemIngame>().ApplyNewBlock(BlockItem.GetComponent < BlockItem >());

                        }
                    }
                }
                else
                {
                    if (PlayerPrefs.GetString("all") == "true")
                    {
                        if (PlayerPrefs.GetInt("background_" + boardItemTrans.name, 0) != 0)
                        {
                            GameObject BlockItem = Instantiate(ItemBlockPrefab, boardItemTrans);

                            BlockItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Decos[Random.Range(0, Decos.Count)];
                            boardItemTrans.GetComponent<BgItemIngame>().ApplyOldBlock(BlockItem.GetComponent<BlockItem>(), PlayerPrefs.GetInt("background_" + boardItemTrans.name), PlayerPrefs.GetInt("star_" + boardItemTrans.name));
                            //PlayerPrefs.SetInt("background_" + boardItemTrans.name, 0);
                        }
                    }
                }
            }
        }

        _bgItemIngamePrefab.gameObject.SetActive(false);

        foreach (var raycasCheckFull in RaycasCheckFulls)
        {
            raycasCheckFull.Init();
        }
    }

    public void CreateRaycastFull(Vector3 position, Vector3 direct)
    {
        Transform container = transform.Find("RaycastContainer");
        // Debug.Log("position1 " + position);
        // Debug.Log("direct " + direct);
        position -= direct * 5;
        //Debug.Log("position2 " + position);
        RaycasCheckFull raycasCheckFull =
            Instantiate(RaycasCheckFullPrefab, container).GetComponent<RaycasCheckFull>();
        raycasCheckFull.transform.position = position;

        //Debug.Log("Pos: " + position);
        raycasCheckFull.Direct = direct;

        RaycasCheckFulls.Add(raycasCheckFull);
    }

    public void SpawnBreakEffect(Vector3 pos, Color color)
    {
        Transform effect = EffectPool.Pool.Spawn(_breakEffect);
        effect.position = pos;
        //        effect.GetComponent<ParticleSystem>().startColor = color;
        var listEffects = new List<ParticleSystem>();
        GetListEffect(listEffects, effect);
        foreach (var ef in listEffects)
        {
            ef.startColor = color;
        }
    }

    private void GetListEffect(List<ParticleSystem> lst, Transform t)
    {
        ParticleSystem p = t.GetComponent<ParticleSystem>();
        if (p != null)
            lst.Add(p);
        foreach (Transform child in t)
        {
            GetListEffect(lst, child);
        }
    }

    public void SpawnScoreEffect(Vector3 pos, int score)
    {
        Transform effect = EffectPool.Pool.Spawn(_scoreEffect);
        effect.GetComponent<ScoreEffect>().Init(pos, score);
    }

    public void SpawnScoreEffect(Vector3 pos, string str)
    {
        Transform effect = EffectPool.Pool.Spawn(_scoreEffect);
        effect.GetComponent<ScoreEffect>().Init(pos, str);
    }

    public void SpawnBonusEffect(Vector3 pos, int index)
    {
        DOVirtual.DelayedCall(.4f, () =>
        {
            Transform effect = EffectPool.Pool.Spawn(_bonusEffect);
            effect.position = pos;
            effect.GetComponent<BonusEffect>().ShowBonus(index);

            SoundController.Instance.PlaySoundEffect(SoundController.Instance.Bonus);
        });
    }

    [Header("Shake Camera value")] public float ShakeTime = .1f;
    public float ShakeLeng = 1;
    public int ShakeVibrato = 10;
    public float ShakeRandomness = 90;

    [ContextMenu("Shake Camera")]
    public void ShakeCam()
    {
        _bgItemInGameContainer.DOKill();
        _bgItemInGameContainer.position = _bgItemInGameContainerDefaultPosition;
        _bgItemInGameContainer.DOShakePosition(ShakeTime, ShakeLeng, ShakeVibrato, ShakeRandomness, false, true);
    }

    [ContextMenu("Shake Camera")]
    public void ShakeCam2()
    {
        _bgItemInGameContainer.DOKill();
        _bgItemInGameContainer.position = _bgItemInGameContainerDefaultPosition;
        _bgItemInGameContainer.DOShakePosition(ShakeTime / 2f, ShakeLeng / 10f, ShakeVibrato, ShakeRandomness, false,
            true);
    }

    private void ShowTabTheBlockToRotate()
    {
        _tapTheBlockToRotate.gameObject.SetActive(true);
    }

    private void HideTabTheBlockToRotate()
    {
        _tapTheBlockToRotate.gameObject.SetActive(false);
    }

    public void AddMoreRotate(int number)
    {
        NumRotate += number;
        _btnGetMoveRotate.SetActive(false);
    }

    public void CreateStarEffect(Vector3 position)
    {
        var effect = EffectPool.Pool.Spawn(_starFlyEffect);
        effect.position = position;
        effect.GetComponent<StarFlyEffect>().MoveToTargetPosition(_rotateButtonTransform.position);
    }
}