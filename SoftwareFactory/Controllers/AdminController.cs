using System;
using System.Collections.Generic;
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
    public class AdminController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        [AuthorizeUserModules(1)]
        public ActionResult Index()
        {
            //Enviar mensajes por medio de otro ActionResult o controlador//
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            //-----------------//


            ViewBag.QuienesSomos = (from quienes in db.Quienes_Somos select quienes.descripcion).FirstOrDefault();
            ViewBag.ComoLoHacemos = (from hacemos in db.Como_Lo_Hacemos select hacemos.descripcion).FirstOrDefault();

            Quienes_Somos EditQuienes = db.Quienes_Somos.Find(1);
            ViewBag.Quienes = EditQuienes;
            Como_Lo_Hacemos editcomo = db.Como_Lo_Hacemos.Find(1);
            ViewBag.como = editcomo;
            ViewBag.idTipo = new SelectList(db.Tipo_Imagen, "idTipo", "descripcion");

            return View();
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListarServicio()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Servicio = (from servicio in db.Nuestros_Servicios
                            select new
                            {
                                idServicio = servicio.id,
                                nombre = servicio.nombre,
                                descripcion = servicio.descripcion
                            }
                            ).ToList();

            return Json(new { data = Servicio }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListarAdquirir()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Adquirir = (from adquirir in db.Adquirir_Servicios
                            select new
                            {
                                idAdquirir = adquirir.id,
                                nombre = adquirir.nombre,
                                descripcion = adquirir.descripcion
                            }
                            ).ToList();

            return Json(new { data = Adquirir }, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeUserModules(1)]
        public JsonResult Imagenes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var imagenes = (from imagen in db.Imagenes
                            join tipo in db.Tipo_Imagen on imagen.idTipo equals tipo.idTipo
                            select new
                            {
                                idImagen = imagen.idImage,
                                TipoImagen=tipo.descripcion,
                                nombre=imagen.nombreUsuario,
                                imagenUrl=imagen.imageUrl

                            }
                            ).ToList();

            return Json(new { data = imagenes }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUserModules(1)]
        public ActionResult EditarServicio(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Nuestros_Servicios servicios = db.Nuestros_Servicios.Find(id);
                if (servicios == null)
                {
                    return HttpNotFound();
                }
                return View(servicios);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }

            
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarServicio([Bind(Include = "id,nombre,descripcion")] Nuestros_Servicios servicios)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(servicios).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "¡Editado Correctamente!";
                    return RedirectToAction("Index", "Admin");
                }

                TempData["Error"] = "¡No se pudo modificar la información, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult EditarAdquirir(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Adquirir_Servicios adquirir = db.Adquirir_Servicios.Find(id);
                if (adquirir == null)
                {
                    return HttpNotFound();
                }
                return View(adquirir);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }

           
            
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAdquirir([Bind(Include = "id,nombre,descripcion")] Adquirir_Servicios adquirir)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(adquirir).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "¡Editado Correctamente!";
                    return RedirectToAction("Index", "Admin");
                }

                TempData["Error"] = "¡No se pudo modificar la información, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
            
        }

        [AuthorizeUserModules(1)]
        public ActionResult EditQuienes()
        {
            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult EditQuienes([Bind(Include = "id,descripcion")] Quienes_Somos somos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Quienes_Somos quienes = new Quienes_Somos()
                    {
                        id = 1,
                        descripcion = somos.descripcion
                    };

                    Quienes_Somos FindQuienes = db.Quienes_Somos.Find(quienes.id);
                    FindQuienes.descripcion = quienes.descripcion;
                    db.SaveChanges();
                    TempData["Success"] = "¡Editado Correctamente!";
                    return RedirectToAction("Index", "Admin");
                }

                TempData["Error"] = "¡No se pudo modificar la información, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");

            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            } 
        }

        [AuthorizeUserModules(1)]
        public ActionResult EditComoLoHacemos()
        {
            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult EditComoLoHacemos([Bind(Include = "id,descripcion")] Como_Lo_Hacemos como)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Como_Lo_Hacemos hacemos = new Como_Lo_Hacemos()
                    {
                        id = 1,
                        descripcion = como.descripcion
                    };

                    Como_Lo_Hacemos FindQuienes = db.Como_Lo_Hacemos.Find(hacemos.id);
                    FindQuienes.descripcion = hacemos.descripcion;
                    db.SaveChanges();
                    TempData["Success"] = "¡Editado Correctamente!";
                    return RedirectToAction("Index", "Admin");
                }

                TempData["Error"] = "¡No se pudo modificar la información, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");

            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
        }

        //Agregar imagenes al index

        [AuthorizeUserModules(1)]
        public ActionResult AgregarImagenes()
        {
            try
            {
                //validacion para que solo el lider de fabrica pueda entrar al metodo de agregar imagenes
                if (Session["Logged"] != null && Session["Usuario"].Equals("1"))
                {
                    
                    ViewBag.idTipo = new SelectList(db.Tipo_Imagen, "idTipo", "descripcion");

                    return View();

                }

                return RedirectToAction("Cerrar", "Acceso");
            }
            catch (Exception)
            {
                return RedirectToAction("Cerrar", "Acceso");
            }
           
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult AgregarImagenes(HttpPostedFileBase file,int idTipo, string nombreUsuario)
        {

            try
            {


                Imagenes imagen = new Imagenes();

                Random rand = new Random((int)DateTime.Now.Ticks);
                int numIterations = 0;
                numIterations = rand.Next(1, 999);

                if (file != null && file.ContentLength > 0)
                {
                    //Verifica formato para que se carguen solo formatos validos
                    var tp = file.ContentType.ToString();

                    if (tp.Equals("image/png") || tp.Equals("image/jpeg") || tp.Equals("image/jpg"))
                    {

                        string path = Path.Combine(Server.MapPath("~/Vendor/ImgIndex"),
                                          Path.GetFileName(numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName));

                        file.SaveAs(path);
                        imagen.imageUrl = numIterations + "-" + DateTime.Now.ToString("dd-MM-yy") + "-" + file.FileName;
                        imagen.idTipo = idTipo;
                        imagen.nombreUsuario = nombreUsuario;
                        db.Imagenes.Add(imagen);
                        db.SaveChanges();

                        TempData["Success"] = "¡Se ha añadido correctamente!";
                        return RedirectToAction("Index", "Admin");

                    }
                    //Verifica formato para que se carguen solo formatos validos
                    else
                    {
                        //De lo contrario será devuelto al index con una alerta 
                        TempData["Error"] = "Debes elegir un formato de imagen válido";
                        return RedirectToAction("Index", "Admin");
                    }

                }
                else
                {
                    //Si no hay archivo, el usuario es devuelto al index con un mensaje que debe cargar una imagen
                    TempData["Error"] = "Debes cargar una imagen";
                    return RedirectToAction("Index", "Admin");
                }


            }
            catch (Exception)
            {
                //En caso de error de algun tipo, se devuelve al index con alerta
                TempData["Error"] = "Algo salió mal! vuelve a intentarlo";
                return RedirectToAction("Index", "Admin");
            }


        }

        [AuthorizeUserModules(1)]
        public ActionResult EliminarImagen(int id)
        {
            try
            {
                Imagenes imagen = db.Imagenes.Find(id);
                db.Imagenes.Remove(imagen);
                db.SaveChanges();

                string archivoviejo = Path.Combine(Server.MapPath("~/Vendor/ImgIndex"),
                                                                      Path.GetFileName(imagen.imageUrl));
                System.IO.File.Delete(archivoviejo);
                TempData["Success"] = "Imagen borrada con éxito";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index", "Admin");
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult Pqrs()
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
        public JsonResult ListaPqrs()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var pqrs = (from p in db.pqrs where p.nombre != "Recuperación de contraseña" select p).ToList();
                return Json(new { data = pqrs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var pqrs = new { };
                return Json(new { data = pqrs }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult DetailsPqrs(int? id)
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

                pqrs pqr = db.pqrs.Find(id);
                if (pqr == null)
                {
                    return HttpNotFound();
                }

                return View(pqr);
            }
            catch (Exception)
            {

                TempData["Error"] = "¡Ha ocurrido un error, intenta nuevamente!";
                return RedirectToAction("Index");
            }


           
        }

        [AuthorizeUserModules(1)]
        public ActionResult Delete(int id)
        {
            try
            {
                pqrs pqr = db.pqrs.Find(id);

                if (pqr == null)
                {
                    TempData["Error"] = "¡No podemos encontrar el registro para eliminarlo!";
                    return RedirectToAction("Pqrs");
                }

                db.pqrs.Remove(pqr);
                db.SaveChanges();
                TempData["Success"] = "¡Se ha eliminado correctamente!";
                return RedirectToAction("Pqrs");

            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error, intenta nuevamente!";
                return RedirectToAction("Pqrs");
            }
        }

        public ActionResult RecuperarContraseña()
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
                return View();
            }
            catch (Exception)
            {

                return View();
            }
            
        }

        [HttpPost]
        public ActionResult RecuperarContraseña(pqrs pqrs)
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

                var consulta = (from u in db.Usuarios where u.email == pqrs.correo select u).FirstOrDefault();

                if (consulta == null)
                {
                    TempData["Error"] = "¡El correo no se encuentra registrado!";
                    return RedirectToAction("RecuperarContraseña");
                }

                if (ModelState.IsValid)
                {
                    pqrs.nombre = "Recuperación de contraseña";
                    pqrs.descripcion = "Solicita amablemente recuperar mi contraseña, comunicarse conmigo al correo ingresado";
                    db.pqrs.Add(pqrs);
                    db.SaveChanges();
                    TempData["Success"] = "¡Solicitud enviada!";
                    return RedirectToAction("RecuperarContraseña");
                }
                TempData["Error"] = "¡Ha ocurrido un error, intenta nuevamente!";
                return RedirectToAction("RecuperarContraseña");
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error, intenta nuevamente!";
                return RedirectToAction("RecuperarContraseña");
            }

        }

        [AuthorizeUserModules(1)]
        public ActionResult SolicitudRecuperacion()
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
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error, intenta nuevamente!";
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        [AuthorizeUserModules(1)]
        public JsonResult ListaSolicitudRecuperacion()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var pqrs = (from p in db.pqrs where p.nombre == "Recuperación de contraseña" select p).ToList();
                return Json(new { data = pqrs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var pqrs = new { };
                return Json(new { data = pqrs }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}