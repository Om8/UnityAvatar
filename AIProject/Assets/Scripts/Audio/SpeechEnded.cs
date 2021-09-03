using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AI.Volume.Bot.Audio
{
	public class SpeechEnded : MonoBehaviour
	{
		[SerializeField, Tooltip("Once the speech has finished, call this function")]
		private UnityEvent finished;
		[SerializeField, Tooltip("Audio source to check")]
		private AudioSource source;

		bool isPlaying;

		//Calls every frame.
		void Update()
		{
			if (source != null)
			{
				//If the source is playing, set the bool to true.
				if (source.isPlaying)
				{
					isPlaying = true;
				}
				//If the source is not playing, the AI has stopped talking.
				if (isPlaying && !source.isPlaying)
				{
					isPlaying = false;
					if(finished != null) finished.Invoke();
				}

			}
		}
	}
}
