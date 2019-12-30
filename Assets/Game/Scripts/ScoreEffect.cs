using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEffect : MonoBehaviour
{
    public TextMesh TxtScore;

    public void Init(Vector3 initPosition, int score)
    {
        transform.position = initPosition;
        TxtScore.text = score.ToString();
    }

    public void Init(Vector3 initPosition, string score)
    {
        transform.position = initPosition;
        TxtScore.text = score;
    }
}