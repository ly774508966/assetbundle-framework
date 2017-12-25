using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using System.IO;

public class test : MonoBehaviour {

    public List<string> download_list = new List<string>() {
        "myresources/cube0_prefab",
        "materials/20121222171635_xdprn_thumb_600_0_jpeg",
        "materials/newfolder/1390616300363_jpg",
        "materials/newfolder/1390616300363_mat",
        "atlas/knightatlas_prefab",
        "atlas/knightatlas_mat",
        "myresources/ui_prefab",
        "atlas/knightatlas_png",
    };

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

        string path = "C:/Directory/SubDirectory.tt";
        Debug.Log(Path.GetExtension(path));
        Debug.Log(Path.GetDirectoryName(path));
        Debug.Log(Path.GetFileName(path));
        Debug.Log(Path.GetFileNameWithoutExtension(path));
        Debug.Log(Path.GetPathRoot(path));
        Debug.Log(this.GetType().Name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (Application.loadedLevelName == "test")
        {
            if (GUI.Button(new Rect(450, 10, 200, 80), "切换场景到Dependance"))
            {
                Application.LoadLevel("Dependance");
            }

            if (GUI.Button(new Rect(450, 110, 200, 80), "切换场景到depresstest"))
            {
                Application.LoadLevel("depresstest");
            }

            if (GUI.Button(new Rect(150, 140, 200, 80), "清缓存并重下载"))
            {
                StartCoroutine(download());
            }

            if (GUI.Button(new Rect(150, 240, 200, 80), "创建Assetbundle"))
            {
                StartCoroutine(CreateAssetbundle());
            }


            if (GUI.Button(new Rect(150, 340, 200, 80), "创建AssetbundleFromMem"))
            {
                StartCoroutine(CreateFromMem());
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

    IEnumerator download()
    {
        //download
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[test]download start!"));
        foreach (string str in download_list)
        {
            WWW www = new WWW("http://127.0.0.1:7888/AssetBundles/" + str);
            yield return www;

            // If downloading fails.
            if (www.error != null)
            {
                UnityEngine.Debug.LogWarning(string.Format("[test]Failed downloading bundle {0} from {1}: {2}", str, www.url, www.error));
                www.Dispose();
                continue;
            }

            // If downloading succeeds.
            if (www.isDone)
            {
                byte[] bundle_bytes = www.bytes;
                if (bundle_bytes == null || bundle_bytes.Length == 0)
                {
                    UnityEngine.Debug.LogWarning(string.Format("[test]{0} is empty", str));
                    www.Dispose();
                    continue;
                }

                string local_path = AssetBundleUtility.GetPlatformPersistentDataPath(str);
                FileInfo file_info = new FileInfo(local_path);
                DirectoryInfo dir_info = file_info.Directory;
                if (!dir_info.Exists)
                {
                    Directory.CreateDirectory(dir_info.FullName);
                }

                FileStream fs = new FileStream(file_info.FullName, FileMode.Create);
                fs.Write(bundle_bytes, 0, bundle_bytes.Length);
                fs.Close();
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
        UnityEngine.Debug.LogWarning("[test]download successfully in " + elapsedTime + " seconds");
        yield break;

    }

    IEnumerator CreateAssetbundle()
    {
        //Create Assetbundle
        assetbundle_list.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[test]CreateAssetbundle start!"));
        foreach (string str in download_list)
        {
            string local_path = AssetBundleUtility.GetPlatformPersistentDataPath(str);
            AssetBundle asset_bundle = AssetBundle.LoadFromFile(local_path);
            if (asset_bundle == null)
            {
                UnityEngine.Debug.LogWarning("[test]AssetBundle null, path = " + local_path);
            }
            else
            {
                assetbundle_list.Add(asset_bundle);
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[test]Create Assetbundle successfully in " + elapsedTime + " seconds");
        Debug.Log("Create Assetbundle successfully in " + elapsedTime + " seconds");
        yield break;
    }

    IEnumerator CreateFromMem()
    {
        //Create Assetbundle
        assetbundle_list.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[test]CreateAssetbundle  from mem start!"));
        foreach (string str in download_list)
        {
            string local_path = Path.Combine(AssetBundleUtility.GetPlatformPersistentDataPath(), str);
            FileInfo file_info = new FileInfo(local_path);
            if (!file_info.Exists)
            {
                UnityEngine.Debug.LogWarning("File not exist!");
                continue;
            }
            byte[] byes = File.ReadAllBytes(file_info.FullName);
            //AssetBundle asset_bundle = AssetBundle.CreateFromMemoryImmediate(byes);
            AssetBundleCreateRequest abcq = AssetBundle.LoadFromMemoryAsync(byes);
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[test]Create Assetbundle from mem successfully in " + elapsedTime + " seconds");
        Debug.Log("Create Assetbundle successfully in " + elapsedTime + " seconds");
        yield break;
    }

    IEnumerator LoadAsset()
    {
        //load asset
        all_objs.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[test]LoadAsset start!"));
        foreach (AssetBundle ab in assetbundle_list)
        {
            if (ab.Contains("Cube0") || ab.Contains("ui"))
            {
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[test]load asset successfully in " + elapsedTime + " seconds");
        Debug.Log("load asset successfully in " + elapsedTime + " seconds");
        yield break;
    }

    IEnumerator InstantiateObjs()
    {
        //Instantiate
        all_gos.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[test]InstantiateObjs start!"));
        foreach (Object obj in all_objs)
        {
            if (obj.name.Equals("Cube0") || obj.name.Equals("ui"))
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                all_gos.Add(go);
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[test]Instantiate successfully in " + elapsedTime + " seconds");
        Debug.Log("Instantiate successfully in " + elapsedTime + " seconds");


        GameObject goo = GameObject.Find("Labelllllllllllllllllllllllllllllllllll");
        if (goo != null)
        {
            UILabel lb = goo.GetComponent<UILabel>();
            if (lb != null)
            {
                lb.text = "test";
            }
        }

        yield break;
    }

    void UnloadAssetbundle(bool bbb)
    {
        UnityEngine.Debug.LogWarning(string.Format("[test]UnloadAssetbundle start!"));
        foreach (AssetBundle ab in assetbundle_list)
        {
            ab.Unload(bbb);
        }
        UnityEngine.Debug.LogWarning("[test]UnloadAssetbundle(" + bbb + "): " + assetbundle_list.Count);
        assetbundle_list.Clear();
    }

    void DestoryGameObjs()
    {
        UnityEngine.Debug.LogWarning(string.Format("[test]DestoryGameObjs start!"));
        foreach (GameObject go in all_gos)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(go);
#else
            GameObject.Destroy(go);
#endif
        }
        UnityEngine.Debug.LogWarning("[test]DestoryGameObjs: " + all_gos.Count);
        all_gos.Clear();
    }

    void DestoryAssets()
    {
        UnityEngine.Debug.LogWarning(string.Format("[test]DestoryAssets start!"));
        int cnt = all_objs.Count;
        for (int i = cnt - 1; i >= 0; i--)
        {
            Object obj = all_objs[i];
            all_objs.RemoveAt(i);
            DestroyObject(obj);
            Object.DestroyObject(obj);
            Object[] objs = Resources.FindObjectsOfTypeAll(typeof(Object));
            if (objs != null)
            {
                foreach (Object ddd in objs)
                {
                    UnityEngine.Debug.Log(ddd.GetInstanceID().ToString());
                }
            }
            else
            {
                UnityEngine.Debug.Log("objs null!");
            }
            //Resources.UnloadAsset(obj);
        }
        UnityEngine.Debug.LogWarning("[test]DestoryAssets: " + cnt);
    }

    void ClearAssetRecord()
    {
        UnityEngine.Debug.LogWarning("[test]ClearAssetRecord: " + all_objs.Count);
        all_objs.Clear();
    }

    IEnumerator UnloadUnusedAsset()
    {
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset satrt!");
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset finished");

        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(new System.Diagnostics.StackFrame(true));
        int aaaa = st.GetFrame(0).GetFileLineNumber();
        UnityEngine.Debug.Log("dddddddddd");
        int aaa = 10;
    }
}
