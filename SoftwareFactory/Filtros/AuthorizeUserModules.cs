using SoftwareFactory.Models;

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftwareFactory.Filtros
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUserModules : AuthorizeAttribute
    {


        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();
        private int IdRol;

        public AuthorizeUserModules(int idRol = 0)
        {
            this.IdRol = idRol;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {




                if (IdRol == 1 || IdRol == 2 || IdRol == 3 || IdRol == 4)
                {
                    var oUser = HttpContext.Current.Session["Usuario"];
                    var oUser2 = 0;
                    if (oUser != null)
                    {
                        oUser2 = int.Parse(oUser.ToString());
                    }


                    var PermitedRol = from n in db.Usuarios
                                      where n.id_rol == IdRol && n.id_usuario == oUser2
                                      select n;

                    if (PermitedRol.ToList().Count() == 0)
                    {
                        filterContext.Result = new RedirectResult("~/Dashboard/Dashboard");
                    }
                }
                else
                {

                    var state = HttpContext.Current.Session["state"];
                    var state2 = 0;
                    if (state != null)
                    {
                        state2 = int.Parse(state.ToString());
                    }

                    if (state2 != 1)
                    {
                        filterContext.Result = new RedirectResult("~/Dashboard/Dashboard");
                    }
                }




            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Dashboard/Dashboard");
            }




        }


    }
}