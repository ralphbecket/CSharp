using System;
using System.Collections.Generic;
using System.Linq;

namespace Unforth
{
    public static class Unforth
    {
        // This is all written in "Z80 style".
        // That is, it is intended to be easy
        // to translate into Z80 assembler.

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        // The memory where everything happens.
        internal static byte[] Mem = new byte[0x10000];

        // The source code, copied into Mem.  Source code is NUL terminated.
        internal static string Src {
            get => string.Concat(Mem.Skip(SrcStart).TakeWhile(x => x != 0));
            set {
                Array.Copy(value.ToCharArray(), 0, Mem, SrcStart, value.Length);
                Mem[SrcStart + value.Length] = 0;
            }
        }
        internal const int SrcStart = 0x6000;

        // The Mem address in the source code reached by the parser.
        internal static int SrcIdx = 0;

        internal enum BindingKind
        {
            ForwardRef,
            Label,
            Const,
            Var
        }

        internal class Binding
        {
            internal string Name; // This is for debugging.
            internal int Hash; // In 0..255. If zero then this is "hidden".
            internal int Start; // Index into Src of first occurrence.
            internal int Length;
            internal BindingKind Kind;
            internal int Refs; // Index into Mem of last reference (these form a chain).
            internal int Value;
            internal byte[] Encoding;
            // Encoding is any sequence of the following
            // n b1 b2 .. bn
            // 1 for inserting the following value
            // 0 terminates the sequence.
        }

        public enum Instr
        {
            I,
            At,
            Comma,
            Add,
            Load,
            Store,
            LoadGlobal,
            StoreGlobal,
            Jump,
            JumpIfZ,
            Call,
            Enter,
            Exit
        }

        internal static List<Binding> Bindings = new List<Binding> { };

        internal static void Add(Binding b)
        {
            Bindings.Add(b);
        }

        internal static Binding Lookup(string name, int hash, int start, int length)
        {
            for (var i = Bindings.Count - 1; 0 <= i; i--)
            {
                var binding = Bindings[i];
                if (hash != binding.Hash) continue;
                if (length != binding.Length) continue;
                if (StrNEq(start, binding.Start, length)) return binding;
            }
            var newBinding = new Binding {
                Name = name,
                Hash = hash,
                Start = start,
                Length = length,
                Kind = BindingKind.ForwardRef
            };
            Add(newBinding);
            return newBinding;
        }

        internal static bool StrNEq(int a, int b, int n)
        {
            while (true)
            {
                if (n-- == 0) return true;
                if (Mem[a++] == Mem[b++]) continue;
            }
        }

        // TokHash is 0 iff we have reached the EOF.
        internal static int TokHash;
        internal static int TokStart;
        internal static int TokLength;

        // This returns a hash of zero iff we hit the EOF.
        internal static void ScanToken()
        {
            TokHash = 0;
            TokStart = 0;
            TokLength = 0;
            var result = (hash: 0, start: SrcIdx, length: 0);
            // Skip whitespace.
            while (true)
            {
                var c = Mem[SrcIdx];
                if (c == 0) return;
                if (!char.IsWhiteSpace((char)c)) break;
                SrcIdx++;
            }
            // Scan until we hit whitespace or EOF.
            TokStart = SrcIdx;
            while (true)
            {
                var c = Mem[SrcIdx];
                if (c == 0) return;
                if (!char.IsWhiteSpace((char)c)) return;
                SrcIdx++;
                TokHash = ((TokHash << 1) | c) & 0xff;
                if (TokHash == 0) result.hash = 1;
                TokLength++;
            }
        }

        internal static int Hash(string name)
        {
            var hash = 0;
            foreach (var c in name.Cast<int>())
            {
                hash = ((hash << 1) | c) & 0xff;
                if (hash == 0) hash = 1;
            }
            return hash;
        }
    }
}
