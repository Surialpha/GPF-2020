
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
    
public partial class Fases_de_Ciclos
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Fases_de_Ciclos()
    {

        this.Asignar_Fase = new HashSet<Asignar_Fase>();

    }


    public int id_fase_ciclo { get; set; }

    public Nullable<int> id_ciclo { get; set; }

    public Nullable<int> id_fase { get; set; }

    public Nullable<int> id_estado { get; set; }

    public string descripcion { get; set; }

    public Nullable<System.DateTime> fecha_creacion { get; set; }

    public Nullable<int> tiempo_estimado { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Asignar_Fase> Asignar_Fase { get; set; }

    public virtual Ciclos Ciclos { get; set; }

    public virtual Estados_Fases Estados_Fases { get; set; }

    public virtual Fases Fases { get; set; }

}

}
