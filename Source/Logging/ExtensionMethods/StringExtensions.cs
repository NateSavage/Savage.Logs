using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Savage.Logs {
    internal static class StringExtensions {

        internal static string WithoutWhiteSpace(this string @string) {
            return new string(@string.Where((char c) => c != ' ').ToArray());
        }
    }
}
