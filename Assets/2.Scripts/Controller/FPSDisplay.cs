using UnityEngine;
using System.Collections;
using MEC;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	float LastTime = 0.0f;
	int fps;
	public Text fpsText;

	private void Start()
    {
		InvokeRepeating("ShowFps", 0f, 1f);

	}
	void ShowFps()
	{
			 fps = (int)(1.0f / Time.unscaledDeltaTime);
			fpsText.text = string.Format("{0:0} fps", fps.ToString());

	}
}