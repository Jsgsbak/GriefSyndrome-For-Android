#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define DISABLE_SYNCANIMATION
#endif

using UnityEngine;
using System.Collections;

namespace FutureCartographer.FCAudioTools.Demo
{
	public class LoopMusicDemo : MonoBehaviour
	{
		public AudioSource bgm;
		public GUISkin skin;

		private AnimationClipEventReceive eventReceive;
		private Animator animator;

		private Rigidbody[] worldrbs;

		void Awake()
		{
			animator = GetComponentInChildren<Animator>();
			eventReceive = GetComponentInChildren<AnimationClipEventReceive>();
		}

		void OnEnable()
		{
#if !DISABLE_MANUALTIME
			eventReceive.OnDidAudioMarkerEvent += OnAudioMarkerEvent;
			eventReceive.OnDidAudioLoopEvent += OnAudioLoopEvent;
#endif
		}

		void OnDisable()
		{
#if !DISABLE_MANUALTIME
			eventReceive.OnDidAudioMarkerEvent -= OnAudioMarkerEvent;
			eventReceive.OnDidAudioLoopEvent -= OnAudioLoopEvent;
#endif
		}

		// Use this for initialization
		void Start()
		{
			worldrbs = FindObjectsOfType<Rigidbody>();
			// bgm.Play();
		}

		void OnGUI()
		{
			GUI.skin = skin;

			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();

			GUILayout.Space(10);

			if (bgm.isPlaying == false)
			{
				if (GUILayout.Button("Play") == true)
				{
					animator.SetBool("play", true);
					bgm.Play();
				}
			}
			else
			{
				if (GUILayout.Button("Stop") == true)
				{
					animator.SetBool("play", false);
					bgm.Stop();
				}
			}
			if (GUILayout.Button("Loop Enable") == true)
				bgm.loop = true;
			if (GUILayout.Button("Loop Disable") == true)
				bgm.loop = false;

			GUILayout.Space(10);

			GUILayout.EndHorizontal();

			GUILayout.Box(string.Format("Loop: {0}  Play position: {1}", bgm.loop, bgm.timeSamples));

			GUILayout.FlexibleSpace();

			GUILayout.HorizontalSlider(bgm.timeSamples, 0, bgm.clip.samples);

			GUILayout.EndArea();
		}

		//--------------------------------------------------
		// rigidbody force

		private bool updateTorque;
		private bool updateForce;

		void FixedUpdate()
		{
			if (updateTorque == true)
			{
				for (int i = 0; i < worldrbs.Length; i++)
				{
					worldrbs[i].AddRelativeTorque(Vector3.forward * 10, ForceMode.Impulse);
				}
				updateTorque = false;
			}

			if (updateForce == true)
			{
				for (int i = 0; i < worldrbs.Length; i++)
				{
					worldrbs[i].AddForce(Vector3.up * UnityEngine.Random.Range(10.0f, 15.0f), ForceMode.Impulse);
				}
				updateForce = false;
			}
		}

		private void OnAudioMarkerEvent(AnimationEvent evt)
		{
			updateTorque = true;
		}

		private void OnAudioLoopEvent(AnimationEvent evt)
		{
			updateForce = true;
		}
	}
}
