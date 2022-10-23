using System.Collections;
using System.Collections.Generic;

namespace Parser
{
    internal class Comment : IEnumerable<Comment>
    {
        public string Value { get; set; }

        public List<Comment> Nested { get; set; } = new List<Comment>();

        public static List<Comment> GetComments(List<string> comments)
        {
            var result = new List<Comment>();

            for (int i = 0; i < comments.Count; i++)
            {
                comments[i] = comments[i].Remove(0, 1);
                comments[i] = comments[i].Remove(comments[i].Length - 1, 1);

                if (comments[i].Contains("<"))
                {
                    List<string> nested = TrimComments(comments[i]);

                    nested.ForEach(x => comments[i] = comments[i].Replace(x, string.Empty));

                    result.Add(new Comment()
                    {
                        Value = comments[i],
                        Nested = GetComments(nested)
                    });
                }
                else
                {
                    result.Add(new Comment()
                    {
                        Value = comments[i]
                    });
                }
            }


            return result;
        }

        public static List<string> TrimComments(string line)
        {
            List<string> comments = new List<string>();

            int startIndex = 0;
            int countInvestments = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '<')
                {
                    if (countInvestments == 0)
                        startIndex = i;

                    countInvestments++;
                }
                else if (line[i] == '>')
                {
                    countInvestments--;
                    if (countInvestments == 0)
                        comments.Add(line.Substring(startIndex, (i + 1) - startIndex));
                }
            }

            return comments;
        }

        public override string ToString() => Value;

        #region Реализация IEnumerable

        public IEnumerator<Comment> GetEnumerator()
        {
            yield return this;
            if (Nested.Count > 0)
            {
                foreach (var nested in Nested)
                {
                    foreach (var comment in nested)
                        yield return comment;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this).GetEnumerator();

        #endregion
    }
}
