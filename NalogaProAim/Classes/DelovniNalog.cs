using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NalogaProAim.Classes
{
    public class DelovniNalog
    {
        public int Id { get; set; }

        public int StKosov { get; set; }

        public DelovniNalog() { }
        public DelovniNalog(int id, int stKosov)
        {
            Id = id;
            StKosov = stKosov;
        }


    }
}
