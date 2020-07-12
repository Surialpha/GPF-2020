using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftwareFactory.Filtros;
using SoftwareFactory.Models;

namespace SoftwareFactory.Controllers
{
    public class BitacorasController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();


        // GET: Bitacoras
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
        // GET: Bitacoras
        [AuthorizeUserModules(1)]
        public ActionResult IndexBitacorasRealizadas()
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

        public  class DetallesBi{

            public int? id_bitacora { get; set; }
            public string fecha_creacion { get; set; }
            public int id { get; set; }
            public string encargado { get; set; }
            public string archivo { get; set; }
            public string proyecto { get; set; }
            public string descripcion { get; set; }

        }

        [AuthorizeUserModules(1)]
        public ActionResult DetallesAprendiz(int id)
        {

            try
            {

                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                if (TempData["Success"] != null)
                {
                    ViewBag.Success = TempData["Success"].ToString();

                }

                var bitacoraAprendiz = (from bitacora in db.Bitacora_Aprendiz
                                        join personas in db.Personas on bitacora.id_aprendiz equals personas.documento
                                        join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                                        join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                                        where bitacora.id == id
                                        select new DetallesBi
                                        {
                                            id_bitacora = bitacora.id_bitacora,
                                            fecha_creacion = bitacora.fecha_creacion.ToString(),
                                            id = bitacora.id,
                                            encargado = personas.documento + " - " + personas.nombres + " " + personas.apellidos,
                                            archivo = bitacora.archivo,
                                            proyecto = solicitud.id_solicitud + " - " + solicitud.nombre_empresa,
                                            descripcion=bitacora.descripcion

                                        }).FirstOrDefault();

                if (bitacoraAprendiz!=null) {

                    ViewBag.Bitacora = bitacoraAprendiz;
                    return View("DetallesBitacorasAP");
                }
                else
                {
                    TempData["Error"] = "Algo salió mal";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal";
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }





        [AuthorizeUserModules(6)]
        public JsonResult ChartBitacoras()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var usuario = int.Parse(Session["Usuario"].ToString());
                var rol = int.Parse(Session["Rol"].ToString());

                if (rol == 4)
                {


                    var variablesAsesor = (from b in db.Bitacora_Asesor
                                           join p in db.Proyecto on b.id_bitacora equals p.id_bitacora
                                           join so in db.Solicitud on p.id_proyecto equals so.id_solicitud
                                           join g in db.Grupos on p.id_grupo equals g.id_grupo
                                           where g.id_lider == usuario
                                           select new
                                           {
                                                proyecto=p.id_proyecto+"-"+so.nombre_empresa
                                           }).ToList();

                    var arrayAserorDesarrollo = variablesAsesor.GroupBy(c => c.proyecto).Select(c => Tuple.Create(c.Key, c.Count())).OrderBy(c => c.Item1).ToArray();

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

                    var variablesApremdiz = (from b in db.Bitacora_Aprendiz
                                             join p in db.Proyecto on b.id_bitacora equals p.id_bitacora
                                             join so in db.Solicitud on p.id_proyecto equals so.id_solicitud
                                             join g in db.Grupo_Aprendices on p.id_grupo equals g.id_grupo
                                             where g.id_aprendiz == usuario
                                             select new
                                             {
                                                 proyecto = p.id_proyecto + "-" + so.nombre_empresa
                                             }).ToList();

                    var arrayAprenizDesarrollo = variablesApremdiz.GroupBy(c => c.proyecto).Select(c => Tuple.Create(c.Key, c.Count())).OrderBy(c => c.Item1).ToArray();

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








        [AuthorizeUserModules(1)]
        public ActionResult DetallesAsesor(int id)
        {

            try
            {

                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                if (TempData["Success"] != null)
                {
                    ViewBag.Success = TempData["Success"].ToString();

                }

                var bitacoraAsesor = (from bitacora in db.Bitacora_Asesor
                                        join personas in db.Personas on bitacora.id_asesor equals personas.documento
                                        join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                                        join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                                        where bitacora.id == id
                                        select new DetallesBi
                                        {
                                            id_bitacora = bitacora.id_bitacora,
                                            fecha_creacion = bitacora.fecha_creacion.ToString(),
                                            id = bitacora.id,
                                            encargado = personas.documento + " - " + personas.nombres + " " + personas.apellidos,
                                            archivo = bitacora.archivo,
                                            proyecto = solicitud.id_solicitud + " - " + solicitud.nombre_empresa,
                                            descripcion=bitacora.descripcion

                                        }).FirstOrDefault();

                if (bitacoraAsesor != null) {

                    ViewBag.Bitacora = bitacoraAsesor;
                    return View("DetallesBitacorasAP");
                }
                else
                {
                    TempData["Error"] = "Algo salió mal";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal";
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }      
        
        public ActionResult IndexAP()
        {

            try
            {

                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                if (TempData["Success"] != null)
                {
                    ViewBag.Success = TempData["Success"].ToString();

                }


                var Usuario = int.Parse(Session["Usuario"].ToString());
                var Rol = int.Parse(Session["Rol"].ToString());

                if (Rol == 2 || Rol == 4)
                {
                    if (Rol == 2)
                    {

                        ViewBag.bitacora = "ListaAprendiz";
                        ViewBag.descarga = "Aprendiz";
                        return View();
                    }
                    else
                    {

                        ViewBag.bitacora = "ListarAsesor";
                        ViewBag.descarga = "Asesor";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal";
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }


        //Listas Json
        [AuthorizeUserModules(1)]
        public JsonResult ListarBitacoras()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Bitacoras = (from bitacora in db.Bitacoras
                             select new { id_bitacora = bitacora.id_bitacora, fecha_creacion = bitacora.fecha_creacion.ToString(), descripcion = bitacora.descripcion }).ToList();

                    
            return Json(new { data = Bitacoras }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUserModules(1)]
        public JsonResult BitacorasAsesores()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Bitacoras = (from bitacora in db.Bitacora_Asesor
                             join personas in db.Personas on bitacora.id_asesor equals personas.documento
                             join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                             join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                             select new { 
                                 id_bitacora = bitacora.id_bitacora,
                                 fecha_creacion = bitacora.fecha_creacion.ToString(),
                                 id=bitacora.id,
                                 asesor=personas.documento+" - "+personas.nombres+" "+personas.apellidos,
                                 archivo=bitacora.archivo,
                                 proyecto=solicitud.id_solicitud+" - "+ solicitud.nombre_empresa
                                  
                             }).ToList();

                    
            return Json(new { data = Bitacoras }, JsonRequestBehavior.AllowGet);
        }
        
        [AuthorizeUserModules(1)]
        public JsonResult BitacorasAprendices()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Bitacoras = (from bitacora in db.Bitacora_Aprendiz
                             join personas in db.Personas on bitacora.id_aprendiz equals personas.documento
                             join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                             join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                             select new
                             {
                                 id_bitacora = bitacora.id_bitacora,
                                 fecha_creacion = bitacora.fecha_creacion.ToString(),
                                 id = bitacora.id,
                                 aprendiz = personas.documento + " - " + personas.nombres + " " + personas.apellidos,
                                 archivo = bitacora.archivo,
                                 proyecto = solicitud.id_solicitud + " - " + solicitud.nombre_empresa

                             }).ToList();


            return Json(new { data = Bitacoras }, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeUserModules(2)]
        public JsonResult ListaAprendiz()
        {

            var usuario = int.Parse(Session["Usuario"].ToString());

            db.Configuration.ProxyCreationEnabled = false;

            var Bitacoras = (from bitacora in db.Bitacora_Aprendiz
                             join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                             join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                             where bitacora.id_aprendiz == usuario
                             select new { 
                                 id_bitacora = bitacora.id_bitacora,
                                 fecha_creacion = bitacora.fecha_creacion.ToString(),
                                 descripcion = bitacora.descripcion,
                                 documento=bitacora.archivo,
                                 id= bitacora.id, proy=solicitud.id_solicitud + " - "+ solicitud.nombre_empresa }).ToList();

                
            return Json(new { data = Bitacoras }, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeUserModules(4)]
        public JsonResult ListarAsesor()
        {

            var usuario = int.Parse(Session["Usuario"].ToString());

            db.Configuration.ProxyCreationEnabled = false;
            var Bitacoras = (from bitacora in db.Bitacora_Asesor
                             join proyecto in db.Proyecto on bitacora.id_bitacora equals proyecto.id_bitacora
                             join solicitud in db.Solicitud on proyecto.id_proyecto equals solicitud.id_solicitud
                             where bitacora.id_asesor == usuario
                             select new {
                                 id_bitacora = bitacora.id_bitacora,
                                 fecha_creacion = bitacora.fecha_creacion.ToString(),
                                 descripcion = bitacora.descripcion,
                                 documento = bitacora.archivo,
                                 id = bitacora.id,
                                 proy = solicitud.id_solicitud + " - " + solicitud.nombre_empresa }).ToList();


            return Json(new { data = Bitacoras }, JsonRequestBehavior.AllowGet);
        }

        //Listas Json

        // GET: Bitacoras/Details/5
        [AuthorizeUserModules(1)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bitacoras bitacoras = db.Bitacoras.Find(id);
            if (bitacoras == null)
            {
                return HttpNotFound();
            }
            return View(bitacoras);
        }

        // GET: Bitacoras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bitacoras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult Create([Bind(Include = "descripcion")] Bitacoras bitacoras)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bitacoras.fecha_creacion = DateTime.Now;
                    db.Bitacoras.Add(bitacoras);
                    db.SaveChanges();
                    TempData["Success"] = "Bitacora Agregada con exito";
                    return RedirectToAction("Index", "Bitacoras");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Algo salió mal!";
                return RedirectToAction("Index", "Bitacoras");
            }

            TempData["Error"] = "Algo salió mal!";
            return RedirectToAction("Index", "Bitacoras");

        }

        // GET: Bitacoras/Edit/5
        [AuthorizeUserModules(1)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bitacoras bitacoras = db.Bitacoras.Find(id);
            if (bitacoras == null)
            {
                return HttpNotFound();
            }
            return  View(bitacoras);
        }

        // POST: Bitacoras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult Edit([Bind(Include = "id_bitacora,fecha_creacion,descripcion")] Bitacoras bitacoras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bitacoras).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Bitacora modificada con exito!";
                return RedirectToAction("Index", "Bitacoras");
            }

            TempData["Error"] = "Bitacora modificada con exito!";
            return RedirectToAction("Index", "Bitacoras");
        }

        // GET: Bitacoras/Delete/5
        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bitacoras bitacoras = db.Bitacoras.Find(id);
            if (bitacoras == null)
            {
                return HttpNotFound();
            }
            try
            {

                db.Bitacoras.Remove(bitacoras);
                db.SaveChanges();
                TempData["Success"] = "Bitacora eliminada con exito!";
                return RedirectToAction("Index", "Bitacoras");

            }
            catch (Exception)
            {
                TempData["Error"] = "La Bitacora se encuentra registrada en un proyecto, no puede ser eliminada";
                return RedirectToAction("Index", "Bitacoras");
            }

           
        }


        //Gestion Bitacoras Asesores y Aprendices!
        public ActionResult Registrarbitacora(int? id)
        {
            try
            {
                var rol = int.Parse(Session["Rol"].ToString());
                var usuario = int.Parse(Session["Usuario"].ToString());

                if (rol == 3)
                {
                    TempData["Error"] = "No tienes acceso a esta opcion";
                    return RedirectToAction("Dashboard", "Dashboard");
                }


                if (rol == 2)
                {
                    var proyecto = (from proy in db.Proyecto
                                    join AprnAsig in db.Grupo_Aprendices on proy.id_grupo equals AprnAsig.id_grupo
                                    where proy.id_bitacora == id && AprnAsig.id_aprendiz == usuario
                                    select AprnAsig).FirstOrDefault();


                    if (usuario!=proyecto.id_aprendiz)
                    {

                        TempData["Error"] = "No tienes acceso a esta opcion";
                        return RedirectToAction("Dashboard", "Dashboard");

                    }


                }
                else if(rol==4){


                    var proyecto = (from proy in db.Proyecto
                                    join grupo in db.Grupos on proy.id_grupo equals grupo.id_grupo
                                    where proy.id_bitacora == id
                                    select grupo).FirstOrDefault();



                    if (proyecto.id_lider != usuario)
                    {

                        TempData["Error"] = "No tienes acceso a esta opcion";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }

                }

                ViewBag.idBitacora = id;

                return View();

            }
            catch (Exception)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult Registrarbitacora(HttpPostedFileBase file, int id_bitacora, string descripcion)
        {
            try
            {
                var rol = int.Parse(Session["Rol"].ToString());
                var usuario = int.Parse(Session["Usuario"].ToString());




                if (rol == 3)
                {
                    TempData["Error"] = "No tienes acceso a esta opcion";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                if (rol == 4)
                {


                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);


                    //Evalidar que el archivo sea anexado al formulario y pese más de 0kb
                    if (file != null && file.ContentLength > 0)
                    {

                        var tp = file.ContentType.ToString();

                        //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                        if (tp.Equals("application/pdf"))
                        {


                            //en caso de existir el archivo se guarda la ruta y el nombre del archivo es agregado a la base de datos
                            string path = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Asesor"),
                                                                      Path.GetFileName(usuario + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName));

                            file.SaveAs(path);

                            Bitacora_Asesor bitacora = new Bitacora_Asesor();

                            bitacora.archivo = usuario + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName;
                            bitacora.fecha_creacion = DateTime.Now;
                            bitacora.id_bitacora = id_bitacora;
                            bitacora.id_asesor = usuario;
                            bitacora.descripcion = descripcion;
                            db.Bitacora_Asesor.Add(bitacora);
                            db.SaveChanges();

                            TempData["Success"] = "La bitacora fue agregada exitosamente!";
                            return RedirectToAction("IndexAsesor", "Proyectos");
                        }

                        else
                        {
                            TempData["Error"] = "El documento debe tener un formato PDF";
                            return RedirectToAction("IndexAsesor", "Proyectos");
                        }
                    }
                    else
                    {

                        TempData["Error"] = "Debes cargar un archivo";
                        return RedirectToAction("IndexAsesor", "Proyectos");
                    }


                }
                else if(rol==2)
                {

                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int numIterations = 0;
                    numIterations = rand.Next(1, 999);


                    //Evalidar que el archivo sea anexado al formulario y pese más de 0kb
                    if (file != null && file.ContentLength > 0)
                    {

                        var tp = file.ContentType.ToString();

                        //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                        if (tp.Equals("application/pdf"))
                        {


                            //en caso de existir el archivo se guarda la ruta y el nombre del archivo es agregado a la base de datos
                            string path = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Aprendiz"),
                                                                      Path.GetFileName(usuario + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName));

                            file.SaveAs(path);

                            Bitacora_Aprendiz bitacora = new Bitacora_Aprendiz();

                            bitacora.archivo = usuario + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm") + "-" + file.FileName;
                            bitacora.fecha_creacion = DateTime.Now;
                            bitacora.id_bitacora = id_bitacora;
                            bitacora.id_aprendiz = usuario;
                            bitacora.descripcion = descripcion;
                            db.Bitacora_Aprendiz.Add(bitacora);
                            db.SaveChanges();

                            TempData["Success"] = "La bitacora fue agregada exitosamente!";
                            return RedirectToAction("IndexAprendiz", "Proyectos");
                        }

                        else
                        {
                            TempData["Error"] = "El documento debe tener un formato PDF";
                            return RedirectToAction("IndexAprendiz", "Proyectos");
                        }
                    }
                    else
                    {
                        //En caso contrario el proyecto es guardado sin archivo

                        Bitacora_Aprendiz bitacoras = new Bitacora_Aprendiz();

                        bitacoras.archivo = null;
                        bitacoras.fecha_creacion = DateTime.Now;
                        bitacoras.id_bitacora = id_bitacora;
                        bitacoras.id_aprendiz = usuario;
                        bitacoras.descripcion = descripcion;
                        db.Bitacora_Aprendiz.Add(bitacoras);
                        db.SaveChanges();

                        TempData["Success"] = "La bitacora fue agregada exitosamente!";
                        return RedirectToAction("IndexAprendiz", "Proyectos");
                    }


                }
                else
                {
                    TempData["Error"] = "No tienes acceso a esta opcion";
                    return RedirectToAction("Dashboard", "Dashboard");
                }



            }
            catch (Exception err)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

        }


        public ActionResult EditarBitacora(int? id)
        {
            try
            {
                var Usuario = int.Parse(Session["Usuario"].ToString());
                var Rol = int.Parse(Session["Rol"].ToString());

                if (Rol == 2 || Rol == 4)
                {
                    if(Rol == 2)
                    {
                        var bitacoraAprendiz = (from listApren in db.Bitacora_Aprendiz
                                             where listApren.id_aprendiz == Usuario && listApren.id == id
                                             select listApren).FirstOrDefault();
                        if(bitacoraAprendiz == null)
                        {
                            TempData["Error"] = "Algo salió mal";
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                        var fechaBitacora = bitacoraAprendiz.fecha_creacion.ToString().Split('/');
                        var fehcaReal = DateTime.Now.ToString().Split('/');

                        if(!(fechaBitacora[1].Equals(fehcaReal[1]) && fechaBitacora[0].Equals(fehcaReal[0])))
                        {
                            TempData["Error"] = "No puedes modificar bitacoras que no sean actuales";
                            return RedirectToAction("IndexAP", "Bitacoras");
                        }

                        ViewBag.Bitacora = bitacoraAprendiz;
                        return View();
                    }
                    else
                    {
                        var bitacoraAsesor = (from listApren in db.Bitacora_Asesor
                                                where listApren.id_asesor == Usuario && listApren.id == id
                                                select listApren).FirstOrDefault();

                        if (bitacoraAsesor == null)
                        {
                            TempData["Error"] = "Algo salió mal";
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                        var fechaBitacora = bitacoraAsesor.fecha_creacion.ToString().Split('/');
                        var fehcaReal = DateTime.Now.ToString().Split('/');

                        if (!(fechaBitacora[1].Equals(fehcaReal[1]) && fechaBitacora[0].Equals(fehcaReal[0])))
                        {
                            TempData["Error"] = "No puedes modificar bitacoras que no sean actuales";
                            return RedirectToAction("IndexAP", "Bitacoras");
                        }

                        ViewBag.Bitacora = bitacoraAsesor;
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }  
        
        [HttpPost]
        public ActionResult EditarBitacora(HttpPostedFileBase file , int id, string descripcion)
        {
            try
            {
                var Usuario = int.Parse(Session["Usuario"].ToString());
                var Rol = int.Parse(Session["Rol"].ToString());

                if (Rol == 2 || Rol == 4)
                {
                    if(Rol == 2)
                    {

                        Bitacora_Aprendiz BitacoraAprendiz = db.Bitacora_Aprendiz.Find(id);

                        Random rand = new Random((int)DateTime.Now.Ticks);
                        int numIterations = 0;
                        numIterations = rand.Next(1, 999);


                        if (file != null && file.ContentLength > 0)
                        {

                            var tp = file.ContentType.ToString();

                            //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                            if (tp.Equals("application/pdf"))
                            {
                                string archivoviejo = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Aprendiz"),
                                                                      Path.GetFileName(BitacoraAprendiz.archivo));
                                System.IO.File.Delete(archivoviejo);

                                string path = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Aprendiz"),
                                                                          Path.GetFileName(Usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName));

                                file.SaveAs(path);


                                BitacoraAprendiz.descripcion = descripcion;
                                BitacoraAprendiz.archivo = Usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName;

                                db.Entry(BitacoraAprendiz).State = EntityState.Modified; ;
                                db.SaveChanges();


                                TempData["Success"] = "Bitacora Modificada!";
                                return RedirectToAction("IndexAP", "Bitacoras");
                            }
                            else
                            {
                                TempData["Error"] = "El documento debe tener un formato PDF";
                                return RedirectToAction("IndexAP", "Bitacoras");
                            }
                        }
                        else
                        {


                            BitacoraAprendiz.descripcion = descripcion;

                            db.Entry(BitacoraAprendiz).State = EntityState.Modified; ;
                            db.SaveChanges();


                            TempData["Success"] = "Bitacora Modificada!";
                            return RedirectToAction("IndexAP", "Bitacoras");

                        }


                       
                    }
                    else
                    {
                        if (file != null && file.ContentLength > 0)
                        {



                            var tp = file.ContentType.ToString();

                            //Validar que el archivo sea word o dpf y que pese menos de 12mb 
                            if (tp.Equals("application/pdf"))
                            {
                                Bitacora_Asesor Bitacora_Asesor = db.Bitacora_Asesor.Find(id);

                                Random rand = new Random((int)DateTime.Now.Ticks);
                                int numIterations = 0;
                                numIterations = rand.Next(1, 999);

                                string archivoviejo = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Asesor"),
                                                                      Path.GetFileName(Bitacora_Asesor.archivo));
                                System.IO.File.Delete(archivoviejo);

                                string path = Path.Combine(Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Asesor"),
                                                                          Path.GetFileName(Usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName));

                                file.SaveAs(path);


                                Bitacora_Asesor.descripcion = descripcion;
                                Bitacora_Asesor.archivo = Usuario + "-" + numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName;

                                db.Entry(Bitacora_Asesor).State = EntityState.Modified; ;
                                db.SaveChanges();


                                TempData["Success"] = "Bitacora Modificada!";
                                return RedirectToAction("IndexAP", "Bitacoras");
                            }
                            else
                            {
                                TempData["Error"] = "El documento debe tener un formato PDF";
                                return RedirectToAction("IndexAP", "Bitacoras");
                            }
                        }
                        else
                        {
                            Bitacora_Asesor Bitacora_Asesor = db.Bitacora_Asesor.Find(id);

                            Bitacora_Asesor.descripcion = descripcion;

                            db.Entry(Bitacora_Asesor).State = EntityState.Modified; ;
                            db.SaveChanges();

                            TempData["Success"] = "Bitacora Modificada!";
                            return RedirectToAction("IndexAP", "Bitacoras");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "El documento debe tener un formato PDF";
                return RedirectToAction("IndexAP", "Bitacoras");
            }
        }

        //Descargar Archivos
        [AuthorizeUserModules(6)]
        public ActionResult DescargarBitacorasAsesor(string ArchivoName)
        {
            try
            {
                if (!String.IsNullOrEmpty(ArchivoName) || ArchivoName != "null")
                {
                    string path = Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Asesor");
                    string fileName = Path.GetFileName(ArchivoName);
                    string fullPath = Path.Combine(path, fileName);

                    return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, ArchivoName);
                }
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

        }

        //Descargar Archivos
        [AuthorizeUserModules(6)]
        public ActionResult DescargarBitacorasAprendiz(string ArchivoName)
        {
            try
            {
                if (!String.IsNullOrEmpty(ArchivoName) || ArchivoName != "null")
                {
                    string path = Server.MapPath("~/Archivos/Bitacoras/Bitacoras_Aprendiz");
                    string fileName = Path.GetFileName(ArchivoName);
                    string fullPath = Path.Combine(path, fileName);

                    return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, ArchivoName);
                }
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

        }



        //Final Gestion Bitacoras Asesores y Aprendices!


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
