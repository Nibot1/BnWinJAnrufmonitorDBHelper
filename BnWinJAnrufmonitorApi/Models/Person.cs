using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnWinJAnrufmonitorApi.Models
{
    public class Person
    {
        public int AnsprechpartnerID { get; set; }
        public String AdrNrGes { get; set; }
        public String Nachname { get; set; }
        public String Vorname { get; set; }
        public String Telefonnummer { get; set; }
        public int OrtID { get; set; }
        public String Ort { get; set; }
        public String PLZ { get; set; }
        public String Strasse { get; set; }

    }
}
