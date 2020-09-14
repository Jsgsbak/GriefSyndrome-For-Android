using MEC;
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
    [Header("2个staff文本框")]
    public Text[] staff;

    RectTransform[] StaffRectTr = new RectTransform[2];

    /// <summary>
    /// 提示正在下载staff的那个
    /// </summary>
    public GameObject s;

   [SerializeField] bool IsOnline = true;

    [Header("结局图")]
    /// <summary>
    /// 结局图
    /// </summary>
    public Image image;
    public Sprite[] images;


    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;


    GameScoreSettingsIO gameScoreSettings;

    /// <summary>
    /// 储存死亡的玩家
    /// </summary>
    List<Variable.PlayerFaceType> DeadMahoshoujos = new List<Variable.PlayerFaceType>();//麻花焰规划到了黑长直手里


    private void Awake()
    {
#if UNITY_EDITOR
        //检查是否存在BGMCtrl
        if (GameObject.FindObjectOfType<EasyBGMCtrl>() == null)
        {
            EasyBGMCtrl easyBGMCtrl = Instantiate(PerfebInAsset).GetComponent<EasyBGMCtrl>();
            easyBGMCtrl.IsClone = true;
        }
#endif


        //获取组件
        StaffRectTr[0] = staff[0].rectTransform;
        StaffRectTr[1] = staff[1].rectTransform;
    }



    void Start()
    {
        gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");

        //下载与存档
        StartCoroutine(GetText());
        //告知用户在下载
        s.SetActive(true);

        //背景图与BGM
        SetConcImageAndBGM();

       gameScoreSettings.MajoSceneToTitle = false;


    }

    /// <summary>
    /// 从网络获取staff文本
    /// </summary>
    /// <returns></returns>
    IEnumerator GetText()
    {
        //获取魔女文staff
        UnityWebRequest request = UnityWebRequest.Get("https://gitee.com/pureamaya/GriefSyndrome-For-Android/raw/master/StaffForGame/MAJO.stf");
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
            //修改在线状态，不再下载另外一个
            IsOnline = false;
        }
        else
        {
            if (request.responseCode == 200)
            {
                
                staff[1].text = request.downloadHandler.text;
            }
        }


        //获取staff
        if (IsOnline)
        {
            request = UnityWebRequest.Get("https://gitee.com/pureamaya/GriefSyndrome-For-Android/raw/master/StaffForGame/HUMAN.stf");
        // 
        // UnityWebRequest request = new UnityWebRequest("http://example.com");
        // 
        // request.method = UnityWebRequest.kHttpVerbGET;

        // 
        yield return request.Send();
        }

    

        if (request.isNetworkError)
        {
           //网络错误，使用离线版staff
        }
        else
        {
            if (request.responseCode == 200)
            {
                staff[0].text = request.downloadHandler.text;


            }
        }

        //网络部分处理完之后，使2个staff的上边对齐
        StaffRectTr[1].position = new Vector3(StaffRectTr[1].position.x, StaffRectTr[0].position.y);
        //这里才准备显示staff
        staff[0].gameObject.SetActive(true);
        staff[1].gameObject.SetActive(true);
        //修正文本框高度
        StaffRectTr[0].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, staff[0].preferredHeight);
        StaffRectTr[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, staff[1].preferredHeight);

        //取消下载提示的显示，另外还用来通知staff滚动
        s.SetActive(false);

        //最后，逐渐显现结局图
        image.gameObject.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            image.color = new Color(1f, 1f, 1f, image.color.a + 0.1f);

            yield return new WaitForSeconds(0.1f);
        }


    }


    private void Update()
    {

        //staff被激活，开始滚动 
        if (!s.gameObject.active && Mathf.Abs(StaffRectTr[0].localPosition.y - StaffRectTr[0].sizeDelta.y) > 0.1f)
        {
            StaffRectTr[0].Translate(Vector2.up * 0.22f * Time.deltaTime);
        }

        //staff被激活，开始滚动
        if (!s.gameObject.active && Mathf.Abs(StaffRectTr[1].localPosition.y - StaffRectTr[1].sizeDelta.y) > 0.1f)
        {
            StaffRectTr[1].transform.Translate(Vector2.up * 0.22f * Time.deltaTime);
        }

        //轻触屏幕，返回标题界面
        if (Input.touchCount >= 1 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            //返回音效
            EasyBGMCtrl.easyBGMCtrl.PlaySE(1);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// 设置结局图与BGM
    /// </summary>
    void SetConcImageAndBGM()
    {
        //先隐藏结局图
        image.gameObject.SetActive(false);
        image.color = new Color(1f, 1f, 1f, 0f);

        //结局图+bgm
        for (int i = 0; i < 5; i++)
        {
            //储存死亡的玩家便于判断何种结局
            if (gameScoreSettings.MagicalGirlsDie[i])
            {
                DeadMahoshoujos.Add((Variable.PlayerFaceType)i);
            }
        }
        //全员死亡
        if (DeadMahoshoujos.Count == 5)
        {
            //BE BGM
            EasyBGMCtrl.easyBGMCtrl.PlayBGM(3);
            //BE图片
            image.sprite = images[10];
            //深红色文字
            staff[0].color = new Color(0.6132076f, 0.130162f, 0.130162f);
            staff[1].color = new Color(0.6132076f, 0.130162f, 0.130162f);
            return;
          
        }
        else
        {
            //GE BGM
            EasyBGMCtrl.easyBGMCtrl.PlayBGM(4);
        }

        //全员幸存
        if (DeadMahoshoujos.Count == 0)
        {
            image.sprite = images[0];
        }
        //只有鹿目圆死亡
        else if (DeadMahoshoujos.Count == 1 && DeadMahoshoujos[0] == Variable.PlayerFaceType.Madoka)
        {
            image.sprite = images[1];
        }
        //只有可怜的蓝毛死亡
        else if (DeadMahoshoujos.Count == 1 && DeadMahoshoujos[0] == Variable.PlayerFaceType.Sayaka)
        {
            image.sprite = images[2];
        }
        //只有杏子死亡
        else if (DeadMahoshoujos.Count == 1 && DeadMahoshoujos[0] == Variable.PlayerFaceType.Kyoko)
        {
            image.sprite = images[3];
        }
        //除了蓝毛红毛都死了
        else if (DeadMahoshoujos.Count == 3 && !DeadMahoshoujos.Contains(Variable.PlayerFaceType.Kyoko) && !DeadMahoshoujos.Contains(Variable.PlayerFaceType.Sayaka))
        {
            image.sprite = images[4];
        }
        //除了学姐都死了
        else if (DeadMahoshoujos.Count == 4 && !DeadMahoshoujos.Contains(Variable.PlayerFaceType.Mami))
        {
            image.sprite = images[5];
        }
        //只有学姐死了
        else if (DeadMahoshoujos.Count == 1 && DeadMahoshoujos[0] == Variable.PlayerFaceType.Mami)
        {
            image.sprite = images[6];
        }
        //只有鹿目圆和沙耶加死亡
        else if (DeadMahoshoujos.Count == 2 && DeadMahoshoujos.Contains(Variable.PlayerFaceType.Madoka) && DeadMahoshoujos.Contains(Variable.PlayerFaceType.Sayaka))
        {
            image.sprite = images[7];
        }
        //除了粉圆全挂了
        else if (DeadMahoshoujos.Count == 3 && !DeadMahoshoujos.Contains(Variable.PlayerFaceType.Madoka) && !DeadMahoshoujos.Contains(Variable.PlayerFaceType.Homura))
        {
            image.sprite = images[8];
        }
        //轮回吧，吼姆拉
        else
        {
            image.sprite = images[9];
        }



    }


}
