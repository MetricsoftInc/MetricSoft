using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Reflection;
using SQM.Shared;

namespace SQM.Website
{

    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SQMLoader dataLoader = new SQMLoader().Initialize(1, "");

//            if (dataLoader.LoadPerson() != 99)
//                return;
            if (dataLoader.LoadBusOrgs() != 99)
                return;
//            if (dataLoader.LoadPlants() != 99)
//                return;
//            if (dataLoader.LoadParts() != 99)
 //               return;
//            if (dataLoader.LoadNonConformances() != 99)
//                return;

            using (SQM.Website.PSsqmEntities ctx = new SQM.Website.PSsqmEntities())
            {
                ctx.ContextOptions.LazyLoadingEnabled = false;

                BUSINESS_ORG buOrg = new BUSINESS_ORG();
                buOrg.COMPANY_ID = 3;
                buOrg.BUS_ORG_ID = 2;
                SQMModelMgr.SelectBusOrgList(ctx, 5, 0, true);
                //SQMModelMgr.CreateBusOrgNonConf(ctx, buOrg);
                //List<vw_CompanyNonConformance> NCList = SQMModelMgr.SelectCompanyNCList(1, 1, false);
                //SQMModelMgr.UpdateCompanyNCList(NCList, 1, 1);


                //COMPANY company = new COMPANY();
                //SQMModelMgr.SetGridCode((object)company, company.EntityState, 1);

                //PERSON person = SQMModelMgr.LookupPerson(ctx, "mike", false);


                //COMPANY co = SQMModelMgr.LookupCompany(ctx, "dunsSUPC", false);
                //List<PART> partList = SQMModelMgr.SelectPartList(ctx, 1);
                //PART pt = partList[0];

                //List<PLANT> plantList = SQMModelMgr.SelectPlantList(ctx, 3, 0);
                //PLANT pl = plantList[0];


               // var bu_rec = (from b in ctx.COMPANies
               //               select b).ToList();

               
                //COMPANY comp = bu_rec[1];
                //comp.LAST_UPD_BY = "me";
                //comp.LAST_UPD_DT = DateTime.Now;
               // comp.CS_COMPANY_OVER_F = "Y";

               // comp = (COMPANY)SQMModelMgr.SetPropertyValue((object)comp, "LAST_UPD_BY", "jack");
               // comp = (COMPANY)SQMModelMgr.SetPropertyValue((object)comp, "CS_COMPANY_OVER_F", "Y");

                /*
                COMPANY newcomp = new COMPANY();
                newcomp.SD_ID = 0;
                newcomp.ULT_DUNS_CODE = "duns002";
                newcomp.ULT_GRID_CODE = "";
                newcomp.COMPANY_NAME = newcomp.UNIQUE_ID_NAME = "SUPPLIER B";
                newcomp.CS_COMPANY_OVER_F = "";
                newcomp.STATUS = "A";
                newcomp.CREATE_BY = newcomp.LAST_UPD_BY = "me";
                newcomp.CREATE_DT = newcomp.LAST_UPD_DT = DateTime.Now;
                ctx.COMPANies.AddObject(newcomp);
                */

                //ctx.SaveChanges();

            }
        }

    }
}