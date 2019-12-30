using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    public Transform CurTransform;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DisplayEditor();
    }

    private void DisplayEditor()
    {
        Block t = target as Block;

        int width = 50;
        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        if (GUILayout.Button("UP", GUILayout.Width(width), GUILayout.Height(width)))
        {
            AddMoreObj(t, 0);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("LEFT", GUILayout.Width(width), GUILayout.Height(width)))
        {
            AddMoreObj(t, 3);
        }

        GUILayout.Space(50);
        if (GUILayout.Button("RIGHT", GUILayout.Width(width), GUILayout.Height(width)))
        {
            AddMoreObj(t, 1);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        if (GUILayout.Button("Down", GUILayout.Width(width), GUILayout.Height(width)))
        {
            AddMoreObj(t, 2);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset", GUILayout.Height(width)))
        {
            ResetGroup(t);
        }

        if (GUILayout.Button("Center", GUILayout.Height(width)))
        {
            ResetToCenter(t);
        }

        if (GUILayout.Button("Focus", GUILayout.Height(width)))
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) as GameObject;
            Selection.activeObject = prefab;
        }

        GUILayout.EndHorizontal();

        if (t.gameObject.scene.IsValid())
        {
            if (GUILayout.Button("Apply Prefab", GUILayout.Height(width)))
            {
                if (t.gameObject.scene.IsValid())
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) as GameObject;
                    PrefabUtility.ReplacePrefab(t.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }
    }

    private void ResetGroup(Block t)
    {
        var children = new List<GameObject>();
        if (t.BlocksContain == null)
        {
            t.BlocksContain = t.transform.Find("Items");
        }

        foreach (Transform child in t.BlocksContain) children.Add(child.gameObject);
        for (int i = 1; i < children.Count; i++)
        {
            DestroyImmediate(children[i], true);
        }

//        if (!t.gameObject.scene.IsValid())
//            return;
//
//        while (t.BlocksContain.childCount > 1)
//        {
//            DestroyImmediate(t.BlocksContain.GetChild(1).gameObject);
//        }
    }

    private void ResetToCenter(Block t)
    {
        Vector3 center = new Vector3();
//        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (Transform child in t.BlocksContain)
        {
            center += child.localPosition;
//            if (child.localPosition.x < minX)
//                minX = child.localPosition.x;
//            if (child.localPosition.x > maxX)
//                maxX = child.localPosition.x;
//
//            if (child.localPosition.y < minY)
//                minY = child.localPosition.y;
//            if (child.localPosition.x > maxY)
//                maxY = child.localPosition.y;
        }

//        center = new Vector3(maxX - minX, maxY - minY) / 2;
        center /= t.BlocksContain.childCount;
        foreach (Transform child in t.BlocksContain)
        {
            child.localPosition -= center;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="direct">0: up, 1: right, 2: down, 3: left</param>
    private void AddMoreObj(Block t, int direct)
    {
        if (CurTransform == null)
        {
            CurTransform = t.BlocksContain.GetChild(0);
        }

        float scale = 1;
//        scale += 0.02f;

        if (direct == 0)
        {
            // up
            Transform objTrans = t.IsHasObjectAtLocalPostion(CurTransform.position + Vector3.up * scale);
            if (objTrans == null)
            {
                var obj = Instantiate(CurTransform, CurTransform.parent);
                obj.localPosition += Vector3.up * scale;
                CurTransform = obj;
                obj.name = "item";
            }
            else
            {
                CurTransform = objTrans;
            }
        }

        else if (direct == 1)
        {
            // up
            Transform objTrans = t.IsHasObjectAtLocalPostion(CurTransform.position + Vector3.right * scale);
            if (objTrans == null)
            {
                var obj = Instantiate(CurTransform, CurTransform.parent);
                obj.localPosition += Vector3.right * scale;
                CurTransform = obj;
            }
            else
            {
                CurTransform = objTrans;
            }
        }
        else if (direct == 2)
        {
            // up
            Transform objTrans = t.IsHasObjectAtLocalPostion(CurTransform.position + Vector3.down * scale);
            if (objTrans == null)
            {
                var obj = Instantiate(CurTransform, CurTransform.parent);
                obj.localPosition += Vector3.down * scale;
                CurTransform = obj;
            }
            else
            {
                CurTransform = objTrans;
            }
        }
        else if (direct == 3)
        {
            // up
            Transform objTrans = t.IsHasObjectAtLocalPostion(CurTransform.position + Vector3.left * scale);
            if (objTrans == null)
            {
                var obj = Instantiate(CurTransform, CurTransform.parent);
                obj.localPosition += Vector3.left * scale;
                CurTransform = obj;
            }
            else
            {
                CurTransform = objTrans;
            }
        }
    }
}