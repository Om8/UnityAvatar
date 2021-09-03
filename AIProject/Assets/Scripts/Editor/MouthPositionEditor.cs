using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AI.Volume.Bot.Audio;
using AI.Volume.Bot.Data;
using AI.Volume.Bot.Visual;

public class MouthPositionEditor : EditorWindow
{
	//Public variables
	[SerializeField, Tooltip("Mesh that you want to pose and save"), CannotBeNullObjectField]
	private SkinnedMeshRenderer mesh = null;
	[SerializeField, Tooltip("List that you want to save your data to"), CannotBeNullObjectField]
	private FacePositionList list = null;
	[SerializeField]
	private AzureLocalVoice localSaver;
	[SerializeField, CannotBeNullObjectField]
	private GameObject mainParentBody;

	private SerializedProperty listOfShapes;

	private int currentEditing = 0;

	//Selected pose
	private int selected = 0;
	private int lastSelected = 0;

	private int faceLength = 8;

	//Blend space scroll bar
	private Vector2 scrollPosNums;
	private Vector2 scrollPosArray;

	private int currentTab = 0;

	private string[] tabOptions = { "Adjustments", "Local Response" };

	//Local saving
	private string voiceOutput = "Test Audio";
	private string fileName = "AudioTest";

	[MenuItem("VolumeAI/Open Editor Window")]
	static void Init()
	{
		MouthPositionEditor window = (MouthPositionEditor)EditorWindow.GetWindow(typeof(MouthPositionEditor), false, "Mouth Pose Editor");
		window.Show();
	}


	private void OnGUI()
	{
		currentTab = GUILayout.Toolbar(currentTab, tabOptions);
		switch (currentTab)
		{
			case 0:
				FacePoseAdjusting();
				break;
			case 1:
				SavingVoice();
				break;
		}
	}

	private void SavingVoice()
	{
		fileName = EditorGUILayout.TextField("File name: ", fileName);
		voiceOutput = EditorGUILayout.TextField("Text to speech: ", voiceOutput);

		if (GUILayout.Button("Save")){
			AzureLocalVoice.CreateAudioFile(voiceOutput, fileName);
		}
	}


	private void FacePoseAdjusting()
	{
		//Variable inputs
		GUILayout.Label("Required inputs", EditorStyles.boldLabel);
		GUILayout.Label("Body to pose", EditorStyles.label);
		mesh = EditorGUILayout.ObjectField(mesh, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
		GUILayout.Label("List to save to", EditorStyles.label);
		list = EditorGUILayout.ObjectField(list, typeof(FacePositionList), true) as FacePositionList;

		GUILayout.Label("Pose Editor", EditorStyles.boldLabel);

		//Check if user can start posing mesh
		if (mesh == null)
		{
			EditorGUILayout.TextArea("No skinned mesh selected", GUI.skin.GetStyle("HelpBox"));
		}
		if (list == null)
		{
			EditorGUILayout.TextArea("No list selected", GUI.skin.GetStyle("HelpBox"));
		}
		//If both are valid show the editor.
		if (mesh != null && list != null)
		{
			int lastEdit = currentEditing;
			string[] faceOptions = System.Enum.GetNames(typeof(FaceRegion));
			currentEditing = EditorGUILayout.Popup("Face Region", currentEditing, faceOptions);
			if(lastEdit != currentEditing)
			{
				selected = 0;
			}

			MouthPositionEdits();


			
			//Apply the current face shape to the array.
			if (GUILayout.Button("Apply Pose"))
			{

				List<float> blendShapeValues = new List<float>();
				for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
				{
					blendShapeValues.Add(mesh.GetBlendShapeWeight(i));
				}
				MouthPosition localMouthPos = new MouthPosition(blendShapeValues.ToArray());

				if(currentEditing == 0) list.SetMouthPosition(selected, localMouthPos);
				if (currentEditing == 1) list.SetFacePosition(selected, localMouthPos);
			}
			GUILayout.Label("Mouth Blend Region", EditorStyles.boldLabel);
			BlendShapeSelection();
	
		}		
	}

	private void BlendShapeArray()
	{
		if (Event.current.type == EventType.Layout)
		{
			if (list.blendShapeList.Length != mesh.sharedMesh.blendShapeCount)
			{
				list.blendShapeList = new FaceRegion[mesh.sharedMesh.blendShapeCount];
			}
		}
		SerializedObject serialisedObject = new SerializedObject(list);
		listOfShapes = serialisedObject.FindProperty("blendShapeList");
		EditorGUILayout.PropertyField(listOfShapes);
		serialisedObject.ApplyModifiedProperties();
	}

	private void BlendShapeSelection()
	{
		if (mesh != null && list != null)
		{

			scrollPosNums = GUILayout.BeginScrollView(scrollPosNums, false, false, GUILayout.Width(500), GUILayout.Height(200));
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical();
			GUILayout.Label("Blend shape numbers", EditorStyles.label);
			EditorGUILayout.Space(30);

			for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
			{
				EditorGUILayout.LabelField(i + " - " + mesh.sharedMesh.GetBlendShapeName(i));
			}
			EditorGUILayout.EndVertical();

			//GUILayout.EndScrollView();
			EditorGUILayout.BeginVertical();
			GUILayout.Label("Region array", EditorStyles.label);
			BlendShapeArray();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			GUILayout.EndScrollView();

		}
	}

	private void MouthPositionEdits()
	{
		//The 16 face shapes + base shape
		string[] options = { "Base" };
		switch (currentEditing) {
			case 0:
				//15 visemes from OVR https://developer.oculus.com/documentation/unity/audio-ovrlipsync-viseme-reference/?locale=en_GB
				options = new string[] { "sil", "PP", "FF", "TH", "DD", "kk", "CH", "SS", "nn", "RR", "aa", "E", "I", "O", "U" };
				break;
			case 1:
				options = new string[] { "Base", "Happy", "Surprise", "Fear", "Disgust", "Anger", "Sad", "Think" };
				break;
		}
		selected = EditorGUILayout.Popup("Pose", selected, options);
		if (selected != lastSelected)//If mouth position does not equal the last one, change the face shape.
		{
			if ((currentEditing == 0 ? list.mouthPositions.Length : list.facePositions.Length) == (currentEditing == 0 ? 15 : faceLength))
			{
				for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
				{
					if (selected != 0)
					{
						if ((currentEditing == 0 ? list.mouthPositions[selected].blendShapes.Length : list.facePositions[selected].blendShapes.Length) == mesh.sharedMesh.blendShapeCount)
						{
							switch (currentEditing)
							{
								case 0:
									mesh.SetBlendShapeWeight(i, list.mouthPositions[selected].blendShapes[i]);
									break;
								case 1:
									mesh.SetBlendShapeWeight(i, list.facePositions[selected].blendShapes[i]);
									break;
							}
						}
						else
						{
							switch (currentEditing)
							{
								case 0:
									list.mouthPositions[selected].blendShapes = new float[mesh.sharedMesh.blendShapeCount];
									break;
								case 1:
									list.facePositions[selected].blendShapes = new float[mesh.sharedMesh.blendShapeCount];
									break;
							}
							mesh.SetBlendShapeWeight(i, 0);
						}
					}
					else
					{
						mesh.SetBlendShapeWeight(i, 0);
					}
				}

				lastSelected = selected;
			}
			else
			{
				switch (currentEditing)
				{
					case 0:
						list.mouthPositions = new MouthPosition[mesh.sharedMesh.blendShapeCount];
						break;
					case 1:
						list.facePositions = new MouthPosition[faceLength];
						break;
				}
			}
		}
	}
}
