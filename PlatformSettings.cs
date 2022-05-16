using System;
using TMPro;
using UnityEngine;

public class PlatformSettings : MonoBehaviour
{
	private void SetToDebug_PS4()
	{
		this.SetToDebug("ps4");
	}

	private void SetToDebug_PS5()
	{
		this.SetToDebug("ps5");
	}

	private void SetToDebug_XBOX()
	{
		this.SetToDebug("xbox");
	}

	private void SetToDebug_Switch()
	{
		this.SetToDebug("switch");
	}

	private void SetToDebug_Standalone()
	{
		this.SetToDebug("standalone");
	}

	private void SetToDebug_Actual()
	{
		this.SetToDebug("");
	}

	public static PlatformSettings Instance { get; private set; }

	public TMP_SpriteAsset GetSpriteSheetForInputDevice(GameMaster.InputDevice inputDevice)
	{
		RuntimePlatform runtimePlatformWithDebugIfInEditor = this.GetRuntimePlatformWithDebugIfInEditor();
		if (runtimePlatformWithDebugIfInEditor != RuntimePlatform.PS4)
		{
			if (runtimePlatformWithDebugIfInEditor != RuntimePlatform.XboxOne)
			{
				switch (runtimePlatformWithDebugIfInEditor)
				{
				case RuntimePlatform.Switch:
					return this.switchControllerSprites;
				case RuntimePlatform.GameCoreScarlett:
				case RuntimePlatform.GameCoreXboxOne:
					goto IL_3F;
				case RuntimePlatform.PS5:
					goto IL_38;
				}
				switch (inputDevice)
				{
				default:
					return this.keyboardSprites;
				case GameMaster.InputDevice.GamepadXBOXOneOrGeneric:
					return this.xboxControllerSprites;
				case GameMaster.InputDevice.GamepadPS4:
				case GameMaster.InputDevice.GamepadPS5:
					return this.playstationControllerSprites;
				case GameMaster.InputDevice.GamepadSwitch:
					return this.switchControllerSprites;
				}
			}
			IL_3F:
			return this.xboxControllerSprites;
		}
		IL_38:
		return this.playstationControllerSprites;
	}

	public static RuntimePlatform debugSafeRuntimePlatform { get; private set; }

	private void SetToDebug(string platform)
	{
		this.debugPlatformInEditor.Init();
		this.debugPlatformInEditor.SetValue(platform);
		this.debugPlatformInEditor.SaveToDiskAll();
	}

	private void Awake()
	{
		PlatformSettings.Instance = this;
		this.isStandalone.Init();
		this.isConsole.Init();
		this.isSwitch.Init();
		this.isPlaystation4.Init();
		this.isPlaystation5.Init();
		this.isPlaystationAny.Init();
		this.isXBOX.Init();
		this.debugPlatformInEditor.Init();
		this.isStandalone.SaveToDiskAll();
		this.isConsole.SaveToDiskAll();
		this.isSwitch.SaveToDiskAll();
		this.isPlaystation4.SaveToDiskAll();
		this.isPlaystation5.SaveToDiskAll();
		this.isPlaystationAny.SaveToDiskAll();
		this.isXBOX.SaveToDiskAll();
		this.debugPlatformInEditor.SaveToDiskAll();
		PlatformSettings.debugSafeRuntimePlatform = Application.platform;
		this.CheckPlatforms();
		StringConfigurable stringConfigurable = this.debugPlatformInEditor;
		stringConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(stringConfigurable.OnValueChanged, new Action<LiveData>(this.CheckPlatforms));
	}

	private void OnDestroy()
	{
		StringConfigurable stringConfigurable = this.debugPlatformInEditor;
		stringConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(stringConfigurable.OnValueChanged, new Action<LiveData>(this.CheckPlatforms));
	}

	private void Start()
	{
		this.CheckPlatforms();
	}

