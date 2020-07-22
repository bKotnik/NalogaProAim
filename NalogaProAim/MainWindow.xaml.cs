using Dapper;
using Emgu.CV;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using NalogaProAim.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NalogaProAim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Initialize data lists
            Utility.kosData = new ObservableCollection<Kos>();
            Utility.delovniNalogData = new ObservableCollection<DelovniNalog>();

            //assign these lists to listviews
            lvKosi.ItemsSource = Utility.kosData;
            lvDelovniNalog.ItemsSource = Utility.delovniNalogData;

            //create tables
            CreateDelovniNalogi();
            CreateKosi();
            CreateTableHole(); // za EmguCV del

            //set indexes
            SetIndexes();
        }

        private void InsertKos(object sender, RoutedEventArgs e)
        {
            // generate GUID
            Guid guid = Guid.NewGuid();
            Console.WriteLine("GUID: " + guid);

            //generate random number for ID
            Random rnd = new Random();
            //int id = rnd.Next(0, 100);

            //get current time
            DateTime currentTime = DateTime.Now;

            //generate random string for delovni_nalog
            int length = rnd.Next(10, 30);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string delovni_nalog = new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());

            //id delovni nalog - just a random number
            int id_delovni_nalog = rnd.Next(0, 20);

            //create object
            Kos kos = new Kos(0, guid, delovni_nalog, currentTime, id_delovni_nalog);

            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //insert into table kosi
                connection.Query("INSERT INTO kosi(guid, delovni_nalog, cas_vnosa, id_dn)" +
                        "VALUES(@guid, @delovni_nalog, @currentTime, @id_dn);", 
                        new { guid = kos.Guid, delovni_nalog = kos.DelovniNalog, currentTime = kos.CasVnosa, id_dn = kos.Id_delovni_nalog });

                //select last added id
                int tmp = connection.Query<int>("SELECT TOP 1 id_kos FROM kosi ORDER BY id_kos DESC").Single();
                kos.Id = tmp;
                Utility.kosData.Add(kos);
                lvKosi.ItemsSource = Utility.kosData;
            }

            //read guid after inserting
            ReadGuid();
        }

        private void ReadGuid()
        {
            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //select guid
                string output = connection.Query<string>("SELECT TOP 1 guid FROM kosi ORDER BY id_kos DESC;").Single();
                readGuid.Text = output;

            }
        }

        private void CreateDelovniNalogi()
        {
            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //first drop table if it already exists
                connection.Query("DROP TABLE IF EXISTS DelovniNalogi;");

                //create table DelovniNalogi
                connection.Query("CREATE TABLE DelovniNalogi(id_dn int IDENTITY(1,1) not null primary key, st_kosov int not null);");
            }
        }

        private void CreateKosi()
        {
            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //first drop table if it already exists
                connection.Query("DROP TABLE IF EXISTS kosi;");

                //create table kosi
                connection.Query("CREATE TABLE kosi(id_kos int IDENTITY(1,1) not null primary key," +
                        "guid char(40) not null, delovni_nalog char(30) not null, cas_vnosa datetime not null," +
                        "id_dn int not null);");

                /*
                 * Tukaj sem dodal atribut "id_dn", ki služi kot tuji ključ
                 */
            }
        }

        private void InsertDelovniNalog(object sender, RoutedEventArgs e)
        {
            //generate random number for ID
            Random rnd = new Random();
            //int id = rnd.Next(0, 20);

            //generate random number of Kos objects
            int st_kos = rnd.Next(0, 100);

            //create object
            DelovniNalog delovniNalog = new DelovniNalog(0, st_kos);

            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //insert into table DelovniNalogi
                connection.Query("INSERT INTO DelovniNalogi(st_kosov) VALUES(@st_kosov);",
                        new { st_kosov = delovniNalog.StKosov });

                //select last added id
                int tmp = connection.Query<int>("SELECT TOP 1 id_dn FROM DelovniNalogi ORDER BY id_dn DESC").Single();

                delovniNalog.Id = tmp;
                Utility.delovniNalogData.Add(delovniNalog);
                lvDelovniNalog.ItemsSource = Utility.delovniNalogData;
            }
        }

        private void SetIndexes()
        {
            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //creates non-clustered non-unique index on table kosi on attribute cas_vnosa
                connection.Query("CREATE INDEX ix_kosi_casVnosa ON kosi(cas_vnosa);");

                //creates non-clustered non-unique index on table DelovniNalogi on attribute st_kosov
                connection.Query("CREATE INDEX ix_dn_stKosov ON DelovniNalogi(st_kosov);");

                /*
                 * non-clustered sem izbral, ker imamo privzeto že narejen clustered index na primarnem ključu
                 * pri tabeli kosi sem izbral atribut cas_vnosa, ker se mi zdi, da se velikokrat išče vrstice po času in je smiselno, da imamo to indeksirano
                 * pri tabeli DelovniNalogi sem izbral atribut st_kosov z istim razlogom in pa edini atribut je, ki ni indeksiran (primarni kljuc ima clustered index privzeto)
                */
            }
        }

        /*EMGUCV DEL*/
        private void CreateTableHole()
        {
            //connect to database
            using (IDbConnection connection = new SqlConnection(Utility.ConnVal("ec38")))
            {
                //first drop table if it already exists
                connection.Query("DROP TABLE IF EXISTS Holes;");

                //create table kosi
                connection.Query("CREATE TABLE Holes(id_hole int IDENTITY(1,1) not null primary key," +
                                 "hole_height int not null, hole_width int not null, hole_area float not null);");
            }
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            //EmguCV emguCV = new EmguCV();
            //emguCV.ShowDialog();

            string filename = "C:\\Users\\Kekc\\Desktop\\slike-rotor\\slika1.png";
            Image<Bgr, byte> img = new Image<Bgr, byte>(filename);
            img = img.Resize(1024, 768, Emgu.CV.CvEnum.Inter.Linear);
            CvInvoke.Imshow("slika", img);
            CvInvoke.WaitKey(0);
        }
    }
}
