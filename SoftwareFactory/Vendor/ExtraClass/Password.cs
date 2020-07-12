using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftwareFactory.Vendor.ExtraClass
{
    public class Password
    {
        public int idU { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        public String oldPass { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        [MaxLength(20,ErrorMessage ="Excediste el maximo estimado para la contraseña")]
        [MinLength(8,ErrorMessage ="El minimo de caracteres es de 8")]
        public String newPass { get; set; }

    }
}