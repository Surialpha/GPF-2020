
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace SoftwareFactory.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Agenda
{

    public int idAgenda { get; set; }

    public System.DateTime FechaAgenda { get; set; }

    public int ResponsableAgenda { get; set; }

    public int ProyectoAgenda { get; set; }

    public int estadoAgenda { get; set; }

    public System.TimeSpan HoraFinal { get; set; }

    public string Asunto { get; set; }

    public Nullable<System.TimeSpan> HoraInicio { get; set; }

    public string ActionUser { get; set; }



    public virtual EstadoAgenda EstadoAgenda1 { get; set; }

    public virtual Proyecto Proyecto { get; set; }

    public virtual Asesores Asesores { get; set; }

}

}
