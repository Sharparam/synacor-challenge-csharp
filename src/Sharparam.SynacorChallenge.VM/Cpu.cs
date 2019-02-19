namespace Sharparam.SynacorChallenge.VM
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    public class Cpu
    {
        private const ushort MemorySize = 0x7FFF;

        private const int RegisterCount = 8;

        private readonly ILogger _log;

        private readonly ushort[] _memory;

        private readonly ushort[] _registers;

        private readonly Stack<ushort> _stack;

        private readonly IOutputWriter _outputWriter;

        private readonly IInputReader _inputReader;

        private ushort _pointer;

        private bool _running;

        public Cpu(ILogger<Cpu> log, IOutputWriter outputWriter, IInputReader inputReader)
        {
            _log = log;
            _outputWriter = outputWriter;
            _inputReader = inputReader;
            _memory = new ushort[MemorySize];
            _registers = new ushort[RegisterCount];
            _stack = new Stack<ushort>();
        }

        public void LoadProgram(Program program)
        {
            var counter = 0;

            foreach (var entry in program)
            {
                _memory[counter++] = entry;
            }

            _pointer = 0;
        }

        public void LoadProgram(ushort[] program)
        {
            for (var i = 0; i < program.Length; i++)
            {
                _memory[i] = program[i];
            }

            _pointer = 0;
        }

        public void Run()
        {
            _running = true;
            _pointer = 0;

            while (_running)
            {
                Step();
            }
        }

        private void Step()
        {
            //_log.LogTrace("0x{Address:X}", _pointer);
            var opByte = _memory[_pointer++];
            var opCode = (OpCode)opByte;

            switch (opCode)
            {
                case OpCode.Halt:
                    _running = false;
                    break;

                case OpCode.Set:
                {
                    var target = NextMem();
                    var val = NextValue();
                    Set((byte)target, val);
                }
                    break;

                case OpCode.Push:
                    _stack.Push(NextValue());
                    break;

                case OpCode.Pop:
                    if (_stack.Count == 0)
                    {
                        throw new InvalidOperationException("Cannot pop empty stack");
                    }

                    Set(_stack.Pop());
                    break;

                case OpCode.Equal:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a == b ? 1 : 0));
                }
                    break;

                case OpCode.Gt:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a > b ? 1 : 0));
                }
                    break;

                case OpCode.Jump:
                    _pointer = NextValue();
                    break;

                case OpCode.JumpTrue:
                    var truthValue = NextValue();
                    var truthDest = NextValue();
                    if (truthValue != 0)
                    {
                        _pointer = truthDest;
                    }

                    break;

                case OpCode.JumpFalse:
                    var falseValue = NextValue();
                    var falseDest = NextValue();
                    if (falseValue == 0)
                    {
                        _pointer = falseDest;
                    }

                    break;

                case OpCode.Add:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, NumberHelper.Add(a, b));
                }

                    break;

                case OpCode.Multiply:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, NumberHelper.Multiply(a, b));
                }
                    break;

                case OpCode.Mod:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a % b));
                }
                    break;

                case OpCode.And:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a & b));
                }
                    break;

                case OpCode.Or:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a | b));
                }
                    break;

                case OpCode.Not:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Set((byte)target, (ushort)(a ^ b));
                }
                    break;

                case OpCode.ReadMem:
                {
                    var target = NextMem();
                    var val = _memory[NextValue()];
                    Set((byte)target, val);
                }
                    break;

                case OpCode.WriteMem:
                {
                    var target = NextValue();
                    var val = NextValue();
                    _memory[target] = val;
                }
                    break;

                case OpCode.Call:
                    var value = NextValue();
                    _stack.Push(_pointer);
                    _pointer = value;
                    break;

                case OpCode.Ret:
                    if (_stack.Count == 0)
                    {
                        _running = false;
                    }
                    else
                    {
                        _pointer = _stack.Pop();
                    }

                    break;

                case OpCode.Out:
                    var charByte = _memory[_pointer++];
                    var charValue = (char)charByte;
                    _outputWriter.Write(charValue);
                    break;

                case OpCode.In:
                    Set(_inputReader.Read());
                    break;

                case OpCode.NoOp:
                    break;

                default:
                    throw new NotSupportedException($"Unsupported OpCode: {opCode} (0x{opCode:X})");
            }
        }

        private ushort ValueOf(ushort number)
        {
            if (number >= 32776)
            {
                throw new ArgumentOutOfRangeException(nameof(number), number, "Values 32776 and higher are invalid");
            }

            if (number > 32767)
            {
                return _registers[number - 32768];
            }

            return number;
        }

        private ushort NextMem() => _memory[_pointer++];

        private ushort NextValue() => ValueOf(NextMem());

        private void Set()
        {
            var val = NextValue();
            Set(val);
        }

        private void Set(ushort value)
        {
            var idx = _pointer;
            var reg = _memory[idx];
            if (reg >= _registers.Length)
            {
                reg -= 32768;
            }

            _registers[reg] = value;
            _pointer++;
        }

        private void Set(byte reg)
        {
            _registers[reg] = NextValue();
        }

        private void Set(byte reg, ushort value)
        {
            _registers[reg] = value;
        }
    }
}
