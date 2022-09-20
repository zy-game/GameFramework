using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    /// <summary>
    /// 内存数据流
    /// </summary>
    public sealed class DataStream : IRefrence
    {
        private static MemoryPool<byte> manager = MemoryPool<byte>.Shared;
        public static DataStream Empty = Generate(0);
        private byte[] buffer;
        public byte[] bytes
        {
            get
            {
                return buffer;
            }
        }
        public int length
        {
            get
            {
                return buffer.Length;
            }
        }

        public int position
        {
            get;
            private set;
        }
        public void Release()
        {
            position = 0;
        }

        public static DataStream Generate(byte[] bytes)
        {
            DataStream stream = Loader.Generate<DataStream>();
            stream.buffer = bytes;
            return stream;
        }

        public static DataStream Generate()
        {
            return Generate(4096);
        }

        public static DataStream Generate(int length)
        {
            DataStream stream = Loader.Generate<DataStream>();
            stream.EnsureSizeAndResize(length);
            return stream;
        }
        private void EnsureSizeAndResize(int length)
        {
            if (length < 0)
            {
                return;
            }

            if (length == 0)
            {
                length = 1;
            }
            if (buffer == null)
            {
                buffer = new byte[length];
                return;
            }

            if (length < buffer.Length - position)
            {
                return;
            }
            int newSize = checked(buffer.Length + length);
            Array.Resize(ref buffer, newSize);
        }

        public override string ToString()
        {
            return UTF8Encoding.UTF8.GetString(buffer, 0, position);
        }

        public DataStream Read(int offset)
        {
            return Read(offset, position - offset);
        }

        public DataStream Read(int offset, int length)
        {
            if (length <= 0)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            if (offset > position)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            DataStream stream = Generate(length);
            Array.Copy(buffer, offset, stream.buffer, position, length);
            return stream;
        }

        public void Write(DataStream stream)
        {
            Write(stream, 0);
        }

        public void Write(DataStream stream, int offset)
        {
            Write(stream, offset, stream.position);
        }

        public void Write(DataStream stream, int offset, int length)
        {
            Write(position, stream.buffer, offset, length);
        }
        public void Write(byte[] bytes)
        {
            Write(bytes, 0);
        }

        public void Write(byte[] bytes, int offset)
        {
            Write(bytes, offset, bytes.Length);
        }

        public void Write(byte[] bytes, int scrOffset, int length)
        {
            Write(position, bytes, scrOffset, length);
        }

        public void Write(int desOffset, byte[] bytes, int scrOffset, int length)
        {
            EnsureSizeAndResize(length);
            Array.Copy(bytes, scrOffset, buffer, desOffset, length);
        }

        public void Trim(int offset)
        {
            Trim(offset, position - offset);
        }

        public void Trim(int offset, int length)
        {
            if (offset > position || length <= 0)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            int endOffset = offset + length;
            Array.Copy(buffer, endOffset, buffer, offset, position - endOffset);
        }

        public byte ReadByte()
        {
            return GetByte(position++);
        }

        public byte GetByte(int offset)
        {
            if (offset + sizeof(byte) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return buffer[offset];
        }

        public void WriteByte(byte value)
        {
            SetByte(position++, value);
        }

        public void SetByte(int offset, byte value)
        {
            if (offset + sizeof(byte) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            buffer[offset] = value;
        }

        public bool GetBoolean(int offset)
        {
            if (offset + sizeof(byte) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return buffer[offset] == 1;
        }

        public bool ReadBoolean()
        {
            return GetBoolean(position++);
        }

        public void SetBoolean(int offset, bool value)
        {
            if (offset + sizeof(byte) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            buffer[offset] = (byte)(value ? 1 : 0);
        }

        public void WriteBoolean(bool value)
        {
            EnsureSizeAndResize(sizeof(byte));
            SetBoolean(position++, value);
        }

        public short GetShort(int offset)
        {
            if (offset + sizeof(short) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return unchecked((short)(buffer[offset] << 8 | buffer[offset + 1]));
        }

        public short ReadShort()
        {
            short result = GetShort(position);
            position += sizeof(short);
            return result;
        }

        public void SetShort(int offset, short value)
        {
            if (offset + sizeof(short) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            buffer[offset] = (byte)((short)value >> 8);
            buffer[offset + 1] = (byte)value;
        }

        public void WriteShort(short value)
        {
            EnsureSizeAndResize(sizeof(short));
            SetShort(position, value);
            position += sizeof(short);
        }

        public ushort ReadUShort()
        {
            ushort value = GetUShort(position);
            position += sizeof(ushort);
            return value;
        }

        public ushort GetUShort(int offset)
        {
            if (offset + sizeof(ushort) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return unchecked((ushort)(buffer[offset] << 8 | buffer[offset + 1]));
        }

        public void WriteUShort(ushort value)
        {
            EnsureSizeAndResize(sizeof(ushort));
            SetUShort(position, value);
            position += sizeof(ushort);
        }

        public void SetUShort(int offset, ushort value)
        {
            if (offset + sizeof(ushort) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            buffer[offset] = (byte)((ushort)value >> 8);
            buffer[offset + 1] = (byte)value;
        }

        public int ReadInt32()
        {
            int value = GetInt32(position);
            position += sizeof(int);
            return value;
        }

        public int GetInt32(int offset)
        {
            if (offset + sizeof(int) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return unchecked(
                 buffer[offset] << 24 |
                 buffer[offset + 1] << 16 |
                 buffer[offset + 2] << 8 |
                 buffer[offset + 3]);
        }

        public void WriteInt32(int value)
        {
            EnsureSizeAndResize(sizeof(int));
            SetInt32(position, value);
            position += sizeof(int);
        }

        public void SetInt32(int offset, int value)
        {
            if (offset + sizeof(int) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            unchecked
            {
                int unsignedValue = value;
                buffer[offset] = (byte)(unsignedValue >> 24);
                buffer[offset + 1] = (byte)(unsignedValue >> 16);
                buffer[offset + 2] = (byte)(unsignedValue >> 8);
                buffer[offset + 3] = (byte)unsignedValue;
            }
        }

        public uint ReadUInt32()
        {
            uint value = GetUInt32(position);
            position += sizeof(uint);
            return value;
        }

        public uint GetUInt32(int offset)
        {
            if (offset + sizeof(uint) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return (uint)unchecked(
                 buffer[offset] << 24 |
                 buffer[offset + 1] << 16 |
                 buffer[offset + 2] << 8 |
                 buffer[offset + 3]);
        }

        public void WriteUInt32(uint value)
        {
            EnsureSizeAndResize(sizeof(uint));
            SetUInt32(position, value);
            position += sizeof(uint);
        }

        public void SetUInt32(int offset, uint value)
        {
            if (offset + sizeof(uint) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            unchecked
            {
                uint unsignedValue = (uint)value;
                buffer[offset] = (byte)(unsignedValue >> 24);
                buffer[offset + 1] = (byte)(unsignedValue >> 16);
                buffer[offset + 2] = (byte)(unsignedValue >> 8);
                buffer[offset + 3] = (byte)unsignedValue;
            }
        }

        public long ReadLong()
        {
            long value = GetLong(position);
            position += sizeof(long);
            return value;
        }

        public long GetLong(int offset)
        {
            if (offset + sizeof(long) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return unchecked(
                    (long)buffer[offset] << 56 |
                    (long)buffer[offset + 1] << 48 |
                    (long)buffer[offset + 2] << 40 |
                    (long)buffer[offset + 3] << 32 |
                    (long)buffer[offset + 4] << 24 |
                    (long)buffer[offset + 5] << 16 |
                    (long)buffer[offset + 6] << 8 |
                    buffer[offset + 7]);
        }

        public void WriteLong(long value)
        {
            EnsureSizeAndResize(sizeof(long));
            SetLong(position, value);
            position += sizeof(long);
        }

        public void SetLong(int offset, long value)
        {
            unchecked
            {
                ulong unsignedValue = (ulong)value;
                buffer[offset] = (byte)unsignedValue;
                buffer[offset + 1] = (byte)(unsignedValue >> 8);
                buffer[offset + 2] = (byte)(unsignedValue >> 16);
                buffer[offset + 3] = (byte)(unsignedValue >> 24);
                buffer[offset + 4] = (byte)(unsignedValue >> 32);
                buffer[offset + 5] = (byte)(unsignedValue >> 40);
                buffer[offset + 6] = (byte)(unsignedValue >> 48);
                buffer[offset + 7] = (byte)(unsignedValue >> 56);
            }
        }

        public float ReadFloat()
        {
            float value = GetFloat(position);
            position += sizeof(float);
            return value;
        }

        public float GetFloat(int offset)
        {
            if (offset + sizeof(float) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return BitConverter.ToSingle(buffer, offset);
        }

        public void WriteFloat(float value)
        {
            EnsureSizeAndResize(sizeof(float));
            SetFloat(position, value);
            position += sizeof(float);
        }

        public void SetFloat(int offset, float value)
        {
            if (offset + sizeof(float) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            Write(offset, BitConverter.GetBytes(value), 0, sizeof(float));
        }

        public double ReadDouble()
        {
            double value = GetDouble(position);
            position += sizeof(double);
            return value;
        }

        public double GetDouble(int offset)
        {
            if (offset + sizeof(double) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return BitConverter.ToDouble(buffer, offset);
        }

        public void WriteDouble(double value)
        {
            EnsureSizeAndResize(sizeof(double));
            SetDouble(position, value);
            position += sizeof(double);
        }

        public void SetDouble(int offset, double value)
        {
            if (offset + sizeof(double) > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            Write(offset, BitConverter.GetBytes(value), 0, sizeof(double));
        }

        public string ReadString()
        {
            int length = ReadInt32();
            string value = GetString(position, length);
            position += length;
            return value;
        }

        public string GetString(int offset, int length)
        {
            if (offset + length > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            return Encoding.UTF8.GetString(buffer, offset, length);
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteInt32(bytes.Length);
            Write(bytes, 0, bytes.Length);
            position += bytes.Length;
        }

        public void SetString(int offset, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            if (offset + bytes.Length > buffer.Length)
            {
                throw GameFrameworkException.Generate<IndexOutOfRangeException>();
            }
            SetInt32(position, bytes.Length);
            Write(offset, bytes, 0, bytes.Length);
        }
    }
}
