using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Struct of mouth positions, an easier way to store data instead of a 2D array.
[System.Serializable]
public struct FacePose
{
	public float[] blendShapes;

	public FacePose(float[] shapes)
	{
		blendShapes = shapes;
	}
}
