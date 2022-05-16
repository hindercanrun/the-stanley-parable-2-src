using System;
using TMPro;
using UnityEngine;

public class IntroTimeDisplay : MonoBehaviour
{
	private void Start()
	{
		if (this.timeOutput != null)
		{
			this.timeOutput.Init();
			this.timeOutput.SaveToDiskAll();
		}
	}

	public void RecordTimeToConfigurable()
	{
		if (this.timeOutput != null)
		{
			this.timeOutput.Init();
			this.timeOutput.SetValue(this.GetFormattedTimeForText());
			this.timeOutput.SaveToDiskAll();
		}
	}

	public string GetFormattedTimeForText()
	{
		return string.Concat(new string[]
		{
			this.hours.text,
			":",
			this.minutes.text,
			" ",
			this.ampmText.text
		});
	}

	public static string GetFormattedStringForTime(int minutes, int hours)
	{
		string text;
		string hourString = IntroTimeDisplay.GetHourString(hours, out text);
		string minuteString = IntroTimeDisplay.GetMinuteString(hours);
		return string.Concat(new string[]
		{
			hourString,
			":",
			minuteString,
			" ",
			text
		});
	}

	public static string GetMinuteString(int minute)
	{
		if (minute > 9)
		{
			return minute.ToString();
		}
		return "0" + minute;
	}

	public static string GetHourString(int hour, out string ampmString)
	{
		int num = hour;
		string text = "AM";
		if (num >= 12)
		{
			num -= 12;
			text = "PM";
		}
		if (num == 0)
		{
			num += 12;
		}
		ampmString = text;
		if (num > 9)
		{
			return num.ToString();
		}
		return "0" + num;
	}

	[SerializeField]
	private TextMeshProUGUI hours;

	[SerializeField]
	private TextMeshProUGUI minutes;

	[SerializeField]
	private TextMeshProUGUI ampmText;

	[SerializeField]
	private StringConfigurable timeOutput;
}
