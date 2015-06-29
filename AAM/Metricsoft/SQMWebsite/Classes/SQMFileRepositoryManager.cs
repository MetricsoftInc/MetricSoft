using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using SQM.Website.Interfaces;

namespace SQM.Website.Classes
{
    public class SQMFileRepositoryManager
    {
        private IUnityContainer all_repositories;

        public IUnityContainer AllRepositories
        {
            get {
                    IUnityContainer container = new UnityContainer();
                    container.LoadConfiguration();
                   return container;
                }
            set { }
        }
        
       

        public SQMFileRepositoryManager()
        {
           
        }

        public List<IFile> Get()
        {
            ISQMFileRepository rep = AllRepositories.Resolve<ISQMFileRepository>();
            List<IFile> ret = rep.Get();
            return ret;
        }

        public IFile Get(String doc_id)
        {
            ISQMFileRepository rep = AllRepositories.Resolve<ISQMFileRepository>();
            IFile ret = rep.Get(doc_id);
            return ret;
        }

        public void Delete(String doc_id)
        {
            ISQMFileRepository rep = AllRepositories.Resolve<ISQMFileRepository>();
            rep.Delete(doc_id);
            return;
        }

        public List<IFile> Get(decimal?[] displayTypes)
        {
            ISQMFileRepository rep = AllRepositories.Resolve<ISQMFileRepository>();
            List<IFile> ret = rep.Get(displayTypes);
            return ret;
        }
    }
}