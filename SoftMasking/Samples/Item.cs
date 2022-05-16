using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoftMasking.Samples
{
	public class Item : MonoBehaviour
	{
		public void Set(string name, Sprite sprite, Color color, float health, float damage)
		{
			if (this.image)
			{
				this.image.sprite = sprite;
				this.image.color = color;
			}
			if (this.title)
			{
				this.title.text = name;
			}
			if (this.description)
			{
				this.description.text = "The short description of " + name;
			}
			if (this.healthBar)
			{
				this.healthBar.anchorMax = new Vector2(health, 1f);
			}
			if (this.damageBar)
			{
				this.damageBar.anchorMax = new Vector2(damage, 1f);
			}
		}

		public Image image;

		public Text title;

		public Text description;

		public RectTransform healthBar;

		public RectTransform damageBar;
	}
}
