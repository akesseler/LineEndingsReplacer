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

using Plexdata.Utilities.Attributes;

namespace Plexdata.LineEndingsReplacer.Defines
{
    internal enum LineEndingType
    {
        // Method "Enum.GetNames()" performs an unwanted 
        // sorting. Hence, bring all values in the wanted 
        // order by prepending an index.

        [Annotation(Display = "CR+LF", Remarks = "Carriage return plus line feed (\\r\\n).", Utilize = LineEndingType.CRLF, Visible = true)]
        CRLF = (1 << 16 | '\r' << 8 | '\n'),

        [Annotation(Display = "CR", Remarks = "Carriage return only (\\r).", Utilize = LineEndingType.CR, Visible = true)]
        CR = (2 << 16 | '\r'),

        [Annotation(Display = "LF", Remarks = "Line feed only (\\n).", Utilize = LineEndingType.LF, Visible = true)]
        LF = (3 << 16 | '\n'),

        [Annotation(Visible = false)]
        Default = CRLF,
    }
}
