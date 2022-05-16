using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Collection", menuName = "Data/Audio Collection")]
public class AudioCollection : ScriptableObject
{
	public float AverageDuration
	{
		get
		{
			return this.averageDuration;
		}
	}

	public int GetIndex()
	{
		return this.index;
	}

	public bool SetVolumeAndPitchAndPlayClip(AudioSource audioSource)
	{
		AudioEntry audioEntry;
		AudioClip clip;
		if (this.GetRandomEntry(out audioEntry) && audioSource != null && audioEntry.GetClip(out clip))
		{
			audioSource.clip = clip;
			audioSource.volume = audioEntry.GetVolume() * this.masterVolume;
			audioSource.pitch = audioEntry.GetPitch();
			if (audioSource.gameObject.activeInHierarchy)
			{
				audioSource.Play();
			}
			return true;
		}
		return false;
	}

	private bool GetRandomEntry(out AudioEntry entry)
	{
		if (this.usePlaylistSorting && this.AudioEntries.Count > 0)
		{
			if (!this.setup)
			{
				this.UpdatePlaylist();
				entry = this.GetNextEntryFromPlaylist();
				this.setup = true;
				return true;
			}
			entry = this.GetNextEntryFromPlaylist();
			return true;
		}
		else
		{
			if (this.AudioEntries.Count > 0)
			{
				entry = this.AudioEntries[Random.Range(0, this.AudioEntries.Count)];
				return true;
			}
			entry = null;
			return false;
		}
	}

	private void UpdatePlaylist()
	{
		this.entryPlaylist = new AudioEntry[this.AudioEntries.Count];
		Array.Copy(this.AudioEntries.ToArray(), 0, this.entryPlaylist, 0, this.AudioEntries.Count);
		if (!this.sequentialPlaylist)
		{
			new Random().Shuffle(this.entryPlaylist);
		}
		this.playlistIndex = 0;
		this.playlistLength = this.entryPlaylist.Length;
	}

	private AudioEntry GetNextEntryFromPlaylist()
	{
		if (this.playlistIndex >= this.playlistLength)
		{
			this.UpdatePlaylist();
		}
		AudioEntry result = this.entryPlaylist[this.playlistIndex];
		this.playlistIndex++;
		return result;
	}

	[SerializeField]
	[Range(0f, 2f)]
	private float masterVolume = 1f;

	[SerializeField]
	private int index;

	[SerializeField]
	[Range(0f, 3f)]
	private float averageDuration = 0.25f;

	[SerializeField]
	private bool usePlaylistSorting;

	[SerializeField]
	private bool sequentialPlaylist;

	[Space]
	[SerializeField]
	public List<AudioEntry> AudioEntries;

	[NonSerialized]
	private AudioEntry[] entryPlaylist;

	[NonSerialized]
	private int playlistLength;

	[NonSerialized]
	private int playlistIndex;

	[NonSerialized]
	private bool setup;

	public const bool SHOW_DEBUG = false;
}
