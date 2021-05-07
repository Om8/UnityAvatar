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


public class WatsonVoiceIn : MonoBehaviour
{

	[Space(10)]
	[SerializeField, Tooltip("The service URL (optional). This defaults to \"https://api.us-south.speech-to-text.watson.cloud.ibm.com\"")]
	string _serviceUrl;
	[Header("IAM Authentication")]
	[SerializeField, Tooltip("The IAM apikey.")]
	string _iamApikey;

	[Header("Parameters")]
	// https://www.ibm.com/watson/developercloud/speech-to-text/api/v1/curl.html?curl#get-model
	[SerializeField, Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]
	string _recognizeModel;
	[SerializeField]
	public AudioEvent audioInput;
	[SerializeField]
	public UnityEvent startedInteracting;

	int _recordingRoutine = 0;
	string _microphoneID = null;
	AudioClip _recording = null;
	int _recordingBufferSize = 1;
	int _recordingHZ = 22050;

	SpeechToTextService _service;

	void Start()
	{
		LogSystem.InstallDefaultReactors();
		Runnable.Run(CreateService());
	}

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

	private void StartRecording()
	{
		if (_recordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			_recordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	private void StopRecording()
	{
		if (_recordingRoutine != 0)
		{
			Microphone.End(_microphoneID);
			Runnable.Stop(_recordingRoutine);
			_recordingRoutine = 0;
		}
	}

	private void OnError(string error)
	{
		Active = false;

		Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{
		Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
		_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
		yield return null;      // let _recordingRoutine get set..

		if (_recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		int midPoint = _recording.samples / 2;
		float[] samples = null;

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

	private void OnRecognize(SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				if (res.final)
				{
					foreach (var alt in res.alternatives)
					{
						Debug.Log(alt.transcript);
						audioInput.Invoke(alt.transcript);
						StopRecording();
					}
				}
			}
			startedInteracting.Invoke();
		}
	}

	private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
	{
	
	}

	public void CanSpeakAgain()
	{
		StartRecording();
		Debug.Log("Started listening again");
	}
}

