using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using System.IO;

public class depresstest : MonoBehaviour
{

    public List<string> download_list = new List<string>() {
        "myresources/cube0_prefab",
        "materials/20121222171635_xdprn_thumb_600_0_jpeg",
        "materials/newfolder/1390616300363_jpg",
        "materials/newfolder/1390616300363_mat",
        "atlas/knightatlas_prefab",
        "atlas/knightatlas_mat",
        "myresources/ui_prefab",
        "atlas/knightatlas_png_p_ori",
        "atlas/knightatlas_png_u_ori",
        "atlas/knightatlas_png_u_p_lz4",
        "atlas/knightatlas_png_u_p_lzma",
        "atlas/knightatlas_png_u_p_office",
    };

    AssetBundle m_assetbundle = null;
    GameObject m_gameobject = null;

    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
        assetbundle_list = new List<AssetBundle>();
        all_objs = new List<Object>();
        all_gos = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<AssetBundle> assetbundle_list = new List<AssetBundle>();
    List<Object> all_objs = new List<Object>();
    List<GameObject> all_gos = new List<GameObject>();

    void OnGUI()
    {
        if (Application.loadedLevelName == "depresstest")
        {
            if (GUI.Button(new Rect(450, 10, 200, 80), "切换场景到test"))
            {
                Application.LoadLevel("test");
            }

            if (GUI.Button(new Rect(450, 110, 200, 80), "切换场景到mem_static"))
            {
                Application.LoadLevel("mem_static");
            }

            if (GUI.Button(new Rect(150, 140, 200, 80), "清缓存并重下载"))
            {
                StartCoroutine(download());
            }

            if (GUI.Button(new Rect(150, 440, 200, 80), "CreateFromFile(u)"))
            {
                StartCoroutine(CreateFromFile());
            }

            if (GUI.Button(new Rect(150, 540, 200, 80), "CreateFormMem(u)"))
            {
                StartCoroutine(CreateFormMem());
            }

            if (GUI.Button(new Rect(150, 640, 200, 80), "CreateFormMem(u) imm"))
            {
                StartCoroutine(CreateFormMemImm());
            }

            if (GUI.Button(new Rect(150, 740, 200, 80), "CreateFormMem(p) imm"))
            {
                StartCoroutine(CreateFormMemImmp());
            }

            if (GUI.Button(new Rect(150, 840, 200, 80), "CreateFormWWW(u)"))
            {
                StartCoroutine(CreateFormWWW());
            }

            if (GUI.Button(new Rect(150, 940, 200, 80), "CreateFormWWW(p)"))
            {
                StartCoroutine(CreateFormWWWp());
            }

            if (GUI.Button(new Rect(450, 240, 200, 80), "CreateAllOtherAssetbundle"))
            {
                StartCoroutine(CreateAllOtherAssetbundle());
            }

            if (GUI.Button(new Rect(450, 340, 200, 80), "LoadAsset"))
            { 
                StartCoroutine(LoadAsset());
            }

            if (GUI.Button(new Rect(450, 440, 200, 80), "InstantiateObjs"))
            {
                StartCoroutine(InstantiateObjs());
            }

            if (GUI.Button(new Rect(450, 540, 200, 80), "ClearObjsAndAssets"))
            {
                ClearObjsAndAssets();
            }

            if (GUI.Button(new Rect(450, 640, 200, 80), "Unload atlas(true)"))
            {
                if (m_assetbundle != null)
                {
                    m_assetbundle.Unload(true);
                }
            }

            if (GUI.Button(new Rect(450, 740, 200, 80), "UnloadUnusedAsset"))
            {
                UnloadUnusedAsset();
            }

        }
    }

