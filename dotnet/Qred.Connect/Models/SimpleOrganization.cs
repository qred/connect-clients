using System;

namespace Qred.Connect
{
    public class SimpleOrganization
    {
        public string NationalOrganizationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public Uri Url { get; set; }

        public IntOrRange CurrentMonthlyTurnover { get ;set; }
    }
}
