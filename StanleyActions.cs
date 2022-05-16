using System;
using System.Collections.Generic;
using I2.Loc;
using InControl;

public class StanleyActions : PlayerActionSet
{
	public PlayerAction ExtraAction(int index, bool overrideSimpleControls = false)
	{
		if (!Singleton<GameMaster>.Instance.UsingSimplifiedControls || overrideSimpleControls)
		{
			return this.ExtraActions[index];
		}
		return this.UseAction;
	}

	public PlayerAction UseAction
	{
		get
		{
			if (Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse || !Singleton<GameMaster>.Instance.UsingSimplifiedControls)
			{
				return this.Use;
			}
			return this.AnyButton;
		}
	}

	public PlayerAction JumpAction
	{
		get
		{
			if (!Singleton<GameMaster>.Instance.UsingSimplifiedControls)
			{
				return this.Jump;
			}
			return this.UseAction;
		}
	}

	public event Action OnKeyBindingsSaved;

	public event Action OnKeyBindingsLoaded;

	public PlayerAction HoleTeleportAction
	{
		get
		{
			return this.ExtraActions[StanleyActions.HoleTeleportIndex];
		}
	}

	public static int HoleTeleportIndex
	{
		get
		{
			return 4;
		}
	}

	public StanleyActions.KeyControllerPairSpriteTags GetExtraActionBindingDescription(int i)
	{
		return StanleyActions.GetPlayerActionBindingDescription(this.ExtraAction(i, false));
	}

	public StanleyActions.KeyControllerPairSpriteTags GetJumpBindingDescription()
	{
		return StanleyActions.GetPlayerActionBindingDescription(this.JumpAction);
	}

	public StanleyActions.KeyControllerPairSpriteTags GetUseBindingDescription()
	{
		return StanleyActions.GetPlayerActionBindingDescription(this.UseAction);
	}

	private static StanleyActions.KeyControllerPairSpriteTags GetPlayerActionBindingDescription(PlayerAction a)
	{
		StanleyActions.KeyControllerPair keyControllerPair = default(StanleyActions.KeyControllerPair);
		KeyBindingSource keyBindingSource = new List<BindingSource>(a.Bindings).Find((BindingSource x) => x.BindingSourceType == BindingSourceType.KeyBindingSource) as KeyBindingSource;
		keyControllerPair.key = ((keyBindingSource != null) ? keyBindingSource.Control.GetInclude(0) : Key.None);
		DeviceBindingSource deviceBindingSource = new List<BindingSource>(a.Bindings).Find((BindingSource x) => x.BindingSourceType == BindingSourceType.DeviceBindingSource) as DeviceBindingSource;
		keyControllerPair.gamepadInput = ((deviceBindingSource != null) ? deviceBindingSource.Control : InputControlType.None);
		StanleyActions.KeyControllerPairSpriteTags defaultSpriteTags = keyControllerPair.GetDefaultSpriteTags();
		if (Singleton<GameMaster>.Instance.UsingSimplifiedControls)
		{
			defaultSpriteTags.GamepadSpriteTag = LocalizationManager.GetTranslation("Controls_AnyButton", true, 0, true, false, null, null);
		}
		return defaultSpriteTags;
	}

	public int GetExtraActionInputsLength()
	{
		return StanleyActions.ExtraActionInputs.Length;
	}

