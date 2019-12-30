
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class GameHelper
{
    public static void DeleteAllChilds(this Transform t)
    {
        int count = t.childCount;
        for (int i = 0; i < count; i++)
            Object.Destroy(t.GetChild(0).gameObject);
    }

    public static void DeleteDestroyImmediateAllChilds(this Transform t)
    {
        int count = t.childCount;
        for (int i = 0; i < count; i++)
            Object.DestroyImmediate(t.GetChild(0).gameObject);
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }
    /// <summary>
    /// Return list object in scene (without active)
    /// </summary>
    /// <returns></returns>
    public static List<Object> GetAllObjectsInScene()
    {
        var objs = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Object> listObjs = new List<Object>();
        foreach (var item in objs)
            FindAllChild(item.transform, ref listObjs);
        return listObjs;
    }
    /// <summary>
    /// Find child in <see cref="Transform"/>
    /// </summary>
    /// <param name="t">Trans need to find child</param>
    /// <param name="list">List all child of transform</param>
    private static void FindAllChild(Transform t, ref List<Object> list)
    {
        list.Add(t.gameObject);
        foreach (Transform child in t)
            FindAllChild(child, ref list);
    }
    
    public static void SetSizeFollowWidth(this Image img, int maxWidth)
    {
        if (img.sprite == null)
            return;
        float aspect = img.sprite.bounds.size.y / img.sprite.bounds.size.x;
        img.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, maxWidth * aspect);
    }

    public static void SetSizeFollowHeight(this Image img, int maxHeight)
    {
        if (img.sprite == null)
            return;
        float aspect = img.sprite.bounds.size.y / img.sprite.bounds.size.x;
        img.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHeight / aspect, maxHeight);
    }



#if UNITY_EDITOR

    public static List<T> GetAllAssetAtPath<T>(string filter, string path)
    {
        string[] findAssets = UnityEditor.AssetDatabase.FindAssets(filter, new[] { path });
        List<T> os = new List<T>();
        foreach (var findAsset in findAssets)
        {
            os.Add((T)Convert.ChangeType(UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(findAsset), typeof(T)), typeof(T)));
        }
        return os;
    }

    public static List<string> GetAllScenes()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
            scenes.Add(SceneManager.GetSceneAt(i).name);
        return scenes;
    }
    
    public static List<Object> GetAllAssets(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets(null, paths);
        var assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadMainAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

    public static void PingObj(string path)
    {
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
        UnityEditor.Selection.activeObject = obj;
        UnityEditor.EditorGUIUtility.PingObject(obj);
    }
#endif
}

