using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequests
{

    private class WebRequestsMonoBehaviour : MonoBehaviour
    {
    }

    private static WebRequestsMonoBehaviour webRequestsMonoBehaviour;

    static void Init()
    {
        if (webRequestsMonoBehaviour == null)
        {
            var obj = new GameObject("WebRequests");
            webRequestsMonoBehaviour = obj.AddComponent<WebRequestsMonoBehaviour>();
        }

    }

    public static void Get(String url, Action<string> onError, Action<string> onSuccess)
    {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, onError, onSuccess));
    }

    private static IEnumerator GetCoroutine(String url, Action<string> onError, Action<string> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                onError(request.error);
            else
            {
                onSuccess(request.downloadHandler.text);
            }
        }
    }

    public static void GetTexture(String url, Action<string> onError, Action<Texture2D> onSuccess)
    {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetTextureCoroutine(url, onError, onSuccess));
    }

    private static IEnumerator GetTextureCoroutine(String url, Action<string> onError, Action<Texture2D> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                onError(request.error);
            else
            {
                DownloadHandlerTexture downloadHandlerTexture = request.downloadHandler as DownloadHandlerTexture;
                onSuccess(downloadHandlerTexture.texture);
            }
        }
    }
}
