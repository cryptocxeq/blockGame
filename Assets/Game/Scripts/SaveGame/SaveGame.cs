using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> ListCreated=new List<GameObject>();

    public static SaveGame instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveListCreated(int type, int indexSet, int indexBlock)
    {
        PlayerPrefs.SetInt("type_" + type + "indexSet_" + indexSet, indexBlock);
        PlayerPrefs.SetString("block", "true");
    }

    public void saveSprite( int indexSet,int indexSprite)
    {
        PlayerPrefs.SetInt("indexSet_"+indexSet, indexSprite);
    }

    public void deleteList()
    {
        PlayerPrefs.SetString("block", "false");
    }

    public void saveBlock(string indexBg,int indexSprite,int valueStar)
    {
        PlayerPrefs.SetInt("background_" + indexBg, indexSprite);
        PlayerPrefs.SetInt("star_" + indexBg, valueStar);
        PlayerPrefs.SetString("all", "true");
    }

    public void deleteAll()
    {
        PlayerPrefs.DeleteKey("indexSet_" + 0);
        PlayerPrefs.DeleteKey("indexSet_" + 2);
        PlayerPrefs.DeleteKey("indexSet_" + 1);

        PlayerPrefs.SetString("all", "false");
    }

    public void saveScore(int score)
    {
        Debug.Log(score);
        PlayerPrefs.SetInt("Score", score);
    }
    public void saveBestScore(int score)
    {
        PlayerPrefs.SetInt("bestScore", score);
    }

    public void firstLogin()
    {
        PlayerPrefs.SetInt("firstLogin", 1);
    }
}

