using DataManagement.Models;
using Resty.Utilities;
using System.Web.Mvc;

namespace Resty.Controllers
{
    public class ActivationController : Controller
    {
        [HttpGet]
        public ActionResult ActivateAccount(string activationToken)
        {
            if (string.IsNullOrEmpty(activationToken))
                ViewBag.Successful = "false";

            ServiceCallResultModel serviceCallResultModel = AccountUtilities.ActivateAccount(activationToken);

            if (!serviceCallResultModel.bSuccessful)
            {
                ViewBag.Successful = "false";
            }
            else
            {
                ViewBag.Successful = "true";
            }

            return View();
        }
    }
}