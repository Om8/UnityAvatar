using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using UnityEngine.Events;

public class WindowsVoiceInput : MonoBehaviour
{

	DictationRecognizer voiceDictation;
	[SerializeField]
	public UnityEvent hasInteracted;
	[SerializeField]
	public AudioEvent spokenTo;
	

	// Start is called before the first frame update
	void Start()
    {
		voiceDictation = new DictationRecognizer();


		voiceDictation.DictationHypothesis += (text) =>
		{
			hasInteracted.Invoke();
		};

		voiceDictation.DictationResult += DictationResult;
		voiceDictation.DictationError += DictationError;

		voiceDictation.Start();
		voiceDictation.AutoSilenceTimeoutSeconds = 20000;
		voiceDictation.InitialSilenceTimeoutSeconds = 20000;
	}

	void DictationResult(string text, ConfidenceLevel confidence)
	{
		spokenTo.Invoke(text);
		voiceDictation.Stop();
		//voiceDictation.Dispose();
	}

	public void CanSpeakAgain()
	{
		if (voiceDictation.Status != SpeechSystemStatus.Running)
		{
			voiceDictation.Start();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (voiceDictation != null)
		{
			if (voiceDictation.Status != SpeechSystemStatus.Running)
			{
				voiceDictation.Start();
			}
		}
	}

	private void OnDestroy()
	{
		if (voiceDictation.Status == SpeechSystemStatus.Running)
		{
			voiceDictation.Stop();
			voiceDictation.Dispose();
		}
	}

	void DictationError(string error, int result)
	{
		Debug.Log(error + " || number: " + result);
	}
}
