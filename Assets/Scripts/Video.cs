using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class Video : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;  // 引用 RawImage

    #region 生命週期

    void Start()
    {
        rawImage.color = new Color(1, 1, 1, 1);  // 确保 RawImage 初始是可见的
        videoPlayer.loopPointReached += OnMovieFinished;  // 設定事件 (完成播放)
        videoPlayer.Prepare();  // 预加载视频
        videoPlayer.Play();  // 开始播放
    }

    void Update()
    {
       
    }
    #endregion

    #region Events 
    void OnMovieFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeOut());  // 播放结束后开始淡出
    }

    private IEnumerator FadeOut()
    {
        float duration = 1f;  // 淡出时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rawImage.color = new Color(1, 1, 1, Mathf.Clamp01(1 - (elapsedTime / duration)));  // 更新 alpha 值
            yield return null;
        }

        rawImage.gameObject.SetActive(false);  // 关闭 RawImage
        videoPlayer.Stop();  // 停止 VideoPlayer
    }
    #endregion
}