using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine.Networking;
using System.Threading;
using UnityEngine.Events;

public class AzureVoice : MonoBehaviour
{

	object threadLock = new object();
	[SerializeField, CannotBeNullObjectField, Tooltip("Audio source for voice output")]
	public AudioSource source = null;

	[SerializeField, Tooltip("Required, check the Azure voices page to get a list of voice names, keep the same format.")]
	string voiceName = "en-GB, RyanNeural";
	[SerializeField, Tooltip("Key found on Azure TTS webpage")]
	string key;
	[SerializeField, Tooltip("Region that TTS server is located, default is uksouth")]
	string region = "uksouth";

	[SerializeField, Tooltip("Calls at the end of the speech, used to reset voice input")]
	public UnityEvent finishedEvent;

	/// <summary>
	/// Converts a string to TTS audio, public starting function.
	/// </summary>
	/// <param name="words">Input string with what you want TTS to output</param>
	public void PlayAudio(string words)
	{
		StartCoroutine(AudioPlayer(words));
	}

	/// <summary>
	/// Audio player coroutine, used to call the Azure API and wait for a response
	/// </summary>
	/// <param name="words">String passed through from play audio function. Words you want TTS to say.</param>
	/// <returns></returns>
	IEnumerator AudioPlayer(string words)
	{
		SpeechConfig config = SpeechConfig.FromSubscription(key, region);
		config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (" + voiceName + ")";
		config.SpeechSynthesisLanguage = "en-GB";
		config.SetProperty("3201", "0");
		SpeechSynthesisOutputFormat format = SpeechSynthesisOutputFormat.Riff48Khz16BitMonoPcm;
		config.SetSpeechSynthesisOutputFormat(format);


		SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, null);
		SpeechSynthesisResult result = null;
		bool done = false;
		new Thread(() =>
		{
			result = synthesizer.SpeakTextAsync(words).Result;
			done = true;
		}).Start();

		//Wait for a response.
		while (!done)
		{
			yield return null;
		}

		if (result.Reason == ResultReason.Canceled)
		{
			SpeechSynthesisCancellationDetails cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
			Debug.Log(cancellation.Reason + " || " + cancellation.ErrorDetails + " || " + cancellation.ErrorCode);
		}
		if (result.Reason == ResultReason.SynthesizingAudioCompleted)
		{
			//Play the clip and then wait for the lenght of the clip to then call the finished event.
			source.clip = OpenWavParser.ByteArrayToAudioClip(result.AudioData, "SynthesizedAudio");
			source.Play();
			yield return new WaitForSeconds(source.clip.length);
			finishedEvent.Invoke();
		}
	}
}
