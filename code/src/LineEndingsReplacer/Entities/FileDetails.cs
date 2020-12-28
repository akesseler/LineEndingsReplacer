/*
 * MIT License
 * 
 * Copyright (c) 2020 plexdata.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Linq;
using System.Text;

namespace Plexdata.LineEndingsReplacer.Entities
{
    internal class FileDetails
    {
        public FileDetails(String name, Int32 length)
            : this(name, length, false)
        {
        }

        public FileDetails(String name, Int32 length, Boolean isBigEndian)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            if (length != 1 && length != 2 && length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this.Name = name;
            this.Length = length;
            this.IsBigEndian = isBigEndian;

            // UTF-8         : '\r' = 0d          | '\n' = 0a
            // UTF-8-BOM     : '\r' = 0d          | '\n' = 0a
            // UTF-16-BOM-LE : '\r' = 0d 00       | '\n' = 0a 00
            // UTF-16-BOM-BE : '\r' = 00 0d       | '\n' = 00 0a
            // UTF-32-BOM-LE : '\r' = 0d 00 00 00 | '\n' = 0a 00 00 00
            // UTF-32-BOM-BE : '\r' = 00 00 00 0d | '\n' = 00 00 00 0a

            this.CarriageReturn = new Byte[this.Length];
            this.CarriageReturn[this.IsBigEndian ? this.Length - 1 : 0] = 0x0D;

            this.LineFeed = new Byte[this.Length];
            this.LineFeed[this.IsBigEndian ? this.Length - 1 : 0] = 0x0A;
        }

        /// <summary>
        /// The name that might be used as display text.
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// The number of bytes to read per character.
        /// </summary>
        public Int32 Length { get; }

        /// <summary>
        /// True if big endian byte order is used and false otherwise.
        /// </summary>
        public Boolean IsBigEndian { get; }

        /// <summary>
        /// Gets the carriage return representation of currently chosen settings.
        /// </summary>
        public Byte[] CarriageReturn { get; }

        /// <summary>
        /// Gets the line feed representation of currently chosen settings.
        /// </summary>
        public Byte[] LineFeed { get; }

        public Byte[] GetLineEnding(String ending)
        {
            if (ending.Length == 1)
            {
                if (ending[0] == '\r')
                {
                    return this.CarriageReturn;
                }
                else if (ending[0] == '\n')
                {
                    return this.LineFeed;
                }
            }
            else if (ending.Length == 2 && ending[0] == '\r' && ending[1] == '\n')
            {
                return Enumerable.Concat(this.CarriageReturn, this.LineFeed).ToArray();
            }

            throw new ArgumentOutOfRangeException(nameof(ending));
        }

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder(128);

            builder.AppendFormat(
                "Name: \"{0}\"; Length: {1}; Endianness: \"{2}\"; CR: [{3}]; LF: [{4}]",
                this.Name, this.Length, this.IsBigEndian ? "BE" : "LE",
                this.MakeString(this.CarriageReturn), this.MakeString(this.LineFeed));

            return builder.ToString();
        }

        private String MakeString(Byte[] value)
        {
            return String.Join(",", value.Select(x => $"0x{x:X2}"));
        }
    }
}
