using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Volume.Bot.Data
{
	//Struct of mouth positions, an easier way to store data instead of a 2D array.
	[System.Serializable]
	public struct FacePose
	{
		//Only one float for the moment, allows for future expansion.
		public float[] blendShapes;

		public FacePose(float[] shapes)
		{
			blendShapes = shapes;
		}
	}
}
