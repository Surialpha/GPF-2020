using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftwareFactory.Filtros;
using SoftwareFactory.Models;

namespace SoftwareFactory.Controllers
{
    public class AsesoresController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        // GET: Asesores
        [AuthorizeUserModules(1)]
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
            return View();
        }

        [AuthorizeUserModules(1)]
        public ActionResult Lista()
        { 
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var asesores = (from ases in db.Asesores
                                join pers in db.Personas on ases.id_asesor equals pers.documento
                                select new { nombre = pers.nombres + " " + pers.apellidos, email = pers.email, idasesor = ases.id_asesor, inicontra = ases.inicio_contrato.ToString(), fincontra = ases.fin_contrato.ToString() }
                                ).ToList();
                return Json(new { data = asesores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var asesores = new { };
                return Json(new { data = asesores }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Asesores/Details/5
        [AuthorizeUserModules(1)]
        public ActionResult Details(int? id)
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
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Asesores asesores = db.Asesores.Find(id);
                if (asesores == null)
                {
                    return HttpNotFound();
                }
                return View(asesores);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
            
        }

        // GET: Asesores/Create
        [AuthorizeUserModules(1)]
        public ActionResult Create()
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
                ViewBag.id_asesor = (from pers in db.Personas
                                     join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                     where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                     && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                     && usuarios.id_rol == 4
                                     select pers
                                   ).ToList();
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult Create([Bind(Include = "id_asesor,inicio_contrato,fin_contrato")] Asesores asesores)
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
                if (ModelState.IsValid)
                {
                    TempData["Success"] = "¡Registro exitoso!";
                    db.Asesores.Add(asesores);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.id_asesor = (from pers in db.Personas
                                     join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                     where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                     && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                     && usuarios.id_rol == 4
                                     select pers
                                       ).ToList();
                ViewBag.Error = "¡No se puedo registrar el asesor, intenta nuevamente!";
                return View(asesores);
            }
            catch (Exception)
            {
                ViewBag.id_asesor = (from pers in db.Personas
                                     join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                     where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                     && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                     && usuarios.id_rol == 4
                                     select pers
                                       ).ToList();
                ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return View();
            }
          
        }

        [AuthorizeUserModules(1)]
        public ActionResult Edit(int? id)
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
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Asesores asesores = db.Asesores.Find(id);
                if (asesores == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_asesor = asesores.id_asesor;
                return View(asesores);
            }
            catch (Exception)
            {
                ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Asesores asesores = db.Asesores.Find(id);
                if (asesores == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Error ="Ha occurrido un error";
                ViewBag.id_asesor = asesores.id_asesor;
                return View(asesores);
            }
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult Edit([Bind(Include = "id_asesor,inicio_contrato,fin_contrato")] Asesores asesores)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Success"] = "Modificado con exito";
                    db.Entry(asesores).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.id_asesor = asesores.id_asesor;
                ViewBag.Error = "¡No fue posible modificar el asesor, intenta nuevamente!";
                return View(asesores);
            }
            catch (Exception)
            {
                ViewBag.id_asesor = asesores.id_asesor;
                ViewBag.Error = "¡No fue posible modificar el asesor, intenta nuevamente!";
                return View(asesores);
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Asesores asesores = db.Asesores.Find(id);
                if (asesores == null)
                {
                    return HttpNotFound();
                }
                return View(asesores);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }

            
        }

        // POST: Asesores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Asesores asesores = db.Asesores.Find(id);
                db.Asesores.Remove(asesores);
                db.SaveChanges(); 
                TempData["Success"] = "¡Se ha eliminado correctamente el asesor!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Asesores asesores = db.Asesores.Find(id);
                ViewBag.Error = "El asesor no puede ser eliminado actualmente, asegurese no tenga actividades pendientes";
                return View(asesores);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
