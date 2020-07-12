using SoftwareFactory.Filtros;
using SoftwareFactory.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SoftwareFactory.Controllers
{
    public class ProyectosController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();
        // GET: Proyectos

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


        public class proyectos
        {
            public int proyecto { get; set; }
            public string fecha { get; set; }
            public string mes { get; set; }
        }


        //Count para saber la cantidad de proyectos que tiene un asesor lider activo de fabrica
        public JsonResult ChartProyectosPorAño()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;

               

                var proyectos = (from p in db.Proyecto
                                 select new proyectos
                                 {
                                     proyecto = p.id_proyecto,
                                     fecha = p.fecha_inicio.ToString()
                                 }).ToList();

                int Enero = 0;
                int Febrero = 0;
                int Marzo = 0;
                int Abril = 0;
                int Mayo = 0;
                int Junio = 0;
                int Julio = 0;
                int Agosto = 0;
                int Septiempre = 0;
                int Octubre = 0;
                int Noviembre = 0;
                int Diciempre = 0;

                for (int i = 0; i < proyectos.Count(); i++)
                {

                    var parseo = proyectos[i].fecha.Split('-');
                    var mesAgenda = parseo[1];

                    if (mesAgenda.Equals("01"))
                    {
                        Enero++;
                    }
                    else if (mesAgenda.Equals("02"))
                    {
                        Febrero++;
                    }
                    else if (mesAgenda.Equals("03"))
                    {
                        Marzo++;
                    }
                    else if (mesAgenda.Equals("04"))
                    {
                        Abril++;
                    }
                    else if (mesAgenda.Equals("05"))
                    {
                        Mayo++;
                    }
                    else if (mesAgenda.Equals("06"))
                    {
                        Junio++;
                    }
                    else if (mesAgenda.Equals("07"))
                    {
                        Julio++;
                    }
                    else if (mesAgenda.Equals("08"))
                    {
                        Agosto++;
                    }
                    else if (mesAgenda.Equals("09"))
                    {
                        Septiempre++;
                    }
                    else if (mesAgenda.Equals("10"))
                    {
                        Octubre++;
                    }
                    else if (mesAgenda.Equals("11"))
                    {
                        Noviembre++;
                    }
                    else if (mesAgenda.Equals("12"))
                    {
                        Diciempre++;
                    }
                    
                }


                var JsonFull = "["+Enero+","+Febrero+","+Marzo+","+Abril+","+Mayo+","+Junio+","+Junio+","+Agosto+","+Septiempre+","+Octubre+","+Noviembre+","+Diciempre+"]";

                return Json(JsonFull, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserModules(6)]
        public JsonResult ChartProyectosAsignados()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var usuario = int.Parse(Session["Usuario"].ToString());
                var rol = int.Parse(Session["Rol"].ToString());

                if (rol == 4)
                {


                    var variablesAsesor = (from p in db.Proyecto
                                     join g in db.Grupos on p.id_grupo equals g.id_grupo
                                     join pe in db.Personas on g.id_lider equals pe.documento
                                     join so in db.Solicitud on p.id_proyecto equals so.id_solicitud
                                     where g.id_lider == usuario
                                     select new {
                                        proyecto = p.id_proyecto+" - "+so.nombre_empresa,
                                        estado = p.id_estado
                                     }).ToList();

                    var arrayAserorDesarrollo = variablesAsesor.GroupBy(c => c.estado).Select(c => Tuple.Create(c.Key, c.Count())).OrderBy(c=>c.Item1).ToArray();

                    var proyectoAsesor = "";
                    var countAsesor = "";

                    for (int i = 0; i < arrayAserorDesarrollo.Length; i++)
                    {
                        if (i == 0)
                        {
                            proyectoAsesor = proyectoAsesor + '\u0022' + arrayAserorDesarrollo[i].Item1 + '\u0022';
                            countAsesor = countAsesor + arrayAserorDesarrollo[i].Item2;
                        }
                        else
                        {
                            proyectoAsesor = proyectoAsesor + "," + '\u0022' + arrayAserorDesarrollo[i].Item1 + '\u0022';
                            countAsesor = countAsesor + "," + arrayAserorDesarrollo[i].Item2;
                        }
                    }


                    var JsonFullAsesor = "[ [" + proyectoAsesor + "] ," + "[" + countAsesor + "] ]";

                    return Json(JsonFullAsesor, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    var variablesApremdiz = (from p in db.Proyecto
                                           join g in db.Grupo_Aprendices on p.id_grupo equals g.id_grupo
                                           join pe in db.Personas on g.id_aprendiz equals pe.documento
                                           join so in db.Solicitud on p.id_proyecto equals so.id_solicitud
                                           where g.id_aprendiz == usuario
                                           select new
                                           {
                                               proyecto = p.id_proyecto + " - " + so.nombre_empresa,
                                               estado = p.id_estado
                                           }).ToList();

                    var arrayAprenizDesarrollo = variablesApremdiz.GroupBy(c => c.estado).Select(c => Tuple.Create(c.Key, c.Count())).OrderBy(c => c.Item1).ToArray();

                    var proyectoAprendiz = "";
                    var countApreniz = "";

                    for (int i = 0; i < arrayAprenizDesarrollo.Length; i++)
                    {
                        if (i == 0)
                        {
                            proyectoAprendiz = proyectoAprendiz + '\u0022' + arrayAprenizDesarrollo[i].Item1 + '\u0022';
                            countApreniz = countApreniz + arrayAprenizDesarrollo[i].Item2;
                        }
                        else
                        {
                            proyectoAprendiz = proyectoAprendiz + "," + '\u0022' + arrayAprenizDesarrollo[i].Item1 + '\u0022';
                            countApreniz = countApreniz + "," + arrayAprenizDesarrollo[i].Item2;
                        }
                    }


                    var JsonFullAprendiz = "[ [" + proyectoAprendiz + "] ," + "[" + countApreniz + "] ]";

                    return Json(JsonFullAprendiz, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }


        [AuthorizeUserModules(6)]
        //Count para saber la cantidad de proyectos que tiene un asesor lider activo de fabrica
        public JsonResult ChartAsesoresProyectos()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var variables = (from p in db.Proyecto
                                 join g in db.Grupos on p.id_grupo equals g.id_grupo
                                 join pe in db.Personas on g.id_lider equals pe.documento
                                 join us in db.Usuarios on pe.documento equals us.id_usuario
                                 where us.id_estado==1 && (from Grupos in db.Grupos select Grupos.id_lider).Contains(g.id_lider)
                                 select new { Asesor=g.id_lider+" - "+pe.nombres+" "+pe.apellidos}
                                 ).ToList();

                var array = variables.GroupBy(c => c.Asesor).Select(c => Tuple.Create(c.Key, c.Count())).ToArray(); ;

                var asesor = "";
                var cuenta = "";

                for (int i = 0; i < array.Length; i++)
                {
                    if (i == 0)
                    {
                        asesor = asesor+ '\u0022' + array[i].Item1+ '\u0022';
                        cuenta = cuenta+ array[i].Item2;
                    }
                    else
                    {
                        asesor = asesor + ","+'\u0022' + array[i].Item1 + '\u0022';
                        cuenta = cuenta + "," + array[i].Item2;
                    }
                }


                var JsonFull = "[ [" + asesor + "] ," + "[" + cuenta + "] ]";

                return Json(JsonFull, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }


        [AuthorizeUserModules(3)]
        public JsonResult ListaCliente()
        {
                var usuario = int.Parse(Session["Usuario"].ToString());

            db.Configuration.ProxyCreationEnabled = false;
            var proyecto = (from pyto in db.Proyecto
                            join estado in db.Estado_Proyecto on pyto.id_estado equals estado.id_estado
                            join solicitud in db.Solicitud on pyto.id_proyecto equals solicitud.id_solicitud
                            where solicitud.id_cliente == usuario
                            select new
                            {
                                archivo = pyto.archivo,
                                id_proyecto = pyto.id_proyecto,
                                empresa = solicitud.nombre_empresa,
                                estado = estado.descripcion,
                                grupo = "Grupo #" + pyto.id_grupo,
                                bitacora = "Bitacora #" + pyto.id_bitacora
                            }
                            ).ToList();
            return Json(new { data = proyecto }, JsonRequestBehavior.AllowGet);

        }

        //Descargar Archivos
        [AuthorizeUserModules(6)]
        public ActionResult Descargar(string ArchivoName)
        {
            try
            {

                if (!String.IsNullOrEmpty(ArchivoName) || ArchivoName!="null")
                {
                    string path = Server.MapPath("~/Archivos/FichasProyectos/");
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
        [AuthorizeUserModules(3)]
        public ActionResult DescargarCliente(string ArchivoName)
        {
            try
            {
                if (!String.IsNullOrEmpty(ArchivoName) || ArchivoName!="null")
                {
                    string path = Server.MapPath("~/Archivos/FichasProyectos/");
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

        [AuthorizeUserModules(4)]
        public ActionResult IndexAsesor()
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

        [AuthorizeUserModules(2)]
        public ActionResult IndexAprendiz()
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
        public JsonResult ListaProyectos()
        {

            try
            {
                var rol = int.Parse(Session["Rol"].ToString());

                if (rol == 1)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var proyecto = (from pyto in db.Proyecto
                                    join estado in db.Estado_Proyecto on pyto.id_estado equals estado.id_estado
                                    join solicitud in db.Solicitud on pyto.id_proyecto equals  solicitud.id_solicitud
                                    where pyto.id_estado != 2
                                    select new
                                    {
                                        archivo=pyto.archivo,
                                        id_proyecto= pyto.id_proyecto,
                                        empresa=solicitud.nombre_empresa,
                                        estado=estado.descripcion,
                                        grupo="Grupo#"+pyto.id_grupo,
                                        bitacora= "Bitacora #" + pyto.id_bitacora
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

        [AuthorizeUserModules(1)]
        public JsonResult ListaProyectosTerminados()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var proyecto = (from pyto in db.Proyecto
                                join estado in db.Estado_Proyecto on pyto.id_estado equals estado.id_estado
                                join solicitud in db.Solicitud on pyto.id_proyecto equals solicitud.id_solicitud
                                where pyto.id_estado == 2
                                select new
                                {
                                    archivo = pyto.archivo,
                                    id_proyecto = pyto.id_proyecto,
                                    empresa = solicitud.nombre_empresa,
                                    estado = estado.descripcion,
                                    grupo = "Grupo#" + pyto.id_grupo,
                                    bitacora = "Bitacora #" + pyto.id_bitacora
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



        [AuthorizeUserModules(4)]
        public JsonResult ListaProyectosAsesor()
        {

            try
            {
                var id = int.Parse(Session["Usuario"].ToString());
               
                    db.Configuration.ProxyCreationEnabled = false;
                    var proyecto = (from pyto in db.Proyecto
                                    join estado in db.Estado_Proyecto on pyto.id_estado equals estado.id_estado
                                    join solicitud in db.Solicitud on pyto.id_proyecto equals solicitud.id_solicitud
                                    join grupos in db.Grupos  on pyto.id_grupo equals grupos.id_grupo
                                    where grupos.id_lider == id
                                    select new
                                    {
                                        archivo = pyto.archivo,
                                        id_proyecto = pyto.id_proyecto,
                                        empresa = solicitud.nombre_empresa,
                                        estado = estado.descripcion,
                                        grupo = "Grupo #" + pyto.id_grupo,
                                        bitacora = pyto.id_bitacora,
                                        BitPro = pyto.id_bitacora+"/"+ pyto.id_proyecto
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


        [AuthorizeUserModules(2)]
        public JsonResult ListaProyectosAprendiz()
        {

            try
            {
                var id = int.Parse(Session["Usuario"].ToString());

                db.Configuration.ProxyCreationEnabled = false;
                var proyecto = (from pyto in db.Proyecto
                                join estado in db.Estado_Proyecto on pyto.id_estado equals estado.id_estado
                                join solicitud in db.Solicitud on pyto.id_proyecto equals solicitud.id_solicitud
                                join gpApren in db.Grupo_Aprendices on pyto.id_grupo equals gpApren.id_grupo
                                where gpApren.id_aprendiz == id
                                select new
                                {
                                    archivo = pyto.archivo,
                                    id_proyecto = pyto.id_proyecto,
                                    empresa = solicitud.nombre_empresa,
                                    estado = estado.descripcion,
                                    grupo = "Grupo #" + pyto.id_grupo,
                                    bitacora = pyto.id_bitacora,
                                    BitPro = pyto.id_bitacora + "/" + pyto.id_proyecto
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

        [AuthorizeUserModules(1)]
        public ActionResult NuevoProyecto()
        {

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            var Proyectos = (from n in db.Solicitud
                                 where !(from p in db.Proyecto select p.id_proyecto).Contains(n.id_solicitud) && n.id_viabilidad == 2 && n.id_estado_solicitud==5
                                 select new { id_proyecto = n.id_solicitud,tag=n.id_solicitud + " - "+ n.nombre_empresa }).ToList();

            ViewBag.id_proyecto = new SelectList(Proyectos, "id_proyecto", "tag");

            var Grupo = (from g in db.Grupos where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                         join ps in db.Personas on g.id_lider equals ps.documento
                         select new { id_grupo=g.id_grupo, tag="Grupo: "+g.id_grupo+" - Lider: "+ps.nombres+" "+ps.apellidos}).ToList();

            ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag");

            var Bitacoras = (from b in db.Bitacoras where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                             select new {id_bitacora=b.id_bitacora, tag = b.id_bitacora+" "+b.descripcion }).ToList();

            ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag");

            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult NuevoProyecto(HttpPostedFileBase file,Proyecto proyecto)
        {
            //Envio de errores para el usuario
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            if (proyecto.id_bitacora==null|| proyecto.id_grupo==null)
            {
                TempData["Error"] = "No puedes registrar un proyecto sin bitacora o grupo";
                return View(proyecto);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);
                   

                    //Evalidar que el archivo sea anexado al formulario y pese más de 0kb
                        if (file != null && file.ContentLength > 0)
                    {

                        var tp = file.ContentType.ToString();

                        //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                        if (tp.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document") || tp.Equals("application/pdf"))
                        {


                            //en caso de existir el archivo se guarda la ruta y el nombre del archivo es agregado a la base de datos
                            string path = Path.Combine(Server.MapPath("~/Archivos/FichasProyectos"),
                                                                      Path.GetFileName(numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName));

                            file.SaveAs(path);
                            proyecto.archivo = numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName;
                            proyecto.fecha_inicio = DateTime.Now;
                            proyecto.id_estado = 1;
                            db.Proyecto.Add(proyecto);
                            db.SaveChanges();

                            TempData["Success"] = "Proyecto Agregado exitosamente!";
                            return RedirectToAction("Index", "Proyectos");
                        }

                        else
                        {


                            var Proyectos2 = (from n in db.Solicitud
                                              where !(from p in db.Proyecto select p.id_proyecto).Contains(n.id_solicitud) && n.id_viabilidad == 2 && n.id_estado_solicitud == 5
                                              select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();

                            ViewBag.id_proyecto = new SelectList(Proyectos2, "id_proyecto", "tag", proyecto.id_proyecto);

                            var Grupo2 = (from g in db.Grupos
                                          where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                                          join ps in db.Personas on g.id_lider equals ps.documento
                                          select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                            ViewBag.id_grupo = new SelectList(Grupo2, "id_grupo", "tag", proyecto.id_grupo);

                            var Bitacoras2 = (from b in db.Bitacoras
                                              where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                              select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                            ViewBag.id_bitacora = new SelectList(Bitacoras2, "id_bitacora", "tag", proyecto.id_bitacora);

                            TempData["Error"] = "El archivo debe contener un formato Word o PDF y pesar menos de 12MB";
                            return View(proyecto);
                        }
                    }


                    //En caso contrario el proyecto es guardado sin archivo
                    proyecto.archivo = null;
                    proyecto.fecha_inicio = DateTime.Now;
                    proyecto.id_estado = 1;
                    db.Proyecto.Add(proyecto);
                    db.SaveChanges();
                    db.SaveChanges();

                    TempData["Success"] = "Proyecto Agregado exitosamente!";
                    return RedirectToAction("Index", "Proyectos");

                }

                //Si el modelo no es valido, se reenvia toda la información de nuevo para ser verificada
                var Proyectos = (from n in db.Solicitud
                                 where !(from p in db.Proyecto select p.id_proyecto).Contains(n.id_solicitud) && n.id_viabilidad == 2 && n.id_estado_solicitud == 5
                                 select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();

                ViewBag.id_proyecto = new SelectList(Proyectos, "id_proyecto", "tag",proyecto.id_proyecto);

                var Grupo = (from g in db.Grupos
                             where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                             join ps in db.Personas on g.id_lider equals ps.documento
                             select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag", proyecto.id_grupo);

                var Bitacoras = (from b in db.Bitacoras
                                 where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                 select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag", proyecto.id_bitacora);

                return View(proyecto);
            }
            catch (Exception)
            {
               
                //En caso de errores, se encapsula y se vuelve a cargar el formulario para no tener problemas 
                var Proyectos = (from n in db.Solicitud
                                 where !(from p in db.Proyecto select p.id_proyecto).Contains(n.id_solicitud) && n.id_viabilidad == 2 && n.id_estado_solicitud == 5
                                 select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();

                ViewBag.id_proyecto = new SelectList(Proyectos, "id_proyecto", "tag", proyecto.id_proyecto);

                var Grupo = (from g in db.Grupos
                             where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                             join ps in db.Personas on g.id_lider equals ps.documento
                             select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag", proyecto.id_grupo);

                var Bitacoras = (from b in db.Bitacoras
                                 where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                 select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag", proyecto.id_bitacora);

                TempData["Error"] = "No pudimos agregar el proyecto...";
                return View(proyecto);
            }
        }
        //Crear Nuevo Proyecto fin        


        //Editar Proyecto
        [AuthorizeUserModules(1)]
        public ActionResult EditarProyecto(int? id)
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
            Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }

            if (proyecto.id_estado == 2)
            {
                TempData["Error"] = "No se puede modificar";
                return RedirectToAction("Index");
            }

            var Pro = (from n in db.Solicitud
                             where n.id_solicitud==proyecto.id_proyecto
                             select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();


            ViewBag.id_proyecto = new SelectList(Pro, "id_proyecto", "tag", proyecto.id_proyecto);

            var Grupo = (from g in db.Grupos
                         join ps in db.Personas on g.id_lider equals ps.documento
                         where g.id_grupo == proyecto.id_grupo
                         select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();



            ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag", proyecto.id_grupo);

            var Bitacoras = (from b in db.Bitacoras
                             where b.id_bitacora == proyecto.id_bitacora
                             select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();


            ViewBag.id_estado = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

            ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag", proyecto.id_bitacora);

            ViewBag.idCliente = (from s in db.Solicitud
                                 join c in db.Cliente on s.id_cliente equals c.id_cliente
                                 where s.id_solicitud == proyecto.id_proyecto
                                 select c.id_cliente).FirstOrDefault();
            return View(proyecto);
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult EditarProyecto(HttpPostedFileBase file, Proyecto proyecto)
        {

            try
            {
                if (ModelState.IsValid)
                {



                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);


                    if (file != null && file.ContentLength > 0)
                    {


                        var tp = file.ContentType.ToString();

                        //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                        if (tp.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document") || tp.Equals("application/pdf"))
                        {


                            //en caso de existir el archivo se guarda la ruta y el nombre del archivo es agregado a la base de datos
                            string path = Path.Combine(Server.MapPath("~/Archivos/FichasProyectos"),
                                                                      Path.GetFileName(numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName));

                            string archivoviejo = Path.Combine(Server.MapPath("~/Archivos/FichasProyectos"),
                                                                Path.GetFileName(proyecto.archivo));
                            System.IO.File.Delete(archivoviejo);

                            file.SaveAs(path);
                            proyecto.archivo = numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName;
                            db.Entry(proyecto).State = EntityState.Modified; ;
                            db.SaveChanges();

                            TempData["Success"] = "Proyecto modificado exitosamente!";
                            return RedirectToAction("Index", "Proyectos");
                        }

                        else
                        {


                            var Proyectos2 = (from n in db.Solicitud
                                              where !(from p in db.Proyecto select p.id_proyecto).Contains(n.id_solicitud) && n.id_viabilidad == 2 && n.id_estado_solicitud == 5
                                              select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();

                            ViewBag.id_proyecto = new SelectList(Proyectos2, "id_proyecto", "tag", proyecto.id_proyecto);

                            var Grupo2 = (from g in db.Grupos
                                          where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                                          join ps in db.Personas on g.id_lider equals ps.documento
                                          select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                            ViewBag.id_grupo = new SelectList(Grupo2, "id_grupo", "tag", proyecto.id_grupo);

                            var Bitacoras2 = (from b in db.Bitacoras
                                              where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                              select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                            ViewBag.id_bitacora = new SelectList(Bitacoras2, "id_bitacora", "tag", proyecto.id_bitacora);
                            ViewBag.id_estado = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

                            TempData["Error"] = "El archivo debe contener un formato Word o PDF y pesar menos de 12MB";
                            return View(proyecto);
                        }

                    }



                    db.Entry(proyecto).State = EntityState.Modified; ;
                    db.SaveChanges();

                    TempData["Success"] = "Proyecto modificado con exito!";
                    return RedirectToAction("Index", "Proyectos");

                }

                var Pro = (from n in db.Solicitud
                           where n.id_solicitud == proyecto.id_proyecto
                           select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();


                ViewBag.id_proyecto = new SelectList(Pro, "id_proyecto", "tag", proyecto.id_proyecto);

                var Grupo = (from g in db.Grupos
                             where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                             join ps in db.Personas on g.id_lider equals ps.documento
                             select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                var Gru = (from g in db.Grupos
                           where g.id_grupo == proyecto.id_grupo
                           join ps in db.Personas on g.id_lider equals ps.documento
                           select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).FirstOrDefault();

                Grupo.Add(Gru);

                ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag", proyecto.id_grupo);

                var Bitacoras = (from b in db.Bitacoras
                                 where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                 select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                var Bit = (from b in db.Bitacoras
                           where b.id_bitacora == proyecto.id_bitacora
                           select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).FirstOrDefault();

                Bitacoras.Add(Bit);

                ViewBag.id_estado = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

                ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag", proyecto.id_bitacora);
                ViewBag.Error = "El modelo no es valido";
                return View(proyecto);
            }
            catch (Exception)
            {
                var Pro = (from n in db.Solicitud
                           where n.id_solicitud == proyecto.id_proyecto
                           select new { id_proyecto = n.id_solicitud, tag = n.id_solicitud + " - " + n.nombre_empresa }).ToList();


                ViewBag.id_proyecto = new SelectList(Pro, "id_proyecto", "tag", proyecto.id_proyecto);

                var Grupo = (from g in db.Grupos
                             where !(from pg in db.Proyecto select pg.id_grupo).Contains(g.id_grupo)
                             join ps in db.Personas on g.id_lider equals ps.documento
                             select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).ToList();

                var Gru = (from g in db.Grupos
                           where g.id_grupo == proyecto.id_grupo
                           join ps in db.Personas on g.id_lider equals ps.documento
                           select new { id_grupo = g.id_grupo, tag = "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos }).FirstOrDefault();

                Grupo.Add(Gru);

                ViewBag.id_grupo = new SelectList(Grupo, "id_grupo", "tag", proyecto.id_grupo);

                var Bitacoras = (from b in db.Bitacoras
                                 where !(from pg in db.Proyecto select pg.id_bitacora).Contains(b.id_bitacora)
                                 select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).ToList();

                var Bit = (from b in db.Bitacoras
                           where b.id_bitacora == proyecto.id_bitacora
                           select new { id_bitacora = b.id_bitacora, tag = b.id_bitacora + " " + b.descripcion }).FirstOrDefault();

                Bitacoras.Add(Bit);

                ViewBag.id_estado = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

                ViewBag.id_bitacora = new SelectList(Bitacoras, "id_bitacora", "tag", proyecto.id_bitacora);
                TempData["Error"] = "El modelo no es valido";

                return View(proyecto);
            }

        }



        [AuthorizeUserModules(6)]
        public ActionResult DetallesProyectoLog(int? id)
        {

            try
            {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Proyecto proyecto = db.Proyecto.Find(id);
                    Solicitud solicitud = db.Solicitud.Find(id);

                    if (proyecto == null || solicitud == null)
                    {
                        return HttpNotFound();
                    }

                    //Detalles de grupo
                    Grupos grupos = db.Grupos.Find(proyecto.id_grupo);
                    var query = (from l in db.Asesores
                                 join p in db.Personas on l.id_asesor equals p.documento
                                 where l.id_asesor == grupos.id_lider
                                 select l.id_asesor + "-" + p.nombres + " " + p.apellidos
                         ).FirstOrDefault();


                    //Validar que el usuario  pertenezca al proyecto

                    var rol = int.Parse(Session["Rol"].ToString());

                    if (rol == 2 || rol == 4)
                    {
                        if (rol == 4) //Validar que el asesor sea el lider del proyecto 
                        {
                            var UserId = int.Parse(Session["Usuario"].ToString());

                            if (!(UserId == grupos.id_lider))
                            {
                                TempData["Error"] = "No tienes permitido esta acción";
                                return RedirectToAction("Dashboard", "Dashboard");

                            }
                        }
                        else
                        {
                            var UserId = int.Parse(Session["Usuario"].ToString());

                            var UsuarioPro = (from proyAsig in db.Proyecto
                                              join gruApren in db.Grupo_Aprendices on proyAsig.id_grupo equals gruApren.id_grupo
                                              where gruApren.id_aprendiz == UserId && gruApren.id_grupo == grupos.id_grupo
                                              select proyAsig).FirstOrDefault();
                             

                            if (UsuarioPro==null) //Validar que el Aprendiz esté asignado al proyecto
                            {
                                TempData["Error"] = "No tienes permitido esta acción";
                                return RedirectToAction("Dashboard", "Dashboard");

                            }


                        }

                        ViewBag.TemplateLayout = "~/Views/Shared/_DashLayout.cshtml";
                    }
                    else
                    {
                        var IdCliente = int.Parse(Session["Usuario"].ToString());

                        if (!(solicitud.id_cliente == IdCliente))
                        {
                            TempData["Error"] = "No tienes permitido esta acción";
                            return RedirectToAction("DashboardCliente","Dashboard");
                        }

                        ViewBag.TemplateLayout = "~/Views/Shared/_DashLayoutCliente.cshtml";
                    }

                    //Validar que el usuario 



                    ViewBag.grupos = grupos;
                    ViewBag.LiderGrupo = query;

                    //Detalles de grupo

                    ViewBag.proyecto = proyecto;
                    ViewBag.solicitud = solicitud;

                    //Información cliente
                    var usuario = solicitud.id_cliente;

                    ViewBag.Cliente = (from n in db.Cliente
                                       where n.id_cliente == usuario
                                       select n.tipo_cliente).FirstOrDefault();
                    //Información cliente end


                    ViewBag.id_proyecto = (from n in db.Solicitud
                                           where n.id_solicitud == proyecto.id_proyecto
                                           select n.id_solicitud + " - " + n.nombre_empresa).FirstOrDefault();



                    ViewBag.id_grupo = (from g in db.Grupos
                                        where g.id_grupo == proyecto.id_grupo
                                        join ps in db.Personas on g.id_lider equals ps.documento
                                        select "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos).FirstOrDefault();

                    ViewBag.id_bitacora = (from b in db.Bitacoras
                                           where b.id_bitacora == proyecto.id_bitacora
                                           select b.id_bitacora + " - " + b.descripcion).FirstOrDefault();




                    ViewBag.id_estadop = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

                    ViewBag.listaGrupo = (from gp in db.Grupo_Aprendices
                                          join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                          join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                          where gp.id_grupo == proyecto.id_grupo
                                          select new
                                          {
                                              id_tabla = gp.id,
                                              correo = user.email,
                                              nombre = personas.nombres + " " + personas.apellidos,
                                              id_aprendiz = gp.id_aprendiz
                                          }).ToList();

                    ViewBag.id_tipoCliente = new SelectList(db.Tipo_Solicitante, "tipo_solicitante1", "descripcion", solicitud.id_tipoCliente);
                    ViewBag.id_viabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
                    ViewBag.id_estado = new SelectList(db.Estado_Solicitud, "id_estado", "descripcion", solicitud.id_estado_solicitud);

                    

                    return View();
               

            }
            catch (Exception)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

        }


        [AuthorizeUserModules(1)]
        public ActionResult DetallesProyectoAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto proyecto = db.Proyecto.Find(id);
            Solicitud solicitud = db.Solicitud.Find(id);

            if (proyecto == null || solicitud==null)
            {
                return HttpNotFound();
            }

            //Detalles de grupo
            Grupos grupos = db.Grupos.Find(proyecto.id_grupo);
            var query = (from l in db.Asesores
                         join p in db.Personas on l.id_asesor equals p.documento
                         where l.id_asesor == grupos.id_lider
                         select l.id_asesor + "-" + p.nombres + " " + p.apellidos
                 ).FirstOrDefault();

            ViewBag.grupos = grupos;
            ViewBag.LiderGrupo = query;

            //Detalles de grupo

            ViewBag.proyecto = proyecto;
            ViewBag.solicitud = solicitud;

            //Información cliente
            var usuario = solicitud.id_cliente;

            ViewBag.Cliente = (from n in db.Cliente
                               where n.id_cliente == usuario
                               select n.tipo_cliente).FirstOrDefault();
            //Información cliente end


            ViewBag.id_proyecto = (from n in db.Solicitud
                                       where n.id_solicitud == proyecto.id_proyecto
                                       select  n.id_solicitud + " - " + n.nombre_empresa ).FirstOrDefault();

            ViewBag.id_grupo = (from g in db.Grupos
                                 where g.id_grupo==proyecto.id_grupo
                                 join ps in db.Personas on g.id_lider equals ps.documento
                                 select "Grupo: " + g.id_grupo + " - Lider: " + ps.nombres + " " + ps.apellidos ).FirstOrDefault();

            ViewBag.id_bitacora = (from b in db.Bitacoras
                                     where b.id_bitacora==proyecto.id_bitacora
                                     select  b.id_bitacora + " - " + b.descripcion ).FirstOrDefault();


            ViewBag.id_estadop = new SelectList(db.Estado_Proyecto, "id_estado", "descripcion", proyecto.id_estado);

            ViewBag.listaGrupo = (   from gp in db.Grupo_Aprendices
                                        join user in db.Usuarios on gp.id_aprendiz equals user.id_usuario
                                        join personas in db.Personas on gp.id_aprendiz equals personas.documento
                                        where gp.id_grupo == proyecto.id_grupo
                                        select new
                                        {
                                          id_tabla = gp.id,
                                          correo = user.email,
                                          nombre = personas.nombres + " " + personas.apellidos,
                                          id_aprendiz = gp.id_aprendiz}).ToList();

            ViewBag.id_tipoCliente = new SelectList(db.Tipo_Solicitante, "tipo_solicitante1", "descripcion", solicitud.id_tipoCliente);
            ViewBag.id_viabilidad = new SelectList(db.Estado_Viabilidad, "id_viabilidad", "descripcion", solicitud.id_viabilidad);
            ViewBag.id_estado = new SelectList(db.Estado_Solicitud, "id_estado", "descripcion", solicitud.id_estado_solicitud);

            return View();
        }
    }
}