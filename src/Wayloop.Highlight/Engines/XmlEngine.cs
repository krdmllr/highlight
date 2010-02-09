#region License

// Copyright (c) 2010 Thomas Andre H. Johansen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion


using System.Text;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public class XmlEngine : Engine
    {
        protected override string ElementMatchHandler(Match m)
        {
            var builder = new StringBuilder();
            var builder2 = new StringBuilder();
            const string format = "<{0}>{1}</{0}>";
            foreach (Pattern pattern in Definition.Patterns) {
                if (!m.Groups[pattern.Name].Success) {
                    continue;
                }
                if (pattern is BlockPattern) {
                    return string.Format(format, pattern.Name, m.Value);
                }
                if (pattern is MarkupPattern) {
                    builder.AppendFormat(format, "openTag", m.Groups["openTag"].Value);
                    builder.AppendFormat(format, "whitespace", m.Groups["ws1"].Value);
                    builder.AppendFormat(format, "tagName", m.Groups["tagName"].Value);
                    for (var i = 0; i < m.Groups["attribName"].Captures.Count; i++) {
                        builder2.AppendFormat(format, "whitespace", m.Groups["ws2"].Captures[i].Value);
                        builder2.AppendFormat(format, "attribName", m.Groups["attribName"].Captures[i].Value);
                        builder2.AppendFormat(format, "whitespace", m.Groups["ws3"].Captures[i].Value);
                        builder2.AppendFormat(format, "attribValue", m.Groups["attribSign"].Captures[i].Value + m.Groups["ws4"].Captures[i].Value + m.Groups["attribValue"].Captures[i].Value);
                        builder.AppendFormat(format, "attribute", builder2);
                    }
                    builder.AppendFormat(format, "whitespace", m.Groups["ws5"].Value);
                    builder.AppendFormat(format, "closeTag", m.Groups["closeTag"].Value);

                    return string.Format(format, pattern.Name, builder);
                }
                if (pattern is WordPattern) {
                    return string.Format(format, pattern.Name, m.Value);
                }
            }

            return m.Value;
        }


        protected override void Highlight()
        {
            var evaluator = new MatchEvaluator(ElementMatchHandler);
            var patterns = Definition.GetPatterns();
            Input = string.Format("<highlightedInput>{0}</highlightedInput>", Input);
            Input = Regex.Replace(Input, patterns, evaluator);
        }
    }
}