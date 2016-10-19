using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Department.Model
{
    public class DepartmentSecurityViewModel : SecurityViewModel
    {
        protected override bool OnDoLogin(string pin)
        {
            bool result = false;

            Dictionary<string, string> pins = new Dictionary<string, string>();

            pins.Add("2011", "General, User ");
            pins.Add("1046", "Woulfe, Peter ");
            pins.Add("1057", "Leonard, Anne ");
            pins.Add("1070", "Madden, Claire ");
            pins.Add("1161", "Smyth, Evelyn ");
            pins.Add("1589", "Geraghty, Suzanne Radanovic");
            pins.Add("1550", "Ryan, Lucinda ");
            pins.Add("1368", "Kelly, Helen ");
            pins.Add("1939", "Noori, Nabaz ");
            pins.Add("1974", "Gillespie, Sean ");
            pins.Add("2046", "Dooley, Claire ");
            pins.Add("2292", "Cronin, John ");
            pins.Add("1044", "Fallon, Dympa ");
            pins.Add("1060", "Donoghue, Rita ");
            pins.Add("1123", "Blundell, Cathal ");
            pins.Add("1193", "Brogan, Siobhan ");
            pins.Add("1318", "Kinneen, Mary ");
            pins.Add("1621", "Mangan, Aideen ");
            pins.Add("1659", "Donnellan, Avril ");
            pins.Add("1661", "Kavanagh, Suzanne ");
            pins.Add("1674", "Doheny, Susan ");
            pins.Add("1702", "Lynch, Shane ");
            pins.Add("1713", "Asso, Clara Gyasi");
            pins.Add("1724", "Monaghan, Laura ");
            pins.Add("1761", "Kelly, Deirdre ");
            pins.Add("1815", "Lynch, Caren ");
            pins.Add("1816", "Noonan, Elizabeth ");
            pins.Add("1831", "Jones, Gareth ");
            pins.Add("1918", "Mulhall, Eimear ");
            pins.Add("2076", "O'Regan, Lorraine ");
            pins.Add("1230", "McGuire, Sara ");
            pins.Add("1698", "Harty, Susan ");
            pins.Add("2130", "Hearne, Marie ");
            pins.Add("1882", "Pritchard, Pauline ");
            pins.Add("1942", "Comerford, Sarah ");
            pins.Add("2097", "Greaney, Lisa ");
            pins.Add("2115", "Peyton, Shane ");
            pins.Add("2240", "Kinneen, Laura ");
            pins.Add("2244", "Forde, Anne ");
            pins.Add("2269", "Guilfoyle, Niamh ");
            pins.Add("2337", "Henry, Claire ");
            pins.Add("2340", "Fahy, Margaret ");
            pins.Add("1039", "Farrell, Anna ");
            pins.Add("1283", "McSkimming, Alison ");
            pins.Add("1428", "Mathers, Majella ");
            pins.Add("1807", "Lynch, Margaret ");
            pins.Add("1903", "Samsa, Mark ");
            pins.Add("1945", "Barrett, Sarah ");
            pins.Add("1988", "Barrett, Katrina ");
            pins.Add("2143", "Ward, James ");
            pins.Add("2126", "McMahon, Karla ");
            pins.Add("2277", "Kenny, Irene ");

            if (pins.Keys.Contains(pin))
            {
                result = true;

                LoginName = pins[pin];
            }

            return result;
        }
    }
}
