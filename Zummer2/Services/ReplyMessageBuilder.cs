using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zummer.Services
{
    public class ReplyMessageBuilder
    {
        private const string _newLine = "<br>";
        private const Color defaultColor = Color.Teal;
        private StringBuilder BuilderString;

        public ReplyMessageBuilder(string initial)
        {
            BuilderString = new StringBuilder(initial);
        }

        public ReplyMessageBuilder() : this(string.Empty)
        {
        }

        public ReplyMessageBuilder Append(string value)
        {
            SafeAppend(value);
            return this;
        }

        public ReplyMessageBuilder AppendLine(string value)
        {
            SafeAppend(value + _newLine);
            return this;
        }

        public ReplyMessageBuilder AppendLine() => AppendLine(string.Empty);

        public ReplyMessageBuilder AppendEmphasized(string value)
        {
            SafeAppend(GenerateTag("span") + value + GenerateTag("span", true));
            return this;
        }

        public ReplyMessageBuilder AppendStrongEmphasized(string value)
        {
            SafeAppend(GenerateTag("strong") + value + GenerateTag("strong", true));
            return this;
        }

        public ReplyMessageBuilder AppendTitle(string value)
        {
            SafeAppend(GenerateTag("h1") + value + GenerateTag("h1", true));
            return this;
        }

        public ReplyMessageBuilder AppendSubtitle(string value)
        {
            SafeAppend(GenerateTag("h2") + value + GenerateTag("h2", true));
            return this;
        }

        public ReplyMessageBuilder EmphasizeText(string value, bool allOccurrences = false)
        {
            var tag = "span";
            if (!allOccurrences)
            {
                var openIndex = GetOpenIndex(value);
                var closeIndex = GetCloseIndex(value);

                SafeInsert(openIndex, closeIndex, tag, value);
            }
            else
            {
                var openIndexes = GetAllOpenIndexes(value);
                var closeIndexes = GetAllCloseIndexes(value);
                var zipped = openIndexes.Zip(closeIndexes, Tuple.Create).ToList();
                var compensationLength = GenerateTag(tag).Length + GenerateTag(tag, true).Length;
                for (var i = 0; i < zipped.Count; i++)
                    SafeInsert(zipped[i].Item1 + (compensationLength * i), zipped[i].Item2 + (compensationLength * i), tag, value);
            }

            return this;
        }

        public ReplyMessageBuilder Title(string value)
        {
            Heading(value, "h1");
            return this;
        }

        public ReplyMessageBuilder Subtitle(string value)
        {
            Heading(value, "h2");
            return this;
        }

        public ReplyMessageBuilder AppendList(IEnumerable<string> list)
        {
            var builder = new StringBuilder("<ul>");

            foreach (var i in list)
            {
                builder.Append($"<li>{i}</li>");
            }

            builder.Append("</ul>");
            SafeAppend(builder.ToString());
            return this;
        }

        public ReplyMessageBuilder Clear()
        {
            BuilderString = new StringBuilder();
            return this;
        }

        public ReplyMessageBuilder AppendTable(int numberOfColumns, IEnumerable<string> body, IEnumerable<string> header = null, IEnumerable<string> footer = null)
        {
            SafeAppend(GenerateTag(@"table style=""border-collapse: collapse;"""));

            if (header != null)
            {
                SafeAppend(BuildTableRows(numberOfColumns, "thead", header));
            }

            SafeAppend(BuildTableRows(numberOfColumns, "tbody", body));

            if (footer != null)
            {
                SafeAppend(BuildTableRows(numberOfColumns, "tfoot", footer));
            }
            SafeAppend(GenerateTag("table", true));
            return this;
        }

        public ReplyMessageBuilder AppendTableHeader(params string[] headerList)
        {
            if (!headerList.Any()) return this;
            SafeAppend(GenerateTag("thead") + GenerateTag("tr"));
            for (var counter = 0; counter < headerList.Length; counter++)
            {
                SafeAppend(GenerateTag("th") + headerList[counter] + GenerateTag("th", true));
            }
            SafeAppend(GenerateTag("tr", true) + GenerateTag("thead", true));
            return this;
        }

        public ReplyMessageBuilder AppendTableFooter(params string[] footerList)
        {
            if (!footerList.Any()) return this;
            SafeAppend(GenerateTag("tfoot") + GenerateTag("tr"));
            for (var counter = 0; counter < footerList.Length; counter++)
            {
                SafeAppend(GenerateTag("th") + footerList[counter] + GenerateTag("th", true));
            }
            SafeAppend(GenerateTag("tr", true) + GenerateTag("tfoot", true));
            return this;
        }

        public ReplyMessageBuilder AppendTableRow(params string[] rowList)
        {
            if (!rowList.Any()) return this;
            SafeAppend(GenerateTag("tr"));
            for (var counter = 0; counter < rowList.Length; counter++)
            {
                SafeAppend(GenerateTag("td") + rowList[counter] + GenerateTag("td", true));
            }
            SafeAppend(GenerateTag("tr", true));
            return this;
        }

        public ReplyMessageBuilder AppendTableBody(string value)
        {
            SafeAppend(GenerateTag("tbody") + value + GenerateTag("tbody", true));
            return this;
        }

        public ReplyMessageBuilder AppendMailToLink(string email, string linkText = null)
        {
            if (linkText == null) linkText = email;
            SafeAppend($"<a href = \"mailto:{email}\" target = \"_top\">{linkText}" + GenerateTag("a", true));
            return this;
        }

        public ReplyMessageBuilder AppendLink(string url, string linkText = null)
        {
            if (string.IsNullOrEmpty(linkText)) linkText = url;
            SafeAppend($"<a href = \"{url}\" target = \"_top\">{linkText}" + GenerateTag("a", true));
            return this;
        }

        public override string ToString()
        {
            return BuilderString.ToString();
        }

        private void SafeAppend(string value)
        {
            BuilderString.Append(value);
        }

        private void SafeInsert(int startTagInsert, int endTagInsert, string tag, string value)
        {
            if (startTagInsert == -1)
            {
                SafeAppend(GenerateTag(tag) + value + GenerateTag(tag, true));
                return;
            }

            var openTag = GenerateTag(tag);
            endTagInsert += openTag.Length + 1;
            BuilderString.Insert(startTagInsert, GenerateTag(tag));

            BuilderString.Insert(endTagInsert, GenerateTag(tag, true));
        }

        private void Heading(string value, string headerType)
        {
            var openIndex = GetOpenIndex(value);
            var closeIndex = GetCloseIndex(value);

            SafeInsert(openIndex, closeIndex, headerType, value);
        }

        private int GetOpenIndex(string value)
        {
            var rawString = BuilderString.ToString();
            if (!rawString.Contains(value)) return -1;

            var match = Regex.Match(rawString, $@"(?:<.+>{value}<\/\w+>)");

            return match.Success ?
                rawString.LastIndexOf("<", rawString.IndexOf(value, StringComparison.Ordinal), StringComparison.Ordinal)
                : rawString.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }

        private IEnumerable<int> GetAllOpenIndexes(string value)
        {
            var rawString = BuilderString.ToString();
            if (!rawString.Contains(value)) yield return -1;

            var matches = Regex.Matches(rawString, $@"(?:<.+>{value}<\/\w+>)");
            var nonHtmlMatches = Regex.Matches(rawString, matches.Count > 0 ? $@"(?:^|\s)(?:{value})" : $@"{value}");

            foreach (Match match in matches)
            {
                yield return match.Index;
            }

            foreach (Match match in nonHtmlMatches)
            {
                yield return match.Index;
            }
        }

        private int GetCloseIndex(string value)
        {
            var rawString = BuilderString.ToString();
            if (!rawString.Contains(value)) return -1;

            var match = Regex.Match(rawString, $@"(?:{value}<\/\w+>)");

            return match.Success ?
                rawString.IndexOf(">", rawString.IndexOf(value, StringComparison.Ordinal), StringComparison.Ordinal)
                : rawString.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) + value.Length - 1;
        }

        private IEnumerable<int> GetAllCloseIndexes(string value)
        {
            var rawString = BuilderString.ToString();
            if (!rawString.Contains(value)) yield return -1;

            var matches = Regex.Matches(rawString, $@"(?:<.+>{value}<\/\w+>)");
            var nonHtmlMatches = Regex.Matches(rawString, matches.Count > 0 ? $@"(?:^|\s)(?:{value})" : $@"{value}");

            foreach (Match match in matches)
            {
                yield return rawString.IndexOf(">", match.Index + 1, StringComparison.Ordinal) + value.Length;
            }

            foreach (Match match in nonHtmlMatches)
            {
                yield return rawString[match.Index] == ' ' ? match.Index + value.Length : match.Index + value.Length - 1;
            }


        }

        private string BuildTableRows(int numberOfColumns, string tag, IEnumerable<string> data)
        {
            var grey = "#9E9E9E";
            var paddingStyle = "style=\"margin: 10px;\"";

            var rowStart = GenerateTag($@"tr style=""border-bottom: 1px solid {grey}""");
            var rowEnd = GenerateTag("tr", true);

            var cellStart = tag.ToLowerInvariant() == "thead" ? GenerateTag($"th {paddingStyle}") : GenerateTag($"td {paddingStyle}");
            var cellEnd = tag.ToLowerInvariant() == "thead" ? GenerateTag("th", true) : GenerateTag("td", true);

            var tableBody = data.ToList();
            var builder = new StringBuilder();

            builder.Append(GenerateTag(tag));
            for (var i = 0; i < tableBody.Count; i++)
            {
                if (i % numberOfColumns == 0) builder.Append(rowStart);

                builder.Append($"{cellStart}{tableBody[i]}{cellEnd}");

                if (((i + 1) % numberOfColumns == 0 && i != 0) || i == tableBody.Count - 1) builder.Append(rowEnd);
            }
            builder.Append(GenerateTag(tag, true));

            return builder.ToString();
        }

        private string GenerateTag(string tag, bool endTag = false)
        {

            if (tag.Equals("span", StringComparison.CurrentCultureIgnoreCase) && !endTag)
                return $@"<span style=""color:{GetColorCode(defaultColor)}"">";

            return !endTag ? $"<{tag}>" : $"</{tag}>";
        }

        private static string GetColorCode(Color color)
        {
            switch (color)
            {
                case Color.Red:
                    return "#F44336";
                case Color.Blue:
                    return "#2962FF";
                case Color.Green:
                    return "#4CAF50";
                case Color.Yellow:
                    return "#FFEB3B";
                case Color.Purple:
                    return "#9C27B0";
                case Color.Teal:
                    return "#00897B";
                default:
                    return "#fff";
            }
        }

        public enum Color
        {
            Red,
            Blue,
            Teal,
            Green,
            Yellow,
            Purple,
            Black
        }
    }
}