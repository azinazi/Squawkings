using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Squawkings.Models
{
    public class Squawks
    {
        public List<SquawkDisp> SquawksList { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Squawk { get; set; }
    }
}