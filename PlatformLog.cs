using System;
using System.Diagnostics;

public static class PlatformLog
{
	[Conditional("PLATFORMDEBUG")]
	public static void Log(string msg)
	{
	}
}
