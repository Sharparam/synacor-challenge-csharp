namespace Sharparam.SynacorChallenge.VM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design.Serialization;
    using System.IO;

    using Data;

    using Microsoft.Extensions.Logging;

    public class Cpu
    {
        private readonly ILogger _log;

        private readonly IOutputWriter _outputWriter;

        private readonly IInputReader _inputReader;

        private readonly Queue<char> _inputQueue;

        private State _state;

        private bool _running;

        public Cpu(ILogger<Cpu> log, IOutputWriter outputWriter, IInputReader inputReader)
        {
            _log = log;
            _state = new State();
            _outputWriter = outputWriter;
            _inputReader = inputReader;
            _inputQueue = new Queue<char>();
        }

        private Memory Memory => _state.Memory;

        private Registers Registers => _state.Registers;

        private Stack<ushort> Stack => _state.Stack;

        private ushort Pointer
        {
            get => _state.InstructionPointer;
            set => _state.InstructionPointer = value;
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

        public void Run(bool reset = true)
        {
            if (reset)
            {
                Reset();
            }

            _running = true;

            while (_running)
            {
                Step();
            }
        }

        public void Reset(bool clearMemory = false)
        {
            _log.LogDebug($"Resetting VM state ({nameof(clearMemory)} == {{ClearMemory}}", clearMemory);

            _state.Reset(clearMemory);

            _log.LogTrace("Resetting instruction pointer");
            Pointer = 0;
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
                    var target = NextMem();
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
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.Equal(a, b);
                }
                    break;

                case OpCode.GreaterThan:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.GreaterThan(a, b);
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
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.Add(a, b);
                }

                    break;

                case OpCode.Multiply:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.Multiply(a, b);
                }
                    break;

                case OpCode.Mod:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.Mod(a, b);
                }
                    break;

                case OpCode.And:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.And(a, b);
                }
                    break;

                case OpCode.Or:
                {
                    var target = NextMem();
                    var a = NextValue();
                    var b = NextValue();
                    Registers[target] = NumberHelper.Or(a, b);
                }
                    break;

                case OpCode.Not:
                {
                    var target = NextMem();
                    var a = NextValue();
                    Registers[target] = ~(Literal)a;
                }
                    break;

                case OpCode.ReadMem:
                {
                    var target = NextMem();
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

                        var (handled, adjustPointer) = HandleInput(line);
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

                    var target = NextMem();
                    Registers[target] = _inputQueue.Dequeue();
                }
                    break;

                case OpCode.NoOp:
                    break;

                default:
                    throw new NotSupportedException($"Unsupported OpCode: {opCode} (0x{opCode:X})");
            }
        }

        private ushort ValueOf(ushort number)
        {
            if (number > Operand.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(number), number, "Values 32776 and higher are invalid");
            }

            if (number > Literal.MaxValue)
            {
                return Registers[number];
            }

            return number;
        }

        private ushort NextMem() => Memory[Pointer++];

        private ushort NextValue() => ValueOf(NextMem());

        private void Set(ushort value)
        {
            var idx = Pointer;
            var reg = Memory[idx];
            Registers[reg] = value;
            Pointer++;
        }

        private (bool Handled, bool AdjustPointer) HandleInput(string line)
        {
            if (line.StartsWith("save"))
            {
                var path = line.Substring("save".Length).Trim();

                if (string.IsNullOrWhiteSpace(path))
                {
                    _log.LogError("Target path cannot be empty");
                    return (true, true);
                }

                path = Path.ChangeExtension(path, "state");

                // Rewind the instruction pointer first because we need to go back to the address containing
                // the op code to run.
                _state.InstructionPointer--;
                _state.SaveToDumpFile(path);
                _log.LogInformation("Saved current state to dumpfile \"{Path}\"", path);
                return (true, true);
            }

            if (line.StartsWith("load"))
            {
                var path = line.Substring("load".Length).Trim();

                if (string.IsNullOrWhiteSpace(path))
                {
                    _log.LogError("Target path cannot be empty");
                    return (true, true);
                }

                if (!File.Exists(path))
                {
                    _log.LogError("Target path does not exist");
                    return (true, true);
                }

                _state = State.FromDumpFile(path);
                _log.LogInformation("Loaded state from dumpfile \"{Path}\"", path);
                return (true, false);
            }

            return (false, false);
        }
    }
}
