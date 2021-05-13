using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Audio
{
	//https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-text-to-speech?tabs=script%2Cwindowsinstall&pivots=programming-language-csharp
	public class AzureLocalVoice : MonoBehaviour
	{

		/// <summary>
		/// Public function that calls the creation function.
		/// </summary>
		/// <param name="text">Text you want to turn into speech</param>
		/// <param name="fileName">File name, just that.</param>
		/// <returns></returns>
		public static async Task CreateAudioFile(string text, string fileName)
		{
			await SynthesiseAudio(text, fileName);
		}

		/// <summary>
		/// Create the audio for the local response
		/// </summary>
		/// <param name="textToSpeech">Text to convert to speech</param>
		/// <param name="fileName">Name of the file to save</param>
		/// <returns></returns>
		static async Task SynthesiseAudio(string textToSpeech, string fileName)
		{
			//Use your own API key, this key will most likely be deactivated soon.
			SpeechConfig config = SpeechConfig.FromSubscription("5a943fdec7834e179c76cda059aa689c", "uksouth");
			//Change the voice name here to change voice
			config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (en-GB, RyanNeural)";
			config.SpeechSynthesisLanguage = "en-GB";
			//The format of the audio.
			SpeechSynthesisOutputFormat format = SpeechSynthesisOutputFormat.Riff48Khz16BitMonoPcm;
			//Set the audio format.
			config.SetSpeechSynthesisOutputFormat(format);
			Debug.Log(Application.dataPath);
			//Save the file to the local audio file in the project folder.
			using (AudioConfig audioConfig = AudioConfig.FromWavFileOutput(Application.dataPath + "/LocalAudio/" + fileName + ".wav"))
			{
				using (SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig))
				{
					await synthesizer.SpeakTextAsync(textToSpeech);
				}
			}
		}
	}
}
