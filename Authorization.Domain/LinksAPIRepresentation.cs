namespace Authorization.Domain
{
    public class LinksAPIRepresentation
    {
        public Link First { get; set; }

        public Link Last { get; set; }

        public Link Self { get; set; }

        public Link Previous { get; set; }

        public Link Next { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }
}
