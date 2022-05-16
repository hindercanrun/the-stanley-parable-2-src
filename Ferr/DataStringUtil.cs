using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ferr
{
	public static class DataStringUtil
	{
		public static string Encrypt(string aData, string aKey = null)
		{
			if (string.IsNullOrEmpty(aKey))
			{
				aKey = DataStringUtil._key;
			}
			byte[] bytes = Encoding.Unicode.GetBytes(aData);
			using (Aes aes = Aes.Create())
			{
				Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(aKey, new byte[]
				{
					73,
					118,
					97,
					110,
					32,
					77,
					101,
					100,
					118,
					101,
					100,
					101,
					118
				});
				aes.Key = rfc2898DeriveBytes.GetBytes(32);
				aes.IV = rfc2898DeriveBytes.GetBytes(16);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(bytes, 0, bytes.Length);
						cryptoStream.Close();
					}
					aData = Convert.ToBase64String(memoryStream.ToArray());
				}
			}
			return aData;
		}

		public static string Decrypt(string aData, string aKey = null)
		{
			if (string.IsNullOrEmpty(aKey))
			{
				aKey = DataStringUtil._key;
			}
			aData = aData.Replace(" ", "+");
			byte[] array = Convert.FromBase64String(aData);
			using (Aes aes = Aes.Create())
			{
				Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(aKey, new byte[]
				{
					73,
					118,
					97,
					110,
					32,
					77,
					101,
					100,
					118,
					101,
					100,
					101,
					118
				});
				aes.Key = rfc2898DeriveBytes.GetBytes(32);
				aes.IV = rfc2898DeriveBytes.GetBytes(16);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(array, 0, array.Length);
						cryptoStream.Close();
					}
					aData = Encoding.Unicode.GetString(memoryStream.ToArray());
				}
			}
			return aData;
		}

		public static List<string> SplitSmart(string aData, char aSeparator)
		{
			List<string> list = new List<string>();
			string text = "";
			char c = ' ';
			int num = 0;
			string text2 = aData.Trim();
			if (text2.StartsWith("{"))
			{
				text2 = text2.Substring(1, text2.Length - 2);
			}
			int i = 0;
			while (i < text2.Length)
			{
				char c2 = text2[i];
				if (c == ' ')
				{
					if (c2 == aSeparator)
					{
						list.Add(text);
						text = "";
					}
					else
					{
						if (c2 == '{')
						{
							c = '}';
							goto IL_A1;
						}
						if (c2 == '"')
						{
							c = '"';
							goto IL_A1;
						}
						if (c2 == '\'')
						{
							c = '\'';
							goto IL_A1;
						}
						goto IL_A1;
					}
				}
				else if (c2 == c)
				{
					if (num == 0)
					{
						c = ' ';
						goto IL_A1;
					}
					num--;
					goto IL_A1;
				}
				else
				{
					if (c2 == '{')
					{
						num++;
						goto IL_A1;
					}
					goto IL_A1;
				}
				IL_AF:
				i++;
				continue;
				IL_A1:
				text += c2.ToString();
				goto IL_AF;
			}
			if (text.Length > 0)
			{
				list.Add(text);
			}
			if (c != ' ')
			{
				return null;
			}
			return list;
		}

		private static string _key = "FerrDataStringUtilDefaultKey";
	}
}
