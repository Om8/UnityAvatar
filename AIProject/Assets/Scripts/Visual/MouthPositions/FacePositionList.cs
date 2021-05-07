using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FacePositionList : MonoBehaviour
{
	[HideInInspector,Tooltip("Array of mouth positions")]
	public MouthPosition[] mouthPositions = new MouthPosition[15];
	[HideInInspector,Tooltip("Array of face positions")]
	public MouthPosition[] facePositions = new MouthPosition[8];

	[HideInInspector]
	public FaceRegion[] blendShapeList = new FaceRegion[50];

	/// <summary>
	/// Set face position in the mouth position array
	/// </summary>
	/// <param name="index">Index to assign</param>
	/// <param name="pos">Pose data to assign</param>
	public void SetMouthPosition(int index, MouthPosition pos)
	{
		if (mouthPositions.Length > index)
		{
			mouthPositions[index] = pos;
		}
	}

	public void SetFacePosition(int index, MouthPosition pos)
	{
		if (facePositions.Length >= index + 1)
		{
			facePositions[index] = pos;
		}
	}

	public void SetBlendSpaces(FaceRegion[] newArray)
	{
		blendShapeList = newArray;
	}
}
