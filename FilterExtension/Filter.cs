﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FilterExtensions
{
    public class Filter
    {
        internal List<Check> checks = new List<Check>(); // checks are processed in serial (a && b), inversion gives (!a || !b) logic
        internal bool invert = false;

        public Filter(ConfigNode node)
        {
            foreach (ConfigNode subNode in node.GetNodes("CHECK"))
            {
                checks.Add(new Check(subNode));
            }
            bool.TryParse(node.GetValue("invert"), out invert);
        }

        internal bool checkFilter(AvailablePart part)
        {
            foreach (Check c in checks)
            {
                bool val = c.checkPart(part);
                if (!val)
                {
                    if (invert)
                        return true; // part failed a check, result inverted
                    else
                        return false; // part failed a check
                }
            }
            if (invert)
                return false; // part passed all checks, result inverted
            else
                return true; // part passed all checks, thus meets the filter requirements
        }

        public bool Equals(Filter f2)
        {
            if (f2 == null)
                return false;

            if (this.invert != f2.invert)
                return false;
            else
            {
                foreach (Check c1 in this.checks)
                {
                    if (!f2.checks.Any(c2 => c1.Equals(c2)))
                        return false;
                }
                return true;
            }
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (Check c in this.checks)
            {
                hash *= c.GetHashCode();
            }

            return hash * this.invert.GetHashCode();
        }
    }
}
