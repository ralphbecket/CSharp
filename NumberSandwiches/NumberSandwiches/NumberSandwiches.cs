using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberSandwiches
{
    /// <summary>
    /// A number sandwich of order n is an arrangement of the digit pairs
    /// 1, 1, 2, 2, 3, 3, ..., n, n such that for each pair k, k there are
    /// exactly k other digits between them and there are no gaps in the
    /// arrangement.  For example, a solution of order 3 is 3 1 2 1 3 2.
    /// </summary>
    public static class NumberSandwiches
    {
        public static void Main(string[] args)
        {
            var n = 48;
            var doms = new Doms(n);
            Console.WriteLine(doms.ToString());
            var ok = doms.Search();
            Console.WriteLine(ok ? doms.ToString() : $"No solution of order {n}.");
        }
    }

    internal class Doms
    {
        internal static void Trace(string s)
        {
            Console.WriteLine(s);
        }
        internal Doms(int n)
        {
            var max = n + n;
            KDom =
                Enumerable
                .Range(0, n + 1)
                .Select(k => (1L << (max - k - 1)) - 1)
                .ToArray();
        }
        internal readonly Int64[] KDom; // Bit domain for each digit.  [0] is not used.
        internal int N => KDom.Length - 1;
        internal string ToString(int k)
        {
            var dom = KDom[k];
            var numDomBits = N + N - k - 1;
            var bits =
                Enumerable
                .Range(0, numDomBits)
                .Select(i => DomHasBit(dom, 1L << i) ? '1' : '.');
            var str = $"[{k}] {string.Concat(bits)}";
            return str;
        }
        public override string ToString()
        {
            var strs =
                Enumerable
                .Range(1, (int)N)
                .Select(k => ToString(k) + "\n");
            var str = string.Concat(strs);
            return str;
        }
        internal static Int64 BitToI(Int64 bit) =>
            1 + Enumerable.Range(0, 64).First(i => bit == 1L << i);
        internal static bool DomIsEmpty(Int64 dom) => dom == 0;
        internal static bool DomIsBit(Int64 dom, Int64 bit) => dom == bit;
        internal static bool DomHasBit(Int64 dom, Int64 bit) => (dom & bit) != 0;
        internal static Int64 MinDomBit(Int64 dom) => dom & (-dom);
        internal static bool DomIsSingleton(Int64 dom) => dom == MinDomBit(dom);
        internal static Int64 ClearDomBit(Int64 dom, Int64 bit) => dom & (~bit);
        internal static Int64 SetDomBit(Int64 dom, Int64 bit) => dom | bit;
        internal Stack<TrailEntry> Trail = new Stack<TrailEntry> { };
        internal void TrailChange(int k, Int64 oldDom, Int64 newDom)
        {
            if (DomIsSingleton(newDom))
            {
                Trace($"[{k}] == {BitToI(newDom)}");
            }
            else
            {
                var bit = oldDom & ~newDom;
                Trace($"[{k}] != {BitToI(bit)}");
            }
            Trail.Push(new TrailEntry { K = k, Dom = oldDom });
            KDom[k] = newDom;
        }
        internal void BacktrackTo(Int64 n)
        {
            if (n == Trail.Count) return;
            Trace($"  backtracking {Trail.Count - n}");
            while (n < Trail.Count)
            {
                var entry = Trail.Pop();
                KDom[entry.K] = entry.Dom;
            }
        }
        internal bool Set(int k, Int64 bit)
        {
            var dom = KDom[k];
            if (DomIsBit(dom, bit)) return true;
            if (!DomHasBit(dom, bit)) throw new ApplicationException("Bit is not in domain.");
            TrailChange(k, dom, bit);
            var n = Trail.Count;
            var ok = true;
            // Remove this bit from all other domains.
            // Also remove any other impossible bits.
            for (var j = 1; ok && j <= N; j++)
            {
                if (j == k) continue;
                ok = ok && Clear(j, bit);
                ok = ok && Clear(j, bit << (k + 1));
                ok = ok && Clear(j, bit >> (j + 1));
                ok = ok && Clear(j, (bit << (k + 1)) >> (j + 1));
            }
            if (!ok)
            {
                Trace("  Conflict!");
                BacktrackTo(n);
            }
            return ok;
        }
        internal bool Clear(int k, Int64 bit)
        {
            var dom = KDom[k];
            if (DomIsBit(dom, bit)) return false; // We can't empty a domain.
            if (!DomHasBit(dom, bit)) return true;
            var n = Trail.Count;
            var ok = true;
            var newDom = ClearDomBit(dom, bit);
            if (DomIsSingleton(newDom))
            {
                // We're down to a single bit in this domain, so we can apply
                // unit propagation.
                Trace($"  [{k}] != {BitToI(bit)} forces...");
                ok = Set(k, newDom);
            }
            else
            {
                TrailChange(k, dom, newDom);
            }
            if (!ok)
            {
                BacktrackTo(n);
            }
            return ok;
        }
        public bool Search()
        {
            // Find the var with the smallest domain.
            var minK = 999;
            var minDomSize = 999;
            for (var k = 1; k <= N; k++)
            {
                var dom = KDom[k];
                if (DomIsSingleton(dom)) continue;
                var domSize = 0;
                while (dom != 0) {
                    dom = ClearDomBit(dom, MinDomBit(dom));
                    domSize++;
                }
                if (domSize <= minDomSize)
                {
                    minK = k;
                    minDomSize = domSize;
                }
            }
            if (minDomSize == 999) return true;
            var minDom = KDom[minK];
            var minBit = MinDomBit(minDom);
            var n = Trail.Count;
            Trace($"  Trying [{minK}]...");
            if (Set(minK, minBit) && Search())
            {
                return true;
            }
            else
            {
                BacktrackTo(n);
                Trace("  Failure, forcing...");
                return (Clear(minK, minBit) && Search());
            }
        }
    }
    internal struct TrailEntry
    {
        internal int K;
        internal Int64 Dom;
    }
}
