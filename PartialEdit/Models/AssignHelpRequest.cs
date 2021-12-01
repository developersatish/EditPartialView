using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartialEdit.Models
{
    public class AssignHelpRequest
    {
        public AssignHelpRequest()
        {
            this.UserList = this.GetAssignedToList();
        }
        public int Id { get; set; }
        public string AssignedTo { get; set; }

        public IList<string> UserList { get; set; }
        private List<string> GetAssignedToList()
        {
            var list = new List<string> { "NOT Assigend" };
            list.Add("hello@world.com");
            list.Add("john@dk.com");
            return list;
        }

    }

   
}