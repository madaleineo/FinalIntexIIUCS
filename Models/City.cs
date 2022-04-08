using System;
using System.ComponentModel.DataAnnotations;
namespace UtahCarSafety.Models
{
    public class City
    {

        [Key]
        [Required]
        public string CITY { get; set; }

    }
}
