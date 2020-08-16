using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingCtrl : MonoBehaviour
{
    public Scene Title;
    public Scene Loading;
    public Scene MajoKeikai;
    public Scene UIForMajoKeikai;//staff放在这里
	/// <summary>
	/// 目标加载场景
	/// </summary>
	public static int Target;

	//用于异步加载场景
	public Slider loadingSlider;

	public Text loadingText;

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

		//停止BGM
		EasyBGMCtrl.easyBGMCtrl.PlayBGM(-1);
	}

	public static void LoadScene(int id)
    {
		//设置好目标场景
		Target = id;
		//停止bgm
		EasyBGMCtrl.easyBGMCtrl.PlayBGM(-1);
		//然后进入Loading场景
		SceneManager.LoadScene(3);
		//然后干活（自动）
    }

	IEnumerator AsyncLoading()
	{
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
