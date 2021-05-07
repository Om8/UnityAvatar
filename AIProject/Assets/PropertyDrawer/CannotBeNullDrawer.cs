using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
//https://forum.unity.com/threads/freebie-really-basic-not-null-attribute.520644/
[CustomPropertyDrawer(typeof(CannotBeNullObjectField))]
public class CannotBeNullDrawer : PropertyDrawer
{
	public override void OnGUI(Rect inRect, SerializedProperty inProp, GUIContent label)
	{
		EditorGUI.BeginProperty(inRect, label, inProp);

		bool error = inProp.objectReferenceValue == null;
		if (error)
		{
			label.text = "[Required] " + label.text;
			GUI.color = new Color(1, .6f, 0.6f);
		}

		EditorGUI.PropertyField(inRect, inProp, label);
		GUI.color = Color.white;

		EditorGUI.EndProperty();
	}
}
#endif