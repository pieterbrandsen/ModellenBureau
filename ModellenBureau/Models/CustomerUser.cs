﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModellenBureau.Models
{
    public class CustomerUser : ApplicationUser, IComparable<CustomerUser>
    {
        public string CompanyName { get; set; }
        public string CompanyAddres { get; set; }
        public string Website { get; set; }
        public int KVK_Number { get; set; }
        public int BTW_Number { get; set; }

        int IComparable<CustomerUser>.CompareTo(CustomerUser other)
        {
            if (other.IsVerified)
                return -1;
            else
                return 1;
        }
    }
}
