using System;
using System.Text;

namespace Savage.Logs {

    /// <summary> Color type for displaying <see cref="LogEntry"/> objects. </summary>
    public struct LoggingColor {

        public byte R, G, B, A;

        public LoggingColor(byte red, byte green, byte blue, byte alpha = 255) {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        /// <summary> Maps our three rgb 0-255 values into 4 bit <see cref="System.ConsoleColor"/> space. </summary>
        /// <remarks> 
        /// This method is a little tricky to work with, mapping RGB colors into 4 bit color sucks. <br/>
        /// I haven't been able to figure out a mapping that doesn't warp the input color space in some way. If anyone can come up with a better implementation for this function I'm open to pull requests.
        /// </remarks>
        /// <returns>
        /// <see cref="ConsoleColor.Black"/> when R G B are all below 32. <br/>
        /// <see cref="ConsoleColor.White"/> when R G B are all above 192. <br/>
        /// Color bit true for each color channel above 64.  <br/>
        /// Brightness bit true if any color channel is above 128.
        /// </returns>
        public ConsoleColor ToConsoleColor() {
            // the bits are in big endian order and represent brightness, red, green, blue.
            int index;
            index  = (R > 64) ? 4 : 0; // red bit
            index |= (G > 64) ? 2 : 0; // green bit
            index |= (B > 64) ? 1 : 0; // blue bit

            // all colors low = black
            if (R < 32 && B < 32 && G < 32) 
                return System.ConsoleColor.Black;

            // all colors high = white
            else if (R > 192 && B > 192 && G > 192) 
                return System.ConsoleColor.White;

            // bright bit
            index |= (R > 128 | G > 128 | B > 128) ? 8 : 0; 

            return ConsoleColorLookup[index];
        }

        /// <summary> Lookup table converting 4 bit brgb value to <see cref="ConsoleColor"/>. </summary>
        /// <remarks>
        /// Black and white have been removed from this table and need to be handled seperately because in practice 1000 and 0111 are almost impossible map from rgb space. <br/>
        /// (high brightness but no colors, and no brightness but all colors)
        /// </remarks>
        static ConsoleColor[] ConsoleColorLookup = {
            ConsoleColor.DarkGray,    // 0000 0
            ConsoleColor.DarkBlue,    // 0001 1
            ConsoleColor.DarkGreen,   // 0010 2
            ConsoleColor.DarkCyan,    // 0011 3
            ConsoleColor.DarkRed,     // 0100 4
            ConsoleColor.DarkMagenta, // 0101 5
            ConsoleColor.DarkYellow,  // 0110 6
            ConsoleColor.DarkGray,    // 0111 7 
            ConsoleColor.Gray,        // 1000 8
            ConsoleColor.Blue,        // 1001 9
            ConsoleColor.Green,       // 1010 10
            ConsoleColor.Cyan,        // 1011 11
            ConsoleColor.Red,         // 1100 12
            ConsoleColor.Magenta,     // 1101 13
            ConsoleColor.Yellow,      // 1110 14
            ConsoleColor.Gray,        // 1111 15
        };

        // what the heck did I write this for?
        private string byteToBitsString(byte byteIn) {
            var bitsString = new StringBuilder(8);

            bitsString.Append(Convert.ToString((byteIn / 8) % 2));
            bitsString.Append(Convert.ToString((byteIn / 4) % 2));
            bitsString.Append(Convert.ToString((byteIn / 2) % 2));
            bitsString.Append(Convert.ToString((byteIn / 1) % 2));

            return bitsString.ToString();
        }

        /// <returns> A hex code formatted like #0F0F0F </returns>
        public string ToHexRGB() => ToHex(new byte[] { R, G, B });

        /// <returns> A hex code formatted like #FF0F0F0F </returns>
        public string ToHexARGB() => ToHex(new byte[] { A, R, G, B });

        private string ToHex(byte[] bytes) {
            char[] characters = new char[bytes.Length * 2 + 1];
            characters[0] = '#';
            byte @byte;

            for (int byteIndex = 0, characterIndex = 1; byteIndex < bytes.Length; ++byteIndex, ++characterIndex) {
                // set the first character in the two byte hex pair
                @byte = (byte)(bytes[byteIndex] >> 4);
                characters[characterIndex] = (char)(@byte > 9 ? @byte + 0x37 + 0x20 : @byte + 0x30);

                // set the second character in the two byte hex pair
                @byte = (byte)(bytes[byteIndex] & 0x0F);
                characters[++characterIndex]=(char)(@byte > 9 ? @byte + 0x37 + 0x20 : @byte + 0x30);
            }

            return new string(characters);
        }

        /// <remarks> Formatted in ARGB order. </remarks>
        public override string ToString() => $"({A}, {R}, {G}, {B})";
    }
}