	public StanleyActions()
	{
		this.MoveForward = base.CreatePlayerAction("Move Forward");
		this.MoveBackward = base.CreatePlayerAction("Move Backward");
		this.MoveLeft = base.CreatePlayerAction("Move Left");
		this.MoveRight = base.CreatePlayerAction("Move Right");
		this.Movement = base.CreateTwoAxisPlayerAction(this.MoveLeft, this.MoveRight, this.MoveBackward, this.MoveForward);
		this.LookUp = base.CreatePlayerAction("Look Up");
		this.LookDown = base.CreatePlayerAction("Look Down");
		this.LookLeft = base.CreatePlayerAction("Look Left");
		this.LookRight = base.CreatePlayerAction("Look Right");
		this.View = base.CreateTwoAxisPlayerAction(this.LookLeft, this.LookRight, this.LookDown, this.LookUp);
		this.Up = base.CreatePlayerAction("Up");
		this.Down = base.CreatePlayerAction("Down");
		this.Left = base.CreatePlayerAction("Left");
		this.Right = base.CreatePlayerAction("Right");
		this.Crouch = base.CreatePlayerAction("Crouch");
		this.Jump = base.CreatePlayerAction("Jump");
		this.Use = base.CreatePlayerAction("Interact With Item");
		this.Start = base.CreatePlayerAction("Start");
		this.FastForward = base.CreatePlayerAction("Fast Forward Time");
		this.SlowDown = base.CreatePlayerAction("Slow Time");
		this.ExtraActions = new PlayerAction[StanleyActions.ExtraActionInputs.Length];
		for (int i = 0; i < StanleyActions.ExtraActionInputs.Length; i++)
		{
			this.ExtraActions[i] = base.CreatePlayerAction("ExtraAction" + (i + 1));
		}
		this.MenuTabLeft = base.CreatePlayerAction("Menu Tab Left");
		this.MenuTabRight = base.CreatePlayerAction("Menu Tab Right");
		this.MenuOpen = base.CreatePlayerAction("Menu Open");
		this.MenuBack = base.CreatePlayerAction("Menu Back");
		this.MenuConfirm = base.CreatePlayerAction("Menu Confirm");
		this.AnyButton = base.CreatePlayerAction("Any Button");
	}

	public IEnumerable<PlayerAction> UsedInGameActions
	{
		get
		{
			yield return this.MoveForward;
			yield return this.MoveBackward;
			yield return this.MoveLeft;
			yield return this.MoveRight;
			yield return this.Use;
			yield return this.Crouch;
			if (!Singleton<GameMaster>.Instance.UsingSimplifiedControls)
			{
				yield return this.Jump;
			}
			yield break;
		}
	}

	public static StanleyActions CreateWithDefaultBindings()
	{
		StanleyActions stanleyActions = new StanleyActions();
		stanleyActions.BindKeyboardDefaults();
		stanleyActions.BindControllerButtons();
		return stanleyActions;
	}

	public void BindControllerButtons()
	{
		this.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.LookRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.MoveForward.AddDefaultBinding(InputControlType.LeftStickUp);
		this.MoveBackward.AddDefaultBinding(InputControlType.LeftStickDown);
		this.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
		this.Up.AddDefaultBinding(InputControlType.DPadUp);
		this.Down.AddDefaultBinding(InputControlType.DPadDown);
		this.Left.AddDefaultBinding(InputControlType.DPadLeft);
		this.Right.AddDefaultBinding(InputControlType.DPadRight);
		this.Crouch.AddDefaultBinding(InputControlType.LeftTrigger);
		this.Crouch.AddDefaultBinding(InputControlType.LeftBumper);
		this.Jump.AddDefaultBinding(StanleyActions.JumpButton);
		this.Use.AddDefaultBinding(StanleyActions.ConfirmButton);
		this.FastForward.AddDefaultBinding(InputControlType.LeftBumper);
		this.SlowDown.AddDefaultBinding(InputControlType.RightBumper);
		for (int i = 0; i < StanleyActions.ExtraActionInputs.Length; i++)
		{
			this.ExtraActions[i].AddDefaultBinding(StanleyActions.ExtraActionInputs[i].gamepadInput);
		}
		this.MenuTabLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.MenuTabRight.AddDefaultBinding(InputControlType.RightBumper);
		this.MenuBack.AddDefaultBinding(StanleyActions.BackButton);
		this.MenuBack.AddDefaultBinding(InputControlType.Command);
		this.MenuBack.AddDefaultBinding(InputControlType.Options);
		this.MenuBack.AddDefaultBinding(InputControlType.Back);
		this.MenuOpen.AddDefaultBinding(InputControlType.Command);
		this.MenuOpen.AddDefaultBinding(InputControlType.Options);
		this.MenuConfirm.AddDefaultBinding(StanleyActions.ConfirmButton);
		this.MenuConfirm.AddDefaultBinding(InputControlType.Select);
		this.AnyButton.AddBinding(new DeviceBindingSource(InputControlType.Action1));
		this.AnyButton.AddBinding(new DeviceBindingSource(InputControlType.Action2));
		this.AnyButton.AddBinding(new DeviceBindingSource(InputControlType.Action3));
		this.AnyButton.AddBinding(new DeviceBindingSource(InputControlType.Action4));
	}

