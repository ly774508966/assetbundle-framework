using UnityEngine;
using System.Collections;
using AssetBundles;

public class GameLaunch : MonoBehaviour
{
    const string luachPrefabPath = "UI/Prefab/AssetbundleUpdaterPanel.prefab";

    IEnumerator Start ()
    {
        int frameCount = Time.frameCount;
        yield return AssetBundleManager.Instance.Initialize();
        UnityEngine.Debug.Log("AssetBundleManager Initialized, use frameCount = " + (Time.frameCount -frameCount));
        frameCount = Time.frameCount;
        var loader = AssetBundleManager.Instance.LoadAssetAsync(luachPrefabPath, typeof(GameObject));
        yield return loader;
        UnityEngine.Debug.Log("Open luanch window, use frameCount :" + (Time.frameCount - frameCount));
        var prefab = loader.asset as GameObject;
        loader.Dispose();
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab);
            var ui_root = GameObject.Find("UI Root");
            go.transform.parent = ui_root.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            go.AddComponent<AssetbundleUpdater>();
        }
        else
        {
            UnityEngine.Debug.LogError("LoadAssetAsync luachPrefabPath err : " + luachPrefabPath);
        }

        yield break;
	}
}
