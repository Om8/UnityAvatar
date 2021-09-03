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

namespace AI.Volume.Bot.Audio
{
	public class AzureVoice : MonoBehaviour
	{
		//Variables, these are a mix of public, private and serialised variables. The user can set these in the Unity inspector. 
		private object threadLock = new object();
		[CannotBeNullObjectField, Tooltip("Audio source for voice output")]
		public AudioSource source = null;

		[SerializeField, Tooltip("Required, check the Azure voices page to get a list of voice names, keep the same format.")]
		private string voiceName = "en-GB, RyanNeural";
		[SerializeField, Tooltip("Key found on Azure TTS webpage")]
		private string key;
		[SerializeField, Tooltip("Region that TTS server is located, default is uksouth")]
		private string region = "uksouth";

		[Tooltip("Calls at the end of the speech, used to reset voice input")]
		public UnityEvent finishedEvent;

		/// <summary>
		/// Converts a string to TTS audio, public starting function.
		/// </summary>
		/// <param name="words">Input string with what you want TTS to output</param>
		public void PlayAudio(string words)
		{
			//Starts the coroutine for the voice
			StartCoroutine(AudioPlayer(words));
		}

		/// <summary>
		/// Audio player coroutine, used to call the Azure API and wait for a response
		/// </summary>
		/// <param name="words">String passed through from play audio function. Words you want TTS to say.</param>
		/// <returns></returns>
		private IEnumerator AudioPlayer(string words)
		{
			//Create a speech config from a subscription, handing in a key and a region.
			SpeechConfig config = SpeechConfig.FromSubscription(key, region);
			//Set the voice and language, sort of set to GB, so set this to whatever language you want.
			config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (" + voiceName + ")";
			config.SpeechSynthesisLanguage = "en-GB";
			config.SetProperty("3201", "0");
			SpeechSynthesisOutputFormat format = SpeechSynthesisOutputFormat.Riff48Khz16BitMonoPcm;
			//Set the config to the format we want, which is above.
			config.SetSpeechSynthesisOutputFormat(format);

			//Creates a new synthesiser with the config
			SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, null);
			//Set the result to null.
			SpeechSynthesisResult result = null;
			//It is not done.
			bool done = false;
			//Create a new thread to sythesise the voice.
			new Thread(() =>
			{
				//Set the result to the synthesiser result
				result = synthesizer.SpeakTextAsync(words).Result;
				//once this has stopped freezing, set this to true.
				done = true;
				//Once this is true, the voice is done and the thread can end.
			}).Start();

			//Wait for a response. This loop is holding the thread here while it completes, stops Unity from going mental and freezing.
			while (!done)
			{
				//Just continue your day as normal.
				yield return null;
			}

			//The reason was that it was canceled. 
			if (result.Reason == ResultReason.Canceled)
			{
				//Set the cancellation details to the details of the cancellation.
				SpeechSynthesisCancellationDetails cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
				//Print it out so people can see it and maybe even debug it.
				Debug.Log(cancellation.Reason + " || " + cancellation.ErrorDetails + " || " + cancellation.ErrorCode);
			}
			//It was copmplete, time to output.
			if (result.Reason == ResultReason.SynthesizingAudioCompleted)
			{
				//Play the clip and then wait for the lenght of the clip to then call the finished event.
				source.clip = OpenWavParser.ByteArrayToAudioClip(result.AudioData, "SynthesizedAudio");
				//Play said audio.
				source.Play();
				//Wait for said audios length
				yield return new WaitForSeconds(source.clip.length);
				//Has finished speaking.
				if(finishedEvent != null) finishedEvent.Invoke();
			}
		}
	}
}