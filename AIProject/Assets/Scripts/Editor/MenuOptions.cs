using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class MenuOptions
{

	//Set up main avatar body
	[MenuItem("VolumeAI/Generate Body Components", false, 1)]
	public static void FaceSetup()
	{
		GameObject mainParentBody = (Selection.activeObject) as GameObject;
		if (mainParentBody != null)
		{
			mainParentBody.AddComponent<Animator>();
			mainParentBody.AddComponent<HeadRotator>();
			mainParentBody.AddComponent<FacePositionList>();
			mainParentBody.AddComponent<SetFaceShapes>();
			mainParentBody.AddComponent<SetBody>();
			mainParentBody.GetComponent<SetBody>().animationController = mainParentBody.GetComponent<Animator>();
			mainParentBody.GetComponent<SetFaceShapes>().mesh = mainParentBody.transform.GetComponentInChildren<SkinnedMeshRenderer>();
			mainParentBody.GetComponent<SetFaceShapes>().list = mainParentBody.GetComponent<FacePositionList>();
			mainParentBody.AddComponent<CheckForQuestion>();
			mainParentBody.AddComponent<WatsonTone>();
			mainParentBody.AddComponent<Blink>();

		}
	}
	[MenuItem("VolumeAI/Generate Body Components", true, 1)]
	public static bool FaceSetupValidation()
	{
		return Selection.activeObject != null;
	}

	//Voice out spawning
	[MenuItem("VolumeAI/Voice Out/Generate Watson", false, 2)]
	public static void SpawnWatsonVoiceOut()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "WatsonVoiceOutput+OVR";
		AudioSource source = spawnedObject.AddComponent<AudioSource>();
		spawnedObject.AddComponent<OVRLipSyncContext>().audioSource = source;
		OVRLipSyncContextMorphTarget morphTarget = spawnedObject.AddComponent<OVRLipSyncContextMorphTarget>();
		morphTarget.listOfMouthPositions = GameObject.FindObjectOfType<FacePositionList>();
		spawnedObject.AddComponent<WatsonOut>().source = source;
		spawnedObject.AddComponent<HeadBob>();
	}

	[MenuItem("VolumeAI/Voice Out/Generate Azure", false, 3)]
	public static void SpawnAzureVoiceOut()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "AzureVoiceOutput+OVR";
		AudioSource source = spawnedObject.AddComponent<AudioSource>();
		spawnedObject.AddComponent<OVRLipSyncContext>().audioSource = source;
		OVRLipSyncContextMorphTarget morphTarget = spawnedObject.AddComponent<OVRLipSyncContextMorphTarget>();
		morphTarget.listOfMouthPositions = GameObject.FindObjectOfType<FacePositionList>();
		spawnedObject.AddComponent<AzureVoice>().source = source;
		spawnedObject.AddComponent<HeadBob>();
	}

	//Spawn Voice in
	[MenuItem("VolumeAI/Voice In/Generate Watson", false, 4)]
	public static void SpawnWatsonVoiceIn()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Watson Voice Input";
		spawnedObject.AddComponent<WatsonVoiceIn>();
	}

	[MenuItem("VolumeAI/Voice In/Generate Windows", false, 5)]
	public static void SpawnWindowsVoiceIn()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Windows Voice Input";
		spawnedObject.AddComponent<WindowsVoiceInput>();
	}

	//Understanding/AI dialogue connection

	[MenuItem("VolumeAI/Understanding/Generate Watson", false, 6)]
	public static void SpawnWatsonUnderstanding()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Watson Understanding";
		spawnedObject.AddComponent<WatsonUnderstanding>();

	}
	[MenuItem("VolumeAI/Understanding/Generate Volume (Unavailable)", false, 7)]
	public static void SpawnVolumeUnderstanding()
	{
		//Add spawning of object here.
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Volume Understanding (Not Available)";
	}

	//Tone
	[MenuItem("VolumeAI/Tone/Generate Watson", false, 8)]
	public static void SpawnWatsonTone()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Watson Tone Analyser";
		spawnedObject.AddComponent<WatsonTone>();
	}

	//Local responses
	[MenuItem("VolumeAI/Local Responses/Generate Local Response Object", false, 9)]
	public static void SpawnLocalResponse()
	{
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Local Responses";
		spawnedObject.AddComponent<CheckForQuestion>();
	}

	//API and Wiki
	[MenuItem("VolumeAI/Documentation/Wiki", false, 51)]
	public static void OpenWiki()
	{
		Application.OpenURL("https://github.com/Om8/VolumeAIBot/wiki");
	}

	[MenuItem("VolumeAI/Documentation/Watson Voices", false, 52)]
	public static void OpenWatsonVoices()
	{
		Application.OpenURL("https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices");
	}
	[MenuItem("VolumeAI/Documentation/Azure Voices", false, 53)]
	public static void OpenAzureVoices()
	{
		Application.OpenURL("https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support");
	}

	//Autofill events
	[MenuItem("VolumeAI/Auto fill events", false, 10)]
	public static void AutoFillEvents()
	{
		bool preferAzureVoiceOutput = true;
		bool preferWindowsVoiceIn = true;

		WindowsVoiceInput windowsInputRef = GameObject.FindObjectOfType<WindowsVoiceInput>();
		WatsonVoiceIn watsonInputRef = GameObject.FindObjectOfType<WatsonVoiceIn>();
		WatsonTone toneRef = GameObject.FindObjectOfType<WatsonTone>();
		WatsonOut watsonOutRef = GameObject.FindObjectOfType<WatsonOut>();
		AzureVoice azureVoiceOutRef = GameObject.FindObjectOfType<AzureVoice>();
		WatsonUnderstanding understandingRef = GameObject.FindObjectOfType<WatsonUnderstanding>();
		SkinnedMeshRenderer meshRef = GameObject.FindObjectOfType<SkinnedMeshRenderer>();
		FacePositionList faceListRef = GameObject.FindObjectOfType<FacePositionList>();
		HeadRotator headRotatorRef = GameObject.FindObjectOfType<HeadRotator>();
		SetFaceShapes faceShapeRef = GameObject.FindObjectOfType<SetFaceShapes>();
		SetBody setBodyRef = GameObject.FindObjectOfType<SetBody>();
		CheckForQuestion localResponseRef = GameObject.FindObjectOfType<CheckForQuestion>();

		//Voice Input
		if (preferWindowsVoiceIn)
		{
			if(windowsInputRef != null && headRotatorRef != null && understandingRef != null && toneRef != null && faceShapeRef != null)
			{
				windowsInputRef.spokenTo.RemoveAllListeners();
				UnityEventTools.AddPersistentListener(windowsInputRef.spokenTo, understandingRef.InputMessage);
				UnityEventTools.AddPersistentListener(windowsInputRef.spokenTo, toneRef.GetTone);
				UnityEventTools.AddStringPersistentListener(windowsInputRef.spokenTo, faceShapeRef.SetEmotion, "Think");
				UnityEventTools.AddPersistentListener(windowsInputRef.spokenTo, localResponseRef.QuestionChecker);

				UnityEventTools.AddPersistentListener(windowsInputRef.hasInteracted, headRotatorRef.StartedInteracting);
			}
			else
			{
				Debug.LogError("Missing components | Windows Voice In");
				if (watsonInputRef != null && headRotatorRef != null && understandingRef != null && toneRef != null && faceShapeRef != null)
				{
					UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, understandingRef.InputMessage);
					UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, toneRef.GetTone);
					UnityEventTools.AddStringPersistentListener(watsonInputRef.audioInput, faceShapeRef.SetEmotion, "Think");
					UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, localResponseRef.QuestionChecker);

					UnityEventTools.AddPersistentListener(watsonInputRef.startedInteracting, headRotatorRef.StartedInteracting);
				}
				else
				{
					Debug.LogError("Missing components | Watson Voice In | NO VOICE INPUT IN SCENE");
				}
			}
		}
		else
		{
			if (watsonInputRef != null && headRotatorRef != null && understandingRef != null && toneRef != null && faceShapeRef != null)
			{
				UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, understandingRef.InputMessage);
				UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, toneRef.GetTone);
				UnityEventTools.AddStringPersistentListener(watsonInputRef.audioInput, faceShapeRef.SetEmotion, "Think");
				UnityEventTools.AddPersistentListener(watsonInputRef.audioInput, localResponseRef.QuestionChecker);

				UnityEventTools.AddPersistentListener(watsonInputRef.startedInteracting, headRotatorRef.StartedInteracting);
			}
			else
			{
				Debug.LogError("Missing components | Watson Voice In");
			}
		}

		//Understanding to output
		if (preferAzureVoiceOutput)
		{
			if (understandingRef != null && azureVoiceOutRef != null)
			{
				UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, azureVoiceOutRef.PlayAudio);
			}
			else
			{
				Debug.LogError("Missing components | Azure Voice Out");
			}
		}
		else
		{
			if (understandingRef != null && watsonOutRef != null)
			{ 
				UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, watsonOutRef.InputText);
			}
			else
			{
				Debug.LogError("Missing components | Watson Voice Out");
			}
		}
		if(understandingRef != null && toneRef != null && headRotatorRef != null)
		{
			UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, toneRef.GetTone);
			UnityEventTools.AddVoidPersistentListener(understandingRef.returnedMessage, headRotatorRef.StartedInteracting);
		}

		//Allow user to speak to bot again.
		if (preferAzureVoiceOutput)
		{
			if (azureVoiceOutRef != null && faceShapeRef != null && setBodyRef != null)
			{
				UnityEventTools.AddPersistentListener(azureVoiceOutRef.finishedEvent, faceShapeRef.FinishedSpeaking);
				UnityEventTools.AddStringPersistentListener(azureVoiceOutRef.finishedEvent, setBodyRef.SetBodyEmotion, "Base");
				if (preferWindowsVoiceIn && windowsInputRef != null)
				{
					UnityEventTools.AddPersistentListener(azureVoiceOutRef.finishedEvent, windowsInputRef.CanSpeakAgain);
				}
				else if (watsonInputRef != null)
				{
					UnityEventTools.AddPersistentListener(azureVoiceOutRef.finishedEvent, watsonInputRef.CanSpeakAgain);
				}
				else
				{
					Debug.LogError("No voice input to return to from voice output");
				}
			}
		}
		else
		{
			if (watsonOutRef != null && faceShapeRef != null && setBodyRef != null)
			{
				UnityEventTools.AddPersistentListener(watsonOutRef.finishedEvent, faceShapeRef.FinishedSpeaking);
				UnityEventTools.AddStringPersistentListener(watsonOutRef.finishedEvent, setBodyRef.SetBodyEmotion, "Base");
				if (preferWindowsVoiceIn && windowsInputRef != null)
				{
					UnityEventTools.AddPersistentListener(watsonOutRef.finishedEvent, windowsInputRef.CanSpeakAgain);
				}
				else if (watsonInputRef != null)
				{
					UnityEventTools.AddPersistentListener(watsonOutRef.finishedEvent, watsonInputRef.CanSpeakAgain);
				}
				else
				{
					Debug.LogError("No voice input to return to from voice output");
				}
			}
		}

		//Tone checker
		if(toneRef != null && faceShapeRef != null && setBodyRef != null)
		{
			UnityEventTools.AddPersistentListener(toneRef.responseTone, faceShapeRef.SetEmotion);
			UnityEventTools.AddPersistentListener(toneRef.responseTone, setBodyRef.SetBodyEmotion);
		}
	}


}
