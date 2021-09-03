using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.SpeechToText.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;
using UnityEngine.UI;
using UnityEngine.Events;
using AI.Volume.Bot.Events;

namespace AI.Volume.Bot.Audio
{
	[RequireComponent(typeof(SpatialChecker))]
	public class WatsonVoiceIn : MonoBehaviour
	{

		[Space(10)]
		[SerializeField, Tooltip("The service URL (optional). This defaults to \"https://api.us-south.speech-to-text.watson.cloud.ibm.com\"")]
		private string _serviceUrl;
		[Header("IAM Authentication")]
		[SerializeField, Tooltip("The IAM apikey.")]
		private string _iamApikey;

		[Header("Parameters")]
		// https://www.ibm.com/watson/developercloud/speech-to-text/api/v1/curl.html?curl#get-model
		[SerializeField, Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]//This is a private variable because of the way it is.
		private string _recognizeModel;
		public AudioEvent audioInput;
		public UnityEvent startedInteracting;

		private int _recordingRoutine = 0;
		private string _microphoneID = null;
		private AudioClip _recording = null;
		private int _recordingBufferSize = 1;
		private int _recordingHZ = 22050;

		//Spatial checker, checks if two objects are within range and within eye sight.
		[SerializeField]
		private SpatialChecker spatialChecker => this.gameObject.GetComponent<SpatialChecker>();
		[SerializeField, Tooltip("Does this AI have a range where outside that range, they won't respond.")]
		private bool spatial = false;
		[SerializeField, Tooltip("The max distance for AI communication")]
		private float spatialRange = 2;

		private SpeechToTextService _service;

		//Calls on start
		void Start()
		{
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());
		}


		/// <summary>
		/// Creates the audio input service.
		/// </summary>
		/// <returns></returns>
		private IEnumerator CreateService()
		{
			if (string.IsNullOrEmpty(_iamApikey))
			{
				throw new IBMException("Plesae provide IAM ApiKey for the service.");
			}

			IamAuthenticator authenticator = new IamAuthenticator(apikey: _iamApikey);

			//  Wait for tokendata
			while (!authenticator.CanAuthenticate())
				yield return null;

			_service = new SpeechToTextService(authenticator);
			if (!string.IsNullOrEmpty(_serviceUrl))
			{
				_service.SetServiceUrl(_serviceUrl);
			}
			_service.StreamMultipart = true;

			Active = true;
			Debug.Log("Started Recording");
			StartRecording();
			yield return null;
		}

		
		public bool Active {
			get { return _service.IsListening; }
			set {
				if (value && !_service.IsListening)
				{
					_service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-GB_BroadbandModel" : _recognizeModel);
					_service.DetectSilence = true;
					_service.EnableWordConfidence = false;
					_service.EnableTimestamps = false;
					_service.SilenceThreshold = 0.01f;
					_service.MaxAlternatives = 1;
					_service.EnableInterimResults = true;
					_service.OnError = OnError;
					_service.InactivityTimeout = -1;
					_service.ProfanityFilter = false;
					_service.SmartFormatting = true;
					_service.SpeakerLabels = false;
					_service.WordAlternativesThreshold = null;
					_service.EndOfPhraseSilenceTime = null;
					_service.StartListening(OnRecognize, OnRecognizeSpeaker);
				}
				else if (!value && _service.IsListening)
				{
					_service.StopListening();
				}
			}
		}

		//Start the recordings
		private void StartRecording()
		{
			if (_recordingRoutine == 0)
			{
				UnityObjectUtil.StartDestroyQueue();
				_recordingRoutine = Runnable.Run(RecordingHandler());
			}
		}

		//Stop the recording.
		private void StopRecording()
		{
			if (_recordingRoutine != 0)
			{
				Microphone.End(_microphoneID);
				Runnable.Stop(_recordingRoutine);
				_recordingRoutine = 0;
			}
		}

		//There may be an error.
		private void OnError(string error)
		{
			Active = false;

			Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
		}

		//Handles the audio input.
		private IEnumerator RecordingHandler()
		{
			Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
			_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
			yield return null;      // let _recordingRoutine get set..

			//If it ain't recording, stop recording.
			if (_recording == null)
			{
				StopRecording();
				yield break;
			}

			bool bFirstBlock = true;
			int midPoint = _recording.samples / 2;
			float[] samples = null;

			//Watson SDK code.
			while (_recordingRoutine != 0 && _recording != null)
			{
				int writePos = Microphone.GetPosition(_microphoneID);
				if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
				{
					StopRecording();
					yield break;
				}

				if ((bFirstBlock && writePos >= midPoint) || (!bFirstBlock && writePos < midPoint))
				{
					// front block is recorded, make a RecordClip and pass it onto our callback.
					samples = new float[midPoint];
					_recording.GetData(samples, bFirstBlock ? 0 : midPoint);

					AudioData record = new AudioData();
					record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
					record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
					record.Clip.SetData(samples, 0);

					_service.OnListen(record);

					bFirstBlock = !bFirstBlock;
				}
				else
				{
					// calculate the number of samples remaining until we ready for a block of audio, 
					// and wait that amount of time it will take to record.
					int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
					float timeRemaining = (float)remaining / (float)_recordingHZ;

					yield return new WaitForSeconds(timeRemaining);
				}
			}
			yield break;
		}

		/// <summary>
		/// When Watson gives a resonse, it comes through here.
		/// </summary>
		/// <param name="result"></param>
		private void OnRecognize(SpeechRecognitionEvent result)
		{
			if (spatial)
			{
				if (spatialChecker != null)
				{
					if (spatialChecker.IsInRange(this.gameObject, Camera.main.gameObject, spatialRange) && spatialChecker.LineOfSightChecker(this.gameObject, Camera.main.gameObject))
					{
						if (result != null && result.results.Length > 0)
						{
							foreach (var res in result.results)
							{
								if (res.final)
								{
									foreach (var alt in res.alternatives)
									{
										//Call the spoken to function.
										if (audioInput != null)
										{
											audioInput.Invoke(alt.transcript);
											StopRecording();
										}
									}
								}
							}
							if (startedInteracting != null)
							{
								startedInteracting.Invoke();
							}
						}
					}
				}
			}
			else
			{
				if (result != null && result.results.Length > 0)
				{
					foreach (var res in result.results)
					{
						if (res.final)
						{
							foreach (var alt in res.alternatives)
							{
								//Call the spoken to function.
								if (audioInput != null)
								{
									audioInput.Invoke(alt.transcript);
									StopRecording();
								}
							}
						}
					}
					if (startedInteracting != null)
					{
						startedInteracting.Invoke();
					}
				}
			}
		}

		private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
		{

		}

		/// <summary>
		/// Call to reset the audio, allows voice input again.
		/// </summary>
		public void CanSpeakAgain()
		{
			StartRecording();
		}
	}
}

