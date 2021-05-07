using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetFaceShapes : MonoBehaviour
{
	[SerializeField, CannotBeNullObjectField]
	public SkinnedMeshRenderer mesh;
	[SerializeField, CannotBeNullObjectField]
	public FacePositionList list;
	[SerializeField]
	float lerpSpeed = 10;

	[SerializeField]
	int currentFacePose;

	string[] comparison = { "Base", "Anger", "Fear", "Joy", "Sadness", "Analytical", "Confident", "Tentative"};
	//options = new string[] { "Base", "Happy", "Surprise", "Fear", "Disgust", "Anger", "Sad" };

	public void FinishedSpeaking()
	{
		SetEmotion("Base");
	}

	/// <summary>
	/// Set the current emotion of the bot.
	/// </summary>
	/// <param name="emotion">New emotion, needs to be one of the following: "Base", "Anger", "Fear", "Joy", "Sadness", "Analytical", "Confident", "Tentative" </param>
	public void SetEmotion(string emotion)
	{
		switch (emotion)
		{
			case "Base":
				currentFacePose = 0;
				break;
			case "Anger":
				currentFacePose = 5;
				break;
			case "Fear":
				currentFacePose = 3;
				break;
			case "Joy":
				currentFacePose = 1;
				break;
			case "Sadness":
				currentFacePose = 6;
				break;
			case "Analytical":
				currentFacePose = 2;
				break;
			case "Confident":
				currentFacePose = 0;
				break;
			case "Tentative":
				currentFacePose = 4;
				break;
			case "Think":
				currentFacePose = 7;
				break;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow)) currentFacePose++;
		if (Input.GetKeyDown(KeyCode.LeftArrow)) currentFacePose--;

		if(mesh != null && list != null)
		{
			if (currentFacePose <= 7 && currentFacePose >= 0)
			{
				if (mesh.sharedMesh.blendShapeCount == list.blendShapeList.Length)
				{
					for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
					{
						if (list.blendShapeList[i] == FaceRegion.Face)
						{
							float lerpedValue = Mathf.Lerp(mesh.GetBlendShapeWeight(i), list.facePositions[currentFacePose].blendShapes[i], Time.deltaTime * lerpSpeed);
							mesh.SetBlendShapeWeight(i, lerpedValue);
						}
					}
				}
			}
		}
	}

	public void SetThinking()
	{
		SetEmotion("Think");
	}

	public void StopThinking()
	{
		SetEmotion("Base");
	}
}
