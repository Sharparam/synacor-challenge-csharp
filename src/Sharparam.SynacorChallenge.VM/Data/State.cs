namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    using Newtonsoft.Json;

    [Serializable]
    public class State
    {
        [JsonProperty]
        private readonly Stack<ushort> _stack;

        [JsonProperty]
        private ushort _instructionPointer;

        public State()
        {
            _stack = new Stack<ushort>();
            Memory = new Memory();
            Registers = new Registers();
        }

        [JsonConstructor]
        public State(Stack<ushort> stack, Memory memory, Registers registers, ushort instructionPointer = 0)
        {
            _stack = stack;
            Memory = memory;
            Registers = registers;
            _instructionPointer = instructionPointer;
        }

        public Memory Memory { get; }

        public Registers Registers { get; }

        [JsonIgnore]
        public Stack<ushort> Stack => _stack;

        public ushort InstructionPointer
        {
            get => _instructionPointer;
            set => _instructionPointer = value;
        }

        public State Copy()
        {
            var stackArray = new ushort[_stack.Count];
            _stack.CopyTo(stackArray, 0);
            return new State(new Stack<ushort>(stackArray), Memory.Copy(), Registers.Copy(), _instructionPointer);
        }

        public static State FromDumpFile(string path)
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (State)formatter.Deserialize(fileStream);
            }
        }

        public void Reset(bool clearMemory)
        {
            if (clearMemory)
            {
                Memory.Clear();
            }

            Registers.Reset();
            Stack.Clear();
        }

        public void SaveToDumpFile(string path)
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fileStream, this);
            }
        }
    }
}
