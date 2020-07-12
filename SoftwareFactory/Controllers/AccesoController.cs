using System;
using System.Linq;
using System.Web.Mvc;
using Scrypt;
using SoftwareFactory.Filtros;

namespace SoftwareFactory.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Usuarios
        public ActionResult Iniciar()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Iniciar(string Email, string Password)
        {
            try
            {
                using (Models.FabricaSoftwareEntities db = new Models.FabricaSoftwareEntities())
                {
                    var oUser = (from d in db.Usuarios
                                 where d.email == Email.Trim()
                                 select d).FirstOrDefault();



                    if (oUser == null)
                    {
                        ViewBag.Error = "El usuario es invalido";
                        return View();
                    }
                    else
                    {
                        if (oUser.id_estado == 2)
                        {

                            ViewBag.Error = "El usuario se encuentra inactivo";
                            return View();
                        }
                        ScryptEncoder encoder = new ScryptEncoder();
                        bool validarpass = encoder.Compare(Password.Trim(), oUser.hash_password.Trim());
                       
                        if (validarpass)
                        {
                            var pers = (from n in db.Personas
                                        where n.documento == oUser.id_usuario
                                        select n.nombres).FirstOrDefault();

                            try
                            {
                                Session["Nombres"] = pers.Trim();
                                Session["Logged"] = oUser;
                                Session["Usuario"] = oUser.id_usuario;
                                Session["Rol"] = oUser.id_rol;
                                Session["email"] = oUser.email;
                                Session["state"] = oUser.id_estado;
                            }
                            catch (Exception)
                            {
                                ViewBag.Error = "¡No se encuentra registrado como persona en el sistema, valida con el adminsitrador de Fábrica de Software";
                                return View();
                            }
                        }
                        else{
                            ViewBag.Error = "La contraseña no coincide";
                            return View();
                        }

                    }

                }

                return RedirectToAction("Dashboard", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult IniciarCliente()
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
        public ActionResult IniciarCliente(string Email, string Password)
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
                using (Models.FabricaSoftwareEntities db = new Models.FabricaSoftwareEntities())
                {
                    var oUser = (from d in db.Usuarios
                                 where d.email == Email.Trim()
                                 select d).FirstOrDefault();



                    if (oUser == null)
                    {
                        ViewBag.Error = "El usuario es invalidos";
                        return View();
                    }
                    else
                    {
                        if (oUser.id_rol != 3)
                        {

                            ViewBag.Error = "Inicio de sesión solo para clientes";
                            return View();
                        }
                        ScryptEncoder encoder = new ScryptEncoder();
                        bool validarpass = encoder.Compare(Password.Trim(), oUser.hash_password.Trim());

                        if (validarpass)
                        {
                            var cliente = (from n in db.Cliente
                                           where n.id_cliente == oUser.id_usuario
                                           select n).FirstOrDefault();

                            Session["Logged"] = oUser;
                            Session["Usuario"] = oUser.id_usuario;
                            Session["Rol"] = oUser.id_rol;
                            Session["email"] = oUser.email;
                            Session["state"] = oUser.id_estado;

                            if (cliente == null)
                            {
                                return RedirectToAction("InfoCliente", "Clientes");
                            }
                            else
                            {
                                return RedirectToAction("DashboardCliente", "Dashboard");
                            }

                        }
                        else
                        {
                            ViewBag.Error = "La contraseña no coincide";
                            return View();
                        }

                    }

                }

               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult Cerrar()
        {
            Session["Logged"] = null;
            Session["Usuario"] = null;
            Session["Rol"] = null;
            Session["email"] = null;
            Session["state"] = null;
            Session["Nombres"] = null;
            Session["Apellidos"] = null;
            return RedirectToAction("Index", "Home");
        }






    }
}




