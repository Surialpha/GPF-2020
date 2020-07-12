using SoftwareFactory.Filtros;
using SoftwareFactory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SoftwareFactory.Controllers
{
    public class AgendaController : Controller
    {

        private FabricaSoftwareEntities db = new FabricaSoftwareEntities();

        public object JsonConvert { get; private set; }

        [AuthorizeUserModules(1)]
        public ActionResult Index()
        {

            if (Session["Logged"] != null)
            {

                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                if (TempData["Success"] != null)
                {
                    ViewBag.Success = TempData["Success"].ToString();
                }


                //Se consultan las agendas anteriores al día en curso, y se pasan a estado 4, completado
                var fecha = DateTime.Now;
                var fechaCortada = fecha.ToString().Split('/');
                var mesAgenda = fechaCortada[0];
                var diaAgenda = (int.Parse(fechaCortada[1]) - 1).ToString();
                var AñoAgenda = fechaCortada[2];
                var fechaQuery = DateTime.Parse(mesAgenda + "/" + diaAgenda + "/" + AñoAgenda);

                var agendas = (from a in db.Agenda
                                where a.FechaAgenda < fechaQuery && a.estadoAgenda == 1
                                select a);



                foreach (var item in agendas.ToList())
                {
                    var agendaModificar = db.Agenda.Find(item.idAgenda);
                    agendaModificar.ActionUser = "Sistema";
                    agendaModificar.estadoAgenda = 3;
                    db.Entry(agendaModificar).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                return View();
            }
            else
            {
                return RedirectToAction("Iniciar", "Acceso");
            }

        }
        [AuthorizeUserModules(1)]
        public ActionResult HistorialAgenda(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                Agenda agen = db.Agenda.Find(id);
                if (agen == null)
                {
                    return HttpNotFound();
                }

                var adicional = (from a in db.Agenda
                                 join per in db.Personas on a.ResponsableAgenda equals per.documento
                                 join s in db.Solicitud on a.ProyectoAgenda equals s.id_solicitud
                                 where a.idAgenda == agen.idAgenda
                                 select new
                                 {
                                     responsable = per.documento + " - " + per.nombres + " " + per.apellidos,
                                     proyecto = s.id_solicitud + " - " + s.nombre_empresa

                                 }).FirstOrDefault();

                ViewBag.proyecto = adicional.proyecto;

                var estado = (from a in db.Agenda
                              join ea in db.EstadoAgenda on a.estadoAgenda equals ea.id
                              where a.idAgenda == agen.idAgenda
                              select new
                              {
                                  id = a.idAgenda,
                                  nombre = ea.nombre

                              }).FirstOrDefault();

                ViewBag.estado = estado.nombre;
                var grupo = (from p in db.Proyecto
                             where p.id_proyecto == agen.ProyectoAgenda
                             select p).FirstOrDefault();

                ViewBag.grupo = grupo.id_grupo;

                var cliente = (from s in db.Solicitud
                               join c in db.Cliente on s.id_cliente equals c.id_cliente
                               where s.id_solicitud == agen.ProyectoAgenda
                               select c.id_cliente + " - "+c.nombre
                               ).FirstOrDefault();
                ViewBag.cliente = cliente;
                return View(agen);
            }
            catch (Exception)
            {

                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
           
        }

        [AuthorizeUserModules(1)]
        public JsonResult AgendasLog(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Agenda = (
                    from log in db.LogAgenda
                    join est in db.EstadoAgenda on log.estadoAgenda equals est.id
                    join per in db.Personas on log.ResponsableAgenda equals per.documento
                    where log.idAgenda == id
                    select new
                    {
                        Asistentes = log.asistentes,
                        Responsable = log.ResponsableAgenda + " - " + per.nombres,
                        Asunto = log.Asunto,
                        Estado = est.nombre,
                        Usuario = log.idResponsable,
                        Descripcion = log.descripcion,
                        Fecha = log.fechaLog.ToString()
                    }
                ).ToList();

                return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                var Agenda = new { };
                return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);
            }

        }

        [AuthorizeUserModules(1)]
        public JsonResult ListaAgenda()
        {
            try
            {

                

                    db.Configuration.ProxyCreationEnabled = false;
                    var Agenda = (from gp in db.Agenda
                                  join asesores in db.Asesores on gp.ResponsableAgenda equals asesores.id_asesor
                                  join persona in db.Personas on asesores.id_asesor equals persona.documento
                                  join pro in db.Proyecto on gp.ProyectoAgenda equals pro.id_proyecto
                                  join sol in db.Solicitud on pro.id_proyecto equals sol.id_solicitud
                                  join estado in db.EstadoAgenda on gp.estadoAgenda equals estado.id
                                  where estado.id == 1
                                  select new
                                  {
                                      idAgenga = gp.idAgenda,
                                      fechaAgenda = gp.FechaAgenda.ToString(),
                                      Hora = gp.HoraInicio + " : " + gp.HoraFinal,
                                      Proyecto = pro.id_proyecto + " - " + sol.nombre_empresa + ", " + sol.nombre_solicitante,
                                      responsableAgenda = persona.nombres + " " + persona.apellidos,
                                      estadoAgenda = estado.nombre
                                  }
                                    ).ToList();

                    return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);

              

            }
            catch (Exception)
            {

                var Agenda = new { };
                return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);
            }


        }

        [AuthorizeUserModules(1)]
        public JsonResult HistorialAgendasJSON()
        {
            try
            {

                var rol = int.Parse(Session["Rol"].ToString());

                if (rol == 1)
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var Agenda = (from gp in db.Agenda
                                  join asesores in db.Asesores on gp.ResponsableAgenda equals asesores.id_asesor
                                  join persona in db.Personas on asesores.id_asesor equals persona.documento
                                  join pro in db.Proyecto on gp.ProyectoAgenda equals pro.id_proyecto
                                  join sol in db.Solicitud on pro.id_proyecto equals sol.id_solicitud
                                  join estado in db.EstadoAgenda on gp.estadoAgenda equals estado.id
                                  where estado.id != 1
                                  select new
                                  {
                                      idAgenga = gp.idAgenda,
                                      fechaAgenda = gp.FechaAgenda.ToString(),
                                      Hora = gp.HoraInicio + " : " + gp.HoraFinal,
                                      Proyecto = pro.id_proyecto + " - " + sol.nombre_empresa + ", " + sol.nombre_solicitante,
                                      responsableAgenda = persona.nombres + " " + persona.apellidos,
                                      estadoAgenda = estado.nombre
                                  }
                                    ).ToList();

                    return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var Agenda = new { };
                    return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                var Agenda = new { };
                return Json(new { data = Agenda }, JsonRequestBehavior.AllowGet);
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

            if (Session["Logged"] != null)/*Validacion de usuarios logueados*/
            {
                var asesores = (from per in db.Personas
                                join ase in db.Asesores on per.documento equals ase.id_asesor
                                select new
                                {
                                    idAsesor = ase.id_asesor,
                                    nombreAsesor = ase.id_asesor + " - " + per.nombres + " " + per.apellidos

                                }).ToList();

                ViewBag.ResponsableAgenda = new SelectList(asesores, "idAsesor", "nombreAsesor");


                var proyecto = (from pro in db.Proyecto
                                join sol in db.Solicitud on pro.id_proyecto equals sol.id_solicitud
                                select new
                                {
                                    idProyecto = pro.id_proyecto,
                                    nombreEmpresa = pro.id_proyecto + " " + sol.nombre_empresa + ", " + sol.nombre_solicitante
                                }).ToList();

                ViewBag.ProyectoAgenda = new SelectList(proyecto, "idProyecto", "nombreEmpresa");

                return View();
            }
            else
            {
                return RedirectToAction("Iniciar", "Acceso");

            }
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Agenda agenda, string FechaAgenda)
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
                agenda.FechaAgenda = DateTime.ParseExact(FechaAgenda, "MM/dd/yyyy", null);


                var fecha = agenda.FechaAgenda.ToString().Split('/');
                //posición 0 = dia
                //posición 1 = mes
                //posición 2 = año



                var dia = DateTime.Now.Day.ToString();
                var mes = DateTime.Now.Month.ToString();

                if (dia.Equals(fecha[0]) && mes.Equals(fecha[1]))
                {
                    var horaActual = DateTime.Now.Hour;
                    var h = agenda.HoraInicio.ToString().Split(':');
                    var hora = int.Parse(h[0]);


                    if (hora <= horaActual)
                    {
                        TempData["Error"] = "¡No se pueden registrar agendas atemporales!";
                        return RedirectToAction("Index");
                    }
                    else if (horaActual >= 14)
                    {
                        TempData["Error"] = "¡No se pueden registrar agendas después de las 2PM!";
                        return RedirectToAction("Index");
                    }
                    else if (!((horaActual + 2) > hora))
                    {
                        TempData["Error"] = "¡No se pueden registrar agendas con menos de 3 horas de anticipación!";
                        return RedirectToAction("Index");
                    }
                }

                var f2 = fecha[2].ToString().Split(' ');
                var f = f2[0] + "/" + fecha[0] + "/" + fecha[1];

                
                var fechaA = DateTime.ParseExact(f, "yyyy/dd/M", null);

                var consultas = (from a in db.Agenda
                                 where a.FechaAgenda == fechaA && a.estadoAgenda != 3 && a.estadoAgenda != 4
                                 orderby a.HoraInicio descending
                                 select a).ToList();

                var numero = consultas.Count();

                for (int i = 0; i < consultas.Count(); i++)
                {
                    if (agenda.HoraInicio < consultas[i].HoraInicio)
                    {
                        if (agenda.HoraFinal > consultas[i].HoraInicio)
                        {
                            TempData["Error"] = "¡La hora final no puede cruzarse con una agenda ya registrada!";
                            return RedirectToAction("Index");
                        }
                    }

                    if (agenda.HoraInicio >= consultas[i].HoraInicio && agenda.HoraInicio < consultas[i].HoraFinal)
                    {
                        TempData["Error"] = "¡Ya esxiste un agenda programada para este horario";
                        return RedirectToAction("Index");
                    }
                }

                    var ActionUser = Session["Usuario"].ToString() + " - "+ Session["Nombres"].ToString();
                    agenda.ActionUser = ActionUser;
                    agenda.estadoAgenda = 1;
                    db.Agenda.Add(agenda);
                    db.SaveChanges();
                    TempData["Success"] = "¡Agenda agregada exitosamente!";
                    return RedirectToAction("Index");
               

            }
            catch (Exception err)
            {
                TempData["Error"] = "¡Ha ocurrido un error inesperado, intenta nuevamente!";
                return RedirectToAction("Index");
            }
        }


        public ActionResult Details(int? id)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            if (Session["Logged"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                Agenda agen = db.Agenda.Find(id);
                if (agen == null)
                {
                    return HttpNotFound();
                }
                var rol = int.Parse(Session["Rol"].ToString());
                var user = int.Parse(Session["Usuario"].ToString());
                var proyecto = agen.ProyectoAgenda;

                var pro = db.Proyecto.Find(proyecto);
                var grupocons = db.Grupos.Find(pro.id_grupo);


                if (pro == null)
                {
                    return HttpNotFound();
                }

                ViewBag.TemplateLayout = "~/Views/Shared/_DashLayout.cshtml";

                if (rol == 3)
                {
                    var sol = db.Solicitud.Find(proyecto);

                    if (sol.id_cliente != user)
                    {
                        TempData["Error"] = "No tienes permitido esta acción";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }

                    ViewBag.TemplateLayout = "~/Views/Shared/_DashLayoutCliente.cshtml";
                }
                else if (rol == 4 || rol == 2)
                {
                    if (rol == 4)
                    {
                        if (grupocons.id_lider != user)
                        {
                            TempData["Error"] = "No tienes permitido esta acción";
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                    }
                    else
                    {
                        var apren = (from gp in db.Grupo_Aprendices
                                     where gp.id_grupo == grupocons.id_grupo && gp.id_aprendiz == user
                                     select gp).FirstOrDefault();

                        if (apren == null)
                        {
                            TempData["Error"] = "No tienes permitido esta acción";
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                    }


                }




                var adicional = (from a in db.Agenda
                                 join per in db.Personas on a.ResponsableAgenda equals per.documento
                                 join s in db.Solicitud on a.ProyectoAgenda equals s.id_solicitud
                                 where a.idAgenda == agen.idAgenda
                                 select new
                                 {
                                     responsable = per.documento + " - " + per.nombres + " " + per.apellidos,
                                     proyecto = s.id_solicitud + " - " + s.nombre_empresa

                                 }).FirstOrDefault();

                ViewBag.responsable = adicional.responsable;
                ViewBag.proyecto = adicional.proyecto;

                var estado = (from a in db.Agenda
                              join ea in db.EstadoAgenda on a.estadoAgenda equals ea.id
                              where a.idAgenda == agen.idAgenda
                              select new
                              {
                                  id = a.idAgenda,
                                  nombre = ea.nombre

                              }).FirstOrDefault();

                ViewBag.estado = estado.nombre;

                var grupo = (from p in db.Proyecto
                             where p.id_proyecto == agen.ProyectoAgenda
                             select p).FirstOrDefault();

                ViewBag.grupo = grupo.id_grupo;

                return View(agen);
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        public ActionResult ListarAsistentes(int? id)
        {
            if (Session["Logged"] != null)
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var asistentes = (from p in db.Proyecto
                                      join g in db.Grupos on p.id_grupo equals g.id_grupo
                                      join gp in db.Grupo_Aprendices on g.id_grupo equals gp.id_grupo
                                      join aprendiz in db.Personas on gp.id_aprendiz equals aprendiz.documento
                                      where p.id_proyecto == id
                                      select new
                                      {
                                          identificador = aprendiz.documento,
                                          nombreCompleto = aprendiz.nombres + " " + aprendiz.apellidos,
                                          correo = aprendiz.email,
                                          telefono = aprendiz.numero_contacto,
                                          rol = "Aprendiz"

                                      }).ToList();
                    var lider = (from p in db.Proyecto
                                 join g in db.Grupos on p.id_grupo equals g.id_grupo
                                 join ase in db.Personas on g.id_lider equals ase.documento
                                 where p.id_proyecto == id
                                 select new
                                 {
                                     identificador = ase.documento,
                                     nombreCompleto = ase.nombres + " " + ase.apellidos,
                                     correo = ase.email,
                                     telefono = ase.numero_contacto,
                                     rol = "Asesor líder"

                                 }).FirstOrDefault();

                    asistentes.Add(lider);
                    var cliente = (from p in db.Proyecto
                                   join s in db.Solicitud on p.id_proyecto equals s.id_solicitud
                                   join c in db.Cliente on s.id_cliente equals c.id_cliente
                                   where p.id_proyecto == id
                                   select new
                                   {
                                       identificador = c.id_cliente,
                                       nombreCompleto = c.nombre,
                                       correo = c.email,
                                       telefono = c.numero_contacto,
                                       rol = "Cliente"
                                   }).FirstOrDefault();
                    asistentes.Add(cliente);
                    asistentes.Reverse();
                    return Json(new { data = asistentes }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    var asistentes = new { };
                    return Json(new { data = asistentes }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
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

            if (Session["Logged"] != null)
            {
                var rol = int.Parse(Session["Rol"].ToString());

                if (rol == 1)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Agenda agen = db.Agenda.Find(id);
                    if (agen == null)
                    {
                        return HttpNotFound();
                    }

                    var asesores = (from per in db.Personas
                                    join ase in db.Asesores on per.documento equals ase.id_asesor
                                    select new
                                    {
                                        idAsesor = ase.id_asesor,
                                        nombreAsesor = ase.id_asesor + " - " + per.nombres + " " + per.apellidos

                                    }).ToList();

                    ViewBag.ResponsableAgenda = new SelectList(asesores, "idAsesor", "nombreAsesor", agen.ResponsableAgenda);

                    var proyecto = (from a in db.Agenda
                                    join pro in db.Proyecto on a.ProyectoAgenda equals pro.id_proyecto
                                    join sol in db.Solicitud on pro.id_proyecto equals sol.id_solicitud
                                    where a.idAgenda == id
                                    select
                                     pro.id_proyecto + " - " + sol.nombre_empresa + ", " + sol.nombre_solicitante
                                    ).FirstOrDefault();
                    ViewBag.ProyectoAgenda = proyecto;

                    var estado = (from e in db.EstadoAgenda where e.id != 2 select e).ToList();
                    ViewBag.estadoAgenda = new SelectList(estado, "id", "nombre", agen.estadoAgenda);

                    return View(agen);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");

            }
        }

        [AuthorizeUserModules(1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Agenda agen)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            int estadoA = agen.estadoAgenda;

            if (estadoA == 3)
            {
                var fecha = agen.FechaAgenda.ToString().Split('/');
                var mesAgenda = int.Parse(fecha[0]);
                var diaAgenda = int.Parse(fecha[1]);
                var hI = agen.HoraInicio.ToString().Split(':');
                var horaI = int.Parse(hI[0]);

                var horaReal = DateTime.Now.Hour;
                var dia = DateTime.Now.Day;
                var mes = DateTime.Now.Month;

                if (dia < diaAgenda && mes <= mesAgenda)
                {
                    if (ModelState.IsValid)
                    {
                        var ActionUser = Session["Usuario"].ToString() + " - " + Session["Nombres"].ToString();
                        agen.ActionUser = ActionUser;
                        agen.estadoAgenda = 1;
                        db.Entry(agen).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["Error"] = "¡La agenda no se puede completar, pero la demás información se modificó!";
                    return RedirectToAction("Index");
                }
                else if (dia == diaAgenda && mes == mesAgenda)
                {
                    if (horaReal < horaI)
                    {
                        if (ModelState.IsValid)
                        {
                            var ActionUser = Session["Usuario"].ToString() + " - " + Session["Nombres"].ToString();
                            agen.ActionUser = ActionUser;
                            agen.estadoAgenda = 1;
                            db.Entry(agen).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        TempData["Error"] = "¡La agenda no se puede completar, pero la demás información se modificó!";
                        return RedirectToAction("Index");
                    }
                }
            }



            if (ModelState.IsValid)
            {
                var ActionUser = Session["Usuario"].ToString() + " - " + Session["Nombres"].ToString();
                agen.ActionUser = ActionUser;
                db.Entry(agen).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "¡Se ha modificado la agenda exitosamente!";
                return RedirectToAction("Index");
            }

            var asesores = (from per in db.Personas
                            join ase in db.Asesores on per.documento equals ase.id_asesor
                            select new
                            {
                                idAsesor = ase.id_asesor,
                                nombreAsesor = ase.id_asesor + " - " + per.nombres + " " + per.apellidos

                            }).ToList();

            ViewBag.ResponsableAgenda = new SelectList(asesores, "idAsesor", "nombreAsesor");

            var proyecto = (from a in db.Agenda
                            join pro in db.Proyecto on a.ProyectoAgenda equals pro.id_proyecto
                            join sol in db.Solicitud on pro.id_proyecto equals sol.id_solicitud
                            where a.idAgenda == agen.idAgenda
                            select new
                            {
                                idProyecto = pro.id_proyecto,
                                nombreEmpresa = pro.id_proyecto + " - " + sol.nombre_empresa + ", " + sol.nombre_solicitante
                            }).ToList();

            ViewBag.ProyectoAgenda = new SelectList(proyecto, "idProyecto", "nombreEmpresa");

            var estado = (from e in db.EstadoAgenda where e.id != 2 select e).ToList();
            ViewBag.estadoAgenda = new SelectList(estado, "id", "nombre", agen.estadoAgenda);


            TempData["Error"] = "No se pudo editar la agenda ¡Intenta nuevamente!";
            return View(agen);
        }

        [AuthorizeUserModules(1)]
        public ActionResult CancelarAgenda(int idAgenda)
        {


            try
            {
                Agenda agenda = db.Agenda.Find(idAgenda);

                if (agenda == null)
                {
                    TempData["Error"] = "¡La agenda no existe!";
                    return RedirectToAction("Index");
                }


                var horaAgenda = agenda.HoraInicio.ToString().Split(':');
                var horaInt = int.Parse(horaAgenda[0]);

                var fecha = agenda.FechaAgenda.ToString().Split('/');
                var mesAgenda = int.Parse(fecha[0]);
                var diaAgenda = int.Parse(fecha[1]);


                var horaReal = DateTime.Now.Hour;
                var dia = DateTime.Now.Day;
                var mes = DateTime.Now.Month;
                var sumahora = horaInt - horaReal;


                if (dia == diaAgenda && mes == mesAgenda)
                {
                    if (sumahora >= 0 && sumahora <= 3)
                    {
                        TempData["Error"] = "Solo se pueden cancelar agendas con tres (3) horas de anticipación";
                        return RedirectToAction("Index");
                    }
                    else if (sumahora < 0)
                    {
                        TempData["Error"] = "No puede ser cancelada porque está en proceso o ya terminó";
                        return RedirectToAction("Index");
                    }

                }
                var ActionUser = Session["Usuario"].ToString() + " - " + Session["Nombres"].ToString();
                agenda.ActionUser = ActionUser;
                agenda.estadoAgenda = 2;
                db.Entry(agenda).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Agenda Cancelada";
                return Redirect(Request.UrlReferrer.ToString());

            }
            catch (Exception)
            {
                TempData["Error"] = "La agenda no pudo ser cancelada. ¡Intente nuevamente!";
                return Redirect(Request.UrlReferrer.ToString());
            }

        }

        [AuthorizeUserModules(3)]
        public JsonResult AgendaCliente()
        {
            var user = int.Parse(Session["Usuario"].ToString());

            db.Configuration.ProxyCreationEnabled = false;

            var agenda = (from a in db.Agenda
                          join s in db.Solicitud on a.ProyectoAgenda equals s.id_solicitud
                          where s.id_cliente == user
                          orderby a.FechaAgenda descending
                          select new
                          {
                              idAgenda = a.idAgenda,
                              fecha = a.FechaAgenda + " - " + a.HoraInicio + " - " + a.HoraFinal,
                              responsable = a.ResponsableAgenda,
                              asunto = a.Asunto,
                              estado = a.estadoAgenda
                          }).Take(5);

            return Json(new { data = agenda }, JsonRequestBehavior.AllowGet);

        }

        public class Notificaciones
        {
            public int id { get; set; }
            public string fecha { get; set; }
            public string empresa { get; set; }
            public int estado { get; set; }
        }

        public class NotificacionesAdminstradorSolicitud
        {
            public int id { get; set; }
            public string nombreEmpresa { get; set; }
            public string fecha { get; set; }

        }
        
        public class NotificacionesAdminstradorCliente
        {
            public int id { get; set; }
            public string email { get; set; }
            public string fecha { get; set; }
        }

        public class NotificacionesAdminstradorPQRS
        {
            public int id { get; set; }
            public string nombre { get; set; }
        }




        public ActionResult NotificacionAgenda()
        {
            if (Session["Logged"] != null)
            {
                try
                {
                    var user = int.Parse(Session["Usuario"].ToString());
                    var rol = int.Parse(Session["rol"].ToString());

                    if (rol == 3)
                    {
                        TempData["Error"] = "¡No tiene autorización para ingresar a este módulo!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        if (rol == 2)
                        {
                            var AgendasAprendiz = (from a in db.Agenda
                                                   join p in db.Proyecto on a.ProyectoAgenda equals p.id_proyecto
                                                   join s in db.Solicitud on p.id_proyecto equals s.id_solicitud
                                                   join gp in db.Grupo_Aprendices on p.id_grupo equals gp.id_grupo
                                                   where gp.id_aprendiz == user
                                                   orderby a.FechaAgenda descending
                                                   select new Notificaciones
                                                   {
                                                       id = a.idAgenda,
                                                       fecha = a.FechaAgenda.ToString(),
                                                       empresa = s.nombre_empresa,
                                                       estado = (int) a.estadoAgenda
                                                   }

                                                   ).Take(5).ToList();

                            ViewBag.rol = 2;
                            if (AgendasAprendiz.Count() == 0)
                            {
                                ViewBag.Agendas = null;
                            }
                            else
                            {
                                ViewBag.Agendas = AgendasAprendiz;
                            }
                            return View();
                        }
                        else if (rol == 4)
                        {


                            var AgendasAsesor = (from a in db.Agenda
                                                 join p in db.Proyecto on a.ProyectoAgenda equals p.id_proyecto
                                                 join s in db.Solicitud on p.id_proyecto equals s.id_solicitud
                                                 join g in db.Grupos on p.id_grupo equals g.id_grupo
                                                 where g.id_lider == user
                                                 orderby a.FechaAgenda descending
                                                 select new Notificaciones
                                                   {
                                                       id = a.idAgenda,
                                                       fecha = a.FechaAgenda.ToString(),
                                                       empresa = s.nombre_empresa,
                                                       estado = (int)a.estadoAgenda

                                                 }).Take(5).ToList();

                            ViewBag.rol = 4;
                            if (AgendasAsesor.Count() == 0)
                            {
                                ViewBag.Agendas = null;
                            }
                            else
                            {
                                ViewBag.Agendas = AgendasAsesor;
                            }
                            

                            return View();
                        }
                        else
                        {
                            ViewBag.rol = 1;

                            var Solicitudes = (from pyto in db.Solicitud
                                               where pyto.id_estado_solicitud != 5 && pyto.id_estado_solicitud != 6
                                               orderby pyto.fecha_solicitud descending
                                                select new NotificacionesAdminstradorSolicitud
                                                {
                                                    id = pyto.id_solicitud,
                                                    nombreEmpresa = pyto.nombre_empresa,
                                                    fecha = pyto.fecha_solicitud.ToString()
                                                }).Take(2).ToList();

                            if (Solicitudes.Count() == 0)
                            {
                                ViewBag.Solicitudes = null;
                            }
                            else
                            {
                                ViewBag.Solicitudes = Solicitudes;
                            }

                            var clientes = (from c in db.Usuarios
                                               where c.id_rol == 3 && c.id_estado == 2
                                               select new NotificacionesAdminstradorCliente
                                               {
                                                   id = c.id_usuario,
                                                   email = c.email,
                                                   fecha = c.fecha_creacion.ToString()

                                               }).Take(2).ToList();
                            if (clientes.Count() == 0)
                            {
                                ViewBag.Cliente = null;
                            }
                            else
                            {
                                ViewBag.Cliente = clientes;
                            }


                            var pqrs = (from p in db.pqrs
                                        where p.nombre != "Recuperación de contraseña"
                                        orderby p.id ascending
                                        select new NotificacionesAdminstradorPQRS
                                        {
                                            id = p.id,
                                            nombre = p.nombre
                                        }).Take(2).ToList();
                            if (pqrs.Count() == 0)
                            {
                                ViewBag.Pqrs = null;
                            }
                            else
                            {
                                ViewBag.Pqrs = pqrs;
                            }


                            return View();
                        }
                    }


                }
                catch (Exception)
                {
                    TempData["Error"] = "Ha ocurrido un error inesperado ¡Intenta de nuevamente!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
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