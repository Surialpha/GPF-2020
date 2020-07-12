using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftwareFactory.Models;


namespace SoftwareFactory.Controllers
{
    public class HomeController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        // GET: Home
        public ActionResult Index()
        {

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            var imagen = (from imagenes in db.Imagenes select imagenes);

            if (imagen.Any()){
                imagen.ToList();
                ViewBag.Imagenes = imagen;
            }
            else
            {
                ViewBag.Imagenes = null;
            }

            ViewBag.QuienesSomos = (from quienes in db.Quienes_Somos select quienes.descripcion).FirstOrDefault();
            ViewBag.ComoLoHacemos = (from hacemos in db.Como_Lo_Hacemos select hacemos.descripcion).FirstOrDefault();
            ViewBag.Servicio = (from servicio in db.Nuestros_Servicios select servicio).ToList();
            ViewBag.Adquirir = (from adquirir in db.Adquirir_Servicios select adquirir).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Index(pqrs pqr)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }


            try
            {
                if (pqr == null)
                {
                    TempData["Error"] = "¡Error al enviar la información!";
                    return RedirectToAction("Index");
                }

                if (ModelState.IsValid)
                {
                    db.pqrs.Add(pqr);
                    db.SaveChanges();

                    TempData["Success"] = "¡Gracias por comunicarse con nosotros!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "¡Error al enviar la información!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }

        }



        public ActionResult Formulario()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }


            try
            {
                if (Session["Logged"] != null)
                {
                    var rol = int.Parse(Session["Rol"].ToString());
                    if (rol == 3)
                    {
                        return RedirectToAction("Solicitar", "Solicitud");
                    }
                    else
                    {
                        TempData["Error"] = "¡Solo un cliente puede solicitar proyectos!";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                else
                {
                    TempData["Error"] = "¡Para solicitar un proyecto, por favor Inicie sesión o Registrese!";
                    return RedirectToAction("RegistroCliente", "Usuarios");
                }
            }
            catch (Exception)
            {

                TempData["Error"] = "¡Ha ocurrido un error inesperado!";
                return RedirectToAction("Index");
            }
        }




    }
}