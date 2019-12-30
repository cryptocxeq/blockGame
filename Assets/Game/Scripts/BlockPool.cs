using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
	public static SpawnPool Pool;

	private void Awake()
	{
		Pool = gameObject.AddComponent<SpawnPool>();
	}
}
