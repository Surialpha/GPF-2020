
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
    
public partial class Tipo_Imagen
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Tipo_Imagen()
    {

        this.Imagenes = new HashSet<Imagenes>();

    }


    public int idTipo { get; set; }

    public string descripcion { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Imagenes> Imagenes { get; set; }

}

}
