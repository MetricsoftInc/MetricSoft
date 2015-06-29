using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
public enum ListStyle
{
	Unix,
	Windows
}

namespace SQMFileUpload
{
	class FTPUtilities
	{
	}
	public class FTPLineResult
	{
		public ListStyle Style { get; set; }
		public string Name { get; set; }
		public DateTime DateTime { get; set; }
		public bool IsDirectory { get; set; }
		public long Size { get; set; }
	}

	public class FTPLineParser
	{
		private Regex winStyle = new Regex(@"^(?<month>\d{1,2})-(?<day>\d{1,2})-(?<year>\d{1,2})\s+(?<hour>\d{1,2}):(?<minutes>\d{1,2})(?<ampm>am|pm)\s+(?<dir>[<]dir[>])?\s+(?<size>\d+)?\s+(?<name>.*)$", RegexOptions.IgnoreCase);
		public FTPLineResult Parse(string line)
		{
			Match match = winStyle.Match(line);
			if (match.Success)
			{
				return ParseMatch(match.Groups, ListStyle.Windows);
			}
			throw new Exception("Invalid line format");
		}
		private FTPLineResult ParseMatch(GroupCollection matchGroups, ListStyle style)
		{
			string dirMatch = (style == ListStyle.Unix ? "d" : "<dir>");
			FTPLineResult result = new FTPLineResult();
			result.Style = style;
			result.IsDirectory = matchGroups["dir"].Value.Equals(dirMatch, StringComparison.InvariantCultureIgnoreCase);
			result.Name = matchGroups["name"].Value;
			result.DateTime = new DateTime(2000 + int.Parse(matchGroups["year"].Value), int.Parse(matchGroups["month"].Value), int.Parse(matchGroups["day"].Value), int.Parse(matchGroups["hour"].Value) + (matchGroups["ampm"].Value.Equals("PM") && matchGroups["hour"].Value != "12" ? 12 : 0), int.Parse(matchGroups["minutes"].Value), 0);
			if (!result.IsDirectory)
				result.Size = long.Parse(matchGroups["size"].Value);
			return result;
		}
	}
	/// <summary>
	/// FTP Class to retreieve files that match an expression and file date cutoff
	/// </summary>
	public class FTPHelper
	{
		/// <summary>
		/// Get a list of files from the FTP server, in a specified path, that match a naming regular expression, and who's file date is after a cutoff
		/// </summary>
		/// <param name="ftpPath">The path the to FTP location</param>
		/// <param name="nameRegex">Regurlar expression to match when finding files</param>
		/// <param name="cutoff">A datetime that files on the FP server must be greater than</param>
		/// <returns></returns>
		public static List<FTPLineResult> GetFilesListSortedByDate(string ftpPath, Regex nameRegex, DateTime cutoff)
		{
			List<FTPLineResult> output = new List<FTPLineResult>();
			FtpWebRequest request = FtpWebRequest.Create(ftpPath) as FtpWebRequest;
			ConfigureProxy(request);
			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			FtpWebResponse response = request.GetResponse() as FtpWebResponse;
			StreamReader directoryReader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
			var parser = new FTPLineParser();
			while (!directoryReader.EndOfStream)
			{
				var result = parser.Parse(directoryReader.ReadLine());
				if (!result.IsDirectory && result.DateTime > cutoff && nameRegex.IsMatch(result.Name))
				{
					output.Add(result);
				}
			}
			// need to ensure the files are sorted in ascending date order
			output.Sort(
				new Comparison<FTPLineResult>(
					delegate(FTPLineResult res1, FTPLineResult res2)
					{
						return res1.DateTime.CompareTo(res2.DateTime);
					}
				)
			);
			return output;
		}
		/// <summary>
		/// set up the web proxy
		/// </summary>
		/// <param name="request"></param>
		private static void ConfigureProxy(FtpWebRequest request)
		{
			request.Proxy = WebRequest.GetSystemWebProxy();
			request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
		}
	}
}
