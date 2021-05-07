using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpeechEnded : MonoBehaviour
{
	[SerializeField]
	UnityEvent finished;
	[SerializeField]
	AudioSource source;

	bool isPlaying;

    void Update()
    {
     if(source != null)
		{
			if (source.isPlaying)
			{
				isPlaying = true;
			}
			if (isPlaying && !source.isPlaying) 
			{
				Debug.Log("stopped speaking");
				isPlaying = false;
				finished.Invoke();
			}

		}   
    }
}
