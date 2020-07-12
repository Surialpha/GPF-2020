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
    public class ClientesController : Controller
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
            return View();
        }

        [AuthorizeUserModules(1)]
        public ActionResult Lista()
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
                var rol = int.Parse(Session["Rol"].ToString());
                if (rol == 1)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var Clientes = (from cliente in db.Cliente
                                    join tipo_cliente in db.Tipo_Cliente on cliente.tipo_cliente equals tipo_cliente.id_TipoCliente
                                    join tipo_documento in db.Documento on cliente.id_documento equals tipo_documento.id_documento
                                    select new
                                    {
                                        id_cliente = cliente.id_cliente,
                                        nombre = cliente.nombre,
                                        email = cliente.email,
                                        tipo_cliente = tipo_cliente.descripcion,
                                        tipo_documento = tipo_documento.descripcion,
                                        numero = cliente.numero_contacto

                                    }
                                    ).ToList();
                    return Json(new { data = Clientes }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Clientes = new { };
                    return Json(new { data = Clientes }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                var Clientes = new { };
                return Json(new { data = Clientes }, JsonRequestBehavior.AllowGet);
            }

        }

        [AuthorizeUserModules(1)]
        public JsonResult ChartTipoClientes()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;



                var cliente = (from c in db.Cliente 
                               join tp in db.Tipo_Cliente on c.tipo_cliente equals tp.id_TipoCliente
                               select c).ToList();

                int ClExterno = cliente.Count(c => c.tipo_cliente == 1);
                int ClSena = cliente.Count(c => c.tipo_cliente == 2);




                var JsonFull ="["+ ClExterno + ","+ ClSena + "]";

                return Json(JsonFull, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserModules(3)]
        public JsonResult ProyectosCliente()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var user = int.Parse(Session["Usuario"].ToString());

                var cliente = (from s in db.Solicitud
                               where s.id_cliente == user
                               select s).ToList();

                var cliente2 = (from p in db.Proyecto
                                join s in db.Solicitud on p.id_proyecto equals s.id_solicitud
                               where s.id_cliente == user && p.id_estado==1
                               select s).Count();


                int NumEnviados, NumRecibidos, NumRevision, NumModificar, NumAprobados, NumDescartados = 0;


                NumEnviados = cliente.Count(x => x.id_estado_solicitud == 1);
                NumRecibidos = cliente.Count(x => x.id_estado_solicitud == 2);
                NumRevision = cliente.Count(x => x.id_estado_solicitud == 3);
                NumModificar = cliente.Count(x => x.id_estado_solicitud == 4);
                NumDescartados = cliente.Count(x => x.id_estado_solicitud == 6);

                var array = "[" + NumEnviados + "," + NumRecibidos + "," + NumRevision + "," + NumModificar + "," + cliente2 + "," + NumDescartados + "]";

                return Json(array, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Clientes/Details/5

        [AuthorizeUserModules(3)]
        public ActionResult InfoCliente()
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
                var id = int.Parse(Session["Usuario"].ToString());
                ViewBag.clientes = (from user in db.Usuarios
                                    where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                    select user).ToList();
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
                ViewBag.id_cliente = (from n in db.Usuarios where n.id_usuario == id select n.id_usuario).FirstOrDefault();
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("DashboardCliente", "Dashboard");
            }
            
        }

        [AuthorizeUserModules(3)]
        public ActionResult EditarInfoCliente()
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
                var id = int.Parse(Session["Usuario"].ToString());
                Cliente cliente = db.Cliente.Find(id);
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                ViewBag.tipo_cliente1 = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                return View(cliente);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("DashboardCliente", "Dashboard");
            }
            
        }

        [AuthorizeUserModules(3)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarInfoCliente([Bind(Include = "id_cliente,nombre,email,numero_contacto,id_documento,tipo_cliente")] Cliente cliente)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    cliente.email = (from n in db.Usuarios
                                     where n.id_usuario == cliente.id_cliente
                                     select n.email).FirstOrDefault();
                    db.Entry(cliente).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "¡Ha modificado correctamente!";
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                return View(cliente);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                return View(cliente);
            }
        }

        [AuthorizeUserModules(3)]
        [HttpPost]
        public ActionResult InfoCliente([Bind(Include = "id_cliente,nombre,numero_contacto,id_documento,tipo_cliente")] Cliente cliente)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    cliente.email = (from n in db.Usuarios
                                     where n.id_usuario == cliente.id_cliente
                                     select n.email).FirstOrDefault();

                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    TempData["Success"] = "¡Registro completado!";
                    return RedirectToAction("DashboardCliente", "Dashboard");
                }

                var id = int.Parse(Session["Usuario"].ToString());
                ViewBag.clientes = (from user in db.Usuarios
                                    where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                    select user).ToList();
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
                ViewBag.id_cliente = (from n in db.Usuarios where n.id_usuario == id select n.id_usuario).FirstOrDefault();
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return View();
            }
            catch (Exception err)
            {
                var id = int.Parse(Session["Usuario"].ToString());
                ViewBag.clientes = (from user in db.Usuarios
                                    where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                    select user).ToList();
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
                ViewBag.id_cliente = (from n in db.Usuarios where n.id_usuario == id select n.id_usuario).FirstOrDefault();
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return View();
            }



            
        }


        [AuthorizeUserModules(1)]
        public ActionResult Create()
        {
            ViewBag.clientes = (from user in db.Usuarios
                                               where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                               select user).ToList();
            ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
            ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_cliente,nombre,numero_contacto,id_documento,tipo_cliente")] Cliente cliente)
        {
            try
            {
                if (cliente.id_cliente==0)
                {
                    ViewBag.clientes = (from user in db.Usuarios
                                        where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                        select user).ToList();
                    ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                    ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
                    ViewBag.Error = "No hay clientes por registrar";
                    return View(cliente);


                }
                if (ModelState.IsValid)
                {
                    cliente.email = (from n in db.Usuarios
                                     where n.id_usuario == cliente.id_cliente
                                     select n.email).FirstOrDefault();

                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.clientes = (from user in db.Usuarios
                                    where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                    select user).ToList();
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");
                ViewBag.Error = "Completa correctamente el formulario";
                return View(cliente);

            }
            catch (Exception)
            {
                ViewBag.clientes = (from user in db.Usuarios
                                    where user.id_rol == 3 && !(from clientes in db.Cliente select clientes.id_cliente).Contains(user.id_usuario)
                                    select user).ToList();
                ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion");
                ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion");

                ViewBag.Error = "Error al Registrar";
                return View(cliente);
            }
          
        }


        [AuthorizeUserModules(6)]
        public ActionResult DetalleCliente(int? id)
        {
            try
            {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Cliente cliente = db.Cliente.Find(id);
                    if (cliente == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                    ViewBag.id_cliente = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_cliente);
                    ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                    return View(cliente);
               
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Dashboard", "Dashboard");
            }

            
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (Session["Logged"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Cliente cliente = db.Cliente.Find(id);
                    if (cliente == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                    ViewBag.id_cliente = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_cliente);
                    ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                    return View(cliente);
                }
                else
                {
                    TempData["Error"] = "¡Ingresa para ver este módulo!";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Dashboard", "Dashboard");
            }     
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_cliente,nombre,email,numero_contacto,id_documento,tipo_cliente")] Cliente cliente)
        {
            if (Session["Logged"] != null)
            {

                try
                {

                    if (ModelState.IsValid)
                    {
                        cliente.email = (from n in db.Usuarios
                                         where n.id_usuario == cliente.id_cliente
                                         select n.email).FirstOrDefault();
                        db.Entry(cliente).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Success"] = "¡Modificado exitosamente!";
                        return RedirectToAction("Index");
                    }
                    ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                    ViewBag.id_cliente = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_cliente);
                    ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                    ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                    return View(cliente);
                }
                catch (Exception)
                {
                    ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                    ViewBag.tipo_cliente = new SelectList(db.Tipo_Cliente, "id_TipoCliente", "descripcion", cliente.tipo_cliente);
                    ViewBag.id_cliente = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_cliente);
                    ViewBag.id_documento = new SelectList(db.Documento, "id_documento", "descripcion", cliente.id_documento);
                    return View(cliente);
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
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
           
            Cliente cliente = db.Cliente.Find(id);
            ViewBag.cliente = (from n in db.Tipo_Cliente
                               where n.id_TipoCliente==cliente.tipo_cliente
                               select n.descripcion).FirstOrDefault();

            ViewBag.documento = (from n in db.Documento
                               where n.id_documento == cliente.id_documento
                               select n.descripcion).FirstOrDefault();
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [AuthorizeUserModules(1)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cliente cliente = db.Cliente.Find(id);
                db.Cliente.Remove(cliente);
                db.SaveChanges();
                TempData["Success"] = "¡Se ha eliminado correctamente!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = "¡No es posible eliminar el cliente, valida que no se encuentra asociado a un proyecto o solicitud!";
                return RedirectToAction("Index");
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
