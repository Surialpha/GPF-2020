
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
    
public partial class Estado_asignacion
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Estado_asignacion()
    {

        this.Asignar_Fase = new HashSet<Asignar_Fase>();

    }


    public int id_estado { get; set; }

    public string descripcion { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Asignar_Fase> Asignar_Fase { get; set; }

}

}
