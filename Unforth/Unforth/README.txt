This is a little "just above assembly" programming language designed with
the ancient Z80 processor in mind.  I have called it "Unforth" because 
this is my conception of a half-way decent language in the Forth niche,
but without all that incomprehensible stack munging, and with a reasonably
efficient implementation path for the Z80 (Forth is actually painfully
slow on a Z80 compared to hand crafted machine code).

Note: Forth is really slow on the Z80.  Taking Brad Rodriguez' CamelForth
as an high quality example (http://www.bradrodriguez.com/papers/camel80.txt)
we see the following costs (in Z80 T-states):
    enter:      68 + 34 = 102 T-states to call a word
    exit:       58 + 38 =  98 T-states to return from a word
    next:                  38 T-states after every primitive
    +:          29 + 38 =  67 T-states to add two numbers
    lit:        45 + 38 =  83 T-states to load a number
So the simple Forth program
    : foo + ;
    1 2 foo
is going to take 433 T-states to execute!  This is a far cry from the
30-odd T-states plain old Z80 machine code would take.  If we inline the
code (i.e., use "subroutine threaded code") then things go quite a bit
faster, but I'm not aware of any 1980s Z80 Forth that did that.

In Unforth, the basic idea is to provide a virtual machine that comfortably
supports C-like stack frames.  Each operation is an op-code followed by zero or
more constants.  There are two primary virtual registers, A and B, a frame
pointer register FP, a frame size register FN, and a separate stack pointer SP.

    n       A = n
    @       A = *A
    !       *A = B
    ,       B = A
    +       A = A + B
etc.
    L0      A = FP[0]   (Loading args, locals.)
    L1      A = FP[1]
    L2      A = FP[2]
(There is obvious room for shorthand operations, such as LI n == L n @.)
etc.
    S0      FP[0] = A   (Storing args, locals, caller args.)
    S1      FP[1] = A
    S2      FP[2] = A
etc.
    R a     A = *a      (Read/write globals.)
    G a     *a = A
    C a     call a
    F n                 (Function entry, reserve n slots for args and locals.)
    R                   (Function exit.)
and so forth.

Let's posit the following implementation:
- Instructions are compiled inline -- there is no interpreter.
- The Z80 stack will be used for return addresses and temporary storage.
- A separate "frame stack" will be indexed via IX.
- Registers A and B will be held in HL and DE respectively.
- BC will hold the current frame size.

Our basic instructions are encoded like this:

    n       ld HL, n                10 Ts       A = n

    ,       ex DE, HL                4 Ts       B = A

    @       ld A, (HL)                          A = *A
            inc HL
            ld H, (HL)
            ld L, A                 24 Ts

    !       ld (HL),                            *A = BE
            inc HL
            ld (HL), D
            dec HL                  26 Ts

    +       add HL, DE              11 Ts       A = A + B

    Ln      ld L, (IX+2n)                       A = F[n]
            ld H, (IX+2n+1)         38 Ts

    Sn      ld (IX+2n), L
            ld (IX+2n+1), H         38 Ts       F[n] = A

    R a     ld HL, (a)              16 Ts       A = *a

    W a     ld (a), HL              16 Ts       *a = A

    C a     call a                  17 Ts       Call a

    F n     push IX                             F = F + n -- reserve a stack frame
            push BC
            add IX, BC
            ld BC, n                51 Ts

    R       pop BC                              Restore F and return
            pop IX
            ret                     34 Ts

Okay, how would our little example hold up here?

    ; fun foo x y = x + y ;
    foo: + R                        45 Ts
    ; foo(1, 2)
    1 , 2 C foo                     41 Ts

for a total of 86 Ts -- almost exactly five times faster than CamelForth,
but without having to expend all that mental effort thinking about the stack at
every point.  Of course, this isn't "idiomatic" Unforth which would look like
this:

    ; fun foo x y = x + y ;
    foo: F 2 L0 , L1 + R           176 Ts
    1 S0 2 S1 C foo                113 Ts

for a total of 289 Ts, a touch less than twice the speed of Z80 Forth
(admittedly taking nearly thrice the space).


