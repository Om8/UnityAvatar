/**
* Copyright 2020 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK;
using UnityEngine.Events;

namespace AI.Volume.Bot.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class WatsonOut : MonoBehaviour
	{
		#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
		[Space(10)]
		[Tooltip("The IAM apikey.")]
		[SerializeField]
		private string iamApikey;
		[Tooltip("The service URL (optional). This defaults to \"https://api.us-south.text-to-speech.watson.cloud.ibm.com\"")]
		[SerializeField]
		private string serviceUrl;
		private TextToSpeechService service;
		[SerializeField]
		string allisionVoice = "en-US_AllisonV3Voice";
		private string synthesizeMimeType = "audio/wav";
		#endregion

		[SerializeField, Tooltip("The audio source for the audio output")]
		public AudioSource source;

		[SerializeField, Tooltip("Finished talking, do stuff here")]
		public UnityEvent finishedEvent;

		private void Start()
		{
			if (source != null)
			{
				source = this.GetComponent<AudioSource>();
			}
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());
		}


		private IEnumerator CreateService()
		{
			if (string.IsNullOrEmpty(iamApikey))
			{
				throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
			}

			IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

			while (!authenticator.CanAuthenticate())
			{
				yield return null;
			}

			service = new TextToSpeechService(authenticator);
			if (!string.IsNullOrEmpty(serviceUrl))
			{
				service.SetServiceUrl(serviceUrl);
			}
		}

		#region Synthesize Example
		private IEnumerator SynthesiseAudio(string text)
		{
			byte[] synthesizeResponse = null;
			AudioClip clip = null;
			service.Synthesize(
				callback: (DetailedResponse<byte[]> response, IBMError error) =>
				{
					synthesizeResponse = response.Result;
					clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
					PlayClip(clip);
				},
				text: text,
				voice: allisionVoice,
				accept: synthesizeMimeType
			);

			while (synthesizeResponse == null)
				yield return null;

			yield return new WaitForSeconds(clip.length);
			if(finishedEvent != null) finishedEvent.Invoke();
		}
		#endregion

		#region PlayClip
		/// <summary>
		/// Play audio clip
		/// </summary>
		/// <param name="clip">clip to play</param>
		private void PlayClip(AudioClip clip)
		{
			if (Application.isPlaying && clip != null)
			{
				source.spatialBlend = 0.0f;
				source.loop = false;
				source.clip = clip;
				source.Play();
			}
		}
		#endregion


		/// <summary>
		/// Input text to convert to speech.
		/// </summary>
		/// <param name="words">The text to turn to speech</param>
		public void InputText(string words)
		{
			Runnable.Run(SynthesiseAudio(words));
		}
	}
}

