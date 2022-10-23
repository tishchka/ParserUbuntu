namespace Parser
{
    internal class Analysis
    {
        private readonly List<string> _AllStates = new List<string>();

        private readonly List<string> _InputAlphabet = new List<string>();

        private readonly List<string> _StackAlphabet = new List<string>();

        private readonly List<string> _Lines;

        public Analysis(string path) => _Lines = File.ReadAllLines(path).ToList();


        public void Analyze()
        {
            if (_Lines.Count >= 6)
            {
                OutputState();
                OutputAlphane();
                OutputInstruction();
            }
            else
            {
                Console.WriteLine($"Общая ошибка: Файл должен содержать как минимум 6 строк.");
            }
        }

        private void OutputState()
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    try
                    {
                        StateProcessing(_Lines[0], i == 0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка состояния автомана. Строка: {i + 1}. Ошибка: {ex.Message}");
                        return;
                    }
                    Console.WriteLine("[Конец строки]\r\n");
                    _Lines.RemoveAt(0);
                }
                catch
                {
                    Console.WriteLine($"Ошибка. Строка: {i}.");
                }
            }
        }

        private void OutputAlphane()
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string line = _Lines[0];
                    OutputComment(ref line);
                    char symbol = i == 0 ? '\'' : '`';
                    string alphabelToken = i == 0 ? "Входной алфавит" : "Стековый алфавит";
                    List<string> alphabet = GetValues(line, symbol);
                    if (alphabet.Count > 0)
                    {
                        foreach (var item in alphabet)
                        {
                            if (i == 0)
                                _InputAlphabet.Add(item);
                            else
                                _StackAlphabet.Add(item);
                            Console.WriteLine($"[{alphabelToken}] - \"{item}\"");
                        }
                        Console.WriteLine("[Конец строки]\r\n");
                        _Lines.RemoveAt(0);
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка [{alphabelToken}] - отсутсвуют символы. Строка: {i + 5}.");
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine($"Ошибка. Строка: {i + 5}.");
                }
            }
        }

        private void OutputInstruction()
        {
            if (_Lines.Count > 0)
            {
                try
                {
                    List<string> lines = EditLine(_Lines);
                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        OutputComment(ref line);
                        if (line.Contains("~"))
                        {
                            Instruction instruction = null;
                            try
                            {
                                instruction = GetInstruction(line);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка синтаксиса: {ex.Message}");
                                return;
                            }
                            Console.WriteLine($"[Состояние автомата] - {instruction.AutomataState}");
                            Console.WriteLine($"[Входной алфавит] - {instruction.InputAlpabet}");
                            Console.WriteLine($"[Стековый алфавит] - {instruction.StackAlpabet}");
                            Console.WriteLine($"[Сцец. символ] - ~");
                            Console.WriteLine($"[Состояние автомата] - {instruction.NewAutomataState}");
                            Console.WriteLine($"[Стековый алфавит] - {instruction.NewStackAlpabet}");
                            Console.WriteLine("[Конец строки]\r\n");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка синтаксиса. В строке: {line} - отсутствует знай ~.");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine($"Ошибка синтаксиса в инструкций.");
                }
            }
            else
            {
                Console.WriteLine("Нет инструкций.");
            }
        }

        private Instruction GetInstruction(string line)
        {
            string args = line.Substring(0, line.IndexOf("~"));
            List<string> state = GetValues(args, '\"');
            List<string> inputAlpabet = GetValues(args, '\'');
            List<string> stackAlpabet = GetValues(args, '`');
            if (state.Count != 1)
                throw new Exception($"В строке: {line} - кол. состояний автомата не равно 1.");

            if (_AllStates.Contains(state[0]) is false)
                throw new Exception($"В строке: {line} - неизвестное состояние автомата.");

            if (inputAlpabet.Count != 1)
            {
                if (inputAlpabet.Count == 0 && args.Contains("EPS"))
                    inputAlpabet.Add("EPS");
                else
                    throw new Exception($"В строке: {line} - кол. символов входного алфавита не равно 1.");
            }
            else if (_InputAlphabet.Contains(inputAlpabet[0]) is false)
            {
                throw new Exception($"{inputAlpabet[0]} - неизвестная строка входного алфавита.");
            }
            if (stackAlpabet.Count != 1)
            {
                if (stackAlpabet.Count == 0 && args.Contains("EMPTY"))
                    stackAlpabet.Add("EMPTY");
                else
                    throw new Exception($"В строке: {line} - кол. символов стекового алфавита не равно 1.");
            }
            else if (_StackAlphabet.Contains(stackAlpabet[0]) is false)
            {
                throw new Exception($"{stackAlpabet[0]} - неизвестная строка стекового алфавита.");
            }
            string value = line.Remove(0, line.IndexOf("~") + 1);

            List<string> newState = GetValues(value, '\"');
            if (newState.Count != 1)
                throw new Exception($"В строке: {line} - кол. новых состояний автомата не равно 1.");

            if (_AllStates.Contains(newState[0]) is false)
                throw new Exception($"{newState[0]} - неизвестное новое состояние автомата.");

            List<string> newStackAlpabet = GetValues(value, '`');
            if (newStackAlpabet.Count != 1)
            {
                if (newStackAlpabet.Count == 0 && (value.Contains("EMPTY") || value.Contains("NULL")))
                {
                    if (value.Contains("EMPTY"))
                        newStackAlpabet.Add("EMPTY");
                    else
                        newStackAlpabet.Add("NULL");
                }
                else
                    throw new Exception($"В строке: {line} - кол. символов стекового алфавита не равно 1.");
            }

            return new Instruction(state[0], inputAlpabet[0], stackAlpabet[0], newState[0], newStackAlpabet[0]);
        }

        private static List<string> EditLine(List<string> lines)
        {
            List<string> result = new List<string>();
            string currentLine = "";
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (lines[i].EndsWith(","))
                {
                    line = line.Remove(line.Length - 1, 1);
                    currentLine += line;
                    result.Add(currentLine);
                    currentLine = "";
                }
                else if (i == lines.Count - 1)
                {
                    currentLine += line;
                    result.Add(currentLine);
                }
                else
                {
                    currentLine += line + " ";
                }
            }

            return result;
        }

        private void StateProcessing(string line, bool isAllState)
        {
            if (string.IsNullOrWhiteSpace(line) is false && line.Contains("\"") && line.Contains(","))
            {
                OutputComment(ref line);
                List<string> states = GetValues(line, '\"');
                if (states.Count != 0)
                {
                    foreach (string state in states)
                    {
                        if (isAllState)
                        {
                            _AllStates.Add(state);
                        }
                        else if (_AllStates.Contains(state) is false)
                        {
                            throw new ArgumentException($"Неизвестное состояние автомата: {state}.");
                        }

                        Console.WriteLine($"[Состояние автомата] - \"{state}\"");
                    }
                }
                else
                {
                    throw new ArgumentException($"Нет ни одного состояния автомата.");
                }
            }
            else
            {
                throw new ArgumentException($"Неккоректная строка: {line}");
            }
        }

        private List<string> GetValues(string line, char symbol)
        {
            List<string> result = new List<string>();
            int startIndex = -1;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == symbol)
                {
                    int predI = i - 1;
                    if (predI >= 0 && line[predI] == '\\')
                    {
                        continue;
                    }
                    else
                    {
                        if (startIndex == -1)
                        {
                            startIndex = i;
                        }
                        else
                        {
                            result.Add(line.Substring(startIndex + 1, i - startIndex - 1));
                            startIndex = -1;
                        }
                    }
                }
            }

            return result;
        }

        public static void OutputComment(ref string line)
        {
            if (line.Contains("<"))
            {
                List<string> commentsStr = Comment.TrimComments(line);

                foreach (string comment in commentsStr)
                    line = line = line.Replace(comment, string.Empty);

                var comments = Comment.GetComments(commentsStr);
                if (comments != null && comments.Count > 0)
                {
                    foreach (var comment in comments)
                    {
                        foreach (var item in comment)
                        {
                            Console.WriteLine($"[Комментарий] - {item}");
                        }
                    }
                }
            }
        }
    }
}
