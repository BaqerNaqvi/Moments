using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;
using Moments.RepositoryDAL;
using System.Web.Mvc;

namespace Moments.Utility
{
    public class MVCUtility
    {
        public SelectList GetLookupsByLookupType(bool? IsActive, string LookupType)
        {
            SuperAdminDAL DAL = new SuperAdminDAL();
            List<sp_GetLookupsByLookupsType_Result> GetLookups = new List<sp_GetLookupsByLookupsType_Result>();
            GetLookups = DAL.GetLookupsByLookupType(IsActive, LookupType);
            List<SelectListItem> Lookups = new List<SelectListItem>();
            foreach (sp_GetLookupsByLookupsType_Result LookupNames in GetLookups)
            {
                Lookups.Add(new SelectListItem { Selected = false, Text = LookupNames.LookupName, Value = LookupNames.Id.ToString() });
            }
            return new SelectList(Lookups, "Value", "Text", null);
        }
        public SelectList GetLookupsByLookupTypeTranslatedAsValueForUserType(bool? IsActive, string LookupType, string UserType)
        {
            SuperAdminDAL DAL = new SuperAdminDAL();
            ViewModels.SuperAdminViewModel viewModel = new ViewModels.SuperAdminViewModel();
            List<sp_GetLookupsByLookupsType_Result> GetLookups = new List<sp_GetLookupsByLookupsType_Result>();
            if (UserType == "SuperAdmin")
            {
                GetLookups = DAL.GetLookupsByLookupType(IsActive, LookupType);
            }
            else if (UserType == "Admin")
            {
                GetLookups = DAL.GetLookupsByLookupType(IsActive, LookupType).Where(x => x.LookupTranslate == "Customer" || x.LookupTranslate == "Admin").ToList();
            }
            else if (UserType == "Customer")
            {
                GetLookups = DAL.GetLookupsByLookupType(IsActive, LookupType).Where(x => x.LookupTranslate == "Customer").ToList();
            }
            else
            {
                GetLookups = DAL.GetLookupsByLookupType(IsActive, LookupType);
            }

            List<SelectListItem> Lookups = new List<SelectListItem>();
            foreach (sp_GetLookupsByLookupsType_Result LookupNames in GetLookups)
            {
                Lookups.Add(new SelectListItem { Selected = false, Text = LookupNames.LookupName, Value = LookupNames.LookupTranslate });
            }
            return new SelectList(Lookups, "Value", "Text", null);
        }
    }
}