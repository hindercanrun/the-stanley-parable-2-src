using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoviePlayer : HammerEntity
{
	public void SetMoviePath(string newMovie)
	{
		this.MoviePath = newMovie;
	}

	private void Awake()
	{
	}

	public void Input_PlayMovie()
	{
		string path = this.MoviePath;
		if (PlatformManager.UseLowerFPSVideos && MoviePlayer.sixtyFPSToThirtyFPS.ContainsKey(this.MoviePath))
		{
			path = MoviePlayer.sixtyFPSToThirtyFPS[this.MoviePath];
		}
		string moviePath = Path.Combine(Application.streamingAssetsPath, "video", path);
		GameMaster.MoviePlaybackContext moviePlaybackContext = Singleton<GameMaster>.Instance.StartMovie(this.skippable, this, this.CameraObjectName, moviePath, this.isFullScreenMovie);
		if (this.autoCameraActivation == MoviePlayer.AutoCameraActivation.AutoEnableOnStart || this.autoCameraActivation == MoviePlayer.AutoCameraActivation.AutoEnableOnStartAndDisableOnStop)
		{
			moviePlaybackContext.CameraEnabled = true;
		}
	}

	public void Ended(GameMaster.MoviePlaybackContext context)
	{
		if (this.autoCameraActivation == MoviePlayer.AutoCameraActivation.AutoEnableOnStartAndDisableOnStop)
		{
			context.CameraEnabled = false;
		}
		base.FireOutput(Outputs.OnPlaybackFinished);
	}

	public void Skipped()
	{
		base.FireOutput(Outputs.OnSkip);
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		if (this.child != null)
		{
			this.child.SetActive(true);
		}
	}

	public override void Input_Disable()
	{
		base.Input_Disable();
		if (this.child != null)
		{
			this.child.SetActive(false);
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static MoviePlayer()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["bucketchoice1.mp4"] = "bucketchoice1_30fps.mp4";
		dictionary["bucketchoice2.mp4"] = "bucketchoice2_30fps.mp4";
		dictionary["intro.mp4"] = "intro_30fps.mp4";
		dictionary["intro_balloon1.mp4"] = "intro_balloon1_30fps.mp4";
		dictionary["intro_balloon2.mp4"] = "intro_balloon2_30fps.mp4";
		dictionary["intro_reverse.mp4"] = "intro_reverse_30fps.mp4";
		dictionary["sillybirds.mp4"] = "sillybirds_30fps_5mbps.mp4";
		MoviePlayer.sixtyFPSToThirtyFPS = dictionary;
	}

	public bool skippable;

	public bool loop;

	public string MoviePath;

	public string CameraObjectName;

	[HideInInspector]
	public GameObject child;

	public bool isFullScreenMovie = true;

	public MoviePlayer.AutoCameraActivation autoCameraActivation;

	private static readonly Dictionary<string, string> sixtyFPSToThirtyFPS;

	public enum AutoCameraActivation
	{
		Manual,
		AutoEnableOnStart,
		AutoEnableOnStartAndDisableOnStop
	}
}
