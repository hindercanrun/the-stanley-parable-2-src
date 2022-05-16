using System;
using System.Collections;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StanleyUI
{
	[RequireComponent(typeof(InControlInputModule))]
	public class StanleyInputModuleAssistant : MonoBehaviour
	{
		public ISelectableHolderScreen CurrentHolderScreen { get; private set; }

		public static StanleyInputModuleAssistant Instance
		{
			get
			{
				return EventSystem.current.GetComponent<StanleyInputModuleAssistant>();
			}
		}

		public static void SelectDefaultSelectable(bool forceSelectDefault)
		{
			if (!forceSelectDefault && StanleyInputModuleAssistant.Instance.CurrentHolderScreen.LastSelectable != null)
			{
				EventSystem.current.firstSelectedGameObject = StanleyInputModuleAssistant.Instance.CurrentHolderScreen.LastGameObjectOrNull();
			}
			else
			{
				EventSystem.current.firstSelectedGameObject = StanleyInputModuleAssistant.Instance.CurrentHolderScreen.DefaultGameObjectOrNull();
			}
			if (EventSystem.current.firstSelectedGameObject != null)
			{
				Selectable component = EventSystem.current.firstSelectedGameObject.GetComponent<Selectable>();
				StanleyInputModuleAssistant.Instance.StartCoroutine(StanleyInputModuleAssistant.WaitFrameAndSelect(component));
			}
		}

		public static void RegisterScreenAsNewlyVisible(GameObject holderGameObject, bool forceSelectDefault = false)
		{
			ISelectableHolderScreen component = holderGameObject.GetComponent<ISelectableHolderScreen>();
			StanleyInputModuleAssistant.Instance.CurrentHolderScreen = component;
			StanleyInputModuleAssistant.SelectDefaultSelectable(forceSelectDefault);
			StanleyInputModuleAssistant.Instance.currentHolder_DEBUG = holderGameObject;
			StanleyInputModuleAssistant.Instance.currentLastSelectable_DEBUG = StanleyInputModuleAssistant.Instance.currentHolder_DEBUG.GetComponent<ISelectableHolderScreen>().LastSelectable;
			StanleyInputModuleAssistant.IScreenRegisterNotificationReciever[] components = holderGameObject.GetComponents<StanleyInputModuleAssistant.IScreenRegisterNotificationReciever>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].RegisteredScreenVisible();
			}
		}

		private static IEnumerator WaitFrameAndSelect(Selectable s)
		{
			EventSystem.current.SetSelectedGameObject(null);
			yield return null;
			EventSystem.current.SetSelectedGameObject(null);
			s.Select();
			yield break;
		}

		public static void RegisterUIElementSelection(Selectable selectedSelectable)
		{
			if (StanleyInputModuleAssistant.Instance.CurrentHolderScreen != null && StanleyInputModuleAssistant.Instance.CurrentHolderScreen.LastSelectable != null)
			{
				StanleyInputModuleAssistant.Instance.CurrentHolderScreen.LastSelectable = selectedSelectable;
				StanleyInputModuleAssistant.Instance.currentLastSelectable_DEBUG = selectedSelectable;
			}
		}

		private void Awake()
		{
			this.incontrolInput = base.GetComponent<InControlInputModule>();
			this.eventSystem = base.GetComponent<EventSystem>();
			this.incontrolInput.allowMouseInput = true;
			this.incontrolInput.allowTouchInput = true;
			this.incontrolInput.focusOnMouseHover = true;
		}

		public void Move(MoveDirection direction)
		{
			AxisEventData axisEventData = new AxisEventData(EventSystem.current);
			axisEventData.moveDir = direction;
			axisEventData.selectedObject = EventSystem.current.currentSelectedGameObject;
			ExecuteEvents.Execute<IMoveHandler>(axisEventData.selectedObject, axisEventData, ExecuteEvents.moveHandler);
		}

		private void Update()
		{
			if (ReportUI.REPORTUIACTIVE)
			{
				return;
			}
			if (!GameMaster.PAUSEMENUACTIVE && !Singleton<GameMaster>.Instance.MenuManager.InAMenu)
			{
				return;
			}
			MoveDirection moveDirection = MoveDirection.None;
			if (Singleton<GameMaster>.Instance.stanleyActions.Enabled)
			{
				if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
				{
					moveDirection = MoveDirection.Down;
				}
				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				{
					moveDirection = MoveDirection.Up;
				}
				if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				{
					moveDirection = MoveDirection.Left;
				}
				if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				{
					moveDirection = MoveDirection.Right;
				}
			}
			if (moveDirection != MoveDirection.None)
			{
				GameMaster.CursorVisible = false;
				if (EventSystem.current.currentSelectedGameObject == null)
				{
					StanleyInputModuleAssistant.SelectDefaultSelectable(false);
				}
				else
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					if (this.lastMovePressed == MoveDirection.None)
					{
						this.Move(moveDirection);
						this.nextMoveRepeatTime = realtimeSinceStartup + base.GetComponent<InControlInputModule>().moveRepeatFirstDuration;
					}
					else if (realtimeSinceStartup >= this.nextMoveRepeatTime)
					{
						this.Move(moveDirection);
						this.nextMoveRepeatTime = realtimeSinceStartup + base.GetComponent<InControlInputModule>().moveRepeatDelayDuration;
					}
				}
			}
			else
			{
				this.nextMoveRepeatTime = 0f;
			}
			this.lastMovePressed = moveDirection;
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
			{
				ExecuteEvents.Execute<ISubmitHandler>(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}

		private InControlInputModule incontrolInput;

		[Header("DEBUG ONLY")]
		private GameObject currentHolder_DEBUG;

		private Selectable currentLastSelectable_DEBUG;

		private EventSystem eventSystem;

		private float nextMoveRepeatTime;

		private MoveDirection lastMovePressed = MoveDirection.None;

		public interface IScreenRegisterNotificationReciever
		{
			void RegisteredScreenVisible();
		}
	}
}
