
using System.Collections.Generic;

namespace CRP.Tests.Core.Helpers
{
    public struct NameAndType
    {
        public NameAndType(string name, string property, List<string> attributes)
        {
            Name = name;
            Property = property;
            Attributes = attributes;
        }
        public string Name;
        public string Property;
        public List<string> Attributes;
    }
}
