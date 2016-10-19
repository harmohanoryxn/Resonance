using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;

namespace WCS.Services.IPeople.Helpers
{
    public class DataHelper
    {
        internal static int? TryParseInt(OdbcDataReader reader, string item)
        {
            int? result = null;

            if (reader[item] != null)
            {
                if (!DBNull.Value.Equals(reader[item]))
                {
                    try
                    {
                        result = int.Parse((string) reader[item]);
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        internal static string TryParseString(OdbcDataReader reader, string item)
        {
            string result = string.Empty;

            if (reader[item] != null)
            {
                if (!DBNull.Value.Equals(reader[item]))
                {
                    result = (string)reader[item];
                }
            }

            return result;
        }
    }
}