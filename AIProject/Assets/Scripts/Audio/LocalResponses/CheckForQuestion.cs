using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AI.Volume.Bot.Audio
{
	public class CheckForQuestion : MonoBehaviour
	{
		[SerializeField, Tooltip("Different variations of sounds that can be played if the conditions are met")]
		private AudioClip[] variations;
		[SerializeField, Tooltip("OVR audio source to play, make sure audio source is on the same object as an OVR morph target")]
		private AudioSource OVRSource;

		/// <summary>
		/// Check whether a sentence has a question word in it, What, Where, When, Why, Who and How
		/// </summary>
		/// <param name="text">Text to input and check</param>
		public void QuestionChecker(string text)
		{
			text = text.ToLower();
			//These are the words it checks for to do local responses
			if (text.Contains("what") || text.Contains("where") || text.Contains("when") || text.Contains("why") || text.Contains("who") || text.Contains("how"))
			{
				if (OVRSource != null)
				{
					if (variations.Length > 0)
					{
						//Set the audio clip to a variation and play.
						OVRSource.clip = variations[Random.Range(0, variations.Length)];
						OVRSource.Play();
					}
				}
			}
		}
	}
}
