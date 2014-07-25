using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SearchForPro.Controllers
{
   public class ErrorController : Controller
   {
      [Route("error/pagenotfound")]
      public ViewResult PageNotFound()
      {
         Response.StatusCode = (int)HttpStatusCode.NotFound;
         return View();
      }
   }
}
