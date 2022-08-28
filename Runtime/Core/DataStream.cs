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
        private byte[] buffer;
        private int offset;
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
            get
            {
                return offset;
            }
        }
        public void Release()
        {

        }

        public static DataStream Generate(int length)
        {
            throw new NotImplementedException();
        }

        public static DataStream Generate()
        {
            throw new NotImplementedException();
        }

        internal void Write(byte[] bytes, int v, int length)
        {
            throw new NotImplementedException();
        }

        internal void CopyTo(DataStream data)
        {
            throw new NotImplementedException();
        }
    }
}
