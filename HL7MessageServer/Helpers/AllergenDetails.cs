using NHapi.Base.Model;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace H7Message
{
    public static class AllergenDetails
    {
        public static bool HasAllergy(Terser tst, int obxrep, string allergytype)
        {
           
            bool latexallergy = false;
            bool found = false;
            string query = "";
           
           
                for (int i = 0; i < obxrep; i++)
                {
                
                    string querytypefound = tst.Get("/.OBX(" + i + ")-3");
                    string admallergy = tst.Get("/.OBX(" + i + ")-3-2");
                   
                            if (allergytype == "" || admallergy == "")
                            {
                                latexallergy = false;
                            }
                            else
                            {
                                int latexallergyExists = Regex.Matches(admallergy, allergytype).Count;
                                if (latexallergyExists > 0)
                                {
                                    string allergyvalue = tst.Get("/.OBX(" + i + ")-5");
                                    if (allergyvalue == "N")
                                    {
                                        latexallergy = false;
                                       
                                    }
                                    else
                                    {
                                        latexallergy = true;
                                       
                                    }
                                }
                                else
                                {
                                    latexallergy = FromAL1Segment(tst, obxrep, allergytype);
                                    
                                }
                            }
                          
                       
              

            }
            return latexallergy;
        }
        public static bool FromAL1Segment(Terser tst, int reps, string query)
        {
            bool hasalleregen = false;
            for (int i = 0; i < reps; i++)
            {
                string allergencode = tst.Get("/.AL1(" + i + ")-3-1");
                if (allergencode == query)
                {
                    string severitycode = tst.Get("/.AL1(" + i + ")-4");
                    if (severitycode == "U")
                    {
                        hasalleregen = false;
                    }
                    else
                    {
                        hasalleregen = true;
                    }
                }
            }
            return hasalleregen;
        }
        public static bool regex(string query, string message)
        {
            int count1 = Regex.Matches(message, query).Count;
            if (count1 > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
