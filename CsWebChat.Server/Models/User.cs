using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.Models
{
    public class User
    {
        public long Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9]{1,20}$")]
        public string Name { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9]{6,20}$")]
        public string Password { get; set; }
    }
}
