using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;
using AI.Volume.Bot.Audio;
using AI.Volume.Bot.Tone;
using AI.Volume.Bot.Visual;
using AI.Volume.Bot.Understanding;

public class MenuOptions
{

	//Set up main avatar body
	[MenuItem("VolumeAI/Generate Body Components", false, 1)]
	public static void FaceSetup()
	{
		GameObject mainParentBody = (Selection.activeObject) as GameObject;
		if (mainParentBody != null)
		{
			//Add all of the components
			mainParentBody.AddComponent<Animator>();
			mainParentBody.AddComponent<HeadRotator>();
			mainParentBody.AddComponent<FacePositionList>();
			mainParentBody.AddComponent<SetFaceShapes>();
			mainParentBody.AddComponent<SetBody>();
			mainParentBody.GetComponent<SetBody>().animationController = mainParentBody.GetComponent<Animator>();
			mainParentBody.GetComponent<SetFaceShapes>().mesh = mainParentBody.transform.GetComponentInChildren<SkinnedMeshRenderer>();
			mainParentBody.GetComponent<SetFaceShapes>().list = mainParentBody.GetComponent<FacePositionList>();
			mainParentBody.AddComponent<CheckForQuestion>();
			mainParentBody.AddComponent<Blink>();

		}
	}
	//Only enable the menu option if this is valid.
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

	//Azure voice out spawning
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

	//Spawn Windows voice in
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
	//Spawn in the Volume understanding
	[MenuItem("VolumeAI/Understanding/Generate Volume (Unavailable)", false, 7)]
	public static void SpawnVolumeUnderstanding()
	{
		//Add spawning of object here.
		GameObject spawnedObject = new GameObject();
		spawnedObject.name = "Volume Understanding (Not Available)";
	}

	//Tone spawning
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

	//All of the voices that watson have to offer
	[MenuItem("VolumeAI/Documentation/Watson Voices", false, 52)]
	public static void OpenWatsonVoices()
	{
		Application.OpenURL("https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices");
	}
	//All of the voices that azure has to offer
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

		//Find all objects in scene
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
				Debug.LogError("Missing components -> Trying Watson");
				if (windowsInputRef == null) Debug.LogError("Missing (Windows Input)");
				if (headRotatorRef == null) Debug.LogError("Missing (Head Rotator script)");
				if (understandingRef == null) Debug.LogError("Missing (Watson Understanding)");
				if (toneRef == null) Debug.LogError("Missing (Watson Tone)");
				if (faceShapeRef == null) Debug.LogError("Missing (Face Shape script)");


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
					Debug.LogError("FAILED TO LINK WATSON. MAKE SURE TO SPAWN IN ALL REQUIRED COMPONENTS");
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
				Debug.LogError("Missing components -> Trying Windows");
				if (watsonInputRef == null) Debug.LogError("Missing (Watson Input)");
				if (headRotatorRef == null) Debug.LogError("Missing (Head Rotator script)");
				if (understandingRef == null) Debug.LogError("Missing (Watson Understanding)");
				if (toneRef == null) Debug.LogError("Missing (Watson Tone)");
				if (faceShapeRef == null) Debug.LogError("Missing (Face Shape script)");
				if (windowsInputRef != null && headRotatorRef != null && understandingRef != null && toneRef != null && faceShapeRef != null)
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
					Debug.LogError("FAILED TO LINK WINDOWS. MAKE SURE TO SPAWN IN ALL REQUIRED COMPONENTS");
				}
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
				Debug.LogError("Missing components -> Trying Watson");
				if (understandingRef == null) Debug.LogError("Missing (Watson Understanding)");
				if (azureVoiceOutRef == null) Debug.LogError("Missing (Azure Voice Out)");

				if (understandingRef != null && watsonOutRef != null)
				{
					UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, watsonOutRef.InputText);
				}
				else
				{
					Debug.LogError("FAILED TO LINK WINDOWS. MAKE SURE TO SPAWN IN ALL REQUIRED COMPONENTS");
				}
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
				Debug.LogError("Missing components -> Trying Azure");
				if (understandingRef == null) Debug.LogError("Missing (Watson Understanding)");
				if (watsonOutRef == null) Debug.LogError("Missing (Watson Voice Out)");
				if (understandingRef != null && azureVoiceOutRef != null)
				{
					UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, azureVoiceOutRef.PlayAudio);
				}
				else
				{
					Debug.LogError("FAILED TO LINK WINDOWS. MAKE SURE TO SPAWN IN ALL REQUIRED COMPONENTS");
				}
			}
		}
		if(understandingRef != null && toneRef != null && headRotatorRef != null)
		{
			UnityEventTools.AddPersistentListener(understandingRef.returnedMessage, toneRef.GetTone);
			UnityEventTools.AddVoidPersistentListener(understandingRef.returnedMessage, headRotatorRef.StartedInteracting);
		}
		else
		{
			Debug.LogError("Missing components when trying to connect understanding");
			if (understandingRef == null) Debug.LogError("Missing (Watson Understanding)");
			if (toneRef == null) Debug.LogError("Missing (Watson Tone)");
			if (headRotatorRef == null) Debug.LogError("Missing (Head Rotator)");

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
			else
			{
				Debug.LogError("Missing components -> Trying Watson");
				if (azureVoiceOutRef == null) Debug.LogError("Missing (Azure output)");
				if (setBodyRef == null) Debug.LogError("Missing (Body component missing)");
				if (faceShapeRef == null) Debug.LogError("Missing (Face Shape script)");
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
			else
			{
				Debug.LogError("Missing components -> Trying Azure");
				if (watsonOutRef == null) Debug.LogError("Missing (Watson output)");
				if (setBodyRef == null) Debug.LogError("Missing (Body component missing)");
				if (faceShapeRef == null) Debug.LogError("Missing (Face Shape script)");
				if (watsonOutRef != null && faceShapeRef != null && setBodyRef != null)
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
		}

		//Tone checker
		if(toneRef != null && faceShapeRef != null && setBodyRef != null)
		{
			UnityEventTools.AddPersistentListener(toneRef.responseTone, faceShapeRef.SetEmotion);
			UnityEventTools.AddPersistentListener(toneRef.responseTone, setBodyRef.SetBodyEmotion);
		}
		else
		{
			Debug.LogWarning("Missing components. You can ignore this one.");
			if (toneRef == null) Debug.LogWarning("Missing (Watson Tone)");
			if (setBodyRef == null) Debug.LogWarning("Missing (Body component missing)");
			if (faceShapeRef == null) Debug.LogWarning("Missing (Face Shape script)");
		}

	}

}
