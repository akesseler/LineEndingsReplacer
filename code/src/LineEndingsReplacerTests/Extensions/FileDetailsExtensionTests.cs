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
using Plexdata.LineEndingsReplacer.Extensions;
using System;
using System.IO;
using System.Linq;

namespace LineEndingsReplacerTests.Extensions
{
    public class FileDetailsExtensionTests
    {
        [Test]
        public void GetDetails_FileInfoIsNull_ThrowsArgumentNullException()
        {
            FileInfo actual = null;

            Assert.That(() => actual.GetDetails(), Throws.ArgumentNullException);
        }

        [Test]
        public void GetDetails_StreamIsNull_ThrowsArgumentNullException()
        {
            Stream actual = null;

            Assert.That(() => actual.GetDetails(), Throws.ArgumentNullException);
        }

        [TestCase("UTF-8", new Byte[0])]
        [TestCase("UTF-8", new Byte[] { 0xEF })]
        [TestCase("UTF-8", new Byte[] { 0xEF, 0xBB })]
        [TestCase("UTF-8", new Byte[] { 0x30, 0x31, 0x32, 0x33 })]
        [TestCase("UTF-8 with BOM", new Byte[] { 0xEF, 0xBB, 0xBF })]
        [TestCase("UTF-8 with BOM", new Byte[] { 0xEF, 0xBB, 0xBF, 0x30, 0x31, 0x32, 0x33 })]
        public void GetDetails_StreamWithUtf8Data_ResultWithUtf8Details(String name, Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);

            FileDetails actual = stream.GetDetails();

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual.IsBigEndian, Is.False);
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, new Byte[] { 0x0D }), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, new Byte[] { 0x0A }), Is.True);
        }

        [TestCase("UTF-16 (LE) with BOM", new Byte[] { 0xFF, 0xFE })]
        [TestCase("UTF-16 (LE) with BOM", new Byte[] { 0xFF, 0xFE, 0x30, 0x00, 0x31, 0x00, 0x32, 0x00, 0x33, 0x00 })]
        public void GetDetails_StreamWithUtf16LeData_ResultWithUtf16LeDetails(String name, Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);

            FileDetails actual = stream.GetDetails();

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(2));
            Assert.That(actual.IsBigEndian, Is.False);
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, new Byte[] { 0x0D, 0x00 }), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, new Byte[] { 0x0A, 0x00 }), Is.True);
        }

        [TestCase("UTF-16 (BE) with BOM", new Byte[] { 0xFE, 0xFF })]
        [TestCase("UTF-16 (BE) with BOM", new Byte[] { 0xFE, 0xFF, 0x00, 0x30, 0x00, 0x31, 0x00, 0x32, 0x00, 0x33 })]
        public void GetDetails_StreamWithUtf16BeData_ResultWithUtf16BeDetails(String name, Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);

            FileDetails actual = stream.GetDetails();

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(2));
            Assert.That(actual.IsBigEndian, Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, new Byte[] { 0x00, 0x0D }), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, new Byte[] { 0x00, 0x0A }), Is.True);
        }

        [TestCase("UTF-32 (LE) with BOM", new Byte[] { 0xFF, 0xFE, 0x00, 0x00 })]
        [TestCase("UTF-32 (LE) with BOM", new Byte[] { 0xFF, 0xFE, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00 })]
        public void GetDetails_StreamWithUtf32LeData_ResultWithUtf32LeDetails(String name, Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);

            FileDetails actual = stream.GetDetails();

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(4));
            Assert.That(actual.IsBigEndian, Is.False);
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, new Byte[] { 0x0D, 0x00, 0x00, 0x00 }), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, new Byte[] { 0x0A, 0x00, 0x00, 0x00 }), Is.True);
        }

        [TestCase("UTF-32 (BE) with BOM", new Byte[] { 0x00, 0x00, 0xFE, 0xFF })]
        [TestCase("UTF-32 (BE) with BOM", new Byte[] { 0x00, 0x00, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x33 })]
        public void GetDetails_StreamWithUtf32BeData_ResultWithUtf32BeDetails(String name, Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);

            FileDetails actual = stream.GetDetails();

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.Length, Is.EqualTo(4));
            Assert.That(actual.IsBigEndian, Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.CarriageReturn, new Byte[] { 0x00, 0x00, 0x00, 0x0D }), Is.True);
            Assert.That(Enumerable.SequenceEqual(actual.LineFeed, new Byte[] { 0x00, 0x00, 0x00, 0x0A }), Is.True);
        }
    }
}
