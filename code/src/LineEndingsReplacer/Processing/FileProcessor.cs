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
using Plexdata.LineEndingsReplacer.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Plexdata.LineEndingsReplacer.Processing
{
    internal class FileProcessor
    {
        public FileProcessor()
            : base()
        {
        }

        public WorkResult Process(CancellationToken token, FileInfo source, FileInfo target, String ending)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (String.IsNullOrEmpty(ending))
            {
                throw new ArgumentOutOfRangeException(nameof(ending));
            }

            FileDetails details = source.GetDetails();

            using (FileStream reader = source.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream writer = target.Open(FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    Debug.WriteLine($"Source: {source}, Target: {target}, Details: {details}");

                    return this.Process(token, details, reader, writer, ending);
                }
            }
        }

        public WorkResult Process(CancellationToken token, FileDetails details, Stream source, Stream target, String ending)
        {
            if (details is null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (String.IsNullOrEmpty(ending))
            {
                throw new ArgumentOutOfRangeException(nameof(ending));
            }

            if (source.Length % details.Length != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(details), $"Source stream length must be a multiple of {details.Length}.");
            }

            Byte[] le = details.GetLineEnding(ending);
            Byte[] cr = details.CarriageReturn;
            Byte[] lf = details.LineFeed;
            Byte[] bt = new Byte[details.Length];

            Int32 totalCount = 0;
            Int32 countCR = 0;
            Int32 countLF = 0;
            Int32 countCRLF = 0;

            while (source.Position < source.Length)
            {
                token.ThrowIfCancellationRequested();

                this.ReadByte(source, ref bt);

                // Check if current "byte" is either CR or LF.
                Boolean isCR = this.IsEqual(bt, cr);

                if (isCR || this.IsEqual(bt, lf))
                {
                    totalCount++;

                    if (isCR)
                    {
                        countCR++;
                    }
                    else
                    {
                        countLF++;
                    }

                    if (isCR && this.TryPeekByte(source, ref bt))
                    {
                        if (this.IsEqual(bt, lf))
                        {
                            countCR--;
                            countCRLF++;

                            // Skip LF if last "byte" was CR.
                            this.ReadByte(source, ref bt);
                        }
                    }

                    // Write new line ending.
                    this.WriteByte(target, le);
                }
                else
                {
                    this.WriteByte(target, bt);
                }
            }

            return new WorkResult(le, totalCount, countCR, countLF, countCRLF);
        }

        private Boolean IsEqual(Byte[] item1, Byte[] item2)
        {
            if (item1.Length != item2.Length)
            {
                // This should never happen because of the MOD check in method "Process()".
                throw new InvalidOperationException("Unable to compare two items with different length.");
            }

            for (Int32 index = 0; index < item1.Length; index++)
            {
                if (item1[index] != item2[index])
                {
                    return false;
                }
            }

            return true;
        }

        private void ReadByte(Stream stream, ref Byte[] result)
        {
            Int32 total = stream.Read(result, 0, result.Length);

            if (total != result.Length)
            {
                // This should never happen because of the MOD check in method "Process()".
                throw new InvalidOperationException("Number of totally byte read is different from number of expected bytes.");
            }
        }

        private Boolean TryPeekByte(Stream stream, ref Byte[] result)
        {
            if (stream.Position + result.Length <= stream.Length)
            {
                Int64 position = stream.Position;

                Int32 total = stream.Read(result, 0, result.Length);

                stream.Position = position;

                // This should never be different because of the MOD check in method "Process()".
                return total == result.Length;
            }

            return false;
        }

        private void WriteByte(Stream stream, Byte[] value)
        {
            stream.Write(value, 0, value.Length);
        }
    }
}
