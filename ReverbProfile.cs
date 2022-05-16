using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reverb Profile", menuName = "Post Effects Control/Create new Reverb Profile")]
public class ReverbProfile : ProfileBase
{
	public ReverbProfile(AudioReverbZone zone)
	{
		this.Data.Room = zone.room;
		this.Data.RoomHF = zone.roomHF;
		this.Data.RoomLF = zone.roomLF;
		this.Data.DecayTime = zone.decayTime;
		this.Data.DecayHFRatio = zone.decayHFRatio;
		this.Data.Reflections = (float)zone.reflections;
		this.Data.ReflectionsDelay = zone.reflectionsDelay;
		this.Data.Reverb = (float)zone.reverb;
		this.Data.ReverbDelay = zone.reverbDelay;
		this.Data.HFReference = zone.HFReference;
		this.Data.LFReference = zone.LFReference;
		this.Data.Diffusion = zone.diffusion;
		this.Data.Density = zone.density;
	}

	[SerializeField]
	public ReverbProfile.ReverbData Data;

	[Serializable]
	public struct ReverbData
	{
		[Range(-10000f, 0f)]
		public int Room;

		[Range(-10000f, 0f)]
		public int RoomHF;

		[Range(-10000f, 0f)]
		public int RoomLF;

		[Range(-10000f, 20f)]
		public float DecayTime;

		[Range(-10000f, 2f)]
		public float DecayHFRatio;

		[Range(-10000f, 1000f)]
		public float Reflections;

		[Range(-10000f, 0.3f)]
		public float ReflectionsDelay;

		[Range(-10000f, 2000f)]
		public float Reverb;

		[Range(-10000f, 0.1f)]
		public float ReverbDelay;

		[Range(-10000f, 20000f)]
		public float HFReference;

		[Range(-10000f, 1000f)]
		public float LFReference;

		[Range(-10000f, 100f)]
		public float Diffusion;

		[Range(-10000f, 100f)]
		public float Density;
	}
}
