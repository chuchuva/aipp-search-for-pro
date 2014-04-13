using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using SearchForPro.Helpers;
using System.Xml.Linq;

namespace SearchForPro.Controllers
{
   public class ProfileController : Controller
   {
      [Route("")]
      public ActionResult Index()
      {
         var model = GetAllPhotographers();
         return View(model);
      }

      [Route("{id:int}/{slug?}")]
      public ActionResult Show(int id, string slug)
      {
         dynamic photographer;
         using (var connection = GetOpenConnection())
         {
            photographer = connection.Query(
               @"select n.ID, First_Name, Last_Name, Company, Work_Phone, Toll_Free,
               Email, Website, Category, Designation, Profile, Member_Logo_Path 
               from Name n inner join member_info m on n.ID = m.ID 
               where n.id = @id and (Member_Type = 'A' or Member_Type = 'AS')",
               new { id }).FirstOrDefault();
         }

         if (photographer == null)
            return Content("not found");
         photographer.Name = photographer.First_Name + " " + photographer.Last_Name;
         photographer.LogoPath = (string.IsNullOrEmpty(photographer.Member_Logo_Path) ?
            "/IS/img/nologo.jpg" : string.Format("/ProfileImages/{0}/{1}",
            photographer.ID, photographer.Member_Logo_Path));
         if (!string.IsNullOrEmpty(photographer.Website) &&
             photographer.Website.StartsWith("http://"))
         {
            photographer.Website = photographer.Website.Substring(7);
         }
         photographer.ProfileHtml = HtmlUtilities.MarkdownMini(photographer.Profile);
         ViewBag.Title = photographer.Name;
         return View(photographer);
      }

      [Route("sitemap")]
      public ActionResult Sitemap()
      {
         var photographers = GetAllPhotographers();
         XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
         XElement contacts =
            new XElement(ns + "urlset",
               photographers.Select(p => new XElement(ns + "url",
                  new XElement(ns + "loc", GetAbsoluteUrl(p.ID, p.Slug)),
                  new XElement(ns + "lastmod", (p.Last_Updated != null) ?
                     p.Last_Updated.ToString("yyyy-MM-dd") : "2014-01-01")
               ))
            );
         return Content(contacts.ToString(), "text/xml");
      }

      private string GetAbsoluteUrl(string id, string slug)
      {
         return "http://www.aipp.com.au" + Url.Action("Show", new { id, slug });
      }

      private static IEnumerable<dynamic> GetAllPhotographers()
      {
         int topCount = (int)(DateTime.Now - new DateTime(2014, 4, 8)).TotalHours * 10;
         IEnumerable<dynamic> list;
         using (var connection = GetOpenConnection())
         {
            list = connection.Query(string.Format(
               @"select top {0} ID, First_Name, Last_Name, Last_Updated 
               from Name where ID in 
                  (select ContactMaster from UserMain where IsDisabled = 0) and
               (Member_Type = 'A' or Member_Type = 'AS')
               order by Last_Name", topCount));
         }
         foreach (var photographer in list)
         {
            photographer.Name = photographer.First_Name + " " + photographer.Last_Name;
            photographer.Slug = HtmlUtilities.URLFriendly(photographer.Name);
         }
         return list;
      }

      private static SqlConnection GetOpenConnection()
      {
         var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[
            "DataSource.iMIS.Connection"].ConnectionString);
         connection.Open();
         return connection;
      }
   }
}
