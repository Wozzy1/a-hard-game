using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hardGame
{
    internal class LossMilestone(string key, string description, double value) : Achievement(key, description)
    {
        public double Value { get; init; } = value;
    }
}
