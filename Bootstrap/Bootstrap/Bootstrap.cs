using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap
{
    // This is a really, really simple language for generating Z80
    // programs.  The idea is that this could have been written
    // fairly easily in BASIC, then used to bootstrap itself
    // (for performance), then used to develop a compiler for a
    // decent language.
    //
    public static class Bootstrap
    {
        public static void Main(string[] args)
        {
        }

        internal enum SymKind { Const, Var, LabelRef, Label, Macro, Keyword };
        internal class Sym
        {
            internal SymKind Kind;
            internal int Value;
            internal List<byte> MacroBytes;
        }
        internal static string Src; // The source code.
        internal static int Org = 0x8000; // The start of target code.
        internal static int End = 0xffff; // The last byte of target code.
        internal static List<byte> Code = new List<byte> { }; // Generated code.
        internal static int Here { get { return Code.Count; } }
        internal static int VarBot = End + 1;
        internal static int NewVar() { VarBot -= 2; return VarBot; }
        internal static int Gen(params int[] bytes)
        {
            foreach (var x in bytes) Code.Add((byte)x);
            return Here;
        }
        internal static byte Lo(int x) { return (byte)(x & 0x00ff); }
        internal static byte Hi(int x) { return (byte)(x / 0x0100); }
        internal static int PeekWord(int i)
        {
            return (Code[i] + 0x100 * Code[i + 1]);
        }
        internal static void PokeWord(int i, int x)
        {
            Code[i] = Lo(x);
            Code[i + 1] = Hi(x);
        }
        internal static int SrcIdx = 0;
        internal static void Error(string msg)
        {
            throw new ApplicationException(msg);
        }
        internal const string EofTok = " EOF"; // Leading space means internal.
        internal const string EolTok = " EOL";
        internal static string NextTok()
        {
            var n = Src.Length;
            while (SrcIdx < n && char.IsWhiteSpace(Src[SrcIdx])) SrcIdx++;
            if (n <= SrcIdx) return EofTok;
            var start = SrcIdx;
            if (Src[SrcIdx++] == '\n') return EolTok;
            while (SrcIdx < n && !char.IsWhiteSpace(Src[SrcIdx])) SrcIdx++;
            return Src.Substring(start, SrcIdx - start);
        }
        internal static Sym EofSym = new Sym { Kind = SymKind.Keyword };
        internal static Sym EolSym = new Sym { Kind = SymKind.Keyword };
        internal static Sym EqSym = new Sym { Kind = SymKind.Keyword };
        internal static Dictionary<string, Sym> SymTable =
            new Dictionary<string, Sym> {
                { EofTok, EofSym },
                { EolTok, EolSym },
                { "=", EqSym }
            };
        internal static Sym NextSym(
            bool labelDefOk = false,
            bool varDefOk = false,
            bool macroDefOk = false
        ) {
            var tok = NextTok();
            var sym = (Sym)null;
            if (SymTable.TryGetValue(tok, out sym)) return sym;
            switch (tok[0])
            {
                case ':':
                    if (labelDefOk) {
                        sym = new Sym { Kind = SymKind.Label, Value = Here };
                    }
                    else {
                        sym = new Sym { Kind = SymKind.LabelRef };
                    }
                    SymTable[tok] = sym;
                    return sym;
                case '~':
                    if (!macroDefOk) Error("Cannot define a macro here.");
                    Error("Macro definitions NYI.");
                    return null;
                default:
                    if (!varDefOk) Error("Cannot define a var here.");
                    sym = new Sym { Kind = SymKind.Var, Value = NewVar() };
                    SymTable[tok] = sym;
                    return sym;
            }
        }
        internal static void GenArg(int argNo, Sym x)
        {
            // Z80: arg1 goes in HL, arg2 in DE, arg3 in BC.
            var ldVarPrefix = 0;
            var ldConst = 0;
            var ldVar = 0;
            switch (argNo)
            {
                case 1: ldConst = 0x21; ldVar = 0x2a; break;
                case 2: ldConst = 0x11; ldVar = 0x5b; ldVarPrefix = 0xed; break;
                case 3: ldConst = 0x01; ldVar = 0x4b; ldVarPrefix = 0xed; break;
                default: Error("Arg count exceeded."); break;
            }
            switch (x.Kind)
            {
                case SymKind.Const:
                case SymKind.Label:
                    Gen(ldConst, Lo(x.Value), Hi(x.Value));
                    return;
                case SymKind.LabelRef:
                    Gen(ldConst, Lo(x.Value), Hi(x.Value));
                    x.Value = Here; // Add this to the fwd ref chain.
                    return;
                case SymKind.Var:
                    if (ldVarPrefix != 0) Gen(ldVarPrefix);
                    Gen(ldVar, Lo(x.Value), Hi(x.Value));
                    return;
                default:
                    Error("Invalid arg kind.");
                    return;
            }
        }
        internal static void CompileLine()
        {
            var pendingOp = (Sym)null;
            var pendingAssgtVar = (Sym)null;
            var pendingMacro = (Sym)null;
            var labelDefOk = true;
            var assgtOk = true;
            var macroDefOk = true; // XXX NYI!
            
        }


    }
}
