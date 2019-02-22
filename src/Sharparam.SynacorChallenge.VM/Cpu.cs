namespace Sharparam.SynacorChallenge.VM
{
    using System;
    using System.Collections.Generic;

    using Data;

    using Microsoft.Extensions.Logging;

    public class Cpu
    {
        private readonly ILogger _log;

        private readonly IOutputWriter _outputWriter;

        private readonly IInputReader _inputReader;

        private readonly CommandManager _commandManager;

        private readonly Queue<char> _inputQueue;

        private State _state;

        private bool _running;

        public Cpu(ILogger<Cpu> log, IOutputWriter outputWriter, IInputReader inputReader, CommandManager commandManager)
        {
            _log = log;
            _state = new State();
            _outputWriter = outputWriter;
            _inputReader = inputReader;
            _commandManager = commandManager;
            _inputQueue = new Queue<char>();
        }

        private Memory Memory => _state.Memory;

        private Registers Registers => _state.Registers;

        private Stack<ushort> Stack => _state.Stack;

        public ushort Pointer
        {
            get => _state.InstructionPointer;
            private set => _state.InstructionPointer = value;
        }

        public void LoadProgram(Program program)
        {
            Reset(true);

            var counter = 0;

            foreach (var entry in program)
            {
                Memory[counter++] = entry;
            }
        }

        public void LoadProgram(ushort[] program)
        {
            Reset(true);

            for (var i = 0; i < program.Length; i++)
            {
                Memory[i] = program[i];
            }
        }

        public void LoadState(State state)
        {
            _state = state;
        }

        public State CopyState() => _state.Copy();

        public void Run(bool reset = true)
        {
            if (reset)
            {
                Reset();
            }

            _running = true;

            _log.LogInformation("Starting CPU");
            while (_running)
            {
                Step();
            }

            _log.LogInformation("Program terminated");
        }

        public void Reset(bool clearMemory = false)
        {
            _log.LogDebug($"Resetting VM state ({nameof(clearMemory)} == {{ClearMemory}})", clearMemory);

            _state.Reset(clearMemory);

            _log.LogTrace("Resetting instruction pointer");
            Pointer = 0;
            _running = false;
        }

        public void Halt()
        {
            _running = false;
        }

        private void Step()
        {
            //_log.LogTrace("0x{Address:X}", _pointer);
            var opByte = Memory[Pointer++];
            var opCode = (OpCode)opByte;

            switch (opCode)
            {
                case OpCode.Halt:
                    _running = false;
                    break;

                case OpCode.Set:
                {
                    var target = NextOperand();
                    var val = NextValue();
                    Registers[target] = val;
                }
                    break;

                case OpCode.Push:
                    Stack.Push(NextValue());
                    break;

                case OpCode.Pop:
                    if (Stack.Count == 0)
                    {
                        throw new InvalidOperationException("Cannot pop empty stack");
                    }

                    Set(Stack.Pop());
                    break;

                case OpCode.Equal:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = (Literal)(a == b ? 1 : 0);
                }
                    break;

                case OpCode.GreaterThan:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = (Literal)(a > b ? 1 : 0);
                }
                    break;

                case OpCode.Jump:
                    Pointer = NextValue();
                    break;

                case OpCode.JumpTrue:
                    var truthValue = NextValue();
                    var truthDest = NextValue();
                    if (truthValue != 0)
                    {
                        Pointer = truthDest;
                    }

                    break;

                case OpCode.JumpFalse:
                    var falseValue = NextValue();
                    var falseDest = NextValue();
                    if (falseValue == 0)
                    {
                        Pointer = falseDest;
                    }

                    break;

                case OpCode.Add:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = a + b;
                }

                    break;

                case OpCode.Multiply:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = a * b;
                }
                    break;

                case OpCode.Mod:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = a % b;
                }
                    break;

                case OpCode.And:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = a & b;
                }
                    break;

                case OpCode.Or:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = a | b;
                }
                    break;

                case OpCode.Not:
                {
                    var target = NextOperand();
                    var a = NextValue();
                    Registers[target] = ~a;
                }
                    break;

                case OpCode.ReadMem:
                {
                    var target = NextOperand();
                    var val = Memory[NextValue()];
                    Registers[target] = val;
                }
                    break;

                case OpCode.WriteMem:
                {
                    var target = NextValue();
                    var val = NextValue();
                    Memory[target] = val;
                }
                    break;

                case OpCode.Call:
                    var value = NextValue();
                    Stack.Push(Pointer);
                    Pointer = value;
                    break;

                case OpCode.Ret:
                    if (Stack.Count == 0)
                    {
                        _running = false;
                    }
                    else
                    {
                        Pointer = Stack.Pop();
                    }

                    break;

                case OpCode.Out:
                    var charByte = NextValue();
                    var charValue = (char)charByte;
                    _outputWriter.Write(charValue);
                    break;

                case OpCode.In:
                {
                    if (_inputQueue.Count == 0)
                    {
                        var line = _inputReader.ReadLine().Trim();

                        var (handled, adjustPointer) = _commandManager.Handle(this, line);
                        if (handled)
                        {
                            if (adjustPointer)
                            {
                                Pointer--;
                            }

                            return;
                        }

                        foreach (var c in line.ToCharArray())
                        {
                            _inputQueue.Enqueue(c);
                        }

                        _inputQueue.Enqueue('\n');
                    }

                    var target = NextOperand();
                    Registers[target] = _inputQueue.Dequeue();
                }
                    break;

                case OpCode.NoOp:
                    break;

                default:
                    throw new NotSupportedException($"Unsupported OpCode: {opCode} (0x{opCode:X})");
            }
        }

        private Literal ValueOf(ushort number)
        {
            if (number > Operand.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(number), number, "Values 32776 and higher are invalid");
            }

            if (number > Literal.MaxValue)
            {
                return (Literal)Registers[number];
            }

            return (Literal)number;
        }

        private Operand NextOperand() => Memory[Pointer++];

        private Literal NextValue() => ValueOf(NextOperand());

        private void Set(ushort value)
        {
            var idx = Pointer;
            var reg = Memory[idx];
            Registers[reg] = value;
            Pointer++;
        }
    }
}