    IEnumerator download()
    {
        //download
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]download start!"));
        foreach (string str in download_list)
        {
            WWW www = new WWW("http://127.0.0.1:7888/AssetBundles/" + str);
            yield return www;

            // If downloading fails.
            if (www.error != null)
            {
                UnityEngine.Debug.LogWarning(string.Format("[depresstest]Failed downloading bundle {0} from {1}: {2}", str, www.url, www.error));
                www.Dispose();
                continue;
            }

            // If downloading succeeds.
            if (www.isDone)
            {
                byte[] bundle_bytes = www.bytes;
                if (bundle_bytes == null || bundle_bytes.Length == 0)
                {
                    UnityEngine.Debug.LogWarning(string.Format("[depresstest]{0} is empty", str));
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
        UnityEngine.Debug.LogWarning("[depresstest]download successfully in " + elapsedTime + " seconds");
        yield break;

    }
    
    IEnumerator CreateFromFile()
    {
        //Create Assetbundle
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle start!"));
        string local_path = AssetBundleUtility.GetPlatformPersistentDataPath("atlas/knightatlas_png_u_ori");
        AssetBundle asset_bundle = AssetBundle.LoadFromFile(local_path);
        if (asset_bundle == null)
        {
            UnityEngine.Debug.LogWarning("[depresstest]AssetBundle null, path = " + local_path);
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle successfully in " + elapsedTime + " seconds");
        if (asset_bundle != null)
        {
            m_assetbundle = asset_bundle;
        }
        yield break;
    }

    IEnumerator CreateFormMem()
    {
        //Create Assetbundle
        string local_path = AssetBundleUtility.GetPlatformPersistentDataPath("atlas/knightatlas_png_u_ori");
        FileInfo file_info = new FileInfo(local_path);
        if (!file_info.Exists)
        {
            UnityEngine.Debug.LogWarning("File not exist!");
            yield break;
        }
        byte[] byes = File.ReadAllBytes(file_info.FullName);

        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle  from mem start!"));
        AssetBundleCreateRequest abcq = AssetBundle.LoadFromMemoryAsync(byes);
        yield break;
    }

    IEnumerator CreateFormMemImm()
    {
        //Create Assetbundle
        string local_path = AssetBundleUtility.GetPlatformPersistentDataPath("atlas/knightatlas_png_u_ori");
        FileInfo file_info = new FileInfo(local_path);
        if (!file_info.Exists)
        {
            UnityEngine.Debug.LogWarning("File not exist!");
            yield break;
        }
        byte[] byes = File.ReadAllBytes(file_info.FullName);

        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle  from mem imm start!"));
        AssetBundle asset_bundle = AssetBundle.LoadFromMemory(byes);
        if (asset_bundle == null)
        {
            UnityEngine.Debug.LogWarning("[depresstest]AssetBundle null, path = " + local_path);
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from mem imm successfully in " + elapsedTime + " seconds");
        if (asset_bundle != null)
        {
            m_assetbundle = asset_bundle;
        }
        yield break;
    }


    IEnumerator CreateFormMemImmp()
    {
        //Create Assetbundle
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle  from mem imm p start!"));
        string local_path = AssetBundleUtility.GetPlatformPersistentDataPath("atlas/knightatlas_png_u_p_office");
        FileInfo file_info = new FileInfo(local_path);
        if (!file_info.Exists)
        {
            UnityEngine.Debug.LogWarning("File not exist!");
            yield break;
        }
        byte[] byes = File.ReadAllBytes(file_info.FullName);

        AssetBundle asset_bundle = AssetBundle.LoadFromMemory(byes);
        if (asset_bundle == null)
        {
            UnityEngine.Debug.LogWarning("[depresstest]AssetBundle null, path = " + local_path);
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from mem imm p successfully in " + elapsedTime + " seconds");
        if (asset_bundle != null)
        {
            m_assetbundle = asset_bundle;
        }
        yield break;
    }
    

    IEnumerator CreateFormWWW()
    {
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle  from www start!"));
        string path = AssetBundleUtility.GetPlatformPersistentFilePath("atlas/knightatlas_png_u_ori");
        WWW www = new WWW(path);
        yield return www;
        AssetBundle assetBundle = null;//www.assetBundle;
        //assetBundle.Unload(false);
        byte[] bytes = www.bytes;
        Debug.Log(bytes.Length);
        www.Dispose();
        if (assetBundle == null)
        {
            UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from www failed!");
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from www successfully in " + elapsedTime + " seconds");
        if (assetBundle != null)
        {
            //m_assetbundle = assetBundle;
            //assetBundle.Unload(false);
        }
        yield break;
    }

    IEnumerator CreateFormWWWp()
    {
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateAssetbundle  from www(p) start!"));
        string path = AssetBundleUtility.GetPlatformPersistentFilePath("atlas/knightatlas_png_u_p_office");
        WWW www = new WWW(path);
        yield return www;
        byte[] bytes = www.bytes;
        AssetBundle assetBundle = www.assetBundle;
        www.Dispose();
        if (assetBundle == null)
        {
            UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from www(p) failed!");
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create Assetbundle from www(p) successfully in " + elapsedTime + " seconds");
        if (assetBundle != null)
        {
            m_assetbundle = assetBundle;
        }
        yield break;
    }

    IEnumerator CreateAllOtherAssetbundle()
    {
        //Create Assetbundle
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]CreateOtherAssetbundle start!"));
        foreach (string str in download_list)
        {
            if (str.Contains("knightatlas_png"))
            {
                continue;
            }
            string local_path = AssetBundleUtility.GetPlatformPersistentDataPath(str);
            AssetBundle asset_bundle = AssetBundle.LoadFromFile(local_path);
            if (asset_bundle == null)
            {
                UnityEngine.Debug.LogWarning("[depresstest]AssetBundle null, path = " + local_path);
            }
            else
            {
                assetbundle_list.Add(asset_bundle);
                UnityEngine.Debug.LogWarning("[depresstest]Create AssetBundle, path = " + local_path);
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Create other Assetbundle successfully in " + elapsedTime + " seconds");
        yield break;
    }


    IEnumerator LoadAsset()
    {
        //load asset
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]LoadAsset start!"));
        foreach (AssetBundle ab in assetbundle_list)
        {
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]load asset successfully in " + elapsedTime + " seconds");
        Debug.Log("load asset successfully in " + elapsedTime + " seconds");
        yield break;
    }


    IEnumerator InstantiateObjs()
    {
        //Instantiate
        all_gos.Clear();
        float startTime = Time.realtimeSinceStartup;
        UnityEngine.Debug.LogWarning(string.Format("[depresstest]InstantiateObjs start!"));
        foreach (Object obj in all_objs)
        {
            if (obj.name.Equals("Cube0") || obj.name.Equals("ui"))
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                all_gos.Add(go);
            }
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        UnityEngine.Debug.LogWarning("[depresstest]Instantiate successfully in " + elapsedTime + " seconds");
        Debug.Log("Instantiate successfully in " + elapsedTime + " seconds");


        GameObject goo = GameObject.Find("Labelllllllllllllllllllllllllllllllllll");
        if (goo != null)
        {
            UILabel lb = goo.GetComponent<UILabel>();
            if (lb != null)
            {
                lb.text = "depresstest";
            }
        }

        yield break;
    }

    void ClearObjsAndAssets()
    {
        
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

        
        int cnt = all_objs.Count;
        for (int i = cnt - 1; i >= 0; i--)
        {
            Object obj = all_objs[i];
            all_objs.RemoveAt(i);
            //DestroyObject(obj);
            Resources.UnloadAsset(obj);
        }
        UnityEngine.Debug.LogWarning("[test]DestoryAssets: " + cnt);
    }

    IEnumerator UnloadUnusedAsset()
    {
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset satrt!");
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEngine.Debug.LogWarning("[test]UnloadUnusedAsset finished");
    }
}
