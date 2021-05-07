using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMouthLerp : MonoBehaviour
{
	[SerializeField]
	FacePositionList listOfMouthPositions = null;
	[SerializeField]
	SkinnedMeshRenderer skinnedMesh = null;

	[SerializeField]
	int targetMouthPosition = 15;
	int currentStringIndex = 0;
	string wordsToSay = "";
	float currentSpeechTime = 0;
	//[SerializeField]
	//float TimeBetweenCharacters = 2f;

	float adjustedTimeBetweenCharacters = 0;

	[SerializeField]
	float lerpSpeed = 100;

	char[] options = { 'B', 'F', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y', '0', '\0' };

	// Update is called once per frame
	void Update()
    {
		if (wordsToSay.Length > currentStringIndex)
		{
			currentSpeechTime = Mathf.Clamp(currentSpeechTime + Time.deltaTime, 0, adjustedTimeBetweenCharacters);
			if (currentSpeechTime == adjustedTimeBetweenCharacters)
			{
				if(currentStringIndex + 1 < wordsToSay.Length)
				{
					currentStringIndex++;
					int tempTarget = System.Array.IndexOf(options, wordsToSay[currentStringIndex]);
					if (tempTarget == -1)
					{
						targetMouthPosition = 16;
					}
					else
					{
						targetMouthPosition = tempTarget;

					}
				}
				else
				{
					targetMouthPosition = 16;
					wordsToSay = "";
				}
				currentSpeechTime = 0;
			}
		}
		else
		{
			targetMouthPosition = 16;
		}


		if (listOfMouthPositions != null && skinnedMesh != null && (targetMouthPosition >= 0 && targetMouthPosition < listOfMouthPositions.mouthPositions.Length))
		{
			for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; i++)
			{
				float lerpedValue = Mathf.Lerp(skinnedMesh.GetBlendShapeWeight(i), listOfMouthPositions.mouthPositions[targetMouthPosition].blendShapes[i], Time.deltaTime * lerpSpeed);
				skinnedMesh.SetBlendShapeWeight(i, lerpedValue);
			}
		}
    }


	/// <summary>
	/// Start the mouth movement
	/// </summary>
	/// <param name="words">Words you would like to lip sunc with</param>
	/// <param name="length">How long the phrase is</param>
	public void PlayMouth(string words, float length)
	{
		words = words.ToUpper();
		wordsToSay = words;
		currentSpeechTime = 0;
		currentStringIndex = 0;
		targetMouthPosition = System.Array.IndexOf(options, wordsToSay[0]);
		adjustedTimeBetweenCharacters = length / words.Length;
	}
}
