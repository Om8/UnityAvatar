using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AI.Volume.Bot.Audio
{
	public class SpeechEnded : MonoBehaviour
	{
		[SerializeField, Tooltip("Once the speech has finished, call this function")]
		UnityEvent finished;
		[SerializeField, Tooltip("Audio source to check")]
		AudioSource source;

		bool isPlaying;

		void Update()
		{
			if (source != null)
			{
				if (source.isPlaying)
				{
					isPlaying = true;
				}
				if (isPlaying && !source.isPlaying)
				{
					Debug.Log("stopped speaking");
					isPlaying = false;
					if(finished != null) finished.Invoke();
				}

			}
		}
	}
}
