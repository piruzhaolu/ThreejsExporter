using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Piruzhaolu.ThreejsEditor
{
    public class BinBundle
    {
        private struct BinFile
        {
            public int DataType;
            public byte[] Bytes;
        }
        
        private int _index = 0;
        
        private readonly Dictionary<int, BinFile> _datas = new Dictionary<int, BinFile>();

        public int Add(byte[] data, int dataType = 0)
        {
            while (_datas.ContainsKey(_index))
            {
                _index++;
            }
            _datas.Add(_index, new BinFile
            {
                DataType = dataType,
                Bytes = data
            });
            return _index++;
        }

        public int Add(int id, byte[] data, int dataType = 0)
        {
            _datas.Add(id, new BinFile
            {
                Bytes = data,
                DataType = dataType
            });
            return id;
        }


        public byte[] ToBytes()
        {
            if (_datas == null || _datas.Count == 0) return null;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(_datas.Count);
                    var offset = sizeof(int);
                    offset += sizeof(int) * 4 * _datas.Count;//4=(id,dataType,offset, length)
                    foreach (var kv in _datas)
                    {
                        bw.Write(kv.Key);
                        bw.Write(kv.Value.DataType);
                        bw.Write(offset);
                        bw.Write(kv.Value.Bytes.Length);
                        offset += kv.Value.Bytes.Length;
                    }
                    foreach (var kv in _datas)
                    {
                        bw.Write(kv.Value.Bytes);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}