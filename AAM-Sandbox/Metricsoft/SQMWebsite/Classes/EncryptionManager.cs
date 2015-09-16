using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SQM.Website
{
	public static class EncryptionManager
	{
		private const string DefaultEncryptionKey = "luxluXinT";

		static private byte[] key = {
		
		};

		static private byte[] IV = {
			0x12,
			0x34,
			0x56,
			0x78,
			0x90,
			0xab,
			0xcd,
			0xef
		};

		public static string Decrypt(string stringToDecrypt)
		{
			return Decrypt(stringToDecrypt, DefaultEncryptionKey);
		}

		public static string Decrypt(string stringToDecrypt, string customEncryptionKey)
		{
			byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
			try
			{
				key = System.Text.Encoding.UTF8.GetBytes(Left(customEncryptionKey, 8));
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				inputByteArray = Convert.FromBase64String(stringToDecrypt);
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				System.Text.Encoding encoding = System.Text.Encoding.UTF8;
				return encoding.GetString(ms.ToArray());
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static string Encrypt(string stringToEncrypt)
		{
			return Encrypt(stringToEncrypt, DefaultEncryptionKey);
		}

		public static string Encrypt(string stringToEncrypt, string customEncryptionKey)
		{
			try
			{
				key = System.Text.Encoding.UTF8.GetBytes(Left(customEncryptionKey, 8));
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				return Convert.ToBase64String(ms.ToArray());
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		static string Left(this string str, int length)
		{
			return str.Substring(0, Math.Min(length, str.Length));
		}


	}
}