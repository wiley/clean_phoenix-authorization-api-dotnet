using System.Collections.Generic;

namespace Authorization.Domain
{
    public class ListAPIV1Representation<T>
    {
        public List<T> Content { get; set; }

        public LinksAPIV1Representation _links { get; set; }
    }
}
