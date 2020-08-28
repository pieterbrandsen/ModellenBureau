using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ModellenBureau.Models
{
    public class ModelUser : ApplicationUser, IComparable<ModelUser>
    {
        public string Gender { get; set; }
        public int Age { get; set; }
        public float Height { get; set; }
        public float Waist { get; set; }
        public float LegLength { get; set; }
        public float Chest { get; set; }
        public List<FileModel> Photos { get; set; }
        int IComparable<ModelUser>.CompareTo(ModelUser other)
        {
            if (other.IsVerified)
                return -1;
            else
                return 1;
        }
    }
}
