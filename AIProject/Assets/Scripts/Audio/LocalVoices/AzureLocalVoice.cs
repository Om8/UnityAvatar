using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-text-to-speech?tabs=script%2Cwindowsinstall&pivots=programming-language-csharp
public class AzureLocalVoice : MonoBehaviour
{

	public static async Task CreateAudioFile(string text, string fileName)
	{
		await SynthesiseAudio(text, fileName);
	}

	static async Task SynthesiseAudio(string textToSpeech, string fileName)
	{
		SpeechConfig config = SpeechConfig.FromSubscription("5a943fdec7834e179c76cda059aa689c", "uksouth");
		config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (en-GB, RyanNeural)";
		config.SpeechSynthesisLanguage = "en-GB";
		SpeechSynthesisOutputFormat format = SpeechSynthesisOutputFormat.Riff48Khz16BitMonoPcm;
		config.SetSpeechSynthesisOutputFormat(format);
		Debug.Log(Application.dataPath);
		using (AudioConfig audioConfig = AudioConfig.FromWavFileOutput(Application.dataPath + "/LocalAudio/" + fileName + ".wav"))
		{
			using (SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig))
			{
				await synthesizer.SpeakTextAsync(textToSpeech);
			}
		}
	}
}
