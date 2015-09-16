using System;
namespace SQM.Website.Interfaces
{
    public interface IFile
    {
        String Name { get; set; }
        String Description { get; set; }
        decimal? DisplayType { get; set; }
        String ID { get; set; }
        decimal? Size { get; set; }
        DateTime UploadedDateTime { get; set; }

        byte[] GetData();
        
    }
}