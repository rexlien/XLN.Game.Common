using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace XLN.Game.Common.Thrift
{
    public class THeaderProtocol : TProtocol
    {

        public enum PROTOCOL_TYPES
        {
            T_BINARY_PROTOCOL = 0,
            T_JSON_PROTOCOL = 1,
            T_COMPACT_PROTOCOL = 2,
            T_DEBUG_PROTOCOL = 3,
            T_VIRTUAL_PROTOCOL = 4,
            T_SIMPLE_JSON_PROTOCOL = 5,
            T_FROZEN2_PROTOCOL = 6,
        };

        THeaderTransport m_HeaderTransport;

        public THeaderProtocol(THeaderTransport transport, PROTOCOL_TYPES protocolType = PROTOCOL_TYPES.T_BINARY_PROTOCOL)
            :base(transport)
        {
            transport.ProtocolID = (short)protocolType;
            m_HeaderTransport = transport;
            ResetProtcol(protocolType);
        }

        private void ResetProtcol(PROTOCOL_TYPES protocolID)
        {
            switch(protocolID)
            {
                case PROTOCOL_TYPES.T_BINARY_PROTOCOL:
                    m_Protocol = new TBinaryProtocol(Transport);
                    break;
                case PROTOCOL_TYPES.T_COMPACT_PROTOCOL:
                    m_Protocol = new TCompactProtocol(Transport);
                    break;
                default:
                    throw new TProtocolException("protocol not supported");
            }
        }

        public override byte[] ReadBinary()
        {
            return m_Protocol.ReadBinary();
        }

        public override bool ReadBool()
        {
            return m_Protocol.ReadBool();
        }

        public override sbyte ReadByte()
        {
            return m_Protocol.ReadByte();
        }

        public override double ReadDouble()
        {
            return m_Protocol.ReadDouble();
        }

        public override TField ReadFieldBegin()
        {
            return m_Protocol.ReadFieldBegin();
        }

        public override void ReadFieldEnd()
        {
            m_Protocol.ReadFieldEnd();
        }

        public override short ReadI16()
        {
            return m_Protocol.ReadI16();
        }

        public override int ReadI32()
        {
            return m_Protocol.ReadI32();
        }

        public override long ReadI64()
        {
            return m_Protocol.ReadI64();
        }

        public override TList ReadListBegin()
        {
            return m_Protocol.ReadListBegin();
        }

        public override void ReadListEnd()
        {
            m_Protocol.ReadListEnd();
        }

        public override TMap ReadMapBegin()
        {
            return m_Protocol.ReadMapBegin();
        }

        public override void ReadMapEnd()
        {
            m_Protocol.ReadMapEnd();
        }

        public override TMessage ReadMessageBegin()
        {
            m_HeaderTransport.BeginReadMessage();
            return m_Protocol.ReadMessageBegin();
        }

        public override void ReadMessageEnd()
        {
            m_Protocol.ReadMessageEnd();
            m_HeaderTransport.EndReadMessage();
        }

        public override TSet ReadSetBegin()
        {
            return m_Protocol.ReadSetBegin();
        }

        public override void ReadSetEnd()
        {
            m_Protocol.ReadSetEnd();
        }

        public override TStruct ReadStructBegin()
        {
            return m_Protocol.ReadStructBegin();
        }

        public override void ReadStructEnd()
        {
            m_Protocol.ReadStructEnd();
        }

        public override void WriteBinary(byte[] b)
        {
            m_Protocol.WriteBinary(b);
        }

        public override void WriteBool(bool b)
        {
            m_Protocol.WriteBool(b);
        }

        public override void WriteByte(sbyte b)
        {
            m_Protocol.WriteByte(b);
        }

        public override void WriteDouble(double d)
        {
            m_Protocol.WriteDouble(d);
        }

        public override void WriteFieldBegin(TField field)
        {
            m_Protocol.WriteFieldBegin(field);
        }

        public override void WriteFieldEnd()
        {
            m_Protocol.WriteFieldEnd();
        }

        public override void WriteFieldStop()
        {
            m_Protocol.WriteFieldStop();
        }

        public override void WriteI16(short i16)
        {
            m_Protocol.WriteI16(i16);
        }

        public override void WriteI32(int i32)
        {
            m_Protocol.WriteI32(i32);
        }

        public override void WriteI64(long i64)
        {
            m_Protocol.WriteI64(i64);
        }

        public override void WriteListBegin(TList list)
        {
            m_Protocol.WriteListBegin(list);
        }

        public override void WriteListEnd()
        {
            m_Protocol.WriteListEnd();
        }

        public override void WriteMapBegin(TMap map)
        {
            m_Protocol.WriteMapBegin(map);
        }

        public override void WriteMapEnd()
        {
            m_Protocol.WriteMapEnd();
        }

        public override void WriteMessageBegin(TMessage message)
        {
            m_Protocol.WriteMessageBegin(message);
        }
        public override void WriteMessageEnd()
        {
            m_Protocol.WriteMapEnd();
        }

        public override void WriteSetBegin(TSet set)
        {
            m_Protocol.WriteSetBegin(set);
        }

        public override void WriteSetEnd()
        {
            m_Protocol.WriteSetEnd();
        }

        public override void WriteStructBegin(TStruct struc)
        {
            m_Protocol.WriteStructBegin(struc);
        }

        public override void WriteStructEnd()
        {
            m_Protocol.WriteStructEnd();
        }

        private TProtocol m_Protocol;
    }
}
