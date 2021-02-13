using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	GUIStyle style = new GUIStyle();
	Rect rect;

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
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

		//new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec.ToString(), fps.ToString());
		GUI.Label(rect, text, style);
	}
}