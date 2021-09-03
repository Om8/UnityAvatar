using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AI.Volume.Bot.Data;
using AI.Volume.Bot.Visual;

namespace AI.Volume.Bot.Tone
{
	public class SetFaceShapes : MonoBehaviour
	{
		[CannotBeNullObjectField]
		public SkinnedMeshRenderer mesh;
		[CannotBeNullObjectField]
		public FacePositionList list;
		[SerializeField]
		private float lerpSpeed = 10;

		[SerializeField]
		private int currentFacePose;

		private string[] comparison = { "Base", "Anger", "Fear", "Joy", "Sadness", "Analytical", "Confident", "Tentative" };

		//Dictionary to convert emotions to a number, which will set face and body.
		private Dictionary<string, int> emotionStates = new Dictionary<string, int>()
		{
			{"Base", 0 },
			{"Anger", 5 },
			{"Fear", 3 },
			{"Joy", 1 },
			{"Sadness", 6 },
			{"Analytical", 2 },
			{"Confident", 0 },
			{"Tentative", 4 },
		};

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
			//If it is in the dictionary, then set the value
			if (emotionStates.TryGetValue(emotion, out int val))
			{
				currentFacePose = val;
			}
			//Else set it to base.
			else
			{
				currentFacePose = 0;
			}
		}

		private void Update()
		{
			if (mesh != null && list != null)
			{
				if (currentFacePose <= 7 && currentFacePose >= 0)
				{
					if (mesh.sharedMesh.blendShapeCount == list.blendShapeList.Length)
					{
						for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
						{
							if (list.blendShapeList[i] == FaceRegion.Face)
							{
								//Lerp the face to the current shape.
								float lerpedValue = Mathf.Lerp(mesh.GetBlendShapeWeight(i), list.facePositions[currentFacePose].blendShapes[i], Time.deltaTime * lerpSpeed);
								mesh.SetBlendShapeWeight(i, lerpedValue);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Set the thinking state
		/// </summary>
		public void SetThinking()
		{
			SetEmotion("Think");
		}

		/// <summary>
		/// Stop thinking state
		/// </summary>
		public void StopThinking()
		{
			SetEmotion("Base");
		}
	}
}
