namespace Sharparam.SynacorChallenge.VM
{
    public enum OpCode : ushort
    {
        Halt = 0,
        Set = 1,
        Push = 2,
        Pop = 3,
        Equal = 4,
        GreaterThan = 5,
        Jump = 6,
        JumpTrue = 7,
        JumpFalse = 8,
        Add = 9,
        Multiply = 10,
        Mod = 11,
        And = 12,
        Or = 13,
        Not = 14,
        ReadMem = 15,
        WriteMem = 16,
        Call = 17,
        Ret = 18,
        Out = 19,
        In = 20,
        NoOp = 21
    }
}
