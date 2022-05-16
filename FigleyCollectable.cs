using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FigleyCollectable : MonoBehaviour
{
	private void Awake()
	{
		this.figleyOmegaFoundConfigurable.Init();
		this.figleyOmegaFoundConfigurable.SaveToDiskAll();
		BooleanConfigurable booleanConfigurable = this.figleyOmegaFoundConfigurable;
		booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConditions));
		if (this.figleyCollectionOrderConfigurable != null)
		{
			this.figleyCollectionOrderConfigurable.Init();
			this.figleyCollectionOrderConfigurable.SaveToDiskAll();
			IntConfigurable intConfigurable = this.figleyCollectionOrderConfigurable;
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConditions));
		}
		this.figleyPostCompletionCharArrayConfigurable.Init();
		this.figleyPostCompletionCharArrayConfigurable.SaveToDiskAll();
		if (FigleyCollectable.FigleysInCrossHairRange_STATIC == null)
		{
			FigleyCollectable.FigleysInCrossHairRange_STATIC = new HashSet<FigleyCollectable>();
		}
		FigleyCollectable.FigleysInCrossHairRange_STATIC.Clear();
		this.CheckCrosshairStatus();
	}

	private void Start()
	{
		this.CheckConditions(null);
	}

	private void OnDisable()
	{
		this.OnCrosshairTriggerExit();
	}

	private void OnDestroy()
	{
		this.OnCrosshairTriggerExit();
		if (this.figleyOmegaFoundConfigurable != null)
		{
			BooleanConfigurable booleanConfigurable = this.figleyOmegaFoundConfigurable;
			booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConditions));
		}
		if (this.figleyCollectionOrderConfigurable != null)
		{
			IntConfigurable intConfigurable = this.figleyCollectionOrderConfigurable;
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConditions));
		}
	}

	public void OnCrosshairTriggerEnter()
	{
		FigleyCollectable.FigleysInCrossHairRange_STATIC.Add(this);
		this.CheckCrosshairStatus();
	}

	public void OnCrosshairTriggerExit()
	{
		FigleyCollectable.FigleysInCrossHairRange_STATIC.Remove(this);
		this.CheckCrosshairStatus();
	}

	public void ClearAllCrosshairs()
	{
		FigleyCollectable.FigleysInCrossHairRange_STATIC.Clear();
	}

	private void CheckCrosshairStatus()
	{
		Singleton<GameMaster>.Instance.Crosshair(FigleyCollectable.FigleysInCrossHairRange_STATIC.Count != 0);
	}

	private void CheckConditions(LiveData __ignored__ = null)
	{
		if (this.IsAPostColletable)
		{
			if (FigleyCollectable.IsPostCollectableCollected(this.figleyPostCompletionCharArrayConfigurable, this.postCollectableIndex))
			{
				this.OnHidden.Invoke();
				return;
			}
			this.OnVisible.Invoke();
			return;
		}
		else if (this.figleyCollectionOrderConfigurable == null)
		{
			if (!this.figleyOmegaFoundConfigurable.GetBooleanValue())
			{
				this.OnVisible.Invoke();
				return;
			}
			this.OnHidden.Invoke();
			return;
		}
		else
		{
			if (this.figleyOmegaFoundConfigurable.GetBooleanValue() && this.figleyCollectionOrderConfigurable.GetIntValue() == -1)
			{
				this.OnVisible.Invoke();
				return;
			}
			this.OnHidden.Invoke();
			return;
		}
	}

	public void StartCollectableRoutine()
	{
		if (this.IsAPostColletable)
		{
			FigleyCollectable.MarkPostCollectableAsCollected(this.figleyPostCompletionCharArrayConfigurable, this.postCollectableIndex);
			FigleyOverlayController.Instance.StartFigleyPostCollection();
			this.OnHidden.Invoke();
			return;
		}
		if (this.figleyCollectionOrderConfigurable == null)
		{
			this.figleyOmegaFoundConfigurable.SetValue(true);
			this.figleyOmegaFoundConfigurable.SaveToDiskAll();
		}
		else
		{
			this.figleyCollectionOrderConfigurable.SetValue(FigleyOverlayController.Instance.FiglysFound);
			this.figleyCollectionOrderConfigurable.SaveToDiskAll();
		}
		FigleyOverlayController.Instance.StartFigleyCollectionRoutine();
	}

	public static char[] GetPostCollectableCharArray(StringConfigurable countArray, int referenceIndex)
	{
		List<char> list = new List<char>(countArray.GetStringValue().ToCharArray());
		while (list.Count <= referenceIndex)
		{
			list.Add(FigleyCollectable.NotCollectedChar);
		}
		return list.ToArray();
	}

	private bool IsAPostColletable
	{
		get
		{
			return this.postCollectableIndex != -1;
		}
	}

	public static bool IsPostCollectableCollected(StringConfigurable countArray, int referenceIndex)
	{
		return FigleyCollectable.GetPostCollectableCharArray(countArray, referenceIndex)[referenceIndex] == FigleyCollectable.CollectedChar;
	}

	public static void MarkPostCollectableAsCollected(StringConfigurable countArray, int referenceIndex)
	{
		char[] postCollectableCharArray = FigleyCollectable.GetPostCollectableCharArray(countArray, referenceIndex);
		if (referenceIndex >= 0)
		{
			postCollectableCharArray[referenceIndex] = FigleyCollectable.CollectedChar;
		}
		countArray.SetValue(new string(postCollectableCharArray));
		countArray.SaveToDiskAll();
	}

	[SerializeField]
	private BooleanConfigurable figleyOmegaFoundConfigurable;

	[Header("Keep null if this is OMEGA Figley")]
	[SerializeField]
	private IntConfigurable figleyCollectionOrderConfigurable;

	[Header("-1 menas not a post collectalbe")]
	[SerializeField]
	[Range(0f, 64f)]
	private int postCollectableIndex = -1;

	[SerializeField]
	private StringConfigurable figleyPostCompletionCharArrayConfigurable;

	[SerializeField]
	private UnityEvent OnVisible;

	[SerializeField]
	private UnityEvent OnHidden;

	private static HashSet<FigleyCollectable> FigleysInCrossHairRange_STATIC = null;

	public static char NotCollectedChar = '_';

	public static char CollectedChar = 'X';
}
