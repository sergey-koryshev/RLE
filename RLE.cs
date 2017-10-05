using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_program
{
    public class RLE
    {
        private string _pathOpen;
        private string _pathSave;
        private int _begin;

        public string PathOpen
        {
            get { return _pathOpen; }
            set { _pathOpen = value; }
        }
        public string PathSave
        {
            get { return _pathSave; }
            set { _pathSave = value; }
        }
        public int Begin
        {
            get { return _begin; }
            set { _begin = value; }
        }

        public RLE() : this("", "", 0) { }
        public RLE(string pathOpen, string pathSave) : this(pathOpen, pathSave, 0) { }
        public RLE(string pathOpen, string pathSave, int begin)
        {
            PathOpen = pathOpen;
            PathSave = pathSave;
            Begin = begin;
        }

        private void AddKeyByte(List<byte> packedArray, ref int keyIndex)
        {
            packedArray.Add(0);
            keyIndex = packedArray.Count - 1;
        }

        private List<byte> _pack(List<byte> unpackedArray)
        {
            List<byte> packedArray = new List<byte>();
            int i = 0;
            byte count = 0;
            int keyIndex = 0;
            AddKeyByte(packedArray, ref keyIndex);
            while (i < unpackedArray.Count)
            {
                count = 1;
                while (true)
                {
                    if (i + count > unpackedArray.Count - 1 || count >= 0x80)
                        break;
                    if (unpackedArray[i] == unpackedArray[i + count])
                    {
                        count++;
                    }
                    else
                        break;
                }
                if (count > 2)
                {
                    if (packedArray[keyIndex] > 0x80)
                        AddKeyByte(packedArray, ref keyIndex);
                    packedArray.Add(unpackedArray[i]);
                    packedArray[keyIndex] = count;
                    if (i + count < unpackedArray.Count - 1)
                        AddKeyByte(packedArray, ref keyIndex);
                }
                else
                {
                    count = 1;
                    if (((packedArray[keyIndex] + count) & 0x7F) > 0x7E)
                    {
                        AddKeyByte(packedArray, ref keyIndex);
                    }
                    packedArray.Add(unpackedArray[i]);
                    packedArray[keyIndex] = (packedArray[keyIndex] > 0x80) ? (byte)(packedArray[keyIndex] + count) : (byte)(0x80 + count);

                }
                i += count;
            }
            packedArray.Add(0xFF);
            return packedArray;
        }

        public void Pack()
        {
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(PathOpen, FileMode.Open));
                List<byte> unpackedArray = new List<byte>();
                while (fileOpen.PeekChar() > -1)
                {
                    unpackedArray.Add(fileOpen.ReadByte());
                }
                fileOpen.Close();

                List<byte> packedArray = _pack(unpackedArray);

                BinaryWriter fileSave = new BinaryWriter(File.Open(PathSave, FileMode.Create));
                foreach (var c in packedArray)
                {
                    fileSave.Write(c);
                }
                fileSave.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<byte> _unpack(List<byte> packedArray)
        {
            List<byte> unpackedArray = new List<byte>();
            int i = 0;
            int count = 0;
            while (packedArray[i] != 0xFF)
            {
                count = (packedArray[i] > 0x80) ? packedArray[i] - 0x80 : packedArray[i];
                i++;
                if (packedArray[i - 1] > 0x80)
                {
                    for (int j = 0; j < count; j++)
                    {
                        unpackedArray.Add(packedArray[i + j]);
                    }
                    i += count;
                }
                else
                {
                    for (int j = 0; j < count; j++)
                    {
                        unpackedArray.Add(packedArray[i]);
                    }
                    i++;
                }
            }
            return unpackedArray;
        }

        public void Unpack()
        {
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(PathOpen, FileMode.Open));
                fileOpen.BaseStream.Seek(Begin, SeekOrigin.Begin);
                List<byte> packedArray = new List<byte>();
                do
                {
                    packedArray.Add(fileOpen.ReadByte());
                } while (packedArray[packedArray.Count - 1] != 0xFF);
                fileOpen.Close();

                List<byte> unpackedArray = _unpack(packedArray);

                BinaryWriter fileSave = new BinaryWriter(File.Open(PathSave, FileMode.Create));
                foreach (var c in unpackedArray)
                {
                    fileSave.Write(c);
                }
                fileSave.Close();
            } catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
