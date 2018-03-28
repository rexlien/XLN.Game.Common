using System;
using System.IO;

namespace XLN.Game.Common.Utils
{
    public static class Varint
    {
        public static uint ReadVarint32(byte[] inByte)
        {
            uint result = 0;
            int shift = 0;
            int byteIndex = 0;
            while (true)
            {
                byte b = (byte)inByte[byteIndex++];
                result |= (uint)(b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }
            return result;
        }

        public static ulong ReadVarint64(byte[] inByte)
        {
            int shift = 0;
            ulong result = 0;
            int byteIndx = 0;
            while (true)
            {
                byte b = (byte)inByte[byteIndx++];
                result |= (ulong)(b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }

            return result;
        }

        public static uint ReadVarint32(BinaryReader reader)
        {
            uint result = 0;
            int shift = 0;
            while (true)
            {
                byte b = reader.ReadByte();
                result |= (uint)(b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }
            return result;
        }


        public static ulong ReadVarint64(BinaryReader reader)
        {
            int shift = 0;
            ulong result = 0;

            while (true)
            {
                byte b = reader.ReadByte();
                result |= (ulong)(b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }

            return result;
        }

        public static void WriteVarint16(BinaryWriter writer, UInt16 varint)
        {
            WriteVarint32(writer, varint);
        }

        public static void WriteVarint32(BinaryWriter writer, UInt32 varint)
        {
            while (true)
            {
                if ((varint & ~0x7F) == 0)
                {
                    //outByte[idx++] = (byte)varint;
                    writer.Write((byte)varint);
                    // WriteByteDirect((byte)n);
                    break;
                    // return;
                }
                else
                {
                    //outByte[idx++] = (byte)((varint & 0x7F) | 0x80);
                    // WriteByteDirect((byte)((n & 0x7F) | 0x80));
                    writer.Write((byte)((varint & 0x7F) | 0x80));
                    varint >>= 7;
                }
            }

            //trans.Write(i32buf, 0, idx);


        }
        public static void WriteVarint16(byte[] outByte, UInt16 varint)
        {
            WriteVarint32(outByte, varint);
        }

        public static void WriteVarint32(byte[] outByte, UInt32 varint)
        {
            int idx = 0;
            while (true)
            {
                if ((varint & ~0x7F) == 0)
                {
                    outByte[idx++] = (byte)varint;
                    break;
                    // return;
                }
                else
                {
                    outByte[idx++] = (byte)((varint & 0x7F) | 0x80);
                    varint >>= 7;
                }
            }

            //trans.Write(i32buf, 0, idx);

        }

        public static ulong longToZigzag(long n)
        {
            return (ulong)(n << 1) ^ (ulong)(n >> 63);
        }

        /**
         * Convert n into a zigzag int. This allows negative numbers to be
         * represented compactly as a varint.
         */
        public static uint intToZigZag(int n)
        {
            return (uint)(n << 1) ^ (uint)(n >> 31);
        }

        private static int zigzagToInt(uint n)
        {
            return (int)(n >> 1) ^ (-(int)(n & 1));
        }

        /**
         * Convert from zigzag long to long.
         */
        private static long zigzagToLong(ulong n)
        {
            return (long)(n >> 1) ^ (-(long)(n & 1));
        }



    }
}
