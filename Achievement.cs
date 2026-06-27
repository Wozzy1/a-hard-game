using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hardGame
{
    internal abstract class Achievement
    {
        public string Key { get; init; }
        public string Description { get; init; }

        public Achievement(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}
