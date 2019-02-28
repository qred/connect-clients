using System.Collections.Generic;

namespace Qred.Connect
{
    public class FullOrganization:SimpleOrganization 
    { 
        public IntOrRange NumberOfEmployees { get; set; }

        public List<OrganizationOwner> Owners { get; set; }

        public string Iban { get; set; }
    }
}
