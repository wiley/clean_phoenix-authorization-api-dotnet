using System.Collections.Generic;

namespace Authorization.Domain.Entitlements
{
    public class EntitlementListAPIRepresentation
    {
        public List<EntitlementAPIRepresentation> Items { get; set; }

        public int Count { get; set; }

        public LinksAPIRepresentation _links { get; set; }
    }
}
