using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RAP.Research;

namespace RAP
{
    namespace Database
    {
        public class DBAdapter
        {
            private const string db = "kit206";
            private const string user = "kit206";
            private const string pass = "kit206";
            private const string server = "alacritas.cis.utas.edu.au";

            private MySqlConnection conn = null;
            private MySqlCommand cmd;
            private MySqlDataReader rdr;
            public bool conn_status { get; set; }   // true: conn is open;   false: conn is close.
            public string loginInfo { get; set; }

            // Singleton Pattern
            private static DBAdapter uniqueInstance = null;
            private DBAdapter()
            {
                string connectString = String.Format(
                    "Database={0};Data Source={1};User Id={2};Password={3}",
                    db, server, user, pass);
                conn = new MySqlConnection(connectString);
            }

            public static DBAdapter GetInstance()
            {
                if (uniqueInstance == null)
                {
                    uniqueInstance = new DBAdapter();
                }
                return uniqueInstance;
            }

            public bool Connect()
            {   
                try
                {
                    if (conn_status == true)
                        Disconnect();

                    conn.Open();
                    conn_status = true;
                }
                catch (MySqlException ex)
                {
                    loginInfo = ex.Message;        
                    conn_status = false;
                }
                return conn_status;
            }

            // Fetch a list of basic researcher's details 
            public List<Researcher> FetchReasearcherList()
            {
                string sql = "SELECT given_name, family_name, title, id, level FROM researcher";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                List<Researcher> templist = new List<Researcher>();
                while (rdr.Read())
                {
                    Researcher temp = new Researcher();
                    temp.GivenName = rdr["given_name"].ToString();
                    temp.FamilyName = rdr["family_name"].ToString();
                    temp.Name = temp.FamilyName + "," + temp.GivenName;
                    temp.Title = rdr["title"].ToString();
                    temp.ID = rdr["id"].ToString();
                    temp.Level = rdr["level"].ToString();
                    if (temp.Level == "")
                        temp.Level = "Student";
                    templist.Add(temp);
                }
                rdr.Close();
                return templist;
            }

            // Get ResearcherType according to ID
            public ResearcherType FetchResearcherType(string id)
            {
                string sql = "SELECT type FROM researcher WHERE id=" + id;
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                string type = rdr["type"].ToString();
                rdr.Close();
                if (type == "Staff")
                {
                    return ResearcherType.Staff;
                }
                else
                    return ResearcherType.Student;
            }

            // Fetch details of a staff
            public Staff FetchStaffDetails(string id)
            {
                // id type given_name family_name title unit campus email photo
                // degree supervisor_id level utas_start current_start
                string sql = "SELECT * FROM researcher WHERE id=" + id;
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                rdr.Read();

                Staff details = new Staff();
                details.ID = id;
                details.Name = rdr["family_name"].ToString() + "," + rdr["given_name"].ToString();
                details.Title = rdr["title"].ToString();
                details.School = rdr["unit"].ToString();
                details.Campus = rdr["campus"].ToString();
                details.Email = rdr["email"].ToString();
                details.Photo = rdr["photo"].ToString();
                details.Level = rdr["level"].ToString();
                details.EarliestPosition = rdr.GetDateTime("utas_start");
                details.CurrentPosition = rdr.GetDateTime("current_start");
                rdr.Close();

                details.CurrentJob = details.CurrentJobTitle();
                details.Tenure = details.GetTenure();
                details.PositionList = FetchPositionList(id);              // only for staff
                details.SupervisionList = FetchSupervisionList(id);        // only for staff
                details.Supervisions = details.SupervisionList.Count;      // only for staff
                details.PublicationList = FetchPublicationList(id); 
                details.TotalPublications = details.PublicationsCount();
                details.Performance = details.GetPerformance();
                details.Average = details.ThreeYearAverage();
                return details;
            }

