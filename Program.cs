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
            List<byte> myArray = new List<byte> { 0, 0, 0, 0, 4, 0, 5, 6, 4, 4, 4, 4, 5, 8, 8, 8, 9, 9, 9 };
            List<byte> outArray = Pack(myArray);
            foreach (var c in outArray)
            {
                Console.WriteLine(c);
            }
            Console.ReadLine();
        }

        private static void InsertElements(List<byte> packedArray, List<byte> unpackedArray, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                packedArray.Add(unpackedArray[index+i]);
            }
        }

        static List<byte> Pack(List<byte> unpackedArray)
        {
            List<byte> packedArray = new List<byte>();
            int i = 0;
            int count;
            bool match;

            while (i < unpackedArray.Count)
            {
                if (i + 1 > unpackedArray.Count - 1) //если в массиве осталось только два элемента
                {
                    InsertElements(packedArray, unpackedArray, i, 2); //то сжимать их не нужно, просто копируем
                    i += 2;
                }
                else //но если в исходном массиве осталось больше двух элементов, то
                {
                    count = 1; //пока у нас один элемент в повторяющейся последовательности
                    while (true) //цикл нужен для того, чтобы сосчитать сколько повторяющихся элементов у нас есть
                    {
                        if (i + count > unpackedArray.Count - 1 || count > 255)
                            break;
                        if (unpackedArray[i] == unpackedArray[i + count])
                        {
                            count++;
                        }
                        else //если повторяющихся элементов больше нет, то выходим из цикла
                        {
                            break;
                        }
                    }
                    if (count > 2) //если у нас последовательность потовряющихся элементов больше двух
                    {
                        packedArray.Add((byte)count); //то записываем их количество
                        packedArray.Add(unpackedArray[i]); //и записываем значение самого эелемента
                        i += count; //продвигаемся по массиву дальше на длинну повторяющейся последовательности
                    }
                    else
                    {
                        InsertElements(packedArray, unpackedArray, i, count);
                        i += count;
                    }
                }
            }
            return packedArray;
        }
    }
}
