using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmailReporter : MonoBehaviour
{
	private string logPath
	{
		get
		{
			return Environment.ExpandEnvironmentVariables(string.Concat(new string[]
			{
				"%USERPROFILE%/AppData/LocalLow/",
				Application.companyName,
				"/",
				Application.productName,
				"/Player.log"
			}));
		}
	}

	private string logPathCopy
	{
		get
		{
			return Environment.ExpandEnvironmentVariables(string.Concat(new string[]
			{
				"%USERPROFILE%/AppData/LocalLow/",
				Application.companyName,
				"/",
				Application.productName,
				"/Player_logCopy.txt"
			}));
		}
	}

	public bool SendReport(string userName, string userPassword, string userSmtp, string userReceivingAddress, string title, string description, List<Texture2D> screenshots, out string message)
	{
		this.from = userName;
		this.password = userPassword;
		this.smtp = userSmtp;
		this.receivingAddress = userReceivingAddress;
		this.subject = title;
		this.body = description;
		this.attachedScreenshots = screenshots;
		return this.SendEmail(out message);
	}

	private void SetEmailing(bool dir)
	{
		this.isemailing = dir;
		Time.timeScale = (dir ? 0f : 1f);
	}

	private bool SendEmail(out string message)
	{
		MailMessage mailMessage = new MailMessage();
		string text = "";
		int num = 0;
		foreach (Texture2D tex in this.attachedScreenshots)
		{
			byte[] bytes = tex.EncodeToJPG();
			string text2 = string.Format(Application.dataPath + "/../reportscreenshot_{0}.jpg", num);
			File.WriteAllBytes(text2, bytes);
			LinkedResource linkedResource = new LinkedResource(text2);
			linkedResource.ContentId = Guid.NewGuid().ToString();
			Attachment attachment = new Attachment(text2);
			attachment.ContentDisposition.Inline = true;
			mailMessage.Attachments.Add(attachment);
			text += string.Format("<img src=\"cid:{0}\" />", linkedResource.ContentId);
			num++;
		}
		if (File.Exists(this.logPath))
		{
			File.Copy(this.logPath, this.logPathCopy, true);
			Attachment attachment2 = new Attachment(this.logPathCopy);
			attachment2.ContentDisposition.Inline = true;
			mailMessage.Attachments.Add(attachment2);
		}
		mailMessage.From = new MailAddress(this.from);
		mailMessage.To.Add(this.receivingAddress);
		mailMessage.Subject = this.subject;
		mailMessage.Body = string.Format(string.Concat(new string[]
		{
			this.body,
			"<br/><br/><hr>",
			text,
			"<br/>Scene: ",
			SceneManager.GetActiveScene().name,
			"<br/>",
			this.GetHardwareInfo(),
			"<br/><hr>"
		}), Array.Empty<object>());
		mailMessage.IsBodyHtml = true;
		SmtpClient smtpClient = new SmtpClient(this.smtp);
		smtpClient.Port = 587;
		smtpClient.EnableSsl = true;
		smtpClient.Credentials = new NetworkCredential(this.from, this.password);
		ServicePointManager.ServerCertificateValidationCallback = ((object obj, X509Certificate cert, X509Chain chain, SslPolicyErrors sslerrors) => true);
		message = "Report sent!";
		try
		{
			smtpClient.Send(mailMessage);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			message = ex.Message;
			this.SetEmailing(false);
			return false;
		}
		this.SetEmailing(false);
		return true;
	}

	private string GetHardwareInfo()
	{
		return string.Concat(new string[]
		{
			"Graphics Device Name: ",
			SystemInfo.graphicsDeviceName,
			"<br/>Graphics Device Type: ",
			SystemInfo.graphicsDeviceType.ToString(),
			"<br/>Graphics Device Version: ",
			SystemInfo.graphicsDeviceVersion,
			"<br/>Graphics Memory Size: ",
			this.MBtoGB(SystemInfo.graphicsMemorySize),
			"<br/>Graphics Shader Level: ",
			this.ShaderLevel(SystemInfo.graphicsShaderLevel),
			"<br/>Maximum Texture Size: ",
			this.MBtoGB(SystemInfo.maxTextureSize),
			"<br/>Operating System: ",
			SystemInfo.operatingSystem,
			"<br/>Processor Type: ",
			SystemInfo.processorType,
			"<br/>Processor Count: ",
			SystemInfo.processorCount.ToString(),
			"<br/>Processor Frequency: ",
			SystemInfo.processorFrequency.ToString(),
			"<br/>System Memory Size: ",
			this.MBtoGB(SystemInfo.systemMemorySize),
			"<br/>Screen Size: ",
			Screen.width.ToString(),
			"x",
			Screen.height.ToString()
		});
	}

	private string ShaderLevel(int level)
	{
		if (level <= 30)
		{
			if (level == 20)
			{
				return "Shader Model 2.x";
			}
			if (level == 30)
			{
				return "Shader Model 3.0";
			}
		}
		else
		{
			if (level == 40)
			{
				return "Shader Model 4.0 ( DX10.0 )";
			}
			if (level == 41)
			{
				return "Shader Model 4.1 ( DX10.1 )";
			}
			if (level == 50)
			{
				return "Shader Model 5.0 ( DX11.0 )";
			}
		}
		return "";
	}

	private string MBtoGB(int mb)
	{
		return Mathf.Ceil((float)mb / 1024f).ToString() + "GB";
	}

	private IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot("debugscreenshot.png");
		this.SetEmailing(!this.isemailing);
		yield break;
	}

	private string from = "";

	private string password = "";

	private string smtp;

	private string receivingAddress = "";

	private bool isemailing;

	private string subject;

	private string body = "";

	private List<Texture2D> attachedScreenshots = new List<Texture2D>();
}
