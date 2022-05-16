using System;
using UnityEngine;

[Serializable]
public struct MinMax
{
	public MinMax(float low, float high)
	{
		this.Min = low;
		this.Max = high;
	}

	public MinMax(float i)
	{
		this.Min = i;
		this.Max = i;
	}

	public float Range()
	{
		return Mathf.Abs(this.Max - this.Min);
	}

	public float Random()
	{
		return UnityEngine.Random.Range(this.Min, this.Max);
	}

	public float MinOrMax()
	{
		if (UnityEngine.Random.value > 0.5f)
		{
			return this.Min;
		}
		return this.Max;
	}

	public float Lerp(float t)
	{
		t = Mathf.Clamp01(t);
		return this.LerpUnclamped(t);
	}

	public float LerpUnclamped(float t)
	{
		return (1f - t) * this.Min + t * this.Max;
	}

	public float Average()
	{
		return this.Lerp(0.5f);
	}

	public float ILerp(float t)
	{
		return (t - this.Min) / this.Range();
	}

	public static MinMax operator *(MinMax mm, float f)
	{
		return new MinMax(mm.Min * f, mm.Max * f);
	}

	public static MinMax operator *(MinMax mm, MinMax mm2)
	{
		return new MinMax(mm.Min * mm2.Min, mm.Max * mm2.Max);
	}

	public float Min;

	public float Max;
}
