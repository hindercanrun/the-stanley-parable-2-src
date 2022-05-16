using System;
using UnityEngine;

public interface IMoviePlayer
{
	event Action OnMovieLoopPointReached;

	GameObject Play(string cameraName, string moviePath);

	void Pause();

	void Unpause();

	void Stop();

	void SetSpeed(float speed);
}
