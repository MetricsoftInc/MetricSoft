using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public enum MediaAttachmentType
	{
		Video,
		Text,
		Document,
		Image,
		ReleaseForm
	}

	public class MediaVideoData
	{
		public VIDEO Video
		{
			get;
			set;
		}
		public PERSON Person
		{
			get;
			set;
		}
		public PLANT Plant
		{
			get;
			set;
		}
		public List<VIDEO_ATTACHMENT> VideoTextList
		{
			get;
			set;
		}
		public List<VIDEO_ATTACHMENT> ReleaseFormList
		{
			get;
			set;
		}
	}


	public static class MediaVideoMgr
	{
		public static VIDEO Add(String fileName, String fileExtention, String description, string videoTitle, int sourceType, decimal sourceId, string sourceStep, string injuryType, string bodyPart, string videoType, DateTime videoDate, DateTime incidentDate, Stream file)
		{
			VIDEO ret = null;
			try
			{
				using (PSsqmEntities entities = new PSsqmEntities())
				{
					VIDEO video = new VIDEO();
					//video.FILE_NAME = filename;
					video.DESCRIPTION = description;
					video.TITLE = videoTitle;
					video.SOURCE_TYPE = sourceType;
					video.SOURCE_ID = sourceId;
					video.SOURCE_STEP = sourceStep;

					video.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
					video.BUS_ORG_ID = SessionManager.UserContext.Person.BUS_ORG_ID;
					video.PLANT_ID = SessionManager.UserContext.Person.PLANT_ID;
					video.VIDEO_PERSON = SessionManager.UserContext.Person.PERSON_ID;
					video.CREATE_DT = WebSiteCommon.CurrentUTCTime();
					video.VIDEO_TYPE = videoType; // this is the injury/incident type.  Default to 0 for Media & audit
					video.VIDEO_DT = videoDate;
					video.INCIDENT_DT = incidentDate;
					video.INJURY_TYPES = injuryType;
					video.BODY_PARTS = bodyPart;
					video.VIDEO_STATUS = "";
					video.FILE_NAME = fileName;

					entities.AddToVIDEO(video);
					entities.SaveChanges();

					//read in the file contents
					if (file != null)
					{
						file.Seek(0, SeekOrigin.Begin);
						BinaryReader rdr = new BinaryReader(file);
						byte[] fileData = rdr.ReadBytes((int)file.Length);
						rdr.Close();
						file.Close();

						string cmdText = "INSERT INTO VIDEO_FILE VALUES (NEWID(), @VIDEO_ID, @VIDEO_DATA)";
						SqlParameter[] parameters = new[]{
							new SqlParameter("@VIDEO_ID",  video.VIDEO_ID),
							new SqlParameter("@VIDEO_DATA", fileData)
						};

						int status = entities.ExecuteStoreCommand(cmdText, parameters);
					}
					entities.SaveChanges();

					ret = video;
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
				ret = null;
			}

			return ret;
		}

		public static VIDEO SelectVideoById(decimal videoId)
		{
			var entities = new PSsqmEntities();
			return (from i in entities.VIDEO where i.VIDEO_ID == videoId select i).FirstOrDefault();
		}

		public static List<MediaVideoData> SelectVideoDataById(PSsqmEntities entities, decimal videoId)
		{
			var videoData = new List<MediaVideoData>();
			videoData = (from v in entities.VIDEO
						 join p in entities.PLANT on v.PLANT_ID equals p.PLANT_ID
						 join r in entities.PERSON on v.VIDEO_PERSON equals r.PERSON_ID
						 where ((v.VIDEO_ID == videoId))
						 select new MediaVideoData
						 {
							 Video = v,
							 Plant = p,
							 Person = r
						 }).ToList();
			if (videoData != null)
			{
				var vaList = (from v in entities.VIDEO_ATTACHMENT
							  where (v.VIDEO_ID == videoId)
							  select v).ToList();
				foreach (MediaVideoData data in videoData)
				{
					data.VideoTextList = new List<VIDEO_ATTACHMENT>();
					data.VideoTextList.AddRange(vaList.Where(l => l.VIDEO_ID == data.Video.VIDEO_ID && (MediaAttachmentType)l.RECORD_TYPE == MediaAttachmentType.Text).ToList());
					data.ReleaseFormList = new List<VIDEO_ATTACHMENT>();
					data.ReleaseFormList.AddRange(vaList.Where(l => l.VIDEO_ID == data.Video.VIDEO_ID && (MediaAttachmentType)l.RECORD_TYPE == MediaAttachmentType.ReleaseForm).ToList());
				}
			}
			return videoData;
		}

		public static List<MediaVideoData> SelectVideoList(List<decimal> plantIdList, List<decimal> sourceTypeList, DateTime fromDate, DateTime toDate, string videoStatus, string keywords, List<string> injuryTypeList, List<string> bodyPartList, List<string> videoTypeList)
		{
			var videoList = new List<MediaVideoData>();

			var entities = new PSsqmEntities();
			String[] keyword = keywords.Split(' ');
			bool allTypes = false;
			foreach (string item in videoTypeList)
			{
				if (item == "")
					allTypes = true;
			}

			try
			{
				if (sourceTypeList.Count == 0 || (sourceTypeList.Count == 1 && sourceTypeList[0] == 0))
					videoList = (from v in entities.VIDEO
								 join p in entities.PLANT on v.PLANT_ID equals p.PLANT_ID
								 join r in entities.PERSON on v.VIDEO_PERSON equals r.PERSON_ID
								 where ((v.VIDEO_DT >= fromDate && v.VIDEO_DT <= toDate)
								 && plantIdList.Contains((decimal)v.PLANT_ID))
								 select new MediaVideoData
								 {
									 Video = v,
									 Plant = p,
									 Person = r
								 }).OrderByDescending(l => l.Video.VIDEO_DT).ToList();
				else
					videoList = (from v in entities.VIDEO
								 join p in entities.PLANT on v.PLANT_ID equals p.PLANT_ID
								 join r in entities.PERSON on v.VIDEO_PERSON equals r.PERSON_ID
								 where ((v.VIDEO_DT >= fromDate && v.VIDEO_DT <= toDate)
								 && plantIdList.Contains((decimal)v.PLANT_ID)
								 && sourceTypeList.Contains((decimal)v.SOURCE_TYPE))
								 select new MediaVideoData
								 {
									 Video = v,
									 Plant = p,
									 Person = r
								 }).OrderByDescending(l => l.Video.VIDEO_DT).ToList();

				// select only specified status
				if (videoStatus.Length > 0)
					videoList = videoList.Where(l => l.Video.VIDEO_STATUS == videoStatus).ToList();

				// select specific key words
				if (keywords.Count() > 0)
					videoList = videoList.Where(q => keywords.All(k => q.Video.TITLE.Contains(k)) || keywords.All(k => q.Video.DESCRIPTION.Contains(k))).ToList();

				if (videoTypeList.Count > 0 && !allTypes)
					videoList = videoList.Where(q => videoTypeList.All(k => q.Video.VIDEO_TYPE != null && q.Video.VIDEO_TYPE.Contains(k))).ToList();

				if (injuryTypeList.Count > 0 && injuryTypeList[0] != "0")
					videoList = videoList.Where(q => injuryTypeList.All(k => q.Video.INJURY_TYPES.Contains(k))).ToList();

				if (bodyPartList.Count > 0 && bodyPartList[0] != "0")
					videoList = videoList.Where(q => bodyPartList.All(k => q.Video.BODY_PARTS.Contains(k))).ToList();

				if (videoList != null)
				{
					decimal[] ids = videoList.Select(v => v.Video.VIDEO_ID).Distinct().ToArray();
					var vaList = (from v in entities.VIDEO_ATTACHMENT
								  where (ids.Contains(v.VIDEO_ID))
								  select v).ToList();
					foreach (MediaVideoData data in videoList)
					{
						data.VideoTextList = new List<VIDEO_ATTACHMENT>();
						data.VideoTextList.AddRange(vaList.Where(l => l.VIDEO_ID == data.Video.VIDEO_ID && (MediaAttachmentType)l.RECORD_TYPE == MediaAttachmentType.Text).ToList());
						data.ReleaseFormList = new List<VIDEO_ATTACHMENT>();
						data.ReleaseFormList.AddRange(vaList.Where(l => l.VIDEO_ID == data.Video.VIDEO_ID && (MediaAttachmentType)l.RECORD_TYPE == MediaAttachmentType.ReleaseForm).ToList());
					}


				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return videoList;
		}

		public static List<VIDEO_ATTACHMENT> SelectVideoAttachmentList(decimal videoId, int recordType)
		{
			var videoAttachmentList = new List<VIDEO_ATTACHMENT>();

			try
			{
				var entities = new PSsqmEntities();

				if (recordType > 0)
					videoAttachmentList = (from va in entities.VIDEO_ATTACHMENT
									 where va.VIDEO_ID == videoId && va.RECORD_TYPE == recordType
									 orderby va.TITLE
									 select va).ToList();
				else
				{
					videoAttachmentList = (from va in entities.VIDEO_ATTACHMENT
										   where va.VIDEO_ID == videoId
										   orderby va.TITLE
									 select va).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return videoAttachmentList;
		}

		///// <summary>
		///// Select a list of all Media Videos by company
		///// </summary>
		public static List<VIDEO> SelectVideos(decimal companyId, List<decimal> plantIdList)
		{
			var videos = new List<VIDEO>();

			try
			{
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					videos = (from i in entities.VIDEO
							  orderby i.VIDEO_ID descending
							  select i).ToList();
				}
				else
				{
					videos = (from i in entities.VIDEO
							  where plantIdList.Contains((decimal)i.PLANT_ID)
							  orderby i.VIDEO_ID descending
							  select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return videos;
		}

		public static VIDEO_FILE SelectVideoFileById(decimal videoId)
		{
			var entities = new PSsqmEntities();
			return (from i in entities.VIDEO_FILE where i.VIDEO_ID == videoId select i).FirstOrDefault();
		}

		public static int DeleteVideo(decimal videoId, string fileName)
		{
			int status = 0;
			string delCmd = " IN (" + videoId + ") ";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					// delete all attachments
					List<decimal> attachmentIds = (from a in ctx.VIDEO_ATTACHMENT
												   where a.VIDEO_ID == videoId
												   select a.VIDEO_ATTACH_ID).ToList();

					if (attachmentIds != null && attachmentIds.Count > 0)
					{
						status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO_ATTACHMENT_FILE WHERE VIDEO_ATTACH_ID IN (" + String.Join(",", attachmentIds) + ")");
						status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO_ATTACHMENT WHERE VIDEO_ATTACH_ID IN (" + String.Join(",", attachmentIds) + ")");
					}

					// need to delete video file from the server (if file is stored on server)
					////if (System.IO.File.Exists(fileName))
					////{
					////	// Use a try block to catch IOExceptions, to
					////	// handle the case of the file already being
					////	// opened by another process.
					////	try
					////	{
					////		System.IO.File.Delete(fileName);
					////	}
					////	catch (System.IO.IOException e)
					////	{
					////		//Console.WriteLine(e.Message);
					////	}
					////}

					// delete video from database, when stored in database
					status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO_FILE WHERE VIDEO_ID" + delCmd);

					// delete the video header
					status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO WHERE VIDEO_ID" + delCmd);
				}
				catch (Exception ex)
				{
					SQMLogger.LogException(ex);
				}
			}

			return status;
		}

	}
}