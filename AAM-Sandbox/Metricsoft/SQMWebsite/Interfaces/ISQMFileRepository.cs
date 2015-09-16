using System;
namespace SQM.Website.Interfaces
{
    public interface ISQMFileRepository
    {
        IFile Add(string filename, string description, decimal? display_type, System.IO.Stream file);
        void Delete(string Document_ID);
        System.Collections.Generic.List<IFile> Get();
        IFile Get(string doc_id);
        System.Collections.Generic.List<IFile> Get(decimal?[] displayTypes);
        string GetImageSourceString(IFile doc);
        string GetImageSourceString(decimal docID);
    }
}
