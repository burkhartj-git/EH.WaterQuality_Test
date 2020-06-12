using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;

namespace EHWaterQuality.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult Generic()
        {
            Response.StatusCode = 500;
            return View();
        }

        [HttpGet]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult InternalServerError(string ErrorMessage = "")
        {
            //Response.StatusCode = 500;

            EHWQ_Error appError = new EHWQ_Error();

            try
            {
                if (Session["ErrorMessage"] == null && ErrorMessage == "")
                {
                    appError.EHWQ_ErrorMessage = "";
                }
                else
                {
                    if (ErrorMessage == "")
                    {
                        appError.EHWQ_ErrorMessage = Session["ErrorMessage"].ToString();
                    }
                    else
                    {
                        appError.EHWQ_ErrorMessage = ErrorMessage;
                    }
                }
            }
            catch (Exception ex) { };

            return View(appError);
        }

        [HttpGet]
        public ActionResult Unauthorized()
        {
            Response.StatusCode = 401;
            return View();
        }

        [HttpGet]
        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}