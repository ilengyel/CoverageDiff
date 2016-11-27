namespace CoverageDiff
{
    using System.Linq;
    using System.Xml.Linq;

    public static class Extensions
    {
        public static void InsertAttribute(this XElement element, int index, string name, string value)
        {
            var attributes = element.Attributes().ToList();
            attributes.Insert(index, new XAttribute(name, value));
            element.Attributes().Remove();
            element.Add(attributes);
        }

        public static int AttrInt(this XElement element, string name)
        {
            var value = element.Attribute(name)?.Value;
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
        }
    }
}
