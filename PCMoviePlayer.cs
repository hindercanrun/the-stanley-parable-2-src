using System;
using UnityEngine;
using UnityEngine.Video;

public class PCMoviePlayer : IMoviePlayer
{
	public event Action OnMovieLoopPointReached;

	public void SetSpeed(float speed)
	{
		if (this.videoPlayer != null && this.videoPlayer.canSetPlaybackSpeed)
		{
			this.videoPlayer.playbackSpeed = speed;
		}
	}

	public GameObject Play(string cameraName, string path)
	{
		this.videoPlayerGameObject = GameObject.Find(cameraName);
		if (this.videoPlayerGameObject == null)
		{
			return null;
		}
		this.videoPlayer = this.videoPlayerGameObject.GetComponent<VideoPlayer>();
		if (this.videoPlayer == null)
		{
			this.videoPlayer = this.videoPlayerGameObject.AddComponent<VideoPlayer>();
			this.videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
			this.videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
		}
		this.videoPlayer.loopPointReached -= this.BroadcastLoopPointReached;
		this.videoPlayer.loopPointReached += this.BroadcastLoopPointReached;
		Debug.Log("PRE LINUX BLOCK");
		Debug.Log("video->video_webm path=" + path);
		Debug.Log("videoPlayer.url = path=" + path);
		this.videoPlayer.url = path;
		this.videoPlayer.Prepare();
		this.videoPlayer.Play();
		return this.videoPlayerGameObject;
	}

	public void Pause()
	{
		if (this.videoPlayerGameObject != null)
		{
			this.videoPlayer.Pause();
		}
	}

	public void Unpause()
	{
		if (this.videoPlayerGameObject != null)
		{
			this.videoPlayer.Play();
		}
	}

	public void Stop()
	{
		if (this.videoPlayerGameObject != null)
		{
			this.videoPlayer.Stop();
		}
	}

	private void BroadcastLoopPointReached(VideoPlayer source)
	{
		Action onMovieLoopPointReached = this.OnMovieLoopPointReached;
		if (onMovieLoopPointReached == null)
		{
			return;
		}
		onMovieLoopPointReached();
	}

	private VideoPlayer videoPlayer;

	private GameObject videoPlayerGameObject;
}
