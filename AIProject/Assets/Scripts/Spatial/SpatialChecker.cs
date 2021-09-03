using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialChecker : MonoBehaviour
{
	/// <summary>
	/// Check whether two objects have sight of each other
	/// </summary>
	/// <param name="positionOne">The first object to compare from, usuaully the AI.</param>
	/// <param name="positionTwo">The second position to compare, usually the player.</param>
	/// <returns></returns>
	public bool LineOfSightChecker(GameObject positionOne, GameObject positionTwo)
	{
		//Check if anything is in between the two objects.
		RaycastHit hit;
		if(Physics.Linecast(positionOne.transform.position, positionTwo.transform.position, out hit))
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// Are the two objects within range of each other?
	/// </summary>
	/// <param name="positionOne">First position, usually the AI</param>
	/// <param name="positionTwo">Secon dposition, usually the player</param>
	/// <param name="maxDistance">How far can the player be away from the AI before they can't speak to them anymore</param>
	/// <returns></returns>
	public bool IsInRange(GameObject positionOne, GameObject positionTwo, float maxDistance)
	{
		if(Vector3.Distance(positionOne.transform.position, positionTwo.transform.position) <= maxDistance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
