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
            ISegment segment = tst.getSegment("OBX");
            bool latexallergy = false;
            bool found = false;
            string query = "";
            bool segrepcount = segment.Message.IsRepeating("OBX");
           
                for (int i = 0; i <= obxrep; i++)
                {
                    if (found == false)
                    {
                        string querytypefound = tst.Get("/OBX(" + i + ")-3");
                        string admallergy = tst.Get("/OBX(" + i + ")-3-2");
                        switch (allergytype)
                        {
                            case "ADM.ALLERGY":
                                query = "Latex";
                                break;
                            case "PCS.NDADM054D":
                                query = "MRSA";
                                break;
                            case "PCS.NDADM111":
                                query = "Fall";
                                break;
                        }


                    if (admallergy == "" || admallergy == null)
                    {
                        latexallergy = false;
                    }
                    else
                    {
                        int latexallergyExists = Regex.Matches(admallergy, query).Count;
                        if (latexallergyExists > 0)
                        {
                            string allergyvalue = tst.Get("/OBX(" + i + ")-5");
                            if (allergyvalue == "N")
                            {
                                latexallergy = false;
                                break;
                            }
                            else
                            {
                                latexallergy = true;
                                break;
                            }
                        }
                        else
                        {
                            latexallergy = FromAL1Segment(tst, obxrep, query);
                            break;
                        }
                    }
                    }
                    else
                    {
                    latexallergy = false;
                        break;
                    }
              

            }
            return latexallergy;
        }
        public static bool FromAL1Segment(Terser tst, int reps, string query)
        {
            bool hasalleregen = false;
            for (int i = 0; i < reps; i++)
            {
                string allergencode = tst.Get("/AL1(" + i + ")-3-1");
                if (allergencode == query)
                {
                    string severitycode = tst.Get("/AL1(" + i + ")-4");
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
    }
}
