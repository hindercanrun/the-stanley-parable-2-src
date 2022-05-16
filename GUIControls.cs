using System;
using UnityEngine;

public class GUIControls
{
	public static Color RGBSlider(Color c, string label)
	{
		GUI.color = c;
		GUILayout.Label(label, Array.Empty<GUILayoutOption>());
		GUI.color = Color.red;
		c.r = GUILayout.HorizontalSlider(c.r, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUI.color = Color.green;
		c.g = GUILayout.HorizontalSlider(c.g, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUI.color = Color.blue;
		c.b = GUILayout.HorizontalSlider(c.b, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUI.color = Color.white;
		return c;
	}

	public static Color RGBCircle(Color c, string label, Texture2D colorCircle)
	{
		Rect aspectRect = GUILayoutUtility.GetAspectRect(1f);
		aspectRect.height = (aspectRect.width -= 15f);
		Rect position = new Rect(aspectRect.x + 5f, aspectRect.y + aspectRect.width + 20f, aspectRect.width, 15f);
		HSBColor hsbcolor = new HSBColor(c);
		Vector2 vector = new Vector2(aspectRect.x + aspectRect.width / 2f, aspectRect.y + aspectRect.height / 2f);
		if (Input.GetMouseButton(0))
		{
			Vector2 zero = Vector2.zero;
			zero.x = vector.x - Event.current.mousePosition.x;
			zero.y = vector.y - Event.current.mousePosition.y;
			float num = Mathf.Sqrt(zero.x * zero.x + zero.y * zero.y);
			if (num <= aspectRect.width / 2f + 5f)
			{
				num = Mathf.Clamp(num, 0f, aspectRect.width / 2f);
				float num2 = Vector3.Angle(new Vector3(-1f, 0f, 0f), zero);
				if (zero.y < 0f)
				{
					num2 = 360f - num2;
				}
				hsbcolor.h = num2 / 360f;
				hsbcolor.s = num / (aspectRect.width / 2f);
			}
		}
		HSBColor hsbcolor2 = new HSBColor(c);
		hsbcolor2.b = 1f;
		GUI.color = hsbcolor2.ToColor();
		hsbcolor.b = GUI.HorizontalSlider(position, hsbcolor.b, 1f, 0f, "horizontalslider", "horizontalsliderthumb");
		GUI.color = Color.white * hsbcolor.b;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
		GUI.Box(aspectRect, colorCircle, GUIStyle.none);
		Vector2 vector2 = new Vector2(Mathf.Cos(hsbcolor.h * 360f * 0.017453292f), -Mathf.Sin(hsbcolor.h * 360f * 0.017453292f)) * aspectRect.width * hsbcolor.s / 2f;
		GUI.color = c;
		GUI.Box(new Rect(vector2.x - 5f + vector.x, vector2.y - 5f + vector.y, 10f, 10f), "", "ColorcirclePicker");
		GUI.color = Color.white;
		c = hsbcolor.ToColor();
		return c;
	}
}
