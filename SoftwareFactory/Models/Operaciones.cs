
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
    
public partial class Operaciones
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Operaciones()
    {

        this.Rol_Operaciones = new HashSet<Rol_Operaciones>();

    }


    public int id_operacion { get; set; }

    public string descripcion { get; set; }

    public Nullable<int> id_modulo { get; set; }



    public virtual Modulos Modulos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Rol_Operaciones> Rol_Operaciones { get; set; }

}

}
