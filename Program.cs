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
                string method;
                string pathOpen;
                string pathSave;
                long begin = 0;
                if (args.Length >= 3 && args.Length <= 4)
                {
                    method = args[0];
                    pathOpen = args[1];
                    pathSave = args[2];
                    if (args.Length == 4)
                        begin = long.Parse(args[3]);
                }
                else
                    throw new ApplicationException("You entered incorrect count of parameters for packing/unpacking.");

                if (method == "p")
                {
                    RLE.Pack(pathOpen, pathSave, begin);
                }
                else if (method == "u")
                {
                    RLE.Unpack(pathOpen, pathSave, begin);
                }
                else
                {
                    throw new ApplicationException("You entered incorrect method of the program.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }


    }
}
