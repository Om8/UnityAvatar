using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Unity event that takes a string
[System.Serializable]
public class AudioEvent : UnityEvent<string> { }

//Unity event that takes a string and float, used for send the message and the length of the message
[System.Serializable]
public class VisualEvent : UnityEvent<string, float> { }
