using System;

namespace Qred.Connect
{
    public class ApplicationDecisionApproved : ApplicationDecision
    {
        public decimal? Amount { get; set; }
        public int? Term { get; set; }
        public string Campaign { get; set; }
        public Uri OfferUrl { get; set; }

        public override ApplicationDecisionType Decision => ApplicationDecisionType.Approved;
    }
}
