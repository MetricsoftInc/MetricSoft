using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;

namespace SQM.Website.Admin
{
    public partial class Administrate_DefaultDocuments : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uclDocMgr.BindDocMgr("SYS", 0, 0);
        }
      
    }
}