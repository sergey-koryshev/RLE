using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_program
{
    public static class RLE
    {
        private static void AddKeyByte(List<byte> packedArray, ref int keyIndex)
        {
            packedArray.Add(0);
            keyIndex = packedArray.Count - 1;
        }

        private static List<byte> _pack(List<byte> unpackedArray)
        {
            List<byte> packedArray = new List<byte>();
            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return packedArray;
        }

        public static void Pack(string pathOpen, string pathSave, long begin)
        {
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(pathOpen, FileMode.Open));
                fileOpen.BaseStream.Seek(begin, SeekOrigin.Begin);
                List<byte> unpackedArray = new List<byte>();
                while (fileOpen.BaseStream.Position < fileOpen.BaseStream.Length)
                {
                    unpackedArray.Add(fileOpen.ReadByte());
                }
                fileOpen.Close();

                List<byte> packedArray = _pack(unpackedArray);

                BinaryWriter fileSave = new BinaryWriter(File.Open(pathSave, FileMode.Create));
                foreach (var c in packedArray)
                {
                    fileSave.Write(c);
                }
                fileSave.Close();

                Console.WriteLine("Compressing was success! Source data takes {0} bytes, compressed data takes {1} bytes.", unpackedArray.Count, packedArray.Count);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static List<byte> _unpack(string pathOpen, long begin, out long packedCount)
        {
            List<byte> unpackedArray = new List<byte>();
            packedCount = 0;
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(pathOpen, FileMode.Open));
                fileOpen.BaseStream.Seek(begin, SeekOrigin.Begin);
                
                byte count = 0;
                byte value = 0;
                do
                {
                    count = fileOpen.ReadByte();
                    if (count != 0xFF)
                    {
                        if (count > 0x80)
                        {
                            count -= 0x80;
                            for (int j = 0; j < count; j++)
                            {
                                unpackedArray.Add(fileOpen.ReadByte());
                            }
                            packedCount += count + 1;
                        }
                        else
                        {
                            value = fileOpen.ReadByte();
                            for (int j = 0; j < count; j++)
                            {
                                unpackedArray.Add(value);
                            }
                            packedCount += 2;
                        }
                    }
                } while (count != 0xFF);
                fileOpen.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return unpackedArray;
        }

        public static void Unpack(string pathOpen, string pathSave, long begin)
        {
            try
            {
                long packedCount;
                List<byte> unpackedArray = _unpack(pathOpen, begin, out packedCount);

                BinaryWriter fileSave = new BinaryWriter(File.Open(pathSave, FileMode.Create));
                foreach (var c in unpackedArray)
                {
                    fileSave.Write(c);
                }
                fileSave.Close();

                Console.WriteLine("Uncompressing was success! Source data takes {0} bytes, uncompressed data takes {1} bytes.", packedCount, unpackedArray.Count);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
