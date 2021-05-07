using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckForQuestion : MonoBehaviour
{
	[SerializeField, Tooltip("Different variations of sounds that can be played if the conditions are met")]
	AudioClip[] variations;
	[SerializeField, Tooltip("OVR audio source to play, make sure audio source is on the same object as an OVR morph target")]
	AudioSource OVRSource;

	/// <summary>
	/// Check whether a sentence has a question word in it, What, Where, When, Why, Who and How
	/// </summary>
	/// <param name="text">Text to input and check</param>
	public void QuestionChecker(string text)
	{
		text = text.ToLower();
		if(text.Contains("what") || text.Contains("where") || text.Contains("when") || text.Contains("why") || text.Contains("who") || text.Contains("how"))
		{
			if(OVRSource != null)
			{
				if(variations.Length > 0)
				{
					OVRSource.clip = variations[Random.Range(0, variations.Length)];
					OVRSource.Play();
				}
			}
		}
	}
}
