using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Ward.Model
{
    public class DepartmentSecurityViewModel : SecurityViewModel
    {
        protected override bool OnDoLogin(string pin)
        {
            bool result = false;

            Dictionary<string, string> pins = new Dictionary<string, string>();

            // John Paul

            pins.Add("2000", "General, User ");
            pins.Add("1163", "Fahy, Teresa ");
            pins.Add("1091", "Bowes, Anita ");
            pins.Add("1324", "Duggan, Vera ");
            pins.Add("1804", "Carr, Mairead ");
            pins.Add("1775", "Mac, Noelle Namara");
            pins.Add("2206", "Marie, Anne Bohan");
            pins.Add("1312", "Gordon, Annette ");
            pins.Add("1206", "Mitchell, Claire ");
            pins.Add("1983", "Connolly, Grainne ");
            pins.Add("1984", "Hanley, Lorraine ");
            pins.Add("1996", "Cox, Marie ");
            pins.Add("2011", "Ryan, Marie ");
            pins.Add("2038", "McCarrick, Martina ");
            pins.Add("2086", "Morris, Aoife ");
            pins.Add("2099", "O'Neill, Linda ");
            pins.Add("2186", "Walsh, Elaine ");
            pins.Add("2192", "Sammon, Aisling ");
            pins.Add("2225", "Cox, Michelle ");
            pins.Add("2253", "Kenny, Angela ");
            pins.Add("2262", "McNicholas, Claire ");
            pins.Add("2289", "Freeley, Mary ");
            pins.Add("2081", "Heskin, Michelle ");
            pins.Add("2111", "O'Donnell, Niamh Ford");
            pins.Add("2153", "McNamara, Miriam ");
            pins.Add("2170", "Noonan, Claire ");
            pins.Add("2181", "O'Brien, Noreen ");
            pins.Add("2203", "Gallagher, Deborah ");
            pins.Add("2306", "Flanagan, Amanda ");
            pins.Add("2313", "Leonard, Leanne ");
            pins.Add("1978", "Morrisroe, Susan ");
            pins.Add("2053", "Forde, Michelle ");
            pins.Add("2183", "Cunningham, Roisin ");
            pins.Add("2209", "Campbell, Aoife ");
            pins.Add("2214", "Mylotte, Therese ");
            pins.Add("2218", "Keane, Martina ");
            pins.Add("2228", "Tuffy, Michelle ");
            pins.Add("2229", "Kelly, Anne ");
            pins.Add("2231", "Mannion, Orla ");
            pins.Add("2290", "Phelan, Brenda ");
            pins.Add("2291", "O'Reilly, Ann-Marie ");
            pins.Add("2312", "Walsh, Elaine ");
            pins.Add("2315", "Henry, Andrea ");
            pins.Add("2255", "Foy, Valarie ");
            pins.Add("2263", "O'Donnell, Annette ");
            pins.Add("1278", "Duggan, Hillary ");
            pins.Add("1077", "Reynolds, Thomas ");
            pins.Add("1418", "Nalty, Mary ");
            pins.Add("1461", "George, Lalikutty ");
            pins.Add("1581", "O'Brien, Michelle ");
            pins.Add("1765", "Gilchrist, Caroline ");
            pins.Add("1857", "Mitchell-Daly, Josephine ");
            pins.Add("2075", "Marie, Patricia Bewley");
            pins.Add("1217", "Crowe, Coreena ");
            pins.Add("1295", "Gannon, Mary ");
            pins.Add("2331", "Elwood, Deirdre ");

            // Mother Teresa

            pins.Add("1790", "Noone, Sinead ");
            pins.Add("1216", "Duggan, Marguerite ");
            pins.Add("1291", "Luft, Michael ");
            pins.Add("1362", "Hardiman, Teresa ");
            pins.Add("1484", "Connolly, Marie ");
            pins.Add("1539", "Deacy, Denise ");
            pins.Add("1562", "Satya, Nalini Priya");
            pins.Add("1595", "Millar, Yvonne ");
            pins.Add("1602", "O'Gorman, Helena ");
            pins.Add("1611", "McDonagh, Ann ");
            pins.Add("1616", "Poynton, Anne ");
            pins.Add("1641", "McGee, Joesephine ");
            pins.Add("1649", "Tierney, Linda ");
            pins.Add("1752", "Thomas, Sonia ");
            pins.Add("1764", "Coye, Brigid ");
            pins.Add("1766", "Paracka, Shiny Devassy");
            pins.Add("1793", "Susar, Jailby John");
            pins.Add("1923", "Deely, Rita ");
            pins.Add("1933", "Nelepa-Bubis, Malgorzata ");
            pins.Add("2074", "Lane, Sarah ");
            pins.Add("2091", "O'Dea, Saoirse ");
            pins.Add("2323", "Brennan, Cara ");
            pins.Add("1150", "Marie, Anne Lyons");
            pins.Add("1175", "Connaire, Mary ");
            pins.Add("1402", "Collins, Jill ");
            pins.Add("1826", "Harney, Margaret ");
            pins.Add("1919", "Mohan, Rosmarie ");
            pins.Add("2155", "Connors, Margaret ");
            pins.Add("2300", "Higgins, Niamh ");
            pins.Add("2322", "Ryan, Debbie ");

            if (pins.Keys.Contains(pin))
            {
                result = true;

                LoginName = pins[pin];
            }

            return result;
        }
    }
}
