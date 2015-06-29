using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SQM.Website
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            int timeout = 60;
            try
            {
                string tmo = System.Configuration.ConfigurationManager.AppSettings["TimeoutOverride"];
                int tmoValue;
                if (int.TryParse(tmo, out tmoValue)  &&  tmoValue > 0)
                    timeout = tmoValue;
            }
            catch
            {
            }
            HttpContext.Current.Session.Timeout = timeout;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
			try
			{
				Exception ex = Server.GetLastError().GetBaseException();
				//Exception ex = Server.GetLastError();
				
				if (ex != null)
				{
					HttpContext.Current.Session["LastError"] = ex;

					Guid errorIndex = Guid.NewGuid();

					SQMLogger.LogException(ex, errorIndex);

					if (HttpContext.Current.Session != null)
					{
						HttpContext.Current.Session["ErrorIndex"] = errorIndex.ToString();
						//BDW: comment out redirect to error page for now, because we don't
						//     want to mask exceptions while in development mode.
						//Response.Redirect("Error.aspx", false);
					}
				}
			}
			catch { }
            //Server.ClearError();  
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}