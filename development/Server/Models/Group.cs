using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Group
    {

        //@@231/239 Generated a default constructor to avoid errors 
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
        }


        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();

    }
}