using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace StanleyUI.Setters
{
	[RequireComponent(typeof(StanleyMenuSlider))]
	public class VolumeSlider : GenericSlider
	{
		public override void SetValue(float val100)
		{
			this.SetValueSilent(val100);
			base.StartCoroutine(this.PlaySound());
		}

		public void SetValueSilent(float val100)
		{
			base.SetValue(val100);
			this.mixer.SetFloat(this.mixerVariableName, this.VolumeFunction(Mathf.Lerp(0.001f, 1f, val100 / 100f), -80f));
		}

		private IEnumerator Start()
		{
			this.isPlaying = true;
			yield return new WaitForSecondsRealtime(1f);
			this.isPlaying = false;
			yield break;
		}

		private void OnEnable()
		{
			base.StartCoroutine(this.Start());
		}

		private void OnDisable()
		{
			this.isPlaying = false;
			base.StopAllCoroutines();
		}

		public void ForceStop()
		{
			this.timer = this.playtime;
		}

		private IEnumerator PlaySound()
		{
			if (!base.isActiveAndEnabled || this.isPlaying)
			{
				this.timer = 0f;
				yield break;
			}
			this.isPlaying = true;
			if (this.playWhileValueChange != null)
			{
				this.playWhileValueChange.Play();
				float playTimeToWait = this.playtime;
				if (this.autoStopBehaviour == VolumeSlider.AutoStopBehaviour.AfterClipPlayed)
				{
					this.playtime = this.playWhileValueChange.clip.length - this.playWhileValueChange.time;
				}
				this.timer = 0f;
				while (this.timer < playTimeToWait)
				{
					this.timer += Time.unscaledDeltaTime;
					yield return null;
				}
				this.playWhileValueChange.Stop();
				this.isPlaying = false;
				yield break;
			}
			yield break;
		}

		private float VolumeFunctionFake(float x)
		{
			return Mathf.Log(x + 1f) / Mathf.Log(2f);
		}

		private float VolumeFunction(float x, float lowDb = -80f)
		{
			return -lowDb * Mathf.Log10(x) / 2f;
		}

		public AudioMixer mixer;

		public string mixerVariableName;

		public AudioSource playWhileValueChange;

		public float playtime = 100000f;

		public VolumeSlider.AutoStopBehaviour autoStopBehaviour;

		private bool isPlaying;

		private float timer = 100000f;

		public enum AutoStopBehaviour
		{
			AfterSetPlayTime,
			AfterClipPlayed
		}
	}
}
