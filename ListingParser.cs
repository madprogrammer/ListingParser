using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MadProgrammer
{
    public class ListingParser
    {
        public ListingParser(string filename)
            : this(new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
        {
        }

        public ListingParser(TextReader input)
        {
            _reader = input;
        }

        public IEnumerable<ListingItem> Parse()
        {
            string line;
            var result = new List<ListingItem>();

            while ((line = _reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "")
                    continue;
                else if (line.StartsWith("total"))
                    continue;
                else if (line.EndsWith(":"))
                {
                    // This is a directory header line, save the path
                    _currentPath = line.Substring(line.StartsWith(".") ? 1 : 0, line.Length - 2);
                }
                else
                {
                    // Should be normal listing line
                    var split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var mtime = string.Join(" ", split.Skip(5).Take(3).ToArray());
                    var name = string.Join(" ", split.Skip(8).Take(split.Length - 8));

                    yield return new ListingItem()
                    {
                        Mode = split[0],
                        Owner = split[2],
                        Group = split[3],
                        Size = long.Parse(split[4]),
                        Modified = mtime.Contains(":") ? DateTime.ParseExact(mtime, "MMM d HH:mm", CultureInfo.InvariantCulture) : DateTime.Parse(mtime),
                        Type = GetItemType(split[0]),
                        Name = _currentPath + '/' + name
                    };
                }
            }
            _reader.Close();
        }

        private ListingItemType GetItemType(string mode)
        {
            switch (mode[0])
            {
                case 'd': return ListingItemType.Directory;
                case 'l': return ListingItemType.Link;
                case '-': return ListingItemType.File;
                default: return ListingItemType.Unknown;
            }
        }

        private string _currentPath;
        private readonly TextReader _reader;
    }
}
