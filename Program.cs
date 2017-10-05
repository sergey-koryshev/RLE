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
            //List<byte> myArray = new List<byte> { 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 4, 4, 3, 4, 5, 4, 3, 4, 5, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            RLE myFile = new RLE(@"E:\unpacked.hz", @"E:\packed.hz");

            //myFile.Pack();

            RLE myFile2 = new RLE(@"E:\packed.hz", @"E:\unpacked1.hz");

            myFile2.Unpack();



            //BinaryWriter myFile = new BinaryWriter(File.Open(@"E:\data.hz", FileMode.CreateNew));

            //foreach (var c in myArray)
            //{
            //    myFile.Write(c);
            //}

            Console.ReadLine();
        }

        
    }
}
