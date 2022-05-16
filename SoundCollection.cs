using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Collection", menuName = "Stanley/Sound Collection")]
public class SoundCollection : ScriptableObject
{
	public AudioClip GetRandomClip()
	{
		if (this.Sounds.Length == 0)
		{
			return null;
		}
		return this.Sounds[Random.Range(0, this.Sounds.Length)];
	}

	[SerializeField]
	private AudioClip[] Sounds;
}
