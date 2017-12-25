﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine.SceneManagement;

public class AssetbundleUpdater : MonoBehaviour
{
    const int MAX_DOWNLOAD_NUM = 5;
    UILabel statuslable;
    UILabel progresslable;
    UISlider slider;

    bool isDownloading = false;
    bool hasError = false;
    Manifest localManifest = null;
    Manifest hostManifest = null;
    List <string> needDownloadList = new List<string>();
    List<ResourceWebRequester> downloadingRequest = new List<ResourceWebRequester>();

    int totalDownloadCount = 0;
    int finishedDownloadCount = 0;
    
    void Start () {
        statuslable = transform.Find("Label").GetComponent<UILabel>(); ;
        progresslable = transform.Find("progresslable").GetComponent<UILabel>();
        slider = transform.Find("Control - Colored Slider").GetComponent<UISlider>();
        progresslable.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
#if UNITY_EDITOR
        if (AssetBundleConfig.IsEditorMode)
        {
            SwitchSence();
        }
        else
        {
#endif
            StartCoroutine(CheckUpdate());
#if UNITY_EDITOR
        }
#endif
    }

    IEnumerator CheckUpdate()
    {
        statuslable.text = "正在检测资源更新...";
        yield return CheckIfNeededUpdate();
        if (needDownloadList.Count <= 0)
        {
            statuslable.text = "没有资源需要更新！";
            UnityEngine.Debug.Log("No resources to update...");
            SwitchSence();
            yield break;
        }

        // TODO：下载资源确认弹窗
        statuslable.text = "正在更新资源...";
        slider.gameObject.SetActive(true);
        totalDownloadCount = needDownloadList.Count;
        finishedDownloadCount = 0;
        UnityEngine.Debug.Log(totalDownloadCount + " resources to update...");
        yield return StartUpdate();

        statuslable.text = "资源更新完成！";
        UnityEngine.Debug.Log("Update finished...");
        yield return UpdateFinish();
        yield break;
    }

    IEnumerator GetDownloaderUrl()
    {
        var url = string.Empty;
        if (AssetBundleConfig.isDebug)
        {
            var request = AssetBundleManager.Instance.RequestAssetAsync(AssetBundleConfig.AssetBundleServerUrlFileName);
            yield return request;
            url = request.text + AssetBundleUtility.GetCurPlatformName() + "/";
            AssetBundleManager.Instance.DownloadUrl = url;
            request.Dispose();
        }
        else
        {
            // TODO：外网服务器下载url看到时候要不要从服务器拉取
            url = AssetBundleConfig.RemoteServerUrl;
        }
        UnityEngine.Debug.Log("downloadUrl : " + url);
        yield break;
    }

    IEnumerator CheckIfNeededUpdate()
    {
        yield return GetDownloaderUrl();
        localManifest = AssetBundleManager.Instance.curManifest;
        hostManifest = new Manifest();
        var request = AssetBundleManager.Instance.DownloadAssetAsync(hostManifest.ManifestFileName);
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
        {
            // TODO：错误弹窗
            UnityEngine.Debug.LogError("Download host manifest :  " + request.assetbundleName + "\n from url : " + request.url + "\n err : " + request.error);
            request.Dispose();
            yield break;
        }
        var assetbundle = request.assetbundle;
        hostManifest.LoadFromAssetbundle(assetbundle);
        hostManifest.SaveBytes(request.bytes);
        assetbundle.Unload(false);
        request.Dispose();

        needDownloadList = localManifest.CompareTo(hostManifest);
        yield break;
    }

    IEnumerator StartUpdate()
    {
        downloadingRequest.Clear();
        isDownloading = true;
        hasError = false;
        yield return new WaitUntil(()=>
        {
            return isDownloading == false;
        });
        if (needDownloadList.Count > 0)
        {
            // TODO：继续更新确认弹窗
            statuslable.text = "资源更新发生错误！";
            yield return StartUpdate();
        }
        yield break;
    }

    IEnumerator UpdateFinish()
    {
        var request = AssetBundleManager.Instance.DownloadAssetAsync(AssetBundleConfig.AssetsPathMapFileName);
        yield return request;
        var filePath = AssetBundleUtility.GetPlatformPersistentDataPath(request.assetbundleName);
        GameUtility.SafeWriteAllBytes(filePath, request.bytes);
        request.Dispose();

        request = AssetBundleManager.Instance.DownloadAssetAsync(AssetBundleConfig.VariantsMapFileName);
        yield return request;
        filePath = AssetBundleUtility.GetPlatformPersistentDataPath(request.assetbundleName);
        GameUtility.SafeWriteAllBytes(filePath, request.bytes);
        request.Dispose();

        hostManifest.SaveToDiskCahce();
        // 说明：有资源更新时必须清理并重启一下AssetBundleManager
        yield return AssetBundleManager.Instance.Cleanup();
        yield return AssetBundleManager.Instance.Initialize();
        yield break;
    }

    void SwitchSence()
    {
        SceneManager.LoadScene("");
    }
	
	void Update () {
        if (!isDownloading)
        {
            return;
        }

        for (int i = downloadingRequest.Count - 1; i >= 0; i--)
        {
            var request = downloadingRequest[i];
            if (request.isDone)
            {
                if (!string.IsNullOrEmpty(request.error))
                {
                    UnityEngine.Debug.LogError("Error when downloading file : " + request.assetbundleName + "\n from url : " + request.url + "\n err : " + request.error);
                    hasError = true;
                    needDownloadList.Add(request.assetbundleName);
                }
                else
                {
                    // TODO：更新下载流量
                    UnityEngine.Debug.Log("Finish downloading file : " + request.assetbundleName + "\n from url : " + request.url);
                    downloadingRequest.RemoveAt(i);
                    finishedDownloadCount++;
                    var filePath = AssetBundleUtility.GetPlatformPersistentDataPath(request.assetbundleName);
                    GameUtility.SafeWriteAllBytes(filePath, request.bytes);
                }
                request.Dispose();
            }
        }

        if (!hasError)
        {
            while (downloadingRequest.Count < MAX_DOWNLOAD_NUM && needDownloadList.Count > 0)
            {
                var fileName = needDownloadList[needDownloadList.Count - 1];
                needDownloadList.RemoveAt(needDownloadList.Count - 1);
                var request = AssetBundleManager.Instance.DownloadAssetAsync(fileName);
                downloadingRequest.Add(request);
            }
        }

        if (downloadingRequest.Count == 0)
        {
            isDownloading = false;
        }
        progresslable.text = string.Format("{0}/{1}", finishedDownloadCount, totalDownloadCount);
        float progressSlice = 1.0f / totalDownloadCount;
        float progressValue = finishedDownloadCount * progressSlice;
        for (int i = 0; i < downloadingRequest.Count; i++)
        {
            progressValue += progressSlice * downloadingRequest[i].progress;
        }
        slider.value = progressValue;
    }
}
