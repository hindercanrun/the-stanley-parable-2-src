using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMP_ManualWrap : MonoBehaviour
{
	private string GetWordWrappedText(TMP_TextInfo ti)
	{
		string text = ti.textComponent.text;
		TMP_ManualWrap.IntStringPair[] array;
		string text2 = this.ExtractTags(text, out array);
		int num = 0;
		foreach (TMP_LineInfo tmp_LineInfo in ti.lineInfo)
		{
			num += tmp_LineInfo.characterCount;
			text2 = text2.Insert(num, "\n");
			num++;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].i >= num)
				{
					TMP_ManualWrap.IntStringPair[] array2 = array;
					int num2 = j;
					array2[num2].i = array2[num2].i + 1;
				}
			}
			if (num >= text2.Length)
			{
				break;
			}
		}
		for (int k = array.Length - 1; k >= 0; k--)
		{
			text2 = text2.Insert(array[k].i, array[k].s);
		}
		return text2.Replace("\n\n", "\n");
	}

	private string ExtractTags(string raw, out TMP_ManualWrap.IntStringPair[] tagsArray)
	{
		List<TMP_ManualWrap.IntStringPair> list = new List<TMP_ManualWrap.IntStringPair>();
		for (int i = 0; i < 1000; i++)
		{
			int num = raw.IndexOf('<');
			int num2 = raw.IndexOf('>');
			if (num == -1 || num2 == -1 || num2 < num)
			{
				break;
			}
			int num3 = num2 - num;
			list.Add(new TMP_ManualWrap.IntStringPair
			{
				i = num,
				s = raw.Substring(num, num3 + 1)
			});
			raw = raw.Remove(num, num3 + 1);
		}
		tagsArray = list.ToArray();
		return raw;
	}

	private void Start()
	{
		this.tmp.OnPreRenderText += this.Tmp_OnPreRenderText;
	}

	private void Tmp_OnPreRenderText(TMP_TextInfo textInfo)
	{
		if (this.inOnPreRender)
		{
			return;
		}
		this.inOnPreRender = true;
		this.tmp.text = this.GetWordWrappedText(textInfo);
		this.inOnPreRender = false;
	}

	public TMP_Text tmp;

	private bool inOnPreRender;

	private struct IntStringPair
	{
		public int i;

		public string s;
	}
}
