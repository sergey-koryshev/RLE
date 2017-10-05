using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte> myArray = new List<byte> { 4, 0, 0x84, 4, 0, 5, 6, 4, 4, 0x81, 5, 3, 8, 0xFF };

            List<byte> outArray = Unpack(myArray);

            foreach (var c in outArray)
            {
                Console.Write("/{0:X}", c);
            }
            Console.ReadLine();
        }

        private static void AddKeyByte(List<byte> packedArray, ref int keyIndex)
        {
            packedArray.Add(0);
            keyIndex = packedArray.Count - 1;
        }

        static List<byte> Pack(List<byte> unpackedArray)
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
                    packedArray.Add(unpackedArray[i]);
                    if (packedArray[keyIndex] > 0x80)
                        AddKeyByte(packedArray, ref keyIndex);
                    else
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
            packedArray.Add(255);
            return packedArray;
        }

        static List<byte> Unpack(List<byte> packedArray)
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
                    for (int j = 0; j < count ; j++)
                    {
                        unpackedArray.Add(packedArray[i]);
                    }
                    i++;
                }
            }
            return unpackedArray;
        }
    }
}
