using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Sound List")]
public class SoundList : ScriptableObject
{
	public List<AudioClip> sounds = new List<AudioClip>();
}
