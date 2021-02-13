using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float LastTime = 0.0f;
	GUIStyle style = new GUIStyle();
	Rect rect;
	int fps;
	string text;

	private void Start()
    {
		int w = Screen.width, h = Screen.height;
		rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = Color.white;


	}
	void OnGUI()
	{
		if (Time.timeSinceLevelLoad - LastTime>= 1f)
        {
			 fps = (int)(1.0f / Time.unscaledDeltaTime);
			text = string.Format("         {0:0} fps", fps.ToString());
			LastTime = Time.timeSinceLevelLoad;
		}
		GUI.Label(rect, text, style);

	}
}