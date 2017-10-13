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
            /* Вспомогательный метод для метода _pack.
             * Выполняет функцию добавления нового ключевого байтв в запакованную последовательность.
             * Принимает:
             * - packedArray - последовательность запакованных данных
             * - keyIndex - индекс текущего ключевого байта
             * Возвращает:
             * - keyIndex - индекс текущего ключевого байта */
            packedArray.Add(0); // добавляем в массив новый элемент
            keyIndex = packedArray.Count - 1; // сохраняем индекс нового ключевого байта
        }

        private static List<byte> _pack(List<byte> unpackedArray)
        {
            /* Основной метод для запаковки последовательности.
             * Запаковывает последовательность по алгоритму RLE. Описание алгоритма смотрите в файле README.md.
             * Принимает: 
             * - unpackedArray - последовательность незапакованных данных
             * Возвращает:
             * - последовательность запакованных данных
             * Дата написания метода:
             * - октябрь 2017 */
            List<byte> packedArray = new List<byte>(); // запакованне данные
            try
            {
                int i = 0; // счетчик для основного цикла
                byte count = 0; // количество повторений
                int keyIndex = 0; // индекс ключевого байта
                AddKeyByte(packedArray, ref keyIndex); // в начале добавляем первый ключевой байт
                while (i < unpackedArray.Count) // цикл идет пока счетчик меньше длины исходной последовательности
                {
                    count = 1; // присваиваем переменной начальное значение равное 1
                    while (true) // в этом цикле считаем количество одинаковых подряд идущих байтов (дальше просто одинаковых)
                    {
                        if (i + count > unpackedArray.Count - 1 || count > 0x80) // если мы вышли за пределы исходной последовтельности или длина одинаковых байтов больше $80, то 
                            break; // выходим из цикла
                        if (unpackedArray[i] == unpackedArray[i + count]) // если текущий байт равен следующему, то
                        {
                            count++; // увеличиваем длину одинаковых байтов на 1
                        }
                        else // если текущий байт не равен следующему, то
                            break; // выходим из цикла
                    }
                    if (count > 2) // если длина последовательности одинаковых байтов больше двух, то
                    {
                        if (packedArray[keyIndex] > 0x80) // проверяем превышает ли текущий ключевой байт значение $80, и если больше, то
                            AddKeyByte(packedArray, ref keyIndex); // значит до этого запаковывалась последовательность неодинаковых байтов, поэтому добавляем новый ключевой байт
                        packedArray.Add(unpackedArray[i]); // добавляем в последовательность запакованных данных повторяющийся байт
                        packedArray[keyIndex] = count; // и добавляем количество повторений
                        if (i + count < unpackedArray.Count - 1) // если после сдвига текущей позиции в исходной последовательности на количество одинаковых байт мы не превысим ее длину, то
                            AddKeyByte(packedArray, ref keyIndex); // добавляем в запакованную последовательность новый ключевой байт
                    }
                    else // если длина последовательности одинаковых байтов меньше или равна двум, то
                    {
                        count = 1; // в этом случае байты лучше будем добавлять по одному
                        if (((packedArray[keyIndex] + count) & 0x7F) > 0x7E) // проверяем не превышает последовательность неодинаковых данных максимальную длину в $7E байтов и если да, то 
                        {
                            AddKeyByte(packedArray, ref keyIndex); // добавляем новый ключевой байт
                        }
                        packedArray.Add(unpackedArray[i]); // добавляем байт из исходной последовательности в последовательность запакованных данных и
                        packedArray[keyIndex] = (packedArray[keyIndex] > 0x80) ? (byte)(packedArray[keyIndex] + count) : (byte)(0x80 + count); // увеличиваем ключевой байт на единицу, т.к. count вышле было присвоено значение 1
                    }
                    i += count; // продвигаемся в исходной последовательности на длину, равную количеству повторений одинаковых байтов
                }
                packedArray.Add(0xFF); // в конце последовательности запакованных данных добавляем стоп-байт $FF
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return packedArray;
        }

        public static void Pack(string pathOpen, string pathSave, long begin)
        {
            /* Открытый метод, который отвечает за открытие и сохранение файлов.
             * Принимает:
             * - pathOpen - путь к файлу с незапакованной последовательностью
             * - PathSave - путь к файлу, где будет сохранена запакованная последовательность
             * - begin - позиция в файле с незапакованными данными */
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(pathOpen, FileMode.Open)); 
                fileOpen.BaseStream.Seek(begin, SeekOrigin.Begin); // устанавливаем позицию в файле
                List<byte> unpackedArray = new List<byte>(); 
                while (fileOpen.BaseStream.Position < fileOpen.BaseStream.Length) // в этом цикле считываем незапакованные данные из файла
                {
                    unpackedArray.Add(fileOpen.ReadByte());
                }
                fileOpen.Close();

                List<byte> packedArray = _pack(unpackedArray); // пытаемся выполнить процедуру запаковки данных

                BinaryWriter fileSave = new BinaryWriter(File.Open(pathSave, FileMode.Create));
                foreach (var c in packedArray) // если не возникло никаких ошибок, то сохраняем запакованную последовательность в файле
                {
                    fileSave.Write(c);
                }
                fileSave.Close();

                Console.WriteLine("Compressing was success! Source data takes {0} bytes, compressed data takes {1} bytes.", unpackedArray.Count, packedArray.Count); // информация для оценки: размер незапакованной и запакованной последовательностей

            }
            catch (Exception ex)
            {
                Console.WriteLine("Compressing failed.");
                Console.WriteLine(ex.Message);
            }
        }

        private static List<byte> _unpack(string pathOpen, long begin, out long packedCount)
        {
            /* Основной метод для распаковки данных.
             * Распаковывает данные по алгоритму RLE.
             * Принимает:
             * - pathOpen - путь к файлу с запакованными данными
             * - begin - позиция в файле с запакованными данными
             * Возвращает:
             * - последовательность распакованных данных
             * - PackedCount - количество элементов в незапакованной последовательности, нужно для вывода в консоль после успешного завершения процедуры запаковки 
             * Дата написания:
             * - октябрь 2017 */ 
            List<byte> unpackedArray = new List<byte>(); // последовательность незапакованных данных
            packedCount = 0; // длина этой последовательности в начале равна нулю
            try
            {
                BinaryReader fileOpen = new BinaryReader(File.Open(pathOpen, FileMode.Open));
                fileOpen.BaseStream.Seek(begin, SeekOrigin.Begin); // устанавливаем позицию в файле с запакованными данными
                
                byte count = 0; // количество повторений
                byte value = 0; // повторяющийся байт
                do // начинаем главный цикл
                {
                    count = fileOpen.ReadByte(); // считываем количество повторений
                    if (count != 0xFF) // если оно не равно $FF, то
                    {
                        if (count > 0x80) // проверяем превышает ли это количество значение $80, и если превышает, то 
                        {
                            count -= 0x80; // вычитаем из него значение $80
                            for (int j = 0; j < count; j++) // и считываем полученное количество байтов из файла с запакованными данными
                            {
                                unpackedArray.Add(fileOpen.ReadByte());
                            }
                            packedCount += count + 1; // увеличиваем длину исходной последовательности на количество считанных байтов (+1 - потому что мы уже прочитали байт с количеством повторений)
                        }
                        else // если это количество не превышает значение $80, то
                        {
                            value = fileOpen.ReadByte(); // считываем байт
                            for (int j = 0; j < count; j++) // повторяем его count раз в последовательности незапакованных данных
                            {
                                unpackedArray.Add(value);
                            }
                            packedCount += 2; // увеличиваем длину исходной последовательности на 2 (именно столькими байтами запаковывается последовательность повторяющихся байтов)
                        }
                    }
                } while (count != 0xFF); // если байт с количеством повторений равен $FF, выходим из цикла
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
            /* Открытый метод для распаковки данных.
             * Принимает:
             * - pathOpen - путь к файлу с запакованными данными
             * - pathSave - путь для сохранения файла с распакованными данными
             * - begin - позиция в файле с запакованными данными */
            try
            {
                long packedCount; // создаем переменную, в которой будет храниться длина запакованной последовательности
                List<byte> unpackedArray = _unpack(pathOpen, begin, out packedCount); // пытаемся выполнить процедуру распаковки

                BinaryWriter fileSave = new BinaryWriter(File.Open(pathSave, FileMode.Create));
                foreach (var c in unpackedArray) // если никаких ошибок не было, то сохраняем распакованную последовательность
                {
                    fileSave.Write(c);
                }
                fileSave.Close();

                Console.WriteLine("Uncompressing was success! Source data takes {0} bytes, uncompressed data takes {1} bytes.", packedCount, unpackedArray.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Uncompressing failed.");
                Console.Write(ex.Message);
            }
        }
    }
}
