using System;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicAreaPlacer : MonoBehaviour
{
	public void ManuallySetConfigurablesAndPlace()
	{
		this.configurableAreaOrderA.SetValue(this.editorAreaOrderA);
		this.configurableAreaOrderB.SetValue(this.editorAreaOrderB);
		this.configurableAreaOrderC.SetValue(this.editorAreaOrderC);
		this.configurableAreaOrderD.SetValue(this.editorAreaOrderD);
		this.configurableAreaOrderE.SetValue(this.editorAreaOrderE);
		this.configurableAreaOrderA.SaveToDiskAll();
		this.configurableAreaOrderB.SaveToDiskAll();
		this.configurableAreaOrderC.SaveToDiskAll();
		this.configurableAreaOrderD.SaveToDiskAll();
		this.configurableAreaOrderE.SaveToDiskAll();
		this.PlaceAreas();
	}

	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.configurableAreaOrderA.Init();
		this.configurableAreaOrderB.Init();
		this.configurableAreaOrderC.Init();
		this.configurableAreaOrderD.Init();
		this.configurableAreaOrderE.Init();
		this.PlaceAreas();
	}

	private void Update()
	{
	}

	private void PlaceAreas()
	{
		this.dynamicRoomA.transform.position = this.GetNthDRTarget(this.configurableAreaOrderA.GetIntValue()).position;
		this.dynamicRoomB.transform.position = this.GetNthDRTarget(this.configurableAreaOrderB.GetIntValue()).position;
		this.dynamicRoomC.transform.position = this.GetNthDRTarget(this.configurableAreaOrderC.GetIntValue()).position;
		this.dynamicRoomD.transform.position = this.GetNthDRTarget(this.configurableAreaOrderD.GetIntValue()).position;
		this.dynamicRoomE.transform.position = this.GetNthDRTarget(this.configurableAreaOrderE.GetIntValue()).position;
	}

	private void PlaceAreas_EDITOR()
	{
		this.dynamicRoomA.transform.position = this.GetNthDRTarget(this.editorAreaOrderA).position;
		this.dynamicRoomB.transform.position = this.GetNthDRTarget(this.editorAreaOrderB).position;
		this.dynamicRoomC.transform.position = this.GetNthDRTarget(this.editorAreaOrderC).position;
		this.dynamicRoomD.transform.position = this.GetNthDRTarget(this.editorAreaOrderD).position;
		this.dynamicRoomE.transform.position = this.GetNthDRTarget(this.editorAreaOrderE).position;
	}

	private Transform GetNthDRTarget(int drTargetIndex)
	{
		switch (drTargetIndex)
		{
		case 0:
			return this.drTarget1;
		case 1:
			return this.drTarget2;
		case 2:
			return this.drTarget3;
		case 3:
			return this.drTarget4;
		case 4:
			return this.drTarget5;
		default:
			return null;
		}
	}

	[SerializeField]
	private GameObject dynamicRoomA;

	[SerializeField]
	private GameObject dynamicRoomB;

	[SerializeField]
	private GameObject dynamicRoomC;

	[SerializeField]
	private GameObject dynamicRoomD;

	[SerializeField]
	private GameObject dynamicRoomE;

	[SerializeField]
	private Transform drTarget1;

	[SerializeField]
	private Transform drTarget2;

	[SerializeField]
	private Transform drTarget3;

	[SerializeField]
	private Transform drTarget4;

	[SerializeField]
	private Transform drTarget5;

	[SerializeField]
	private IntConfigurable configurableAreaOrderA;

	[SerializeField]
	private IntConfigurable configurableAreaOrderB;

	[SerializeField]
	private IntConfigurable configurableAreaOrderC;

	[SerializeField]
	private IntConfigurable configurableAreaOrderD;

	[SerializeField]
	private IntConfigurable configurableAreaOrderE;

	[Header("Pick up Order [0-4]")]
	public int editorAreaOrderA;

	public int editorAreaOrderB = 1;

	public int editorAreaOrderC = 2;

	public int editorAreaOrderD = 3;

	[InspectorButton("ManuallySetConfigurablesAndPlace", null)]
	public int editorAreaOrderE = 4;

	[SerializeField]
	private bool placeAreasInEditMode;
}
