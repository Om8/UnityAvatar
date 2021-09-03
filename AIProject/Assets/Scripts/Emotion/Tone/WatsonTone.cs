/**
* (C) Copyright IBM Corp. 2019, 2020.
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

using IBM.Watson.ToneAnalyzer.V3;
using IBM.Watson.ToneAnalyzer.V3.Model;
using IBM.Cloud.SDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Text;
using System.IO;
using UnityEngine.Events;
using AI.Volume.Bot.Events;

namespace AI.Volume.Bot.Tone
{
	[System.Serializable]
	public class FaceEmotionJson
	{
		public float score;
		public string tone_id;
		public string tone_name;

		public static FaceEmotionJson CreateFromJson(string json)
		{
			return JsonUtility.FromJson<FaceEmotionJson>(json);
		}
	}

	public class WatsonTone : MonoBehaviour
	{
		#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
		[Space(10)]
		[Tooltip("The IAM apikey.")]
		[SerializeField]
		private string iamApikey;
		[Tooltip("The service URL (optional). This defaults to \"https://api.us-south.tone-analyzer.watson.cloud.ibm.com\"")]
		[SerializeField]
		private string serviceUrl;
		[Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
		[SerializeField]
		private string versionDate;
		#endregion

		private ToneAnalyzerService service;

		private bool ResponseGiven = false;

		[SerializeField]
		public AudioEvent responseTone;

		private List<FaceEmotionJson> listOfEmotions = new List<FaceEmotionJson>();

		private void Start()
		{
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());
		}

		//Create the audio service.
		private IEnumerator CreateService()
		{
			if (string.IsNullOrEmpty(iamApikey))
			{
				throw new IBMException("Plesae provide IAM ApiKey for the service.");
			}

			//  Create credential and instantiate service
			IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

			//  Wait for tokendata
			while (!authenticator.CanAuthenticate())
				yield return null;

			service = new ToneAnalyzerService(versionDate, authenticator);
			if (!string.IsNullOrEmpty(serviceUrl))
			{
				service.SetServiceUrl(serviceUrl);
			}
		}

		/// <summary>
		/// Get the current tone of a string
		/// </summary>
		/// <param name="phrase">string to get the tone of</param>
		public void GetTone(string phrase)
		{
			Runnable.Run(CheckForTone(phrase));
		}

		/// <summary>
		/// Check the string for the tone, but this time you wait for a response.
		/// </summary>
		/// <param name="phraseToCheck">String to check</param>
		/// <returns></returns>
		private IEnumerator CheckForTone(string phraseToCheck)
		{
			ResponseGiven = false;
			byte[] bytes = Encoding.ASCII.GetBytes(phraseToCheck);
			MemoryStream toneInput = new MemoryStream(bytes);

			List<string> tones = new List<string>()
			{
				"emotion",
				"language",
				"social"
			};
			service.Tone(callback: OnTone, toneInput: toneInput, sentences: false, tones: tones, contentLanguage: "en", acceptLanguage: "en", contentType: "text/plain");

			while (!ResponseGiven)
			{
				yield return null;
			}

		}

		//Just the tone returned.
		private void OnTone(DetailedResponse<ToneAnalysis> response, IBMError error)
		{
			if (error != null)
			{
				Log.Debug("ExampleToneAnalyzerV3.OnTone()", "Error: {0}: {1}", error.StatusCode, error.ErrorMessage);
			}
			else
			{
				//Pass the tone on
				if(responseTone != null) responseTone.Invoke(FindStrongestEmotion(response.Response));
			}

			ResponseGiven = true;
		}

		//Parse the tone into something readable and useable.
		private string FindStrongestEmotion(string input)
		{
			//Remove start and end.
			input = input.Replace("\"document_tone\":{\"tones\":[{", "");
			input = input.Replace("]}}", "");
			//Make sure it is long enough
			if (input.Length > 28)
			{
				//Split them all up
				string[] arrayOfJsons = input.Split(new string[] { "},{" }, System.StringSplitOptions.None);

				if (listOfEmotions != null)
				{
					listOfEmotions.Clear();


					for (int i = 0; i < arrayOfJsons.Length; i++)
					{
						if (arrayOfJsons[i][0] != '{') arrayOfJsons[i] = arrayOfJsons[i].Insert(0, "{");
						if (arrayOfJsons[i][arrayOfJsons[i].Length - 1] != '}') arrayOfJsons[i] = arrayOfJsons[i].Insert(arrayOfJsons[i].Length, "}");
						FaceEmotionJson emotion = new FaceEmotionJson();
						emotion = FaceEmotionJson.CreateFromJson(arrayOfJsons[i]);
						//Add the split data to a list
						listOfEmotions.Add(emotion);
					}

					//Get the highest scored value and return it.
					FaceEmotionJson finalResult = new FaceEmotionJson(); ;
					foreach (FaceEmotionJson inst in listOfEmotions)
					{
						if (inst.score > finalResult.score)
						{
							finalResult = inst;
						}
					}
					return finalResult.tone_name;
				}
			}
			return "Base";
		}
	}
}

