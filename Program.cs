using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string method; // метод работы программы: запаковка или распаковка
                string pathOpen; // путь к файлу для открытия
                string pathSave; // путь к файлу для сохранения
                long begin = 0; // начальная позиция в открываемом файле, по умолчанию присваиваем 0
                if (args.Length >= 3 && args.Length <= 4) // если аргументов 3 или 4, то сохраняем значения в текущих переменных 
                {
                    method = args[0];
                    pathOpen = args[1];
                    pathSave = args[2];
                    if (args.Length == 4) // если аргументов 4, то 
                        begin = long.Parse(args[3]); // сохраняем 4 аргумент в переменой начальной позиции в открываемом файле
                }
                else // если аргументов не 3 и не 4, то создаем исключение
                    throw new ApplicationException("You entered incorrect count of arguments for packing/unpacking.");

                if (method == "p") // если метода работы программы - запаковка, то
                {
                    RLE.Pack(pathOpen, pathSave, begin); // вызываем соответсвующий метод статического класса RLE
                }
                else if (method == "u") // иначе если метод работы программы - распаковка, то 
                {
                    RLE.Unpack(pathOpen, pathSave, begin); // вызываем соответствующий метода статического класса RLE
                }
                else // если допущена ошибка в аргументе, отвечающем за метод работы программы, создаем исключение
                {
                    throw new ApplicationException("You entered incorrect method of the program.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


    }
}
