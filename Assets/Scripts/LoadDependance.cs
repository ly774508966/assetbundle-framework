using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using System.IO;

/*
 * •Enable Simulation Mode using the menu item “Assets/AssetBundles/Simulation Mode”.
 * •Open the scene “AssetBundleSample/Scenes/AssetLoader”.\
 * •Note that the scene is essentially empty and only contains a Main Camera, Directional Light and “Loader” GameObject.
 * •Enter Playmode.
 * •Note that a cube has been loaded into the scene from an AssetBundle.
 */

public class LoadDependance : MonoBehaviour
{
    public List<string> download_list = new List<string>() {
        "myresources/cube0_prefab",
        "materials/20121222171635_xdprn_thumb_600_0_jpeg",
        "materials/newfolder/1390616300363_jpg",
        "materials/newfolder/1390616300363_mat",
        "atlas/knightatlas_prefab",
        "atlas/knightatlas_mat",
        "myresources/ui_prefab",
        "atlas/knightatlas_png_",
    };
    AssetBundleManifest m_AssetBundleManifest = null;
    List<AssetBundle> assetbundle_list = new List<AssetBundle>();
    List<Object> all_objs = new List<Object>();
    List<GameObject> all_gos = new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
        assetbundle_list = new List<AssetBundle>();
        all_objs = new List<Object>();
        all_gos = new List<GameObject>();
        string name = "atlas/knightatlas_png";
    }

    void OnGUI()
    {
        if (Application.loadedLevelName == "Dependance")
        {
            if (GUI.Button(new Rect(450, 10, 200, 80), "切换场景到test"))
            {
                GameObject go = GameObject.Find("AssetBundleManager");
                if (go != null)
                {
                    GameObject.Destroy(go);
                }
                Application.LoadLevel("test");
            }

            if (GUI.Button(new Rect(450, 110, 200, 80), "切换场景到mem_static"))
            {
                GameObject go = GameObject.Find("AssetBundleManager");
                if (go != null)
                {
                    GameObject.Destroy(go);
                }
                Application.LoadLevel("mem_static");
            }

            if (GUI.Button(new Rect(150, 140, 200, 80), "下载并另存Assetbundle"))
            {
                StartCoroutine(downloadAndSave());
            }

            if (GUI.Button(new Rect(150, 240, 200, 80), "清缓缓存"))
            {
                if (Caching.CleanCache())
                {
                    UnityEngine.Debug.LogWarning("[Dependance]CleanCache done");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("[Dependance]CleanCache err");
                }
            }

            if (GUI.Button(new Rect(150, 340, 200, 80), "下载并创建Assetbundle"))
            {
                StartCoroutine(downloadAndCreate());
            }

            if (GUI.Button(new Rect(150, 440, 200, 80), "加载Asset"))
            {
                StartCoroutine(LoadAsset());
            }

            if (GUI.Button(new Rect(150, 540, 200, 80), "实例化物体"))
            {
                StartCoroutine(InstantiateObjs());
            }

            if (GUI.Button(new Rect(150, 640, 200, 80), "输出assetbundle_list"))
            {
                string str = "assetbundle_list:\n";
                int i = 0;
                foreach (AssetBundle ab in assetbundle_list)
                {
                    i++;
                    str += "[" + i + "]" + (ab == null ? "Null" : ab.name + "_" + ab.GetInstanceID()) + "\n";
                }
                UnityEngine.Debug.LogWarning(str);
            }

            if (GUI.Button(new Rect(150, 740, 200, 80), "输出all_assets"))
            {
                string str = "all_assets:\n";
                int i = 0;
                foreach (Object obj in all_objs)
                {
                    i++;
                    str += "[" + i + "]" + (obj == null ? "Null" : obj.name + "_" + obj.GetInstanceID()) + "\n";
                }
                UnityEngine.Debug.LogWarning(str);
            }

            if (GUI.Button(new Rect(150, 840, 200, 80), "输出all_gos"))
            {
                string str = "all_gos:\n";
                int i = 0;
                foreach (GameObject go in all_gos)
                {
                    i++;
                    str += "[" + i + "]" + (go == null ? "Null" : go.name + "_" + go.GetInstanceID()) + "\n";
                }
                UnityEngine.Debug.LogWarning(str);
            }

            if (GUI.Button(new Rect(450, 240, 200, 80), "卸载Assetbundle(false)"))
            {
                UnloadAssetbundle(false);
            }

            if (GUI.Button(new Rect(450, 340, 200, 80), "卸载Assetbundle(true)"))
            {
                UnloadAssetbundle(true);
            }

            if (GUI.Button(new Rect(450, 440, 200, 80), "销毁物体"))
            {
                DestoryGameObjs();
            }

            if (GUI.Button(new Rect(450, 540, 200, 80), "销毁asset对象，并清空记录"))
            {
                DestoryAssets();
            }

            if (GUI.Button(new Rect(450, 640, 200, 80), "清空asset记录"))
            {
                ClearAssetRecord();
            }

            if (GUI.Button(new Rect(450, 740, 200, 80), "UnloadUnusedAsset"))
            {
                StartCoroutine(UnloadUnusedAsset());
            }

        }
    }

    IEnumerator downloadAndSave()
    {
        //download
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[dependance]download start!"));
        foreach (string str in download_list)
        {
            WWW www = new WWW("http://127.0.0.1:7888/AssetBundles/" + str);
            yield return www;

            // If downloading fails.
            if (www.error != null)
            {
                UnityEngine.Debug.LogWarning(string.Format("[dependance]Failed downloading bundle {0} from {1}: {2}", str, www.url, www.error));
                www.Dispose();
                continue;
            }

            // If downloading succeeds.
            if (www.isDone)
            {
                byte[] bundle_bytes = www.bytes;
                if (bundle_bytes == null || bundle_bytes.Length == 0)
                {
                    UnityEngine.Debug.LogWarning(string.Format("[dependance]{0} is empty", str));
                    www.Dispose();
                    continue;
                }

                AssetBundle ab = AssetBundle.LoadFromMemory(bundle_bytes);
                if (ab == null)
                {
                    UnityEngine.Debug.LogError("ab null!");
                }
                //TODO
                UnityEngine.Debug.LogWarning("download finish:" + www.url);
                www.Dispose();
            }
            else
            {
                //should never go there
                www.Dispose();
                UnityEngine.Debug.LogWarning("Should never go there!!!!");
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[dependance]download successfully in " + elapsedTime + " seconds");
        yield break;

    }

    IEnumerator downloadAndCreate()
    {
        //download
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[dependance]download start!"));
        assetbundle_list.Clear();
        foreach (string str in download_list)
        {
            WWW www = WWW.LoadFromCacheOrDownload("http://127.0.0.1:7888/AssetBundles/" + str, 
                m_AssetBundleManifest.GetAssetBundleHash(str), 0);
            yield return www;

            // If downloading fails.
            if (www.error != null)
            {
                UnityEngine.Debug.LogWarning(string.Format("[dependance]Failed downloading bundle {0} from {1}: {2}", str, www.url, www.error));
                www.Dispose();
                continue;
            }

            // If downloading succeeds.
            if (www.isDone)
            {
                AssetBundle bundle = www.assetBundle;
                Debug.Log("UnixTimeStempNow = " + UnixTimeStempNow());
                if (bundle == null)
                {
                    UnityEngine.Debug.LogWarning(string.Format("[dependance]{0} is empty", str));
                    www.Dispose();
                    continue;
                }

                assetbundle_list.Add(bundle);
                www.Dispose();
            }
            else
            {
                //should never go there
                www.Dispose();
                UnityEngine.Debug.LogWarning("[dependance]Should never go there!!!!");
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[dependance]downloadAndCreate successfully in " + elapsedTime + " seconds");
        yield break;

    }
    public static long UnixTimeStempNow()
    {
        return (System.DateTime.Now.Ticks - System.DateTime.Parse("1970-01-01 00:00:00").Ticks - 8 * 3600) / 10000;
    }

    IEnumerator LoadAsset()
    {
        //load asset
        all_objs.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[dependance]LoadAsset start!"));
        foreach (AssetBundle ab in assetbundle_list)
        {
            if (ab.Contains("Cube0") || ab.Contains("ui"))
            {
                //all_objs.AddRange(ab.LoadAllAssets());
                //AssetBundleLoadAssetOperationTest ab_req = new AssetBundleLoadAssetOperationTest(ab);
                //if (ab_req == null)
                //    continue;
                //yield return AssetBundleManager.Instance.StartCoroutine(ab_req);
                //Object[] objss = ab_req.GetAllAsset();
                AssetBundleRequest ab_req = ab.LoadAllAssetsAsync();
                yield return ab_req;
                Object[] objss = ab_req.allAssets;
                if (objss != null)
                {
                    all_objs.AddRange(objss);
                }
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[dependance]load asset successfully in " + elapsedTime + " seconds");
        Debug.Log("load asset successfully in " + elapsedTime + " seconds");
        yield break;
    }

    IEnumerator InstantiateObjs()
    {
        //Instantiate
        all_gos.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[dependance]InstantiateObjs start!"));
        foreach (Object obj in all_objs)
        {
            if (obj.name.Equals("Cube0") || obj.name.Equals("ui"))
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                all_gos.Add(go);
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[dependance]Instantiate successfully in " + elapsedTime + " seconds");
        Debug.Log("Instantiate successfully in " + elapsedTime + " seconds");


        GameObject goo = GameObject.Find("Labelllllllllllllllllllllllllllllllllll");
        if (goo != null)
        {
            UILabel lb = goo.GetComponent<UILabel>();
            if (lb != null)
            {
                lb.text = "dependance";
            }
        }

        yield break;
    }

    void UnloadAssetbundle(bool bbb)
    {
        UnityEngine.Debug.LogWarning(string.Format("[dependance]UnloadAssetbundle start!"));
        foreach (AssetBundle ab in assetbundle_list)
        {
            ab.Unload(bbb);
        }
        UnityEngine.Debug.LogWarning("[dependance]UnloadAssetbundle(" + bbb + "): " + assetbundle_list.Count);
        assetbundle_list.Clear();
    }

    void DestoryGameObjs()
    {
        UnityEngine.Debug.LogWarning(string.Format("[dependance]DestoryGameObjs start!"));
        foreach (GameObject go in all_gos)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(go);
#else
            GameObject.Destroy(go);
#endif
        }
        UnityEngine.Debug.LogWarning("[dependance]DestoryGameObjs: " + all_gos.Count);
        all_gos.Clear();
    }

    void DestoryAssets()
    {
        UnityEngine.Debug.LogWarning(string.Format("[dependance]DestoryAssets start!"));
        int cnt = all_objs.Count;
        for (int i = cnt - 1; i >= 0; i--)
        {
            Object obj = all_objs[i];
            all_objs.RemoveAt(i);
        }
        UnityEngine.Debug.LogWarning("[dependance]DestoryAssets: " + cnt);
    }

    void ClearAssetRecord()
    {
        UnityEngine.Debug.LogWarning("[dependance]ClearAssetRecord: " + all_objs.Count);
        all_objs.Clear();
    }

    IEnumerator UnloadUnusedAsset()
    {
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset satrt!");
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset finished");
    }
}
