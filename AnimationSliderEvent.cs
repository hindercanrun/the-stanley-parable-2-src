using System;
using UnityEngine;

public class AnimationSliderEvent : MonoBehaviour
{
	private void Awake()
	{
		this._animation[this._animation.clip.name].speed = 0f;
		this._animation.Play(this._animation.clip.name);
	}

	public void ValueChanged(float value)
	{
		this._animation[this._animation.clip.name].normalizedTime = value;
	}

	[SerializeField]
	private Animation _animation;
}
