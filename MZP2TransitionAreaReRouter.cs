using System;
using UnityEngine;

[ExecuteInEditMode]
public class MZP2TransitionAreaReRouter : MonoBehaviour
{
	private int OrderA
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.roomAEditorOrder;
			}
			return this.roomAOrder.GetIntValue() + 1;
		}
	}

	private int OrderB
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.roomBEditorOrder;
			}
			return this.roomBOrder.GetIntValue() + 1;
		}
	}

	private int OrderC
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.roomCEditorOrder;
			}
			return this.roomCOrder.GetIntValue() + 1;
		}
	}

	private int OrderD
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.roomDEditorOrder;
			}
			return this.roomDOrder.GetIntValue() + 1;
		}
	}

	private int OrderE
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.roomEEditorOrder;
			}
			return this.roomEOrder.GetIntValue() + 1;
		}
	}

	public void ManuallySetConfigurablesAndPlace()
	{
		this.roomAOrder.SetValue(this.roomAEditorOrder - 1);
		this.roomBOrder.SetValue(this.roomBEditorOrder - 1);
		this.roomCOrder.SetValue(this.roomCEditorOrder - 1);
		this.roomDOrder.SetValue(this.roomDEditorOrder - 1);
		this.roomEOrder.SetValue(this.roomEEditorOrder - 1);
		this.roomAOrder.SaveToDiskAll();
		this.roomBOrder.SaveToDiskAll();
		this.roomCOrder.SaveToDiskAll();
		this.roomDOrder.SaveToDiskAll();
		this.roomEOrder.SaveToDiskAll();
		this.MoveAreas();
		this.WentThroughLeftChoiceDoor();
	}

	private Transform GetIntro(int i)
	{
		if (i == this.OrderA)
		{
			return this.voidIntroA;
		}
		if (i == this.OrderB)
		{
			return this.voidIntroB;
		}
		if (i == this.OrderC)
		{
			return this.voidIntroC;
		}
		if (i == this.OrderD)
		{
			return this.voidIntroD;
		}
		if (i == this.OrderE)
		{
			return this.voidIntroE;
		}
		return null;
	}

	private Transform GetExit(int i)
	{
		if (i == this.OrderA)
		{
			return this.exhibitExitA;
		}
		if (i == this.OrderB)
		{
			return this.exhibitExitB;
		}
		if (i == this.OrderC)
		{
			return this.exhibitExitC;
		}
		if (i == this.OrderD)
		{
			return this.exhibitExitD;
		}
		if (i == this.OrderE)
		{
			return this.exhibitExitE;
		}
		return null;
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			if (this.GetIntro(1) == null || this.GetIntro(2) == null || this.GetIntro(3) == null || this.GetIntro(4) == null || this.GetIntro(5) == null || this.GetExit(1) == null || this.GetExit(2) == null || this.GetExit(3) == null || this.GetExit(4) == null || this.GetExit(5) == null)
			{
				Debug.LogError("FigleyCollection Order Incorrect, resetting to a default");
				Debug.LogError("OrderA " + this.OrderA);
				Debug.LogError("OrderB " + this.OrderB);
				Debug.LogError("OrderC " + this.OrderC);
				Debug.LogError("OrderD " + this.OrderD);
				Debug.LogError("OrderE " + this.OrderE);
				this.roomAOrder.SetValue(0);
				this.roomBOrder.SetValue(1);
				this.roomCOrder.SetValue(2);
				this.roomDOrder.SetValue(3);
				this.roomEOrder.SetValue(4);
				this.roomAOrder.SaveToDiskAll();
				this.roomBOrder.SaveToDiskAll();
				this.roomCOrder.SaveToDiskAll();
				this.roomDOrder.SaveToDiskAll();
				this.roomEOrder.SaveToDiskAll();
			}
			this.MoveAreas();
		}
	}

	private void Update()
	{
		if (!Application.isPlaying && this.moveAreasInEditMode)
		{
			this.MoveAreas();
		}
	}

	private void MoveAreas()
	{
		this.Link(this.preambleAreaExit, this.GetIntro(1), "preambleAreaExit, GetIntro(1)");
		this.Link(this.GetExit(1), this.choiceIntro, "GetExit(1), choiceIntro");
		this.Link(this.choiceLeftExit, this.GetIntro(2), "choiceLeftExit, GetIntro(2)");
		this.Link(this.choiceRightExit, this.GetIntro(3), "choiceRightExit, GetIntro(3)");
		this.PinkIntroBackTrack.gameObject.SetActive(false);
		if (!Application.isPlaying)
		{
			if (this.wentThroughLeftDoor_EDITOR)
			{
				this.WentThroughLeftChoiceDoor();
			}
			else
			{
				this.WentThroughRightChoiceDoor();
			}
			this.PreformBacktrackPortalSwitch();
		}
		this.Link(this.PinkExit, this.GetIntro(4), "PinkExit, GetIntro(4)");
		this.room4VideoPlayer.position = this.GetExit(4).position;
		this.Link(this.GetExit(4), this.shortPathExit, "GetExit(4), shortPathExit");
		this.Link(this.shortPathEnter, this.GetIntro(5), "shortPathEnter, GetIntro(5)");
		this.Link(this.GetExit(5), this.FinalIntro, "GetExit(5), FinalIntro");
	}

	private void LinkLeft()
	{
		this.Link(this.GetExit(2), this.PinkIntro, "LinkLeft: GetExit(2), PinkIntro");
		this.Link(this.GetExit(3), this.PinkIntroBackTrack, "LinkLeft: GetExit(3), PinkIntroBackTrack");
	}

	private void LinkRight()
	{
		this.Link(this.GetExit(3), this.PinkIntro, "LinkRight: GetExit(3), PinkIntro");
		this.Link(this.GetExit(2), this.PinkIntroBackTrack, "LinkRight: GetExit(2), PinkIntroBackTrack");
	}

	public void WentThroughLeftChoiceDoor()
	{
		this.LinkLeft();
		this.wentLeft = true;
	}

	public void WentThroughRightChoiceDoor()
	{
		this.LinkRight();
		this.wentLeft = false;
	}

	public void PreformBacktrackPortalSwitch()
	{
		if (this.reverseLinksOnBackwardTravel)
		{
			if (this.wentLeft)
			{
				this.LinkRight();
			}
			else
			{
				this.LinkLeft();
			}
			this.PinkIntroBackTrack.gameObject.SetActive(true);
		}
	}

	public void OG_WentThroughLeftChoiceDoor()
	{
		this.Link(this.GetExit(3), this.PinkIntro, "");
		this.Link(this.GetExit(2), this.PinkIntroBackTrack, "");
	}

	public void OG_WentThroughRightChoiceDoor()
	{
		this.Link(this.GetExit(2), this.PinkIntro, "");
		this.Link(this.GetExit(3), this.PinkIntroBackTrack, "");
	}

	private void Link(Transform exit, Transform entrance, string debug = "")
	{
		if (exit == null && entrance == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Missing Link - exit: ",
				exit,
				", entrance:",
				entrance
			}));
		}
		entrance.position = exit.position;
		entrance.rotation = exit.rotation;
	}

	public bool moveAreasInEditMode;

	[Header("Preamble Area")]
	public Transform preambleAreaExit;

	[Header("A-E")]
	public Transform voidIntroA;

	public Transform exhibitExitA;

	public Transform voidIntroB;

	public Transform exhibitExitB;

	public Transform voidIntroC;

	public Transform exhibitExitC;

	public Transform voidIntroD;

	public Transform exhibitExitD;

	public Transform voidIntroE;

	public Transform exhibitExitE;

	[Header("Short Path")]
	public Transform shortPathExit;

	public Transform shortPathEnter;

	[Header("Choice Room")]
	public Transform choiceIntro;

	public Transform choiceLeftExit;

	public Transform choiceRightExit;

	[Header("Pink Room")]
	public Transform PinkIntro;

	public Transform PinkIntroBackTrack;

	public Transform PinkExit;

	[Header("Pink Room")]
	public Transform FinalIntro;

	[Header("VideoPlayer")]
	public Transform room4VideoPlayer;

	[Header("Configurables")]
	public IntConfigurable roomAOrder;

	public IntConfigurable roomBOrder;

	public IntConfigurable roomCOrder;

	public IntConfigurable roomDOrder;

	public IntConfigurable roomEOrder;

	[Header("Editor Tools")]
	public bool wentThroughLeftDoor_EDITOR;

	public int roomAEditorOrder = 1;

	public int roomBEditorOrder = 2;

	public int roomCEditorOrder = 3;

	public int roomDEditorOrder = 4;

	[InspectorButton("ManuallySetConfigurablesAndPlace", null)]
	public int roomEEditorOrder = 5;

	private bool wentLeft = true;

	private bool reverseLinksOnBackwardTravel;
}
