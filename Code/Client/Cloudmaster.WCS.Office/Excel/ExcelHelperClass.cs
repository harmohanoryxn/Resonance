using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Office.Excel
{
    internal class ExcelHelperClass
    {
        public static string ConvertCellIndexToA1(int colIndex, int rowIndex)
        {
            string result = ConvertCellIndexToA1(colIndex);

            result += rowIndex.ToString();

            return result;
        }

        public static string ConvertCellIndexToA1(int colIndex)
        {
            string result = "";

            char captialA = Char.Parse("A");

            int zeroBasedColIndex = colIndex - 1;

            if (zeroBasedColIndex >= 702)
            {
                int multiplier = (int)Math.Floor(zeroBasedColIndex / 26.0);

                result += (char)(captialA);

                zeroBasedColIndex = zeroBasedColIndex - 676;
            }

            if (zeroBasedColIndex >= 26)
            {
                int multiplier = (int)Math.Floor(zeroBasedColIndex / 26.0);

                result += (char)(captialA + (multiplier - 1));

                zeroBasedColIndex = (zeroBasedColIndex % 26);
            }

            int remainder = (int)((zeroBasedColIndex) % 26);

            result += (char)(captialA + remainder);

            return result;
        }
    }
}
