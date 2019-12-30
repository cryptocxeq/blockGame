using System;
using System.Collections;
using System.Reflection;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PublishCapture : MonoBehaviour
{
#if UNITY_EDITOR

    static object gameViewSizesInstance;
    static MethodInfo getGroup;

    public enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution
    }

    private void Start()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            float tempTimeScale = Time.timeScale;
            Time.timeScale = 0.01f;

            if (Screen.width < Screen.height)
            {
                DOVirtual.DelayedCall(0, () => { StartCoroutine(CaptureScreenShoot(2048, 2732)); }).SetUpdate(true);
                DOVirtual.DelayedCall(.5f, () => { StartCoroutine(CaptureScreenShoot(1242, 2208)); }).SetUpdate(true);
                DOVirtual.DelayedCall(1f, () => { StartCoroutine(CaptureScreenShoot(1242, 2688)); }).SetUpdate(true)
                    .OnComplete(() => { Time.timeScale = tempTimeScale; });
            }
            else
            {
                DOVirtual.DelayedCall(0, () => { StartCoroutine(CaptureScreenShoot(2732, 2048)); }).SetUpdate(true);
                DOVirtual.DelayedCall(.5f, () => { StartCoroutine(CaptureScreenShoot(2208, 1242)); }).SetUpdate(true);
                DOVirtual.DelayedCall(1f, () => { StartCoroutine(CaptureScreenShoot(2688, 1242)); }).SetUpdate(true)
                    .OnComplete(() => { Time.timeScale = tempTimeScale; });
            }
        }
#endif
    }

    private IEnumerator CaptureScreenShoot(int width, int height)
    {
        if (CanvasFollowDevice.OnSolutionChanged != null)
            CanvasFollowDevice.OnSolutionChanged();

        yield return new WaitForEndOfFrame();
        int sizeIndex = 0;
        if (API.IsIOS())
        {
            sizeIndex = FindSize(GameViewSizeGroupType.iOS, width, height);
        }
        else
        {
            sizeIndex = FindSize(GameViewSizeGroupType.Android, width, height);
        }

        if (sizeIndex == -1)
        {
#if UNITY_IOS
            AddCustomSize(GameViewSizeType.FixedResolution, GameViewSizeGroupType.iOS, width, height,
                string.Format("{0}x{1}", width, height));
            sizeIndex = FindSize(GameViewSizeGroupType.iOS, width, height);
#endif

#if UNITY_ANDROID
            AddCustomSize(GameViewSizeType.FixedResolution, GameViewSizeGroupType.Android, width, height,
                string.Format("{0}x{1}", width, height));
            sizeIndex = FindSize(GameViewSizeGroupType.Android, width, height);
#endif
        }

#if UNITY_EDITOR
        if (CanvasFollowDevice.OnSolutionChanged != null)
            CanvasFollowDevice.OnSolutionChanged();
#endif

        Debug.Log("SizeIndex: " + sizeIndex);
        SetSize(sizeIndex);

        string name = (float) Screen.width / Screen.height + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_");
        string path = Application.dataPath.Replace("Assets", string.Empty) + name + ".png";
        Debug.Log(path);
        ScreenCapture.CaptureScreenshot(path);
    }

    private static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] {index, null});
    }


    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int) getBuiltinCount.Invoke(group, null) + (int) getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int) widthProp.GetValue(size, null);
            int sizeHeight = (int) heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }

        return -1;
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width,
        int height, string text)
    {
        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] {typeof(int), typeof(int), typeof(int), typeof(string)});
        var newSize = ctor.Invoke(new object[] {(int) viewSizeType, width, height, text});
        addCustomSize.Invoke(group, new object[] {newSize});
    }


    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] {(int) type});
    }
#endif
}