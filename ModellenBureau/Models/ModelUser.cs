using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModellenBureau.Models
{
    public class ModelUser : ApplicationUser
    {
        public int Age { get; set; }
        public float Height { get; set; }
        public float Waist { get; set; }
        public float LegLength { get; set; }
        public float Chest { get; set; }
    }
}
