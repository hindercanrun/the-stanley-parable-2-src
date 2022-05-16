using System;
using UnityEngine;

namespace Nest.Components
{
	public class AudioInput : NestInput
	{
		public override void Start()
		{
			base.Start();
			this._audioSource = base.GetComponent<AudioSource>();
			this.wLeftChannel = new float[1024];
			this.wRightChannel = new float[1024];
			this.Waveform = new float[1024];
		}

		private void Update()
		{
			if (this._audioSource.isPlaying)
			{
				this.AnalyzeSound();
				this.Value.TargetValue = this.Remap(this._decibels, (float)(-(float)this.DecibelLimit), 0f, 0f, 1f);
				base.Invoke();
			}
		}

		private void AnalyzeSound()
		{
			this._audioSource.GetOutputData(this.wLeftChannel, 0);
			this._audioSource.GetOutputData(this.wRightChannel, 1);
			this.Waveform = this.CombineChannels(this.wLeftChannel, this.wRightChannel);
			float num = 0f;
			for (int i = 0; i < this.Waveform.Length; i++)
			{
				num += this.Waveform[i] * this.Waveform[i];
			}
			this._rmsValue = Mathf.Sqrt(num / 1024f);
			this._decibels = 20f * Mathf.Log10(this._rmsValue / this.DecibelReference);
			if (this._decibels < (float)(-(float)this.DecibelLimit))
			{
				this._decibels = (float)(-(float)this.DecibelLimit);
			}
		}

		private float[] CombineChannels(float[] Left, float[] Right)
		{
			float[] array = new float[Left.Length];
			for (int i = 0; i < Left.Length; i++)
			{
				array[i] = (Left[i] + Right[i]) / 2f / 32768f;
			}
			return array;
		}

		public float Remap(float Value, float From1, float To1, float From2, float To2)
		{
			return (Value - From1) / (To1 - From1) * (To2 - From2) + From2;
		}

		public float DecibelReference = 0.1f;

		public int DecibelLimit = 160;

		private const int SampleCount = 1024;

		private AudioSource _audioSource;

		private float _rmsValue;

		private float _decibels;

		private float[] wLeftChannel;

		private float[] wRightChannel;

		private float[] Waveform;
	}
}
