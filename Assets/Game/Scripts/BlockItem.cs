using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class BlockItem : MonoBehaviour
{
    private BgItemIngame _bg;
    public SpriteRenderer Render;
    public Sprite DefaultSprite;
    public _2dxFX_Shiny_Reflect Effect;
    public _2dxFX_GrayScale GrayScale;
    public bool HasStar;

    private GameObject _child;
    [SerializeField]
    private GameObject _star;

    private void Awake()
    {
        _child = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        Render.color = Color.white;
        _bg = null;
        transform.eulerAngles = Vector3.zero;
        Effect.enabled = true;
        StartCoroutine(DisableEffect());

        GrayScale.enabled = true;
        GrayScale._EffectAmount = 0;

        _child.SetActive(true);
    }

    private IEnumerator DisableEffect()
    {
        yield return new WaitForEndOfFrame();
        Effect.enabled = false;
        GrayScale.enabled = false;
    }

    public void SetDefaulSprite(Sprite sprite)
    {
        DefaultSprite = sprite;
        Render.sprite = sprite;
    }

    public void SetClearSprite(Sprite sprite)
    {
        Render.sprite = sprite;
    }

    public void ResetToDefaultSprite()
    {
        Render.sprite = DefaultSprite;
    }

    public void AddToGround(BgItemIngame ground)
    {
        if (_bg != null && _bg != ground)
        {
            _bg.RemoveBlockItem(this);
            _bg = null;
        }

        _bg = ground;
        _bg.AddBlockItem(this);
    }

    public void RemoveFromGround()
    {
        if (_bg != null)
        {
            if (!_bg.HasItem)
            {
                _bg.RemoveBlockItem(this);
            }

            _bg = null;
        }
    }

    public void ApplyOldBlock()
    {

    }

    public void ApplyBlock()
    {
        transform.SetParent(_bg.transform);
        Effect.enabled = true;
        Effect.ShinyLightCurveTime = 0;
        transform.DOMove(_bg.transform.position, .1f);
        DOVirtual.DelayedCall(.5f, () => { Effect.enabled = false; });
        _bg.ApplyBlock(this);
        _bg = null;
    }

    public void ClearBlockWhenClearRow(float delay)
    {
        // Fade
        Render.DOColor(Color.clear, .2f).OnComplete(() => { BlockPool.Pool.Despawn(transform); }).SetDelay(delay)
            .OnStart(
                () =>
                {
                    // Spawn effect
                    GameController.Instance.SpawnBreakEffect(transform.position,
                        Render.sprite.texture.GetPixel(300, 300));

                    // Add score and spawn score effect
                    GameController.Score += 10;
                    int score = GameController.Score;
                    SaveGame.instance.saveScore(score);
                });

        _child.SetActive(false);
    }

    private void OnDisable()
    {
        transform.DOKill();
        if (_gameoverEffect != null)
        {
            if (_gameoverEffect.IsPlaying())
            {
                _gameoverEffect.Kill();
            }
        }
    }

    private Tween _gameoverEffect;

    public void ShowGameoverEffect()
    {
        GrayScale.enabled = true;
        _gameoverEffect = DOVirtual.Float(0, 1, 1.5f, value => { GrayScale._EffectAmount = value; })
            .SetEase(Ease.Linear);
    }

    public void SetNoStar()
    {
        HasStar = false;
        _star.SetActive(false);
    }

    public void SetHasStar()
    {
        HasStar = true;
        _star.SetActive(true);
    }
}