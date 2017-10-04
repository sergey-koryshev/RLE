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
                packedArray.Add(unpackedArray[index + i]);
            }
        }

        static List<byte> Pack(List<byte> unpackedArray)
        {
            List<byte> packedArray = new List<byte>(); // массив запакованных данных
            int i = 0; // позиция в "незапакованном" массиве
            byte count; // количество "повторяющихся" элементов
            int keyIndex = 0; // индекс ключевого байта
            packedArray.Add(0); // добавляем первый ключевой байт
            while (i < unpackedArray.Count)
            {
                count = 1; //пока у нас один элемент в "повторяющейся" последовательности
                while (true) //цикл, чтобы сосчитать сколько повторяющихся элементов у нас есть
                {
                    if (i + count > unpackedArray.Count - 1 || count >= 0x80) // необходимые условия на переполнение и на все остальное 
                        break;
                    if (unpackedArray[i] == unpackedArray[i + count])
                    {
                        count++;
                    }
                    else //если повторяющихся элементов больше нет, то выходим из цикла
                        break;
                }
                if (count > 2) //если у нас последовательность потовряющихся элементов больше двух
                {
                    packedArray.Add(count); //то записываем их количество
                    packedArray.Add(unpackedArray[i]); //и записываем значение самого эелемента
                    packedArray[keyIndex] = count;
                    packedArray.Add(0);
                    keyIndex = packedArray.Count - 1;
                }
                else
                {
                    InsertElements(packedArray, unpackedArray, i, count); // если в последовательности два элемента и меньше, то просто последовательно записываем
                }
                i += count; //продвигаемся по массиву дальше на длинну повторяющейся последовательности
            }
            packedArray.Add(255);
            return packedArray;
        }
    }
}
