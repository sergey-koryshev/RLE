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
            List<byte> myArray = new List<byte> { 0, 0, 0, 0, 4, 0, 5, 6, 4, 4, 4, 4, 5, 8, 8, 8 };

            for (int i = 0; i < 255; i++)
            {
                myArray.Add(3);
            }

            for (int i = 0; i < 100; i++)
            {
                myArray.Add(1);
                myArray.Add(2);
            }

            List<byte> outArray = Pack(myArray);

            foreach (var c in outArray)
            {
                Console.Write("/{0:X}", c);
            }
            Console.ReadLine();
        }

        private static void InsertElements(List<byte> packedArray, List<byte> unpackedArray, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                packedArray.Add(unpackedArray[index + i]);
            }
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
            byte count; 
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
    }
}
