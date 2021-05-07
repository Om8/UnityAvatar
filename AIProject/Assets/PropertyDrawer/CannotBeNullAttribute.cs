using System;
using UnityEngine;

//https://forum.unity.com/threads/freebie-really-basic-not-null-attribute.520644/
/// <summary> Cannot Be Null will red-flood the field if the reference is null. </summary>
[AttributeUsage(AttributeTargets.Field)]
public class CannotBeNullObjectField : PropertyAttribute { }
