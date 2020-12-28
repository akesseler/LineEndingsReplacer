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

using Plexdata.LineEndingsReplacer.Entities;
using System;
using System.IO;

namespace Plexdata.LineEndingsReplacer.Extensions
{
    internal static class FileDetailsExtension
    {
        public static FileDetails GetDetails(this FileInfo source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (FileStream stream = source.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return stream.GetDetails();
            }
        }

        public static FileDetails GetDetails(this Stream source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // UTF-8         : NOTHING     => '\r' = 0d          | '\n' = 0a
            // UTF-8-BOM     : EF BB BF    => '\r' = 0d          | '\n' = 0a
            // UTF-16-BOM-LE : FF FE       => '\r' = 0d 00       | '\n' = 0a 00  
            // UTF-16-BOM-BE : FE FF       => '\r' = 00 0d       | '\n' = 00 0a  
            // UTF-32-BOM-LE : FF FE 00 00 => '\r' = 0d 00 00 00 | '\n' = 0a 00 00 00  
            // UTF-32-BOM-BE : 00 00 FE FF => '\r' = 00 00 00 0d | '\n' = 00 00 00 0a (guess)

            Byte[] buffer = new Byte[4];

            // Try read first four bytes.
            Int32 total = source.Read(buffer, 0, buffer.Length);

            if (total >= 3)
            {
                // Check for UTF-8 with BOM.
                if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                {
                    return new FileDetails("UTF-8 with BOM", 1);
                }
            }

            if (total >= 4)
            {
                // Check for UTF-32 (LE) with BOM.
                if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
                {
                    return new FileDetails("UTF-32 (LE) with BOM", 4);
                }

                // Check for UTF-32 (BE) with BOM.
                if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF)
                {
                    return new FileDetails("UTF-32 (BE) with BOM", 4, true);
                }
            }

            if (total >= 2)
            {
                // Check for UTF-16 (LE) with BOM.
                if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return new FileDetails("UTF-16 (LE) with BOM", 2);
                }

                // Check for UTF-16 (BE) with BOM.
                if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return new FileDetails("UTF-16 (BE) with BOM", 2, true);
                }
            }

            // Return with defaults if nothing else fits.
            return new FileDetails("UTF-8", 1);
        }
    }
}
