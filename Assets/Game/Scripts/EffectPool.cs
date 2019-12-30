using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static SpawnPool Pool;
    private void Awake()
    {
        Pool = gameObject.AddComponent<SpawnPool>();
    }
}
