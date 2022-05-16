using System;

[Flags]
public enum StanleyPlatform
{
	Invalid = -1,
	NoVariation = 1,
	PC = 2,
	Playstation4 = 4,
	Playstation5 = 8,
	XBOX360 = 16,
	XBOXone = 32,
	Switch = 64,
	Mobile = 128,
	Console = 124,
	Port = 252,
	Playstation = 12,
	XBOX = 48
}
