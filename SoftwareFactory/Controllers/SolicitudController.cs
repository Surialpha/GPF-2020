using SoftwareFactory.Filtros;
using SoftwareFactory.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SoftwareFactory.Controllers
{
    public class SolicitudController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();
        // GET: Solicitud

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
        public ActionResult EditarAdmin(int? id)
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
                var consulta = (from p in db.Proyecto
                                where p.id_proyecto == id
                                select p).FirstOrDefault();
                if (consulta != null)
                {
                    TempData["Error"] = "¡No se puede modificar la solicitud porque ya se encuentra asociado a un proyecto en desarrollo!";
                    return RedirectToAction("Index", "Solicitud");
                }

            }
            catch (Exception)
            {

                throw;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipoCliente = new SelectList(db.Tipo_Solicitante, "tipo_solicitante1", "descripcion", solicitud.id_tipoCliente);
            ViewBag.id_viabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
            ViewBag.estadoViabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
            ViewBag.id_estado_solicitud = new SelectList(db.Estado_Solicitud, "id_estado", "descripcion", solicitud.id_estado_solicitud);

            ViewBag.Cliente = (from n in db.Cliente
                               where n.id_cliente == solicitud.id_cliente
                               select n.tipo_cliente).FirstOrDefault();


            return View(solicitud);
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult EditarAdmin(Solicitud solicitud)
        {

            try
            {
                db.Entry(solicitud).State = EntityState.Modified; ;
                db.SaveChanges();
                TempData["Success"] = "Modificado con exito.";
                return RedirectToAction("Index", "Solicitud");
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal!";
                return RedirectToAction("Index", "Solicitud");
            }

            
        }
        //Editar solicitud para administrador


        //Descargar Archivos
        [AuthorizeUserModules(6)]   
        public ActionResult Descargar(string ArchivoName)
        {
            try
            {
                if (!String.IsNullOrEmpty(ArchivoName) || ArchivoName!="null")
                {
                    string path = Server.MapPath("~/Archivos/");
                    string fileName = Path.GetFileName(ArchivoName);
                    string fullPath = Path.Combine(path, fileName);

                    return File(fullPath,System.Net.Mime.MediaTypeNames.Application.Octet,ArchivoName);
                }
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
           
        }

        //Descargar Archivos


        [AuthorizeUserModules(1)]
        //Listado proyectos Admin *Lista Json
        public JsonResult ChartSolicitud()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var variables = (from pyto in db.Solicitud
                                select pyto
                                ).ToList();
                int NumEnviados, NumRecibidos, NumRevision, NumModificar, NumAprobados, NumDescartados = 0 ;


                NumEnviados = variables.Count(x => x.id_estado_solicitud == 1);
                NumRecibidos = variables.Count(x => x.id_estado_solicitud == 2);
                NumRevision = variables.Count(x => x.id_estado_solicitud == 3);
                NumModificar = variables.Count(x => x.id_estado_solicitud == 4);
                NumAprobados = variables.Count(x => x.id_estado_solicitud == 5);
                NumDescartados = variables.Count(x => x.id_estado_solicitud == 6);

                var array = "[" + NumEnviados + "," + NumRecibidos + "," + NumRevision + "," + NumModificar + "," + NumAprobados + "," + NumDescartados + "]";
                
                return Json(array , JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserModules(1)]
        //Listado proyectos Admin *Lista Json
        public JsonResult ListaAdmin()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var proyecto = (from pyto in db.Solicitud
                                join estSol in db.Estado_Solicitud on pyto.id_estado_solicitud equals estSol.id_estado
                                join estVia in db.Estado_Viabilidad on pyto.id_viabilidad equals estVia.id_viabilidad
                                join clien in db.Cliente on pyto.id_cliente equals clien.id_cliente
                                join tpcli in db.Tipo_Cliente on clien.tipo_cliente equals tpcli.id_TipoCliente
                                where estSol.id_estado != 5 && estSol.id_estado != 6
                                select new
                                {
                                    idSolicitud = pyto.id_solicitud,
                                    idCliente = clien.id_cliente,
                                    tipoCliente = tpcli.descripcion,
                                    fechaSolicitud = pyto.fecha_solicitud.ToString(),
                                    tiempoEstimado = pyto.tiempo_estimado,
                                    EstadoSolicitud = estSol.descripcion,
                                    EstadoViabi = estVia.descripcion,
                                    nombreArchivo = pyto.archivo_adjunto
                                }
                                ).ToList();
                return Json(new { data = proyecto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var proyectos = new { };
                return Json(new { data = proyectos }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserModules(6)]
        public JsonResult SolicitudLog()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var historial = (from pyto in db.Solicitud
                                join estSol in db.Estado_Solicitud on pyto.id_estado_solicitud equals estSol.id_estado
                                join estVia in db.Estado_Viabilidad on pyto.id_viabilidad equals estVia.id_viabilidad
                                join clien in db.Cliente on pyto.id_cliente equals clien.id_cliente
                                join tpcli in db.Tipo_Cliente on clien.tipo_cliente equals tpcli.id_TipoCliente
                                 where estVia.id_viabilidad != 1
                                 select new
                                {
                                    idSolicitud = pyto.id_solicitud,
                                    idCliente = clien.id_cliente,
                                    tipoCliente = tpcli.descripcion,
                                    fechaSolicitud = pyto.fecha_solicitud.ToString(),
                                    EstadoSolicitud = estSol.descripcion,
                                    EstadoViabi = estVia.descripcion
                                 }
                                ).ToList();
                return Json(new { data = historial }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var historial = new { };
                return Json(new { data = historial }, JsonRequestBehavior.AllowGet);
            }
        }



        [AuthorizeUserModules(3)]
        public JsonResult ListaCliente()
        {
            try
            {
                var rol = int.Parse(Session["Rol"].ToString());
                var usuario= int.Parse(Session["Usuario"].ToString());
                if (rol == 3)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var proyecto = (from pyto in db.Solicitud
                                    join estSol in db.Estado_Solicitud on pyto.id_estado_solicitud equals estSol.id_estado
                                    join estVia in db.Estado_Viabilidad on pyto.id_viabilidad equals estVia.id_viabilidad
                                    join clien in db.Cliente on pyto.id_cliente equals clien.id_cliente
                                    where pyto.id_cliente==usuario
                                    select new
                                    {
                                        idSolicitud = pyto.id_solicitud,
                                        fechaSolicitud = pyto.fecha_solicitud.ToString(),
                                        EstadoSolicitud = estSol.descripcion,
                                        EstadoViabi = estVia.descripcion
                                    }
                                    ).ToList();
                    return Json(new { data = proyecto }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var proyectos = new { };
                    return Json(new { data = proyectos }, JsonRequestBehavior.AllowGet);    
                }

            }
            catch (Exception)
            {
                var proyectos = new { };
                return Json(new { data = proyectos }, JsonRequestBehavior.AllowGet);
            }

            

        }

        [AuthorizeUserModules(3)]
        ///Solicitar Proyecto *Gestion clientes**
        public ActionResult Solicitar()
        {

            try
            {
                var usuario = int.Parse(Session["Usuario"].ToString());
                var estado = int.Parse(Session["state"].ToString());
                if (estado == 1)
                {

                    ViewBag.Cliente = (from n in db.Cliente
                                       where n.id_cliente == usuario
                                       select n.tipo_cliente).FirstOrDefault();

                    ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();
                    return View();

                }
                else
                {
                    TempData["Error"] = "Solo los usuarios activos pueden solicitar un proyecto"; 
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("DashboardCliente", "Dashboard");

            }
            
        }

        [AuthorizeUserModules(3)]
        [HttpPost]
        public ActionResult Solicitar(HttpPostedFileBase file,[Bind(Include = "id_tipoCliente,tipo_avance,nombre_solicitante,nombre_empresa,contraprestacion,tiempo_estimado,tema_proyecto,grupo_afectado,descripcion,area")]Solicitud solicitud)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);
                    var usuario = int.Parse(Session["Usuario"].ToString());

                    if (file != null && file.ContentLength > 0)
                    {

                        string path = Path.Combine(Server.MapPath("~/Archivos"),
                                                                  Path.GetFileName(usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy")+"-"+file.FileName));

                        file.SaveAs(path);
                        solicitud.archivo_adjunto = usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName;
                        solicitud.fecha_solicitud = DateTime.Now;
                        solicitud.id_estado_solicitud = 1;
                        solicitud.id_viabilidad = 1;
                        solicitud.id_cliente = usuario;
                        solicitud.id_solicitud = usuario + numIterations + int.Parse(DateTime.Now.ToString("mmddyyyy"));
                        db.Solicitud.Add(solicitud);
                        db.SaveChanges();

                        TempData["Success"] = "Proyecto enviado con exito!";
                        return RedirectToAction("DashboardCliente", "Dashboard");
                    }



                    solicitud.archivo_adjunto = null;
                    solicitud.fecha_solicitud = DateTime.Now;
                    solicitud.id_estado_solicitud = 1;
                    solicitud.id_viabilidad = 1;
                    solicitud.id_cliente = usuario;
                    solicitud.id_solicitud = usuario + numIterations + int.Parse(DateTime.Now.ToString("mmddyyyy"));
                    db.Solicitud.Add(solicitud);
                    db.SaveChanges();

                    TempData["Success"] = "Proyecto enviado con exito!";
                    return RedirectToAction("DashboardCliente", "Dashboard");

                }

                var cliente = int.Parse(Session["Usuario"].ToString());
                ViewBag.Cliente = (from n in db.Cliente
                                   where n.id_cliente == cliente
                                   select n.tipo_cliente).FirstOrDefault();

                ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();
                return View(solicitud);
            }
            catch (Exception)
            {
                try
                {
                    var cliente = int.Parse(Session["Usuario"].ToString());
                    ViewBag.Cliente = (from n in db.Cliente
                                       where n.id_cliente == cliente
                                       select n.tipo_cliente).FirstOrDefault();
                }
                catch (Exception)
                {
                    return RedirectToAction("DashboardClientes", "Dashboard");
                }


                ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();

                ViewBag.Error = "Lo sentimos, algo salió mal...";
                return View(solicitud);
            }

         
        }
        ///Solicitar Proyecto *Gestion clientes** fin



        [AuthorizeUserModules(3)]
        ///Editar Proyecto *Gestion clientes*
        public ActionResult EditarProyectoCl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            //Solo proyectos con el primer estado (enviado) pueden acceder a esta configuración
            if (solicitud.id_estado_solicitud == 2)
            {
                TempData["Error"] = "¡No se puede modificar la solicitud porque se encuentra en estado RECIBIDO!";
                return RedirectToAction("DashboardCliente", "Dashboard");
            }
            else
            {
                if (solicitud.id_estado_solicitud == 3)
                {
                    TempData["Error"] = "¡No se puede modificar la solicitud porque está siendo revisada!";
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
                else if (solicitud.id_estado_solicitud == 5)
                {
                    TempData["Error"] = "¡No es posible modificar la solicitud, porque ya ha sido aprobada!";
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
                else if (solicitud.id_estado_solicitud == 6)
                {
                    TempData["Error"] = "¡No es posible modificar la solicitud, porque ya ha sido descartada!";
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
                else
                {
                    try
                    {
                        var usuario = int.Parse(Session["Usuario"].ToString());
                        ViewBag.Cliente = (from n in db.Cliente
                                           where n.id_cliente == usuario
                                           select n.tipo_cliente).FirstOrDefault();

                        ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();

                        return View(solicitud);
                    }
                    catch (Exception)
                    {
                        TempData["Error"] = "Algo salió mal! intentalo nuevamente";
                        return RedirectToAction("DashboardCliente", "Dashboard");
                    }
                }
            }
        }

        [AuthorizeUserModules(3)]
        [HttpPost]
        public ActionResult EditarProyectoCl(HttpPostedFileBase file, [Bind(Include = "id_tipoCliente,tipo_avance,nombre_solicitante,nombre_empresa,contraprestacion,tiempo_estimado,tema_proyecto,grupo_afectado,descripcion,area,archivo_adjunto,fecha_solicitud,id_viabilidad,id_cliente,id_solicitud")]Solicitud solicitud)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);
                    var usuario = int.Parse(Session["Usuario"].ToString());

                    if (file != null && file.ContentLength > 0)
                    {
                        string archivoviejo= Path.Combine(Server.MapPath("~/Archivos"),
                                                                  Path.GetFileName(solicitud.archivo_adjunto));
                        System.IO.File.Delete(archivoviejo);

                        string path = Path.Combine(Server.MapPath("~/Archivos"),
                                                                  Path.GetFileName(usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName));

                        file.SaveAs(path);
                        solicitud.archivo_adjunto = usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName;
                        solicitud.id_estado_solicitud = 1;
                        db.Entry(solicitud).State = EntityState.Modified;
                        db.SaveChanges();


                        TempData["Success"] = "Proyecto modificado con exito!";
                        return RedirectToAction("DashboardCliente", "Dashboard");
                    }


                    solicitud.id_estado_solicitud = 1;
                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Success"] = "Proyecto modificado con exito!";
                    return RedirectToAction("DashboardCliente", "Dashboard");

                }

                var cliente = int.Parse(Session["Usuario"].ToString());
                ViewBag.Cliente = (from n in db.Cliente
                                   where n.id_cliente == cliente
                                   select n.tipo_cliente).FirstOrDefault();

                ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();
                return View(solicitud);
            }
            catch (Exception)
            {
                try
                {
                    var cliente = int.Parse(Session["Usuario"].ToString());
                    ViewBag.Cliente = (from n in db.Cliente
                                       where n.id_cliente == cliente
                                       select n.tipo_cliente).FirstOrDefault();
                }
                catch (Exception)
                {
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }


                ViewBag.id_tipoCliente = (from n in db.Tipo_Solicitante select n).ToList();

                ViewBag.Error = "Lo sentimos, algo salió mal...";
                return View(solicitud);
            }
        }
        ///Editar Proyecto *Gestion clientes* fin

        ///Detalles Proyecto *Gestion clientes*
        public ActionResult DetallesProyectoCl(int? id)
        {
            if (Session["Logged"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Solicitud solicitud = db.Solicitud.Find(id);
                if (solicitud == null)
                {
                    return HttpNotFound();
                }
                try
                {
                    var usuario = int.Parse(Session["Usuario"].ToString());
                    ViewBag.Cliente = (from n in db.Cliente
                                       where n.id_cliente == usuario
                                       select n.tipo_cliente).FirstOrDefault();


                    ViewBag.id_tipoCliente = new SelectList(db.Tipo_Solicitante, "tipo_solicitante1", "descripcion", solicitud.id_tipoCliente);
                    ViewBag.id_viabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
                    ViewBag.id_estado = new SelectList(db.Estado_Solicitud, "id_estado", "descripcion", solicitud.id_estado_solicitud);
                    return View(solicitud);
                }
                catch (Exception)
                {
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }

            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");

            }

            

        }

        [AuthorizeUserModules(1)]
        public ActionResult DetallesSolicitudAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            try
            {
                var usuario = solicitud.id_cliente;

                ViewBag.Cliente = (from n in db.Cliente
                                   where n.id_cliente == usuario
                                   select n.tipo_cliente).FirstOrDefault();


                ViewBag.id_tipoCliente = new SelectList(db.Tipo_Solicitante, "tipo_solicitante1", "descripcion", solicitud.id_tipoCliente);
                ViewBag.id_estado_solicitud = new SelectList(db.Estado_Solicitud, "id_estado", "descripcion", solicitud.id_estado_solicitud);
                ViewBag.id_viabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
                return View(solicitud);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Dashboard", "Dashboard");
            }


        }
        ///Detalles Proyecto *Gestion Administrador*


        //Eliminar solicitu, en caso de el estado de solicitud no haya cambiado del primer estado
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            try
            {
                var cliente = int.Parse(Session["Usuario"].ToString());

                if (solicitud.id_cliente == cliente)
                {
                    if (solicitud.id_estado_solicitud == 1)
                    {
                        if (solicitud.archivo_adjunto.Length > 0)
                        {
                            string archivoviejo = Path.Combine(Server.MapPath("~/Archivos"),
                                                                             Path.GetFileName(solicitud.archivo_adjunto));
                            System.IO.File.Delete(archivoviejo);//Eliminamos el archivo que se encuentra subido en caso de que exista! 
                        }

                        db.Solicitud.Remove(solicitud);
                        db.SaveChanges();

                        TempData["Success"] = "El Proyecto fue eliminado con Exito!";
                        return RedirectToAction("DashboardCliente", "Dashboard");
                    }
                    else
                    {
                        TempData["Error"] = "La solicitud de proyecto no puede ser eliminada!";
                        return RedirectToAction("DashboardCliente", "Dashboard");
                    }
                }
                else
                {
                    return RedirectToAction("Cerrar", "Acceso");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal";
                return RedirectToAction("DashboardCliente", "Dashboard");
                throw;
            }
            
        }


    }
}