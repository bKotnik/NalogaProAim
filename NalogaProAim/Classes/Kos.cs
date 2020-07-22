using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NalogaProAim.Classes
{
    public class Kos : INotifyPropertyChanged
    {
        private int _id;
        public int Id 
        {
            get { return _id; } 
            set { _id = value;  OnPropertyRaised("Id"); }
        }

        private Guid _guid;
        public Guid Guid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
                OnPropertyRaised("Guid");
            }
        }

        private string _delovniNalog;
        public string DelovniNalog 
        {
            get { return _delovniNalog; }
            set { _delovniNalog = value; OnPropertyRaised("DelovniNalog"); }
        }

        private DateTime _casVnosa;
        public DateTime CasVnosa 
        {
            get { return _casVnosa; }
            set { _casVnosa = value; OnPropertyRaised("CasVnosa"); }
        }

        private int id_delovni_nalog;
        public int Id_delovni_nalog 
        {
            get { return id_delovni_nalog; }
            set { id_delovni_nalog = value; OnPropertyRaised("Id_delovni_nalog"); }
        }

        //Konstruktorji
        public Kos() { }

        public Kos(int id, Guid guid, string delovniNalog, DateTime casVnosa, int id_delovni_nalog)
        {
            Id = id;
            Guid = guid;
            DelovniNalog = delovniNalog;
            CasVnosa = casVnosa;
            Id_delovni_nalog = id_delovni_nalog;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname)); // simplified
        }
    }
}