	private static InputControlType ConfirmButton
	{
		get
		{
			return PlatformGamepad.ConfirmButton;
		}
	}

	private static InputControlType BackButton
	{
		get
		{
			return PlatformGamepad.BackButton;
		}
	}

	private static InputControlType JumpButton
	{
		get
		{
			return PlatformGamepad.JumpButton;
		}
	}

	public void BindKeyboardDefaults()
	{
		this.MoveForward.AddDefaultBinding(new Key[]
		{
			Key.W
		});
		this.MoveBackward.AddDefaultBinding(new Key[]
		{
			Key.S
		});
		this.MoveLeft.AddDefaultBinding(new Key[]
		{
			Key.A
		});
		this.MoveRight.AddDefaultBinding(new Key[]
		{
			Key.D
		});
		this.Crouch.AddDefaultBinding(new Key[]
		{
			Key.LeftControl
		});
		this.Jump.AddDefaultBinding(new Key[]
		{
			Key.Space
		});
		this.Use.AddDefaultBinding(new Key[]
		{
			Key.E
		});
		this.Use.AddDefaultBinding(Mouse.LeftButton);
		this.FastForward.AddDefaultBinding(new Key[]
		{
			Key.Shift
		});
		this.SlowDown.AddDefaultBinding(new Key[]
		{
			Key.Tab
		});
		for (int i = 0; i < StanleyActions.ExtraActionInputs.Length; i++)
		{
			this.ExtraActions[i].AddDefaultBinding(new Key[]
			{
				StanleyActions.ExtraActionInputs[i].key
			});
		}
		this.MenuTabLeft.AddDefaultBinding(new Key[]
		{
			Key.Q
		});
		this.MenuTabRight.AddDefaultBinding(new Key[]
		{
			Key.E
		});
		this.MenuBack.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.MenuOpen.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.MenuConfirm.AddDefaultBinding(new Key[]
		{
			Key.Return
		});
	}

	public void UnBindAll()
	{
		this.MoveForward.ClearBindings();
		this.MoveBackward.ClearBindings();
		this.MoveLeft.ClearBindings();
		this.MoveRight.ClearBindings();
		this.LookUp.ClearBindings();
		this.LookDown.ClearBindings();
		this.LookLeft.ClearBindings();
		this.LookRight.ClearBindings();
		this.Up.ClearBindings();
		this.Down.ClearBindings();
		this.Left.ClearBindings();
		this.Right.ClearBindings();
		this.Crouch.ClearBindings();
		this.Jump.ClearBindings();
		this.Use.ClearBindings();
		this.FastForward.ClearBindings();
		this.SlowDown.ClearBindings();
		for (int i = 0; i < StanleyActions.ExtraActionInputs.Length; i++)
		{
			this.ExtraActions[i].ClearBindings();
		}
		this.MenuTabLeft.ClearBindings();
		this.MenuTabRight.ClearBindings();
		this.MenuOpen.ClearBindings();
		this.MenuBack.ClearBindings();
		this.MenuConfirm.ClearBindings();
		this.AnyButton.ClearBindings();
	}

	public void ResetKeyBindings(StringConfigurable keybindingsConfigurable)
	{
		base.Reset();
		this.SaveCustomKeyBindings(keybindingsConfigurable);
	}

	public void SaveCustomKeyBindings(StringConfigurable keybindingsConfigurable)
	{
		string value = Convert.ToBase64String(base.SaveData());
		keybindingsConfigurable.SetValue(value);
		keybindingsConfigurable.SaveToDiskAll();
		Action onKeyBindingsSaved = this.OnKeyBindingsSaved;
		if (onKeyBindingsSaved == null)
		{
			return;
		}
		onKeyBindingsSaved();
	}

