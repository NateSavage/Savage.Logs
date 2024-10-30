using System.Linq;

namespace Savage.Logs {
    internal static class StringExtensions {

        internal static string WithoutWhiteSpace(this string @string) {
            return new string(@string.Where((char c) => c != ' ').ToArray());
        }
    }
}
