using System;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
	[ContextMenu("ScreenshotF")]
	private void ScreenshotF()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Front.png");
	}

	[ContextMenu("ScreenshotB")]
	private void ScreenshotB()
	{
		base.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Back.png");
	}

	[ContextMenu("ScreenshotL")]
	private void ScreenshotL()
	{
		base.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Left.png");
	}

	[ContextMenu("ScreenshotR")]
	private void ScreenshotR()
	{
		base.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Right.png");
	}

	[ContextMenu("ScreenshotU")]
	private void ScreenshotU()
	{
		base.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Up.png");
	}

	[ContextMenu("ScreenshotD")]
	private void ScreenshotD()
	{
		base.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		ScreenCapture.CaptureScreenshot("Sky Down.png");
	}
}
