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

using NUnit.Framework;
using Plexdata.LineEndingsReplacer.Entities;
using System;
using System.Linq;

namespace LineEndingsReplacerTests.Entities
{
    public class FileDetailsTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void FileDetails_NameLengthConstructionNameInvalid_ThrowsArgumentOutOfRangeException(String name)
        {
            Assert.That(() => new FileDetails(name, 1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(5)]
        public void FileDetails_NameLengthConstructionLengthInvalid_ThrowsArgumentOutOfRangeException(Int32 length)
        {
            Assert.That(() => new FileDetails("some-name", length), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void FileDetails_NameLengthEndianConstructionNameInvalid_ThrowsArgumentOutOfRangeException(String name)
        {
            Assert.That(() => new FileDetails(name, 1, false), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(5)]
        public void FileDetails_NameLengthEndianConstructionLengthInvalid_ThrowsArgumentOutOfRangeException(Int32 length)
        {
            Assert.That(() => new FileDetails("some-name", length, false), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase("some-name", 1, true, new Byte[] { 0x0D }, new Byte[] { 0x0A })]
        [TestCase("some-name", 2, true, new Byte[] { 0x00, 0x0D }, new Byte[] { 0x00, 0x0A })]
        [TestCase("some-name", 4, true, new Byte[] { 0x00, 0x00, 0x00, 0x0D }, new Byte[] { 0x00, 0x00, 0x00, 0x0A })]
        [TestCase("some-name", 1, false, new Byte[] { 0x0D }, new Byte[] { 0x0A })]
        [TestCase("some-name", 2, false, new Byte[] { 0x0D, 0x00 }, new Byte[] { 0x0A, 0x00 })]
        [TestCase("some-name", 4, false, new Byte[] { 0x0D, 0x00, 0x00, 0x00 }, new Byte[] { 0x0A, 0x00, 0x00, 0x00 })]
        public void FileDetails_NameLengthEndianConstructionValuesValid_PropertiesAsExpected(String name, Int32 length, Boolean endian, Byte[] cr, Byte[] lf)
        {
            FileDetails actual = new FileDetails(name, length, endian);

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(length));
            Assert.That(actual.IsBigEndian, Is.EqualTo(endian));
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, cr), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, lf), Is.True);
        }

        [TestCase("")]
        [TestCase("x")]
        [TestCase("xy")]
        [TestCase("\ry")]
        [TestCase("\r\nz")]
        public void GetLineEnding_EndingInvalid_ThrowsArgumentOutOfRangeException(String ending)
        {
            FileDetails actual = new FileDetails("some-name", 1, true);

            Assert.That(() => actual.GetLineEnding(ending), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase("some-name", 1, true, "\r", new Byte[] { 0x0D })]
        [TestCase("some-name", 2, true, "\r", new Byte[] { 0x00, 0x0D })]
        [TestCase("some-name", 4, true, "\r", new Byte[] { 0x00, 0x00, 0x00, 0x0D })]
        [TestCase("some-name", 1, false, "\r", new Byte[] { 0x0D })]
        [TestCase("some-name", 2, false, "\r", new Byte[] { 0x0D, 0x00 })]
        [TestCase("some-name", 4, false, "\r", new Byte[] { 0x0D, 0x00, 0x00, 0x00 })]
        [TestCase("some-name", 1, true, "\n", new Byte[] { 0x0A })]
        [TestCase("some-name", 2, true, "\n", new Byte[] { 0x00, 0x0A })]
        [TestCase("some-name", 4, true, "\n", new Byte[] { 0x00, 0x00, 0x00, 0x0A })]
        [TestCase("some-name", 1, false, "\n", new Byte[] { 0x0A })]
        [TestCase("some-name", 2, false, "\n", new Byte[] { 0x0A, 0x00 })]
        [TestCase("some-name", 4, false, "\n", new Byte[] { 0x0A, 0x00, 0x00, 0x00 })]
        [TestCase("some-name", 1, true, "\r\n", new Byte[] { 0x0D, 0x0A })]
        [TestCase("some-name", 2, true, "\r\n", new Byte[] { 0x00, 0x0D, 0x00, 0x0A })]
        [TestCase("some-name", 4, true, "\r\n", new Byte[] { 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0A })]
        [TestCase("some-name", 1, false, "\r\n", new Byte[] { 0x0D, 0x0A })]
        [TestCase("some-name", 2, false, "\r\n", new Byte[] { 0x0D, 0x00, 0x0A, 0x00 })]
        [TestCase("some-name", 4, false, "\r\n", new Byte[] { 0x0D, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00 })]
        public void GetLineEnding_ValuesAndEndingValid_ResultAsExpected(String name, Int32 length, Boolean endian, String ending, Byte[] expected)
        {
            FileDetails actual = new FileDetails(name, length, endian);

            Assert.That(Enumerable.SequenceEqual(actual.GetLineEnding(ending), expected), Is.True);
        }
    }
}
