using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartialEdit.Models
{
    public class FoundHelpRequest
    {
        public int Id { get; set; }

        public string HelpReferenceNumber { get; set; }

        public DateTimeOffset DateSubmitted { get; set; }

        public bool IsAssgined { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string FamilyName { get; set; }

        public bool IsSingleName { get; set; }

        public string SingleName { get; set; }

        public string MobilePhone { get; set; }

        public string HomePhone { get; set; }


        public string AssignedTo { get; set; }

        public string Name
        {
            get { return IsSingleName ? SingleName : string.Format("{0}, {1} {2}", FamilyName, FirstName, MiddleName).Trim(); }
        }
    }
}