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
    public class GruposController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

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

            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         select new
                         {
                             id_asesor = l.id_asesor,
                             tag = l.id_asesor + "-" + p.nombres + " " + p.apellidos
                         }
                  ).Distinct();

            ViewBag.id_lider = new SelectList(query, "id_asesor", "tag");

            return View();

        }


        [AuthorizeUserModules(1)]
        public ActionResult GruposAprendices()
        {



            return View();
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListaAprendicesGruposInactivos()
        {

            try
            {
                
                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from gp in db.Grupo_Aprendices
                                  join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                  join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                  join apren in db.Aprendices on gp.id_aprendiz equals apren.id_aprendiz
                                  join tipoApren in db.Tipo_Contrato on apren.id_contrato equals tipoApren.id_tipo_contrato
                                  join grups in db.Grupos on gp.id_grupo equals grups.id_grupo
                                  join personas2 in db.Personas on grups.id_lider equals personas2.documento
                                  where user.id_estado==2
                                  select new
                                  {
                                      id_tabla="Grupo#"+gp.id_grupo,
                                      correo=user.email,
                                      nombre=personas.nombres+" "+personas.apellidos,
                                      id_aprendiz=gp.id_aprendiz,
                                      tipo=tipoApren.descripcion,
                                      asesor_acargo=grups.id_lider+"-"+personas2.nombres+" "+personas2.apellidos

                                  }
                                    ).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
               

            }
            catch (Exception)
            {
                var grupos = new { };
                return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
            }


 
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListaAprendicesGruposActivos()
        {

            try
            {
                
                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from gp in db.Grupo_Aprendices
                                  join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                  join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                  join apren in db.Aprendices on gp.id_aprendiz equals apren.id_aprendiz
                                  join tipoApren in db.Tipo_Contrato on apren.id_contrato equals tipoApren.id_tipo_contrato
                                  join grups in db.Grupos on gp.id_grupo equals grups.id_grupo
                                  join personas2 in db.Personas on grups.id_lider equals personas2.documento
                                  where user.id_estado==1
                                  select new
                                  {
                                      id_tabla="Grupo#"+gp.id_grupo,
                                      correo=user.email,
                                      nombre=personas.nombres+" "+personas.apellidos,
                                      id_aprendiz=gp.id_aprendiz,
                                      tipo=tipoApren.descripcion,
                                      asesor_acargo=grups.id_lider+"-"+personas2.nombres+" "+personas2.apellidos

                                  }
                                    ).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
               

            }
            catch (Exception)
            {
                var grupos = new { };
                return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
            }


 
        }
        
        [AuthorizeUserModules(1)]
        public JsonResult ListaAprendices(int ID)
        {

            try
            {
                
                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from gp in db.Grupo_Aprendices
                                  join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                  join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                  where gp.id_grupo==ID
                                  select new
                                  {
                                      id_tabla=gp.id,
                                      correo=user.email,
                                      nombre=personas.nombres+" "+personas.apellidos,
                                      id_aprendiz=gp.id_aprendiz,

                                  }
                                    ).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
               

            }
            catch (Exception)
            {
                var grupos = new { };
                return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
            }


 
        }


        [AuthorizeUserModules(1)]
        public JsonResult HistorialGrupo(int ID)
        {

            try
            {
                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from historial in db.historialGrupos
                                  join usuarios in db.Usuarios on historial.idUsuario equals usuarios.id_usuario
                                  join tpu in db.Roles on usuarios.id_rol equals tpu.id_rol
                                  where historial.idGrupo==ID
                                  select new {
                                      idUsuario = historial.idUsuario,
                                      nombreUsuario = historial.nombreUsuario,
                                      CorreoUsuario = historial.CorreoUsuario,
                                      rolUsuario = tpu.descripcion,
                                      accion = historial.accion,
                                      fechaAccion = historial.fechaAccion.ToString(),
                                  }).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
       

            }
            catch (Exception)
            {
                var grupos = new { };
                return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
            }


 
        }

        [AuthorizeUserModules(6)]
        public ActionResult ListaGruposLog(int ID)
        {

            try
            {

                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from gp in db.Grupo_Aprendices
                                  join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                  join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                  where gp.id_grupo == ID
                                  select new
                                  {
                                      id_tabla = gp.id,
                                      correo = user.email,
                                      nombre = personas.nombres + " " + personas.apellidos,
                                      id_aprendiz = gp.id_aprendiz,

                                  }
                                    ).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return RedirectToAction("Dashboard", "Dashboard");

            }


 
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListaGrupos()
        {

            try
            {

                    db.Configuration.ProxyCreationEnabled = false;
                    var grupos = (from gp in db.Grupos
                                  join asesores in db.Asesores on gp.id_lider equals asesores.id_asesor
                                  join personas in db.Personas on asesores.id_asesor equals personas.documento
                                  select new
                                  {
                                      id_grupo = gp.id_grupo,
                                      id_lider = gp.id_lider,
                                      nombre = personas.nombres + " " + personas.apellidos,
                                      correo = personas.email,
                                      descripcion = gp.descripcion
                                  }
                                    ).ToList();
                    return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                var grupos = new { };
                return Json(new { data = grupos }, JsonRequestBehavior.AllowGet);
            }


 
        }

        // GET: Grupos/Details/5
        [AuthorizeUserModules(6)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }


            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         where l.id_asesor==grupos.id_lider
                         select l.id_asesor + "-" + p.nombres + " " + p.apellidos
                  ).FirstOrDefault();

            //mirar si funciona

            ViewBag.id_lider = query;

            return View(grupos);
        }

        [AuthorizeUserModules(1)]
        public ActionResult NuevoIntegrante(int grupo)
        {
            var query = (from l in db.Aprendices
                         join p in db.Personas on l.id_aprendiz equals p.documento
                         select new
                         {
                             id_aprendiz = l.id_aprendiz,
                             tag = l.id_aprendiz + "-" + p.nombres + " " + p.apellidos
                         }
                        ).Distinct();

            ViewBag.id_aprendiz = new SelectList(query, "id_aprendiz", "tag");


            return View();
        }
        [HttpPost]
        [AuthorizeUserModules(1)]
        public ActionResult NuevoIntegrante(int id_aprendiz, int GrupoIntegrante)
        {

            var a = (from val in db.Grupo_Aprendices where val.id_grupo == GrupoIntegrante && val.id_aprendiz == id_aprendiz select val).FirstOrDefault();

            if(a==null){
                try
                {



                    Grupo_Aprendices integrante = new Grupo_Aprendices();
                    integrante.id_aprendiz = id_aprendiz;
                    integrante.id_grupo = GrupoIntegrante;


                    Personas persona = db.Personas.Find(integrante.id_aprendiz);
                    historialGrupos historial = new historialGrupos();

                    historial.idUsuario = integrante.id_aprendiz;
                    historial.nombreUsuario = persona.nombres + " " + persona.apellidos;
                    historial.CorreoUsuario = persona.email;
                    historial.rolUsuario = "4";
                    historial.accion = "Usuario Agregado";
                    historial.idResponsable = int.Parse(Session["Usuario"].ToString());
                    historial.fechaAccion = DateTime.Now;
                    historial.idGrupo = integrante.id_grupo;

                    db.historialGrupos.Add(historial);


                    db.Grupo_Aprendices.Add(integrante);
                    db.SaveChanges();
                    TempData["Success"] = "Aprendiz vinculado al grupo";
                    return Redirect(Request.UrlReferrer.ToString());
                }
                catch (Exception err)
                {
                    TempData["Error"] = "Algo Salió mal";
                    return Redirect(Request.UrlReferrer.ToString());
                }


            }
            TempData["Error"] = "El aprendiz ya pertenece al grupo";
            return Redirect(Request.UrlReferrer.ToString());
        }

        [AuthorizeUserModules(1)]
        public ActionResult Create()
        {

            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         select new
                         {
                             id_asesor = l.id_asesor,
                             tag = l.id_asesor + "-" + p.nombres + " " + p.apellidos
                         }
                  ).Distinct();

            ViewBag.id_lider = new SelectList(query, "id_asesor", "tag");

            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_lider,descripcion")] Grupos grupos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Grupos.Add(grupos);
                    db.SaveChanges(); 
                    TempData["Success"] = "Grupo agregado exitosamente!";



                    Personas persona = db.Personas.Find(grupos.id_lider);
                    historialGrupos historial = new historialGrupos();

                    historial.idUsuario = grupos.id_lider;
                    historial.nombreUsuario = persona.nombres + " " + persona.apellidos;
                    historial.CorreoUsuario = persona.email;
                    historial.rolUsuario = "4";
                    historial.accion = "Registro lider";
                    historial.idResponsable = int.Parse(Session["Usuario"].ToString());
                    historial.fechaAccion = DateTime.Now;
                    historial.idGrupo = grupos.id_grupo;

                    db.historialGrupos.Add(historial);
                    db.SaveChanges();


                    return Redirect("~/Grupos/Edit/" + grupos.id_grupo);
                }
                else
                {
                    TempData["Error"] = "No se pudo crear el nuevo grupo";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception)
            {
                TempData["Error"] = "Upss! algo salió mal!";
                return RedirectToAction("Index");
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }
            var query1 = (from l in db.Aprendices
                         join p in db.Personas on l.id_aprendiz equals p.documento
                         select new
                         {
                             id_aprendiz = l.id_aprendiz,
                             tag = l.id_aprendiz + "-" + p.nombres + " " + p.apellidos
                         }
            ).Distinct();

            ViewBag.id_aprendiz = new SelectList(query1, "id_aprendiz", "tag");

            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         select new
                         {
                             id_asesor = l.id_asesor,
                             tag = l.id_asesor + "-" + p.nombres + " " + p.apellidos
                         }
                  ).Distinct();

            //mirar si funciona

            ViewBag.id_lider = new SelectList(query, "id_asesor", "tag", grupos.id_lider);

            return View(grupos);
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_grupo,id_lider,descripcion")] Grupos grupos)
        {

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();

            }

            if (ModelState.IsValid)
            {

                Grupos gp = db.Grupos.Find(grupos.id_grupo);
                if (grupos.id_lider != gp.id_lider)
                {
                    Personas persona = db.Personas.Find(grupos.id_lider);
                    historialGrupos historial = new historialGrupos();

                    historial.idUsuario = grupos.id_lider;
                    historial.nombreUsuario = persona.nombres + " " + persona.apellidos;
                    historial.CorreoUsuario = persona.email;
                    historial.rolUsuario = "4";
                    historial.accion = "Nuevo Lider";
                    historial.idResponsable = int.Parse(Session["Usuario"].ToString());
                    historial.fechaAccion = DateTime.Now;
                    historial.idGrupo = grupos.id_grupo;

                    db.historialGrupos.Add(historial);
                }

                gp.id_grupo = grupos.id_grupo;
                gp.id_lider = grupos.id_lider;
                gp.descripcion = grupos.descripcion;

                db.Entry(gp).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cambios realizados con exito!";
                return RedirectToAction("Index");
            }

            var query1 = (from l in db.Aprendices
                         join p in db.Personas on l.id_aprendiz equals p.documento
                         select new
                         {
                             id_aprendiz = l.id_aprendiz,
                             tag = l.id_aprendiz + "-" + p.nombres + " " + p.apellidos
                         }
            ).Distinct();

            ViewBag.id_aprendiz = new SelectList(query1, "id_aprendiz", "tag");

            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         select new
                         {
                             id_asesor = l.id_asesor,
                             tag = l.id_asesor + "-" + p.nombres + " " + p.apellidos
                         }
                  ).Distinct();

            //mirar si funciona

            ViewBag.id_lider = new SelectList(query, "id_asesor", "tag", grupos.id_lider);
            ViewBag.Error = "Verifica los cambios nuevamente, algo salió mal";
            return View(grupos);
        }

        [AuthorizeUserModules(1)]
        public ActionResult EliminarUsuario(int IdRegistro)
        {
            try
            {



                Grupo_Aprendices EliminarApren = EliminarApren=db.Grupo_Aprendices.Find(IdRegistro);


                Personas persona = db.Personas.Find(EliminarApren.id_aprendiz);
                historialGrupos historial = new historialGrupos();

                historial.idUsuario = EliminarApren.id_aprendiz;
                historial.nombreUsuario = persona.nombres + " " + persona.apellidos;
                historial.CorreoUsuario = persona.email;
                historial.rolUsuario = "4";
                historial.accion = "Usuario Eliminado";
                historial.idResponsable = int.Parse(Session["Usuario"].ToString());
                historial.fechaAccion = DateTime.Now;
                historial.idGrupo = EliminarApren.id_grupo;

                db.historialGrupos.Add(historial);

                db.Grupo_Aprendices.Remove(EliminarApren);
                db.SaveChanges();
                TempData["Success"] = "Usuario Eliminado del proyecto";
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception)
            {
                TempData["Error"] = "El usuario no pudo ser eliminado";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }
            return View(grupos);
        }

        [AuthorizeUserModules(1)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grupos grupos = db.Grupos.Find(id);
            db.Grupos.Remove(grupos);
            db.SaveChanges();
            return RedirectToAction("Index");
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
