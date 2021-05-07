using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
//[CustomEditor(typeof(MouthPositionList))]
public class EditorFaceData : Editor
{
	int selected = 0;
	int lastSelected = 0;
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		//DrawDefaultInspector();
		FacePositionList thisScript = (FacePositionList)target;
		if (Selection.activeGameObject != null)
		{
			SkinnedMeshRenderer meshSelected = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
			if (meshSelected != null)
			{
				
				string[] options = { "B", "F", "H", "J", "K", "L", "M", "N", "P", "R", "S", "T", "W", "X", "Y", "0", "Base" };
				selected = EditorGUILayout.Popup("Pose", selected, options);
				if (selected != lastSelected)
				{
					for (int i = 0; i < meshSelected.sharedMesh.blendShapeCount; i++)
					{
						if (selected != 0)
						{
							if (thisScript.mouthPositions[selected].blendShapes.Length == meshSelected.sharedMesh.blendShapeCount)
							{
								meshSelected.SetBlendShapeWeight(i, thisScript.mouthPositions[selected].blendShapes[i]);
							}
							else
							{
								meshSelected.SetBlendShapeWeight(i, 0);
							}
						}
						else
						{
							meshSelected.SetBlendShapeWeight(i, 0);
						}
					}
					lastSelected = selected;
				}
				

				if (GUILayout.Button("Apply Pose")){

					List<float> blendShapeValues = new List<float>();
					for (int i = 0; i < meshSelected.sharedMesh.blendShapeCount; i++)
					{
						blendShapeValues.Add(meshSelected.GetBlendShapeWeight(i));
					}
					MouthPosition localMouthPos = new MouthPosition(blendShapeValues.ToArray());

					thisScript.SetMouthPosition(selected, localMouthPos);
				}
			}
			else
			{
				EditorGUILayout.TextArea("No skinned mesh selected", GUI.skin.GetStyle("HelpBox"));
			}
		}
		else
		{
			EditorGUILayout.TextArea("No object selected", GUI.skin.GetStyle("HelpBox"));
		}
	}

}
#endif