using SoftwareFactory.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Scrypt;
using SoftwareFactory.Vendor.ExtraClass;
using SoftwareFactory.Filtros;

namespace SoftwareFactory.Controllers
{
    public class UsuariosController : Controller
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
        public JsonResult Lista()
        {
            try
            {
                var id = int.Parse(Session["Usuario"].ToString());
               
                    db.Configuration.ProxyCreationEnabled = false;
                    var usuarios = (from user in db.Usuarios
                                    join rols in db.Roles on user.id_rol equals rols.id_rol
                                    join state in db.Estados on user.id_estado equals state.id_estado
                                    where user.id_usuario != id
                                    select new { Rolesname = rols.descripcion, Statename = state.descripcion, id_usuario = user.id_usuario, email = user.email, fecha_creacion = user.fecha_creacion.ToString(), id_rol = user.id_rol, id_estado = user.id_estado }
                        ).ToList();

                    return Json(new { data = usuarios }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                var usuarios = new { };
                return Json(new { data = usuarios }, JsonRequestBehavior.AllowGet);
            }



        }

        [AuthorizeUserModules(1)]
        //Listado proyectos Admin *Lista Json
        public JsonResult ChartUsuarios()
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var variables = (from users in db.Usuarios
                                 select users
                                ).ToList();
                int NumAprendicesA, NumAsesoresA, NumClientesA, NumAdminA, NumAprendicesI, NumAsesoresI, NumClientesI, NumAdminI = 0;


                NumAprendicesA = variables.Where(x => x.id_estado == 1).Where(x=>x.id_rol==2).Count();
                NumAsesoresA = variables.Where(x => x.id_estado == 1).Where(x=>x.id_rol==4).Count();
                NumClientesA = variables.Where(x => x.id_estado == 1).Where(x=>x.id_rol==3).Count();
                NumAdminA = variables.Where(x => x.id_estado == 1).Where(x=>x.id_rol==1).Count();

                NumAprendicesI = variables.Where(x => x.id_estado == 2).Where(x=>x.id_rol==2).Count();
                NumAsesoresI = variables.Where(x => x.id_estado == 2).Where(x=>x.id_rol==4).Count();
                NumClientesI = variables.Where(x => x.id_estado == 2).Where(x=>x.id_rol==3).Count();
                NumAdminI = variables.Where(x => x.id_estado == 2).Where(x=>x.id_rol==1).Count();

                var arrayA = "[" + NumAprendicesA + "," + NumAsesoresA + "," + NumClientesA + "," + NumAdminA + "]";
                var arrayI = "[" + NumAprendicesI + "," + NumAsesoresI + "," + NumClientesI + "," + NumAdminI + "]";
                var arrayCon = "["+ arrayA + ","+ arrayI + "]";

                return Json(arrayCon, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var array = new { };
                return Json(array, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Usuarios/Details/5
        [AuthorizeUserModules(6)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        [AuthorizeUserModules(6)]
        public ActionResult CambiarContraseñaUsers()
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

                    var id = int.Parse(Session["Usuario"].ToString());
                    Password pass = new Password();
                    pass.idU = id;
                    return View(pass);

                }

                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente";
                return RedirectToAction("Cerrar", "Acceso");
            }
            catch (Exception)
            {
                return RedirectToAction("Cerrar", "Acceso");

            }
        }
        [HttpPost]
        [AuthorizeUserModules(6)]
        public ActionResult CambiarContraseñaUsers(Password changePass)
        {
            if (ModelState.IsValid)
            {
                ScryptEncoder encoder = new ScryptEncoder();
                Usuarios usuarios = db.Usuarios.Find(changePass.idU);

                bool Validar = encoder.Compare(changePass.oldPass, usuarios.hash_password);
                if (Validar)
                {
                    usuarios.hash_password = encoder.Encode(changePass.newPass);
                    db.Entry(usuarios).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "¡Contraseña modificada, por favor ingrese nuevamente!";
                    return RedirectToAction("Cerrar", "Acceso");
                }
                else
                {
                    ViewBag.Error = "¡Las contraseñas no coinciden!";
                    return View();
                }
            }

            ViewBag.Error = "¡Ha ocurrido un error, intenta nuevamente!";
            return View();



        }

        [AuthorizeUserModules(1)]
        public ActionResult CambiarContraseñaAdmin(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Password pass = new Password();
                pass.idU = (int)id;

                return View(pass);
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        public ActionResult CambiarContraseñaAdmin(Password changePass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = int.Parse(Session["Usuario"].ToString());

                    var adminpass = (from n in db.Usuarios
                                     where n.id_usuario == user
                                     select n.hash_password).FirstOrDefault();
                    if (adminpass == null)
                    {
                        ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                        return View();
                    }

                    ScryptEncoder encoder = new ScryptEncoder();
                    Usuarios usuarios = db.Usuarios.Find(changePass.idU);
                    if (usuarios == null)
                    {
                        ViewBag.Error = "¡Usuario no encontrado, intenta nuevamente!";
                        return View();
                    }

                    bool Validar = encoder.Compare(changePass.oldPass, adminpass);
                    if (Validar)
                    {
                        usuarios.hash_password = encoder.Encode(changePass.newPass);
                        db.Entry(usuarios).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Success"] = "¡Contraseña de usuario modificada correctamente!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "¡Contraseña de administrador incorrecta, intenta nuevemente!";
                        return View();
                    }
                }

                ViewBag.Error = "¡La contraseña no pudo ser cambiada!";
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
        }

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
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion");
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion");
            return View();
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_usuario,email,hash_password,id_rol,id_estado")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                // Buscamos que el correo no se encuentre registrado
                var validador = (from n in db.Usuarios                 
                                 where n.email == usuarios.email
                                 select n
                                ).FirstOrDefault();
               
                
                if(validador==null)
                {
                    try
                    {
                        ScryptEncoder encoder = new ScryptEncoder();
                        usuarios.email = usuarios.email.ToString().ToLower();
                        usuarios.fecha_creacion = DateTime.Now;
                        usuarios.hash_password = encoder.Encode(usuarios.hash_password);
                        db.Usuarios.Add(usuarios);
                        db.SaveChanges();
                        TempData["Success"] = "¡Usuario registrado exitosamente!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ViewBag.Error = "El documento o identificador ya se encuentra registrado actualmente";
                        ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion", usuarios.id_estado);
                        ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion", usuarios.id_rol);
                        return View(usuarios);
                    }
                    
                }
                else
                {
                    ViewBag.Error = "El correo se encuentra registrado actualmente";
                    ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion", usuarios.id_estado);
                    ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion", usuarios.id_rol);
                    return View(usuarios);
                }
                

            }
            ViewBag.Error = "¡Ha ocurrido un error, intenta nuevamente!";
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion", usuarios.id_estado);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion", usuarios.id_rol);
            return View(usuarios);
        }

        [AuthorizeUserModules(1)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion", usuarios.id_estado);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion", usuarios.id_rol);
            return View(usuarios);
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_usuario,email,fecha_creacion,hash_password,id_rol,id_estado")] Usuarios usuarios)
        {
            var validador = (from n in db.Usuarios
                             where n.email == usuarios.email && n.id_usuario!=usuarios.id_usuario
                             select n).FirstOrDefault();

            var con = db.Usuarios.Find(usuarios.id_usuario);

            if (con.id_rol != usuarios.id_rol)
            {
                if (con.id_rol == 2)
                {
                    var aprendiz = db.Aprendices.Find(usuarios.id_usuario);

                    if (aprendiz != null)
                    {
                        TempData["Error"] = "¡No se puede modificar el Rol de Aprendiz";
                        return RedirectToAction("Index");
                    }
                }
                else if(con.id_rol == 4)
                {
                    var asesor = db.Asesores.Find(usuarios.id_usuario);
                    if (asesor != null)
                    {
                        TempData["Error"] = "¡No se puede modificar el Rol de Asesor";
                        return RedirectToAction("Index");
                    }
                }
                else if(con.id_rol == 3)
                {
                    var cliente = db.Cliente.Find(usuarios.id_usuario);
                    if (cliente != null)
                    {
                        TempData["Error"] = "¡No se puede modificar el Rol de Cliente";
                        return RedirectToAction("Index");
                    }
                }
            }

            if (validador==null)
            {

                if (ModelState.IsValid)
                {
                    con.id_rol = usuarios.id_rol;
                    con.id_estado = usuarios.id_estado;
                    con.email = usuarios.email.ToString().ToLower();
                    db.Entry(con).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "¡Modificado correctamente";
                    return RedirectToAction("Index");
                }
                

            }

            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "descripcion", usuarios.id_estado);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "descripcion", usuarios.id_rol);
            ViewBag.Error = "El correo ya se encuentra registrado";
            return View(usuarios);


        }

        


        public ActionResult RegistroCliente()
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

        [HttpPost]
        public ActionResult RegistroCliente([Bind(Include = "id_usuario,email,hash_password")] Usuarios usuarios)
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

                if (usuarios.email == null)
                {
                    ViewBag.Error = "¡Ingrese un correo electrónico válido!";
                    return View(usuarios);
                }
                if (usuarios.id_usuario <= 0)
                {
                    ViewBag.Error = "¡Ingrese un documento válido!";
                    return View(usuarios);
                }


                var user = (from n in db.Usuarios
                             where n.email == usuarios.email
                             select n).FirstOrDefault();

                var user2 = (from n in db.Usuarios
                            where n.id_usuario==usuarios.id_usuario
                            select n).FirstOrDefault();

                if (user!=null)
                {
                    ViewBag.Error = "¡El correo ya se encuentra registrado!";
                    return View(usuarios);
                }
                else if (user2!=null)
                {
                    ViewBag.Error = "¡El documento ya se encuentra registrado!";
                    return View(usuarios);
                }
                else
                {
                    
                    if (ModelState.IsValid)
                    {
                        usuarios.email = usuarios.email.ToString().ToLower();
                        usuarios.fecha_creacion = DateTime.Now;
                        usuarios.id_rol = 3;
                        usuarios.id_estado = 2;
                        ScryptEncoder encoder = new ScryptEncoder();
                        usuarios.hash_password = encoder.Encode(usuarios.hash_password);
                        db.Usuarios.Add(usuarios);
                        db.SaveChanges();
                        TempData["Success"] = "El usuario se ha registrado exitosamente, puede loguearse pero debe esperar a que su cuenta sea activada poder rellenar formularios de solicitud de proyecto";
                        return RedirectToAction("IniciarCliente","Acceso");
                    }
                    else
                    {
                        ViewBag.Error = "¡Rellene la información correspondiente!";
                        return View(usuarios);
                    }
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return View(usuarios);
            }

            
        }



        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        [AuthorizeUserModules(1)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuarios = db.Usuarios.Find(id);

            if (usuarios.id_rol == 3)
            {
                var cons = (from c in db.Cliente
                            where c.id_cliente == usuarios.id_usuario
                            select c).FirstOrDefault();

                if (cons != null)
                {
                    TempData["Error"] = "¡No es posible eliminar el usuario, se encuentra registrado como cliente!";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                var con = (from p in db.Personas
                           where p.documento == usuarios.id_usuario
                           select p).FirstOrDefault();

                if (con != null)
                {
                    TempData["Error"] = "¡No es posible eliminar el usuario!";
                    return RedirectToAction("Index");
                }

            }
            db.Usuarios.Remove(usuarios);
            db.SaveChanges(); 
            TempData["Success"] = "¡Usuario eliminado exitosamente!";
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
