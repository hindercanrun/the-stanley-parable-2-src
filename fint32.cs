using System;
using UnityEngine;

public struct fint32
{
	public fint32(uint i)
	{
		this.integer = i;
		this.fraction = 0U;
	}

	public fint32(uint i, uint f)
	{
		this.integer = i;
		this.fraction = f;
	}

	public static implicit operator fint32(float f)
	{
		if (f < 0f)
		{
			return new fint32(0U);
		}
		fint32 fint;
		fint.integer = (uint)Mathf.FloorToInt(f);
		fint.fraction = (uint)(100000000f * (f - fint.integer));
		return fint;
	}

	public static implicit operator float(fint32 fi)
	{
		return fi.integer + fi.fraction / fint32.FINT32_MAX_FRACTION;
	}

	public static fint32 operator +(fint32 a, fint32 b)
	{
		fint32 result;
		result.integer = a.integer + b.integer;
		uint num = a.fraction + b.fraction;
		if (num > fint32.FINT32_MAX_FRACTION)
		{
			num -= fint32.FINT32_MAX_FRACTION;
			result.integer += 1U;
		}
		result.fraction = num;
		return result;
	}

	public static fint32 operator +(fint32 a, float b)
	{
		fint32 fint = b;
		fint32 result;
		result.integer = a.integer + fint.integer;
		uint num = a.fraction + fint.fraction;
		if (num > fint32.FINT32_MAX_FRACTION)
		{
			num -= fint32.FINT32_MAX_FRACTION;
			result.integer += 1U;
		}
		result.fraction = num;
		return result;
	}

	public static bool operator >(fint32 a, fint32 b)
	{
		return a.integer > b.integer || (a.integer >= b.integer && a.fraction > b.fraction);
	}

	public static bool operator <(fint32 a, fint32 b)
	{
		return a.integer < b.integer || (a.integer <= b.integer && a.fraction < b.fraction);
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"( ",
			this.integer,
			".",
			this.fraction.ToString("D8"),
			" )"
		});
	}

	private uint integer;

	private uint fraction;

	public static readonly uint FINT32_MAX_FRACTION = 100000000U;
}
