using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BackgroundController : MonoBehaviour
{
    public GameController gameController;

    // 1.d Mengubah background image cerita
    [Obsolete]
    public void SwitchImage(SceneStory currentScene){
        StartCoroutine(LoadImage(currentScene.BackgroundLink.ToString().Trim()));
    }

    [Obsolete]
    IEnumerator LoadImage(string mediaUrl){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            Debug.Log(request.error);
        }
        else{
            this.gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}