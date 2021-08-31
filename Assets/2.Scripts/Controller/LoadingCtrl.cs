using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MEC;
using UnityEngine.U2D;


/// <summary>
/// 场景加载控制
/// </summary>
public class LoadingCtrl : MonoBehaviour
{
	/// <summary>
	/// 目标加载场景
	/// </summary>
	public static int Target;

	//用于异步加载场景
	public Slider loadingSlider;

	public GameScoreSettingsIO gameScoreSettings;//尽在这里弄一个单利

	public Image QB;
	public SpriteAtlas spriteAtlas;

	public Text loadingText;
	public Text StatusText;

	private float loadingSpeed = 1;

	private float targetValue;

	private AsyncOperation operation;

	// Use this for initialization
	void Start()
	{

		loadingSlider.value = 0.0f;

		if (SceneManager.GetActiveScene().name == "Loading")
		{
			//启动协程
			StartCoroutine(AsyncLoading());
		}

		InvokeRepeating("QBAnimation", 0f, 0.1f);
	}

	int qbImage = 0;
	public void QBAnimation()
    {
		//重置动画
		if(qbImage == 10)
        {
			QB.sprite = spriteAtlas.GetSprite(string.Format("emotionA00{0}.cv2", qbImage.ToString()));
			qbImage = 0;

		}
        else
        {
			QB.sprite = spriteAtlas.GetSprite(string.Format("emotionA000{0}.cv2", qbImage.ToString()));
			qbImage++;

		}


	}

	/// <summary>
	/// 加载场景
	/// </summary>
	/// <param name="id">场景id</param>
	/// <param name="UseLoadScene">使用有QB的加载场景吗</param>
	public static void LoadScene(int id,bool AsyncLoadScene = true)
    {
		//设置好目标场景
		Target = id;

		if (AsyncLoadScene)
        {
			//然后进入Loading场景
			SceneManager.LoadScene("Loading");
			//然后干活（自动）
		}
        else
        {
			//直接进入相应场景
			SceneManager.LoadScene(Target,LoadSceneMode.Single);
		}
	}

	IEnumerator AsyncLoading()
	{
		loadingText.text = "灵魂宝石诅咒含量：0%";

		StatusText.text = "载入游戏设置与状态";
		yield return gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");

		//游戏还没通关
		if (!gameScoreSettings.Success)
        {
			StatusText.text = "加载存档与设置";
			gameScoreSettings.Load();
			StatusText.text = "保存设置";
			gameScoreSettings.SaveSettings(); 
		}
		//通关（打完了瓦夜）
		else
        {
			StatusText.text = "保存存档与设置";
			Timing.RunCoroutine(gameScoreSettings.SaveAll());
		}

		StatusText.text = "垃圾回收（Beta）";
		System.GC.Collect();

		StatusText.text = "加载场景";
		operation = SceneManager.LoadSceneAsync(Target);
		//阻止当加载完成自动切换
		operation.allowSceneActivation = false;

		yield return operation;
	}

	// Update is called once per frame
	void Update()
	{
		targetValue = operation.progress;

		if (operation.progress >= 0.9f)
		{
			//operation.progress的值最大为0.9
			targetValue = 1.0f;
		}

		if (targetValue != loadingSlider.value)
		{
			//插值运算
			loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
			if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f)
			{
				loadingSlider.value = targetValue;
			}
		}

		loadingText.text = string.Format("灵魂宝石诅咒含量：{0}%", (loadingSlider.value * 100).ToString("00"));

		if ((int)(loadingSlider.value * 100) == 100)
		{
			//允许异步加载完毕后自动切换场景
			operation.allowSceneActivation = true;
		}
	}
}

/*异步加载场景代码来自：（侵权望告知）
 * https://blog.csdn.net/qq_33747722/article/details/72582213
 */