	public RuntimePlatform GetRuntimePlatformWithDebugIfInEditor()
	{
		RuntimePlatform result = Application.platform;
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			string text = this.debugPlatformInEditor.GetStringValue().ToLower();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1582198420U)
			{
				if (num <= 360195552U)
				{
					if (num != 323435638U)
					{
						if (num != 360195552U)
						{
							return result;
						}
						if (!(text == "ps4"))
						{
							return result;
						}
					}
					else
					{
						if (!(text == "standalone"))
						{
							return result;
						}
						return RuntimePlatform.WindowsPlayer;
					}
				}
				else if (num != 376973171U)
				{
					if (num != 1582198420U)
					{
						return result;
					}
					if (!(text == "ps"))
					{
						return result;
					}
				}
				else
				{
					if (!(text == "ps5"))
					{
						return result;
					}
					goto IL_142;
				}
			}
			else if (num <= 2217645753U)
			{
				if (num != 2144789592U)
				{
					if (num != 2217645753U)
					{
						return result;
					}
					if (!(text == "playstation"))
					{
						return result;
					}
				}
				else
				{
					if (!(text == "xbox"))
					{
						return result;
					}
					return RuntimePlatform.GameCoreScarlett;
				}
			}
			else if (num != 2480955249U)
			{
				if (num != 2706832996U)
				{
					if (num != 2723610615U)
					{
						return result;
					}
					if (!(text == "playstation4"))
					{
						return result;
					}
				}
				else
				{
					if (!(text == "playstation5"))
					{
						return result;
					}
					goto IL_142;
				}
			}
			else
			{
				if (!(text == "switch"))
				{
					return result;
				}
				return RuntimePlatform.Switch;
			}
			return RuntimePlatform.PS4;
			IL_142:
			result = RuntimePlatform.PS5;
		}
		return result;
	}

	private void CheckPlatforms(LiveData d)
	{
		this.CheckPlatforms();
	}

	[ContextMenu("Check Platforms")]
	private void CheckPlatforms()
	{
		RuntimePlatform runtimePlatformWithDebugIfInEditor = this.GetRuntimePlatformWithDebugIfInEditor();
		bool value = true;
		if (runtimePlatformWithDebugIfInEditor <= RuntimePlatform.LinuxPlayer)
		{
			if (runtimePlatformWithDebugIfInEditor > RuntimePlatform.WindowsPlayer && runtimePlatformWithDebugIfInEditor != RuntimePlatform.WindowsEditor && runtimePlatformWithDebugIfInEditor != RuntimePlatform.LinuxPlayer)
			{
				goto IL_A0;
			}
		}
		else
		{
			if (runtimePlatformWithDebugIfInEditor > RuntimePlatform.PS4)
			{
				if (runtimePlatformWithDebugIfInEditor != RuntimePlatform.XboxOne)
				{
					switch (runtimePlatformWithDebugIfInEditor)
					{
					case RuntimePlatform.Switch:
						this.SetConfigurables(false, false, false, false, true);
						goto IL_A0;
					case RuntimePlatform.Lumin:
					case RuntimePlatform.Stadia:
					case RuntimePlatform.CloudRendering:
						goto IL_A0;
					case RuntimePlatform.GameCoreScarlett:
					case RuntimePlatform.GameCoreXboxOne:
						break;
					case RuntimePlatform.PS5:
						this.SetConfigurables(false, false, true, false, false);
						value = false;
						goto IL_A0;
					default:
						goto IL_A0;
					}
				}
				this.SetConfigurables(false, false, false, true, false);
				goto IL_A0;
			}
			if (runtimePlatformWithDebugIfInEditor != RuntimePlatform.LinuxEditor)
			{
				if (runtimePlatformWithDebugIfInEditor != RuntimePlatform.PS4)
				{
					goto IL_A0;
				}
				this.SetConfigurables(false, true, false, false, false);
				value = false;
				goto IL_A0;
			}
		}
		this.SetConfigurables(true, false, false, false, false);
		IL_A0:
		PlatformSettings.debugSafeRuntimePlatform = runtimePlatformWithDebugIfInEditor;
		this.calledAchievements.SetValue(value);
	}

	private void SetConfigurables(bool standalone = false, bool ps4 = false, bool ps5 = false, bool xbox = false, bool swit = false)
	{
		this.isStandalone.SetValue(standalone);
		this.isConsole.SetValue(ps4 || ps5 || xbox || swit);
		this.isPlaystationAny.SetValue(ps4 || ps5);
		this.isPlaystation4.SetValue(ps4);
		this.isPlaystation5.SetValue(ps5);
		this.isXBOX.SetValue(xbox);
		this.isSwitch.SetValue(swit);
	}

	public static StanleyPlatform GetCurrentRunningPlatform()
	{
		return PlatformSettings.GetStanleyPlatform(PlatformSettings.debugSafeRuntimePlatform);
	}

	public static StanleyPlatform GetStanleyPlatform(RuntimePlatform runtimePlatform)
	{
		if (runtimePlatform <= RuntimePlatform.LinuxPlayer)
		{
			if (runtimePlatform <= RuntimePlatform.WindowsEditor)
			{
				if (runtimePlatform > RuntimePlatform.WindowsPlayer && runtimePlatform != RuntimePlatform.WindowsEditor)
				{
					return StanleyPlatform.NoVariation;
				}
			}
			else
			{
				if (runtimePlatform == RuntimePlatform.XBOX360)
				{
					return StanleyPlatform.XBOX360;
				}
				if (runtimePlatform != RuntimePlatform.LinuxPlayer)
				{
					return StanleyPlatform.NoVariation;
				}
			}
		}
		else
		{
			if (runtimePlatform > RuntimePlatform.PS4)
			{
				if (runtimePlatform != RuntimePlatform.XboxOne)
				{
					switch (runtimePlatform)
					{
					case RuntimePlatform.Switch:
						return StanleyPlatform.Switch;
					case RuntimePlatform.Lumin:
					case RuntimePlatform.Stadia:
					case RuntimePlatform.CloudRendering:
						return StanleyPlatform.NoVariation;
					case RuntimePlatform.GameCoreScarlett:
					case RuntimePlatform.GameCoreXboxOne:
						break;
					case RuntimePlatform.PS5:
						return StanleyPlatform.Playstation5;
					default:
						return StanleyPlatform.NoVariation;
					}
				}
				return StanleyPlatform.XBOXone;
			}
			if (runtimePlatform != RuntimePlatform.LinuxEditor)
			{
				if (runtimePlatform != RuntimePlatform.PS4)
				{
					return StanleyPlatform.NoVariation;
				}
				return StanleyPlatform.Playstation4;
			}
		}
		return StanleyPlatform.PC;
	}

	[InspectorButton("SetToDebug_Standalone", null)]
	public BooleanConfigurable isStandalone;

	[InspectorButton("SetToDebug_Actual", null)]
	public BooleanConfigurable isConsole;

	[InspectorButton("SetToDebug_Switch", null)]
	public BooleanConfigurable isSwitch;

	public BooleanConfigurable isPlaystationAny;

	[InspectorButton("SetToDebug_PS4", null)]
	public BooleanConfigurable isPlaystation4;

	[InspectorButton("SetToDebug_PS5", null)]
	public BooleanConfigurable isPlaystation5;

	[InspectorButton("SetToDebug_XBOX", null)]
	public BooleanConfigurable isXBOX;

	public BooleanConfigurable calledAchievements;

	public StringConfigurable debugPlatformInEditor;

	[Header("Sprite Sheet Data")]
	public TMP_SpriteAsset keyboardSprites;

	public TMP_SpriteAsset xboxControllerSprites;

	public TMP_SpriteAsset xboxSeXControllerSprites;

	public TMP_SpriteAsset playstationControllerSprites;

	public TMP_SpriteAsset switchControllerSprites;
}
