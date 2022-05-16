using System;

public class ClientCommand : HammerEntity
{
	public void Input_Command(string commands)
	{
		string[] array = commands.Split(new char[]
		{
			' '
		});
		if (array.Length == 2)
		{
			string text = array[0];
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 3195863109U)
			{
				if (num <= 2661608878U)
				{
					if (num != 2205313174U)
					{
						if (num != 2661608878U)
						{
							return;
						}
						if (!(text == "stanley_drawcredits"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.StartCredits();
						return;
					}
					else
					{
						if (!(text == "bucket_animation"))
						{
							return;
						}
						StanleyController.Instance.Bucket.PlayAdditiveAnimation(array[1]);
						return;
					}
				}
				else if (num != 2920344148U)
				{
					if (num != 3195863109U)
					{
						return;
					}
					if (!(text == "changelevel"))
					{
						return;
					}
				}
				else
				{
					if (!(text == "tsp_reload"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.TSP_Reload(int.Parse(array[1]));
					return;
				}
			}
			else if (num <= 3623156416U)
			{
				if (num != 3490291555U)
				{
					if (num != 3623156416U)
					{
						return;
					}
					if (!(text == "stanley_animation"))
					{
						return;
					}
					if (array[1] == "180")
					{
						Singleton<GameMaster>.Instance.EyelidAnimate(EyelidDir.Close);
						return;
					}
					if (array[1] == "-180")
					{
						Singleton<GameMaster>.Instance.EyelidAnimate(EyelidDir.Open);
						return;
					}
					return;
				}
				else
				{
					if (!(text == "crosshair"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.Crosshair(int.Parse(array[1]) == 1);
					return;
				}
			}
			else if (num != 3751997361U)
			{
				if (num != 3785239655U)
				{
					return;
				}
				if (!(text == "tsp_startapartment"))
				{
					return;
				}
				Singleton<GameMaster>.Instance.StartApartment(array[1]);
				return;
			}
			else if (!(text == "map"))
			{
				return;
			}
			if (array[1] == "buttonworld")
			{
				Singleton<GameMaster>.Instance.ChangeLevel("buttonworld_UD_MASTER", true);
				return;
			}
			if (array[1] == "apartment_ending")
			{
				Singleton<GameMaster>.Instance.ChangeLevel("apartment_ending_UD_MASTER", false);
				return;
			}
			Singleton<GameMaster>.Instance.ChangeLevel(array[1], true);
			return;
		}
		else if (array.Length == 1)
		{
			string text = array[0];
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2154907957U)
			{
				if (num <= 1027326733U)
				{
					if (num <= 245567709U)
					{
						if (num != 222801243U)
						{
							if (num != 245567709U)
							{
								return;
							}
							if (!(text == "tsp_bosspass"))
							{
								return;
							}
							Singleton<GameMaster>.Instance.BossPass();
							return;
						}
						else
						{
							if (!(text == "tsp_bossskip"))
							{
								return;
							}
							Singleton<GameMaster>.Instance.BossSkip();
							return;
						}
					}
					else if (num != 358879747U)
					{
						if (num != 661773287U)
						{
							if (num != 1027326733U)
							{
								return;
							}
							if (!(text == "startfloating"))
							{
								return;
							}
							StanleyController.Instance.StartFloating();
							return;
						}
						else
						{
							if (!(text == "tsp_mainmenu"))
							{
								return;
							}
							Singleton<GameMaster>.Instance.TSP_MainMenu();
							return;
						}
					}
					else
					{
						if (!(text == "spawn_prefab"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.SpawnPrefabOnStanley();
						return;
					}
				}
				else if (num <= 1467154740U)
				{
					if (num != 1054894248U)
					{
						if (num != 1467154740U)
						{
							return;
						}
						if (!(text == "stopsound"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.StopSound();
						return;
					}
					else
					{
						if (!(text == "tsp_seriouspass"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.SeriousPass();
						return;
					}
				}
				else if (num != 1829836752U)
				{
					if (num != 1893045531U)
					{
						if (num != 2154907957U)
						{
							return;
						}
						if (!(text == "tsp_unfreezestanley"))
						{
							return;
						}
						StanleyController.Instance.UnfreezeMotion(true);
						StanleyController.Instance.UnfreezeView(true);
						return;
					}
					else
					{
						if (!(text == "tsp_freezemovement"))
						{
							return;
						}
						StanleyController.Instance.FreezeMotion(true);
						StanleyController.Instance.ResetVelocity();
						return;
					}
				}
				else
				{
					if (!(text == "endfloating"))
					{
						return;
					}
					StanleyController.Instance.EndFloating();
					return;
				}
			}
			else if (num <= 2920344148U)
			{
				if (num <= 2498094530U)
				{
					if (num != 2489820645U)
					{
						if (num != 2498094530U)
						{
							return;
						}
						if (!(text == "tsp_loungepass"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.LoungePass();
						return;
					}
					else
					{
						if (!(text == "tsp_countpass"))
						{
							return;
						}
						Singleton<GameMaster>.Instance.CountPass();
						return;
					}
				}
				else if (num != 2657331714U)
				{
					if (num == 2792527138U)
					{
						text == "_888";
						return;
					}
					if (num != 2920344148U)
					{
						return;
					}
					if (!(text == "tsp_reload"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.TSP_Reload();
					return;
				}
				else
				{
					if (!(text == "tsp_human"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.BarkModeOff();
				}
			}
			else if (num <= 3560917644U)
			{
				if (num != 3073127355U)
				{
					if (num != 3560917644U)
					{
						return;
					}
					if (!(text == "tsp_buttonpass"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.ButtonPass();
					return;
				}
				else
				{
					if (!(text == "tsp_bark"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.BarkModeOn();
					return;
				}
			}
			else if (num != 3843093459U)
			{
				if (num != 3862310250U)
				{
					if (num != 4153511315U)
					{
						return;
					}
					if (!(text == "tsp_broompass"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.BroomPass();
					return;
				}
				else
				{
					if (!(text == "tsp_boxes"))
					{
						return;
					}
					Singleton<GameMaster>.Instance.Boxes();
					return;
				}
			}
			else
			{
				if (!(text == "tsp_freezeview"))
				{
					return;
				}
				StanleyController.Instance.FreezeView(true);
				return;
			}
		}
	}
}
