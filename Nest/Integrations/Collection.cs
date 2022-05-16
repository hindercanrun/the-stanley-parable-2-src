using System;
using System.Collections;
using System.Collections.Generic;
using Nest.Components;
using Nest.Util;
using UnityEngine;

namespace Nest.Integrations
{
	public class Collection : BaseIntegration
	{
		public override float InputValue
		{
			set
			{
				if (this.Type != Collection.CollectionType.None)
				{
					Debug.LogWarning("Called InputValue, but value will not be used!!");
				}
				if (this.SelectNewProperty(value))
				{
					this.InvokeSelected();
				}
			}
		}

		public Collection.Item Selected
		{
			get
			{
				if (this._selectedIndex < this.Items.Length && this._selectedIndex >= 0)
				{
					return this.Items[this._selectedIndex];
				}
				return null;
			}
		}

		public void Start()
		{
			this.@switch = new Dictionary<Type, Action>
			{
				{
					typeof(AudioClip),
					new Action(this.InvokeAudio)
				},
				{
					typeof(NestInput),
					new Action(this.InvokeNest)
				}
			};
			if (this.Type == Collection.CollectionType.ShuffleBag)
			{
				this._shuffledItems = new ShuffleBag<Collection.Item>(this.Items);
			}
		}

		public void Invoke()
		{
			this.SelectNewProperty((float)this._selectedIndex);
			this.InvokeSelected();
		}

		public void Invoke(int value)
		{
			this.SelectNewProperty((float)value);
			this.InvokeSelected();
		}

		private bool SelectNewProperty(float value)
		{
			switch (this.Type)
			{
			case Collection.CollectionType.None:
			{
				int num = Mathf.FloorToInt(value);
				if (this._selectedIndex == num)
				{
					return false;
				}
				this._selectedIndex = num;
				return true;
			}
			case Collection.CollectionType.Sequence:
				this._selectedIndex = int.MinValue;
				return true;
			case Collection.CollectionType.ShuffleBag:
				this._selectedIndex = this._shuffledItems.NextIndex();
				return true;
			case Collection.CollectionType.DefinedRandom:
				this._selectedIndex = this.RandomWeighted(new int?(Mathf.FloorToInt(value * 100f)));
				return true;
			case Collection.CollectionType.UnityRandom:
				this._selectedIndex = this.RandomWeighted(null);
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void InvokeSelected()
		{
			if (-2147483648 == this._selectedIndex)
			{
				if (!this._sequenceRunning)
				{
					base.StartCoroutine(this.InvokeSequence());
				}
				return;
			}
			try
			{
				if (this.Selected != null && !(this.Selected.Object == null))
				{
					this.@switch[this.Selected.Object.GetType()]();
				}
			}
			catch (KeyNotFoundException arg)
			{
				Debug.LogError(string.Format("Collection doesn't support {0} at index {1}... {2}", this.Selected.Object.GetType(), this._selectedIndex, arg));
			}
		}

		private void InvokeAudio()
		{
			if (this.Source == null && (this.Source = Camera.main.GetComponent<AudioSource>()) == null)
			{
				this.Source = Camera.main.gameObject.AddComponent<AudioSource>();
			}
			this.Source.clip = (this.Selected.Object as AudioClip);
			this.Source.Play();
		}

		private void InvokeNest()
		{
			((NestInput)this.Selected.Object).Invoke();
		}

		private IEnumerator InvokeSequence()
		{
			if (this._sequenceRunning)
			{
				yield break;
			}
			this._sequenceRunning = true;
			int num;
			for (int i = 0; i < this.Items.Length; i = num + 1)
			{
				Collection.Item item = this.Items[i];
				this._selectedIndex = i;
				if (item.Object != null)
				{
					this.@switch[item.Object.GetType()]();
				}
				yield return new WaitForGameSeconds(item.Duration);
				num = i;
			}
			this._sequenceRunning = false;
			yield break;
		}

		private int RandomWeighted(int? value)
		{
			bool flag = value != null;
			int num = 0;
			int num2 = value ?? Random.Range(0, 101);
			int i;
			for (i = 0; i < this.Items.Length; i++)
			{
				num += Mathf.FloorToInt(this.Items[i].Weight);
				if (num >= num2)
				{
					break;
				}
			}
			return i;
		}

		public float RepeatInvokeTime;

		public Collection.CollectionType Type = Collection.CollectionType.DefinedRandom;

		public Collection.Item[] Items;

		public AudioSource Source;

		private Dictionary<Type, Action> @switch;

		[SerializeField]
		private int _selectedIndex;

		private bool _sequenceRunning;

		private ShuffleBag<Collection.Item> _shuffledItems;

		private const int _sequenceSelected = -2147483648;

		public enum CollectionType
		{
			None,
			Sequence,
			ShuffleBag,
			DefinedRandom,
			UnityRandom
		}

		[Serializable]
		public class Item
		{
			public float Weight = 50f;

			public float Duration = 1f;

			public Object Object;
		}
	}
}
