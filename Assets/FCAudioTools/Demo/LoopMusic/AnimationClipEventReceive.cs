
#if (UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
#define ENABLE_MANUALTIME
#endif

// animator playable draft documents in 5.6
// https://forum.unity3d.com/threads/animationclipplayable.451854/

using UnityEngine;
using System;
using System.Collections;

namespace FutureCartographer.FCAudioTools.Demo
{
    public class AnimationClipEventReceive : MonoBehaviour
    {
        public AudioSource syncAudio;
        public Action<AnimationEvent> OnDidAudioMarkerEvent;
        public Action<AnimationEvent> OnDidAudioLoopEvent;

#if ENABLE_MANUALTIME
        private Animator animator;

		void Start()
		{
			animator = GetComponent<Animator>();

			animator.SetTimeUpdateMode(UnityEngine.Experimental.Director.DirectorUpdateMode.Manual);
           
        }

		void Update()
		{
			animator.SetTime(syncAudio.time);
		}
#endif

        void OnAudioMarkerEvent(AnimationEvent evt)
		{
			if (Mathf.Abs(syncAudio.time - evt.time) > 0.15f)
				return;

			Debug.Log(string.Format("OnAudioMarkerEvent: {0:0.000} {1} {2}", evt.time, evt.intParameter, evt.stringParameter));

			if (OnDidAudioMarkerEvent != null)
				OnDidAudioMarkerEvent(evt);
		}

		void OnAudioLoopEvent(AnimationEvent evt)
		{
			if (Mathf.Abs(syncAudio.time - evt.time) > 0.15f)
				return;

            Debug.Log(string.Format("OnAudioLoopEvent: {0:0.000} {1} {2}", evt.time, evt.intParameter, evt.stringParameter));

            if (evt.stringParameter == "loopStart")
			{
			}
			else if (evt.stringParameter == "loopEnd" && syncAudio.loop == true)
			{
#if ENABLE_MANUALTIME
                animator.SetTime(syncAudio.time);
#endif
			}

            if (OnDidAudioLoopEvent != null)
				OnDidAudioLoopEvent(evt);
		}
	}
}
