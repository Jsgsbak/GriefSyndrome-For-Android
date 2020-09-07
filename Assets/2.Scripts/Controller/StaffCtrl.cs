using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 瓦夜被击败/全员挂掉的时候转移到这里
/// </summary>
public class StaffCtrl : MonoBehaviour
{
    public Text[] staff;
    public GameObject s;

    public CanvasGroup canvasGroup;

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        //获取魔女文staff
        UnityWebRequest request = UnityWebRequest.Get("https://gitee.com/pureamaya/GriefSyndrome-For-Android/raw/develop/StaffForGame/MAJO.txt");
        request.timeout = 3;
        // 
        // UnityWebRequest request = new UnityWebRequest("http://example.com");
        // 
        // request.method = UnityWebRequest.kHttpVerbGET;

        // 
        yield return request.Send();

        // 
        if (request.isNetworkError)
        {
            //网络错误，使用离线版staff
        }
        else
        {
            if (request.responseCode == 200)
            {
                
                staff[1].text = request.downloadHandler.text;
            }
        }


        //获取staff
        request = UnityWebRequest.Get("https://gitee.com/pureamaya/GriefSyndrome-For-Android/raw/develop/StaffForGame/HUMAN.txt");
        // 
        // UnityWebRequest request = new UnityWebRequest("http://example.com");
        // 
        // request.method = UnityWebRequest.kHttpVerbGET;

        // 
        yield return request.Send();

        //这里才准备显示staff
        staff[0].gameObject.SetActive(true);
        staff[1].gameObject.SetActive(true);
        staff[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, staff[0].preferredHeight);
        staff[1].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, staff[1].preferredHeight);

        // 
        if (request.isNetworkError)
        {
           //网络错误，使用离线版staff
        }
        else
        {
            if (request.responseCode == 200)
            {
                // 
                staff[0].text = request.downloadHandler.text;

                //取消这个的显示，另外还用来通知staff滚动
                s.SetActive(false);

            }
        }

    }


    private void Update()
    {
        //staff被激活，开始滚动
        if (!s.gameObject.active && Mathf.Abs(staff[0].rectTransform.position.y) > 0.1f)
        {
            staff[0].transform.Translate(Vector2.up * 1f * Time.deltaTime);
            staff[1].transform.Translate(Vector2.up * 1f * Time.deltaTime);

        }

        //轻触屏幕，返回标题界面
        if (Input.touchCount >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }


}
