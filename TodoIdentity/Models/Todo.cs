using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoIdentity.Models
{
    public class Todo
    {
        public string TodoName { get; set; }

        [Key]
        public int TodoId { get; set; }

        //identity stringként tárolja a userid-t
        public string TodoOwner { get; set; }
    }
}
