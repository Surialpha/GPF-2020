using SoftwareFactory.Models;
using System.Web.Mvc;
using System.Linq;

namespace SoftwareFactory.Controllers
{
    public class DashboardController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Logged"] != null)/*Validacion de usuarios logueados*/
            {
                if (Session["Rol"].ToString().Equals("3"))
                {
                    if (TempData["Error"] != null)
                    {
                        ViewBag.Error = TempData["Error"].ToString();
                    }
                    if (TempData["Success"] != null)
                    {
                        ViewBag.Success = TempData["Success"].ToString();

                    }

                    

                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
                else
                {
                    if (TempData["Error"] != null)
                    {
                        ViewBag.Error = TempData["Error"].ToString();
                    }

                    if (TempData["Success"] != null)
                    {
                        ViewBag.Success = TempData["Success"].ToString();

                    }
                    return View();
                }
               
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }


        }

        public ActionResult DashboardCliente()
        {
            if (Session["Logged"] == null)/*Validacion de usuarios logueados*/
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (Session["Rol"].ToString().Equals("3"))
                {
                    if (TempData["Error"] != null)
                    {
                        ViewBag.Error = TempData["Error"].ToString();
                    }

                    if (TempData["Success"] != null)
                    {
                        ViewBag.Success = TempData["Success"].ToString();

                    }

                    return View();
                }
                else
                {
                    if (TempData["Error"] != null)
                    {
                        ViewBag.Error = TempData["Error"].ToString();
                    }
                    if (TempData["Success"] != null)
                    {
                        ViewBag.Success = TempData["Success"].ToString();

                    }
                    return RedirectToAction("Dashboard", "Dashboard");
                }



               }


               

            }


        }
    
}