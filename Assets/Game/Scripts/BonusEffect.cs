using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusEffect : MonoBehaviour
{
    public List<Sprite> BonusImgs;
    public List<SoundInfor> Sounds;
    private Animator _ani;

    [SerializeField] private SpriteRenderer _render;

    private void Awake()
    {
        _ani = GetComponent<Animator>();
    }

    public void ShowBonus(int bonusIndex)
    {
        if (bonusIndex == 1)
        {
            bonusIndex = Random.Range(0, 2);
        }

        // Start animation from secend 0
        _ani.Play("Play", 0, 0);
        _render.sprite = BonusImgs[bonusIndex];
        SoundController.Instance.PlaySoundEffect(Sounds[bonusIndex]);
    }
}