namespace Parser
{
    internal class Instruction
    {
        public string AutomataState { get; set; }

        public string InputAlpabet { get; set; }

        public string StackAlpabet { get; set; }

        public string NewAutomataState { get; set; }
        public string NewStackAlpabet { get; set; }

        public Instruction() { }

        public Instruction(string automataState, string inputAlpabet, string stackAlpabet, string newAutomataState, string newStackAlpabet) =>
            (AutomataState, InputAlpabet, StackAlpabet, NewAutomataState, NewStackAlpabet) =
            (automataState, inputAlpabet, stackAlpabet, newAutomataState, newStackAlpabet);
    }
}