	public void LoadCustomKeyBindings(StringConfigurable keybindingsConfigurable)
	{
		string stringValue = keybindingsConfigurable.GetStringValue();
		if (string.IsNullOrEmpty(stringValue))
		{
			return;
		}
		byte[] data = Convert.FromBase64String(stringValue);
		base.LoadData(data);
		Action onKeyBindingsLoaded = this.OnKeyBindingsLoaded;
		if (onKeyBindingsLoaded == null)
		{
			return;
		}
		onKeyBindingsLoaded();
	}

	public PlayerAction MoveForward;

	public PlayerAction MoveBackward;

	public PlayerAction MoveLeft;

	public PlayerAction MoveRight;

	public PlayerTwoAxisAction Movement;

	public PlayerAction LookUp;

	public PlayerAction LookDown;

	public PlayerAction LookLeft;

	public PlayerAction LookRight;

	public PlayerTwoAxisAction View;

	public PlayerAction Up;

	public PlayerAction Down;

	public PlayerAction Left;

	public PlayerAction Right;

	public PlayerAction Crouch;

	protected PlayerAction Jump;

	protected PlayerAction Use;

	public PlayerAction Start;

	protected PlayerAction AnyButton;

	protected PlayerAction[] ExtraActions;

	public PlayerAction MenuTabLeft;

	public PlayerAction MenuTabRight;

	public PlayerAction MenuConfirm;

	public PlayerAction MenuBack;

	public PlayerAction MenuOpen;

	private static StanleyActions.KeyControllerPair[] ExtraActionInputs = new StanleyActions.KeyControllerPair[]
	{
		new StanleyActions.KeyControllerPair
		{
			key = Key.F,
			gamepadInput = InputControlType.Action1
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.G,
			gamepadInput = InputControlType.Action2
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.H,
			gamepadInput = InputControlType.Action3
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.J,
			gamepadInput = InputControlType.Action4
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.G,
			gamepadInput = InputControlType.Action4
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.B,
			gamepadInput = InputControlType.DPadUp
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.C,
			gamepadInput = InputControlType.DPadDown
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.Q,
			gamepadInput = InputControlType.DPadLeft
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.I,
			gamepadInput = InputControlType.DPadRight
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.K,
			gamepadInput = InputControlType.Action1
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.L,
			gamepadInput = InputControlType.Action2
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.M,
			gamepadInput = InputControlType.Action3
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.N,
			gamepadInput = InputControlType.Action4
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.O,
			gamepadInput = InputControlType.DPadUp
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.P,
			gamepadInput = InputControlType.DPadDown
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.R,
			gamepadInput = InputControlType.DPadLeft
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.T,
			gamepadInput = InputControlType.DPadRight
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.U,
			gamepadInput = InputControlType.Action1
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.V,
			gamepadInput = InputControlType.Action2
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.X,
			gamepadInput = InputControlType.Action3
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.Y,
			gamepadInput = InputControlType.Action4
		},
		new StanleyActions.KeyControllerPair
		{
			key = Key.Z,
			gamepadInput = InputControlType.DPadUp
		}
	};

	public PlayerAction FastForward;

	public PlayerAction SlowDown;

	protected struct KeyControllerPair
	{
		public StanleyActions.KeyControllerPairSpriteTags GetDefaultSpriteTags()
		{
			return new StanleyActions.KeyControllerPairSpriteTags
			{
				KeyboardSpriteTag = "<sprite name=\"" + this.KeyTag(this.key.ToString()) + "\">",
				GamepadSpriteTag = "<sprite name=\"" + this.gamepadInput.ToString() + "\">"
			};
		}

		private string KeyTag(string playerActionName)
		{
			if (playerActionName.Length != 1)
			{
				return playerActionName.ToLower();
			}
			return playerActionName;
		}

		public Key key;

		public InputControlType gamepadInput;
	}

	public struct KeyControllerPairSpriteTags
	{
		public string KeyboardSpriteTag;

		public string GamepadSpriteTag;
	}
}
