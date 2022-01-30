using UnityEngine;
using System.Collections;
using MEC;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	int fps;
	public Text fpsText;

	private void Start()
    {
		InvokeRepeating(nameof(ShowFps), 0f, 1f);

	}
	void ShowFps()
	{
			 fps = (int)(1.0f / Time.unscaledDeltaTime);
			fpsText.text = string.Format("{0:0} fps", fps.ToString());

	}
}