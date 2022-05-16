using System;
using UnityEngine;

namespace SoftMasking.Samples
{
	public class ItemsGenerator : MonoBehaviour
	{
		public void Generate()
		{
			this.DestroyChildren();
			int num = Random.Range(0, ItemsGenerator.colors.Length - 1);
			for (int i = 0; i < this.count; i++)
			{
				Item item = Object.Instantiate<Item>(this.itemPrefab);
				item.transform.SetParent(this.target, false);
				item.Set(string.Format("{0} {1:D2}", this.baseName, i + 1), this.image, ItemsGenerator.colors[(num + i) % ItemsGenerator.colors.Length], Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
			}
		}

		private void DestroyChildren()
		{
			while (this.target.childCount > 0)
			{
				Object.DestroyImmediate(this.target.GetChild(0).gameObject);
			}
		}

		public RectTransform target;

		public Sprite image;

		public int count;

		public string baseName;

		public Item itemPrefab;

		private static readonly Color[] colors = new Color[]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.cyan,
			Color.yellow,
			Color.magenta,
			Color.gray
		};
	}
}
