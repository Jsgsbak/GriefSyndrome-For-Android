using UnityEngine;
using UnityEngine.Audio;
using System;

#if UNITY_EDITOR

namespace FutureCartographer.FCAudioTools.PreviewPlay
{
	public class FCATPreviewPlay : MonoBehaviour
	{
		private AudioClip _attachAudioClip;
		public AudioClip AttachAudioClip
		{
			get
			{
				return _attachAudioClip;
			}
			set
			{
				_attachAudioClip = value;

				var source = AudioSource;
				if (source == null) return;
				source.clip = value;
			}
		}

		private AudioMixerGroup _attachAudioMixerGroup;
		public AudioMixerGroup AttachAudioMixerGroup
		{
			get
			{
				return _attachAudioMixerGroup;
			}
			set
			{
				_attachAudioMixerGroup = value;

				var source = AudioSource;
				if (source == null) return;
				source.outputAudioMixerGroup = value;
			}
		}

		private AudioMixerSnapshot _audioMixerSnapshot;
		public AudioMixerSnapshot AttachAudioMixerSnapshot
		{
			get
			{
				return _audioMixerSnapshot;
			}
			set
			{
				_audioMixerSnapshot = value;
				if (value == null) return;

				_audioMixerSnapshot.TransitionTo(0);
			}
		}

		private AudioSource _audioSource;
		private AudioSource AudioSource
		{
			get
			{
				if (_audioSource == null)
				{
					_audioSource = GetComponent<AudioSource>();
					if (_audioSource == null)
					{
						_audioSource = this.gameObject.AddComponent<AudioSource>();
						_audioSource.playOnAwake = false;
					}
				}

				return _audioSource;
			}
		}

		public bool IsPlaying
		{
			get
			{
				var source = AudioSource;
				if (source == null) return false;

				return source.isPlaying;
			}
		}

		[NonSerialized]
		public Action<int> OnChangeStatus;

		void OnDisable()
		{
			Release();
		}

		public void Release()
		{
			AttachAudioClip = null;
			AttachAudioMixerGroup = null;
			AttachAudioMixerSnapshot = null;

			if (Application.isPlaying == true)
				GameObject.Destroy(this.gameObject);
			else
				GameObject.DestroyImmediate(this.gameObject);
		}

		/// <summary>
		/// Preview sound start
		/// </summary>
		public void Play()
		{
			var source = AudioSource;
			if (source == null) return;

			source.loop = true;
			source.timeSamples = 0;

			if (_audioMixerSnapshot != null)
				_audioMixerSnapshot.TransitionTo(0);

			source.clip = _attachAudioClip;
			source.outputAudioMixerGroup = _attachAudioMixerGroup;

			if (source.isPlaying == false)
			{
				source.Play();

				if (OnChangeStatus != null)
					OnChangeStatus(0);
			}
		}

		/// <summary>
		/// Preview sound stop
		/// </summary>
		public void Stop()
		{
			var source = _audioSource;
			if (source == null) return;

			if (source.isPlaying == true)
			{
				source.Stop();
				source.outputAudioMixerGroup = null;
				source.clip = null;

				if (OnChangeStatus != null)
					OnChangeStatus(1);

				if (Application.isPlaying == true)
					GameObject.Destroy(this.gameObject);
				else
					GameObject.DestroyImmediate(this.gameObject);
			}
		}

		public void SeekStart()
		{
			var source = AudioSource;
			if (source == null) return;

			source.timeSamples = 0;
		}

		public void SetAudioMixer(AudioMixerGroup axGroup, AudioMixerSnapshot axSnapshot)
		{
			AttachAudioMixerGroup = axGroup;
			AttachAudioMixerSnapshot = axSnapshot;
		}

		private static string PreviewGOName = "FCAudioToolsPlayPreview";

		public static MonoBehaviour CreatePreviiewPlayGameObject(int identifierID)
		{
			string objname = PreviewGOName + identifierID;

			GameObject go = GameObject.Find(objname);
			if (go == null)
			{
				go = new GameObject(objname);

				go.hideFlags = HideFlags.HideAndDontSave;
				go.tag = "EditorOnly";
			}

			var iaus = go.GetComponent<FCATPreviewPlay>();
			if (iaus == null)
			{
				iaus = go.AddComponent<FCATPreviewPlay>();
				if (iaus == null)
				{
					GameObject.DestroyImmediate(go);
					return null;
				}
			}

			return iaus;
		}
	}
}

#endif
