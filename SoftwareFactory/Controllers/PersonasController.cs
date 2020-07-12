using SoftwareFactory.Filtros;
using SoftwareFactory.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SoftwareFactory.Controllers
{
    public class PersonasController : Controller
    {
        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        // GET: Personas
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
                    db.Configuration.ProxyCreationEnabled = false;
                    var personas = (from person in db.Personas
                                    join gender in db.Sexo on person.id_sexo equals gender.id_sexo
                                    join document in db.Tipo_Documento on person.id_documento equals document.id_documento
                                    join user in db.Usuarios on person.documento equals user.id_usuario
                                    where user.id_rol != 1
                                    select new
                                    {
                                        documentName = document.descripcion,
                                        document = person.documento,
                                        genderName = gender.descripcion,
                                        names = person.nombres,
                                        lastName = person.apellidos,
                                        bornDate = person.fecha_nacimiento.ToString(),
                                        number = person.numero_contacto,
                                        email = person.email
                                    }
                                               ).ToList();

                    return Json(new { data = personas }, JsonRequestBehavior.AllowGet);
 
                }
            catch (System.Exception)
            {
                var personas = new { };
                return Json(new { data = personas }, JsonRequestBehavior.AllowGet);
            }


           

        }

        // GET: Personas/Details/5
        [AuthorizeUserModules(1)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personas personas = db.Personas.Find(id);
            if (personas == null)
            {
                return HttpNotFound();
            }
            return View(personas);
        }

        // GET: Personas/Create
        // En caso de cambios de base de datos! el tres que se ve en la consulta, pertenece a cliente! 
        [AuthorizeUserModules(1)]
        public ActionResult RegistrarPersona()
        {
            var usuarios = (from user in db.Usuarios
                            where user.id_rol != 3 && !(from personas in db.Personas select personas.documento).Contains(user.id_usuario)
                            select user).ToList();
            ViewBag.usuario = usuarios;
            ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion");
            ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion");
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult RegistrarPersona([Bind(Include = "documento,nombres,apellidos,fecha_nacimiento,numero_contacto,id_documento,id_sexo")] Personas personas)
        {

            try
            {

                if(personas.fecha_nacimiento==null || personas.nombres==null || personas.apellidos==null)
                {
                    var usuarios2 = (from user in db.Usuarios
                                    where user.id_rol != 3 && !(from persona in db.Personas select persona.documento).Contains(user.id_usuario)
                                    select user).ToList();
                    ViewBag.usuario = usuarios2;
                    ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
                    ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
                    ViewBag.Error = "¡Por favor rellene los datos e intenta nuevamente";
                    return View(personas);

                }
                if (ModelState.IsValid)
                {

                    var usua = (from user in db.Usuarios
                                select user).ToList();

                    foreach (var item in usua)
                    {
                        if (item.id_usuario == personas.documento)
                        {
                            personas.email = item.email;
                        }
                    }
                    db.Personas.Add(personas);
                    db.SaveChanges();
                    TempData["Success"] = "¡Registro exitoso!";
                    return RedirectToAction("Index");
                }
                var usuarios = (from user in db.Usuarios
                                where user.id_rol != 3 && !(from persona in db.Personas select persona.documento).Contains(user.id_usuario)
                                select user).ToList();
                ViewBag.usuario = usuarios;
                ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
                ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
                ViewBag.Error = "¡No se pudo modificar, intenta nuevamente!";
                return View(personas);
            }
            catch (System.Exception)
            {

                var usuarios = (from user in db.Usuarios
                                where user.id_rol != 3 && !(from persona in db.Personas select persona.documento).Contains(user.id_usuario)
                                select user).ToList();
                ViewBag.usuario = usuarios;
                ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
                ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
                ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return View(personas);
            }

           
        }


        //Editar Persona desde perfil propio


        // GET: Personas/Edit/5
        [AuthorizeUserModules(6)]
        public ActionResult EditPersonal()
        {
            try
            {
                var id = int.Parse(Session["Usuario"].ToString());

                Personas personas = db.Personas.Find(id);
                if (personas == null)
                {
                    TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                    RedirectToAction("Dashboard", "Dashboard");
                }
                ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
                ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
                return View(personas);

            }
            catch (System.Exception)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Dashboard", "Dashboard");
            }

            
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(6)]
        public ActionResult EditPersonal([Bind(Include = "documento,nombres,apellidos,email,fecha_nacimiento,numero_contacto,id_documento,id_sexo")] Personas personas)
        {
            if (ModelState.IsValid)
            {
                var usua = (from user in db.Usuarios
                            select user).ToList();

                foreach (var item in usua)
                {
                    if (item.id_usuario == personas.documento)
                    {
                        personas.email = item.email;
                    }
                }
                db.Entry(personas).State = EntityState.Modified;
                db.SaveChanges(); 
                TempData["Success"] = "¡Modificación exitosa!";
                return RedirectToAction("Dashboard", "Dashboard");
            }
            ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
            ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
            ViewBag.Error = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
            return View(personas);
        }


        //Editar Persona desde perfil propio





        // GET: Personas/Edit/5
        [AuthorizeUserModules(1)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personas personas = db.Personas.Find(id);
            ViewBag.rol  = db.Usuarios.Find(id).id_rol;
            if (personas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
            ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
            return View(personas);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult Edit([Bind(Include = "documento,nombres,apellidos,email,fecha_nacimiento,numero_contacto,id_documento,id_sexo")] Personas personas)
        {
            if (ModelState.IsValid)
            {
                var usua = (from user in db.Usuarios
                            select user).ToList();

                foreach (var item in usua)
                {
                    if (item.id_usuario == personas.documento)
                    {
                        personas.email = item.email;
                    }
                }
                db.Entry(personas).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "¡Modificación exitosa!";
                return RedirectToAction("Index");
            }
            ViewBag.id_sexo = new SelectList(db.Sexo, "id_sexo", "descripcion", personas.id_sexo);
            ViewBag.id_documento = new SelectList(db.Tipo_Documento, "id_documento", "descripcion", personas.id_documento);
            ViewBag.Error = "¡No se pudo modificar, intenta nuevamente!";
            return View(personas);
        }

        // GET: Personas/Delete/5
        [AuthorizeUserModules(1)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personas personas = db.Personas.Find(id);
            if (personas == null)
            {
                return HttpNotFound();
            }
            return View(personas);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserModules(1)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                Personas personas = db.Personas.Find(id);
                db.Personas.Remove(personas);
                db.SaveChanges();
                TempData["Success"] = "¡Eliminado correctamente!";
                return RedirectToAction("Index");

            }
            catch (System.Exception)
            {
                ViewBag.Error = "La persona no puede ser Eliminada, asegurate de que no esté registrada como Asesor o como Aprendiz";
                Personas personas = db.Personas.Find(id);
                return View(personas);
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
