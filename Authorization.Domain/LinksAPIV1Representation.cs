namespace Authorization.Domain
{
    public class LinksAPIV1Representation
    {
        public Link First { get; set; }

        public Link Last { get; set; }

        public Link Previous { get; set; }

        public Link Next { get; set; }

        public int Count { get; set;}
    }
}
