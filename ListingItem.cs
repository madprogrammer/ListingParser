using System;

namespace MadProgrammer
{
    public enum ListingItemType
    {
        File,
        Directory,
        Link,
        Unknown
    }
    public struct ListingItem
    {
        public string Owner;
        public string Group;
        public string Name;
        public ListingItemType Type;
        public long Size;
        public DateTime Modified;
        public string Mode;
    }
}
