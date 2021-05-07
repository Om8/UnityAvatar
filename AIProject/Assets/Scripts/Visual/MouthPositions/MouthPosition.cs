using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Struct of mouth positions, an easier way to store data instead of a 2D array.
[System.Serializable]
public struct MouthPosition
{
	public float[] blendShapes;

	public MouthPosition(float[] shapes)
	{
		blendShapes = shapes;
	}
}