            // Fetch details of a student
            public Student FetchStudentDetails(string id)
            {
                string sql = "SELECT * FROM researcher WHERE id=" + id;
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                Student details = new Research.Student();
                details.ID = id;

                details.Name = rdr["family_name"].ToString() + "," + rdr["given_name"].ToString();
                details.Title = rdr["title"].ToString();
                details.School = rdr["unit"].ToString();
                details.Campus = rdr["campus"].ToString();
                details.Email = rdr["email"].ToString();
                details.Photo = rdr["photo"].ToString();
                details.Degree = rdr["degree"].ToString();                    // only for student
                details.Level = "Student";
                string supervisor_id = rdr["supervisor_id"].ToString();       // only for student
                details.CurrentJob = "Student";                               // null for student
                details.EarliestPosition = rdr.GetDateTime("utas_start");
                details.CurrentPosition = rdr.GetDateTime("current_start");
                details.Tenure = details.GetTenure();
                rdr.Close();
                //details.PositionList = FetchPositionList(id);       //Students do not have positionlist
                details.PublicationList = FetchPublicationList(id);
                details.TotalPublications = details.PublicationsCount();
                details.SupervisorName = FetchStaffDetails(supervisor_id).Name;

                return details;
            }

            // Fetch position list of a researcher
            public List<Position> FetchPositionList(string id)
            {
                List<Position> list = new List<Position>();
                string sql = "SELECT start, end, level FROM position WHERE id=" + id + " ORDER BY start DESC";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Position p = new Position();
                    p.start_date = rdr.GetMySqlDateTime(0).GetDateTime();
                    //p.start_date = Convert.ToDateTime(rdr[0]).ToString("dd/MM/yyyy");
                    if (rdr["end"].ToString() != "")
                    {
                        p.end_date = rdr.GetMySqlDateTime(1).GetDateTime();
                    }
                    else
                    {
                        p.end_date = DateTime.Now;
                    }
                    p.Level = p.GetEmploymentLevel(rdr["level"].ToString());
                    list.Add(p);
                }
                rdr.Close();

                return list;
            }

            // Fetch supervision list of a student
            public Dictionary<string, string> FetchSupervisionList(string id)
            {
                List<string> sl = new List<string>();
                Dictionary<string,string> idlist = new Dictionary<string, string>();
                string sql = "select student_ID from supervision where staff_ID=" + id;
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    sl.Add(rdr["student_ID"].ToString());
                }
                rdr.Close();

                foreach(string item in sl)
                {
                    cmd = new MySqlCommand("select given_name, family_name from researcher where id=" + item, conn);
                    rdr = cmd.ExecuteReader();
                    rdr.Read();

                    idlist.Add(item,rdr["family_name"]+","+rdr["given_name"]);
                    rdr.Close();
                }

                return idlist;
            }

            // Fetch publication list of a researcher
            public List<Publication> FetchPublicationList(string id)
            {
                List<Publication> pl = new List<Publication>();
                List<string> doilist = new List<string>();
                string sql = "select doi from researcher_publication where researcher_id=" + id;
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    doilist.Add(rdr["doi"].ToString());   
                }
                rdr.Close();
                foreach(string s in doilist)
                {
                    pl.Add(FetchPublicationDetails(s));
                }
                
                return pl;
            }

            // Fetch details of a publication
            public Publication FetchPublicationDetails(string doi)
            {
                //doi, title, authors, year, type, cite_as, available
                Publication pub = new Publication();
                string sql = "select * from publication where doi='" + doi+"';";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                pub.DOI = doi;
                pub.Title = rdr["title"].ToString();
                pub.Authors = rdr["authors"].ToString();
                pub.Year = Int32.Parse(rdr["year"].ToString());
                pub.Type = Outputtype.Conference;
                switch(rdr["type"].ToString())
                {
                    case "Conference":
                        pub.Type = Outputtype.Conference;
                        break;
                    case "Journal":
                        pub.Type = Outputtype.Journal;
                        break;
                    default:
                        pub.Type = Outputtype.Other;
                        break;
                }
                pub.CiteAs = rdr["cite_as"].ToString();
                pub.Availability = rdr.GetDateTime("available");
                pub.Age = pub.GetAge();
                rdr.Close();
                return pub;

            }

            private void Disconnect()
            {
                conn.Close();
                conn_status = false;
            }

        }
    }
}
