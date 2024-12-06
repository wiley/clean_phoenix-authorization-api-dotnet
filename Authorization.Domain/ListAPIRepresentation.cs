using System.Collections.Generic;

namespace Authorization.Domain
{
    public class ListAPIRepresentation<T>
    {
        public List<T> Items { get; set; }

        public int Count { get; set; }

        public LinksAPIRepresentation _links { get; set; }
    }
}
