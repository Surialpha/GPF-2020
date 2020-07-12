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
    public class AprendicesController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        // GET: Aprendices
        [AuthorizeUserModules(1)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeUserModules(1)]
        public JsonResult Lista()
        {

            try
            {

                    db.Configuration.ProxyCreationEnabled = false;
                    var aprendices = (from apren in db.Aprendices
                                      join pers in db.Personas on apren.id_aprendiz equals pers.documento
                                      join cont in db.Tipo_Contrato on apren.id_contrato equals cont.id_tipo_contrato
                                      select new
                                      {
                                          id_aprendiz = apren.id_aprendiz,
                                          nombre = pers.nombres + " " + pers.apellidos,
                                          email = pers.email,
                                          finContrato = apren.fin_contrato.ToString()
                                      ,
                                          inicioContrato = apren.inicio_contrato.ToString(),
                                          tipoContrato = cont.descripcion
                                      }).ToList();

                    return Json(new { data = aprendices }, JsonRequestBehavior.AllowGet);
                

            }
            catch (Exception)
            {
                var aprendices = new { };
                    return Json(new { data = aprendices }, JsonRequestBehavior.AllowGet);
            }
           

        }

        [AuthorizeUserModules(1)]
        // GET: Aprendices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendices aprendices = db.Aprendices.Find(id);
            if (aprendices == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_contrato = (from contract in db.Tipo_Contrato
                                   where contract.id_tipo_contrato==aprendices.id_contrato
                                   select contract.descripcion
                                   ).FirstOrDefault();
            return View(aprendices);
        }

        [AuthorizeUserModules(1)]
        // GET: Aprendices/Create
        // En caso de cambio de base de datos! el Rol 2 es equivalente a los usuarios con rol aprendices
        public ActionResult Create()
        {
            
            ViewBag.aprendices = (from pers in db.Personas
                                  join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                  where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                  && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                  && usuarios.id_rol == 2
                                  select pers
                                   ).ToList();
            ViewBag.id_contrato = new SelectList(db.Tipo_Contrato, "id_tipo_contrato", "descripcion");

            return View();
        }

        [AuthorizeUserModules(1)]
        // POST: Aprendices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_aprendiz,inicio_contrato,fin_contrato,id_contrato,descripcion")] Aprendices aprendices)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Aprendices.Add(aprendices);
                    db.SaveChanges();
                    TempData["Success"] = "Creado con exito";
                    return RedirectToAction("Index");
                }
                ViewBag.aprendices = (from pers in db.Personas
                                      join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                      where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                      && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                      && usuarios.id_rol == 2
                                      select pers
                                   ).ToList();
                ViewBag.id_aprendiz = new SelectList(db.Personas, "documento", "nombres", aprendices.id_aprendiz);
                ViewBag.id_contrato = new SelectList(db.Tipo_Contrato, "id_tipo_contrato", "descripcion", aprendices.id_contrato);
                ViewBag.Error = "Algo salió mal!";

                return View(aprendices);

            }
            catch (Exception err)
            {

                ViewBag.aprendices = (from pers in db.Personas
                                      join usuarios in db.Usuarios on pers.documento equals usuarios.id_usuario
                                      where !(from user in db.Aprendices select user.id_aprendiz).Contains(pers.documento)
                                      && !(from ase in db.Asesores select ase.id_asesor).Contains(pers.documento)
                                      && usuarios.id_rol == 2
                                      select pers
                                   ).ToList();
                ViewBag.id_aprendiz = new SelectList(db.Personas, "documento", "nombres", aprendices.id_aprendiz);
                ViewBag.id_contrato = new SelectList(db.Tipo_Contrato, "id_tipo_contrato", "descripcion", aprendices.id_contrato);
                ViewBag.Error = "Algo salió mal!";

                return View(aprendices);
            }

        }

        [AuthorizeUserModules(1)]
        // GET: Aprendices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendices aprendices = db.Aprendices.Find(id);
            if (aprendices == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_contrato = new SelectList(db.Tipo_Contrato, "id_tipo_contrato", "descripcion", aprendices.id_contrato);
            return View(aprendices);
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_aprendiz,inicio_contrato,fin_contrato,id_contrato, descripcion")] Aprendices aprendices)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aprendices).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Modificado con exito";
                return RedirectToAction("Index");
            }

            ViewBag.aprendices = (from pers in db.Personas
                                  where pers.id_documento == aprendices.id_aprendiz
                                  select pers
                                   ).ToList();
            ViewBag.id_aprendiz = new SelectList(db.Personas, "documento", "nombres", aprendices.id_aprendiz);
            ViewBag.id_contrato = new SelectList(db.Tipo_Contrato, "id_tipo_contrato", "descripcion", aprendices.id_contrato);
            return View(aprendices);
        }

        [AuthorizeUserModules(1)]
        // GET: Aprendices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendices aprendices = db.Aprendices.Find(id);
            if (aprendices == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_contrato = (from contract in db.Tipo_Contrato
                                   where contract.id_tipo_contrato == aprendices.id_contrato
                                   select contract.descripcion
                                   ).FirstOrDefault();
            return View(aprendices);
        }
        [AuthorizeUserModules(1)]
        // POST: Aprendices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Aprendices aprendices = db.Aprendices.Find(id);
                db.Aprendices.Remove(aprendices);
                db.SaveChanges();
                TempData["Success"] = "Eliminado con exito";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Aprendices aprendices = db.Aprendices.Find(id);
                ViewBag.Error = "El Aprendiz no puede ser eliminado, asegurese de que no tenga tareas pendientes";
                return View(aprendices);
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
