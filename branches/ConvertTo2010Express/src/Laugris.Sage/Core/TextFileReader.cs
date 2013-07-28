using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Laugris.Sage
{
    public unsafe sealed class TextFileReader : IDisposable
    {
        private string fileName;
        private Encoding encoding;
        private int fileSize;
        private IntPtr nativeMemory;
        private byte* memBytePtr;
        private System.Text.Decoder decoder;
        private char charBuffer;
        private int charLen;
        private int charPos;

        public TextFileReader(string fileName, Encoding encoding)
        {
            this.fileName = fileName;
            this.encoding = encoding;
            fileSize = NativeMethods.FileGetSize(fileName);
            decoder = this.encoding.GetDecoder();
            if (this.encoding.IsSingleByte)
                charLen = 1;
            else
                charLen = 2;
            Load();
        }

        ~TextFileReader()
        {
            Dispose(false);
        }

        private unsafe void Load()
        {
            if (!FileOperations.FileExists(fileName))
                return;
            nativeMemory = NativeMethods.FileReadToBuffer(fileName);
            memBytePtr = (byte*)nativeMemory.ToPointer();
#if PRESSURE
            long pressure = InteropHelper.AlignToPage(fileSize);
            if (pressure != 0L)
            {
                GC.AddMemoryPressure(pressure);
            }
#endif
        }

        public int Size
        {
            get { return fileSize; }
        }

        public string ReadLine()
        {
            bool startLine = true;
            int rest = 0;
            if (charPos >= fileSize)
                return null;

            char* buffer = stackalloc char[1];

            StringBuilder builder = new StringBuilder();
            while (charPos < fileSize)
            {
                decoder.GetChars(memBytePtr + charPos, charLen, buffer, 1, false);
                charBuffer = *buffer;
                switch (charBuffer)
                {
                    case (char)65279:
                        charBuffer = ' ';
                        break;
                    case '\r':
                    case '\n':
                        if ((charBuffer == '\r') && (this.Peek() == '\n'))
                        {
                            charPos += charLen;
                        }
                        charPos += charLen;
                        rest = 0;
                        return builder.ToString();
                }

                if (!char.IsWhiteSpace(charBuffer))
                    startLine = false;
                if (!startLine)
                {
                    if (charBuffer == ' ')
                        rest++;
                    else
                    {
                        if (rest > 0)
                        {
                            builder.Append(' ', rest);
                            rest = 0;
                        }
                        builder.Append(charBuffer);
                    }
                }
                charPos += charLen;
            }
            return builder.ToString();
        }

        private char Peek()
        {
            if ((charPos + charLen * 2) < fileSize)
            {
                char* buffer = stackalloc char[1];
                decoder.GetChars(memBytePtr + charPos + charLen, charLen, buffer, 1, false);
                charBuffer = *buffer;
                return charBuffer;
            }
            else
                return (char)0;
        }


        public void Close()
        {
            Dispose();

        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            NativeMethods.FreeFileBuffer(nativeMemory);
#if PRESSURE
            long pressure = InteropHelper.AlignToPage(fileSize);
            if (pressure != 0L)
            {
                GC.RemoveMemoryPressure(pressure);
            }
#endif
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
