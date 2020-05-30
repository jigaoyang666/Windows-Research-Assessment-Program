
using RAP.View;
using RAP.Research;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Drawing.Imaging;

namespace RAP
{
    namespace Control
    {
        public class ResearcherController : AbstractController
        {
            private Database.DBAdapter dbaconnect;    //   database connection

            private List<Researcher> listname;      //used to store the researcherlist
            private List<Researcher> showlist;    // used to listbox binding
            //private Staff staffdetails;            
            private List<Staff> staffperformancelist; //declare a object for staff performance

            private Researcher details;         //decalre a object for researcher details

            public MainWindow wm { get; }    //declare a object for mainwindow

            public ResearcherController(MainWindow w)
            {
                wm = w;
                showlist = new List<Researcher>();

                dbaconnect = Database.DBAdapter.GetInstance();
                bool flag = dbaconnect.Connect();
                if (flag == false)
                {
                    MessageBox.Show(dbaconnect.loginInfo + "\nPlease check your network!");
                    //Application.Current.Shutdown(-1);
                    Environment.Exit(0);
                }
                else
                {
                    staffperformancelist = new List<Staff>();
                }
            }
            /// <summary>
            /// inherited from base class
            /// different action according to different events
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="arg"></param>
            public override void Update(object sender, EventArgs arg)
            {
                if(sender is ComboBox)
                {
                    ComboBox cb = (ComboBox)sender;
                    if (cb == wm.ComboList)
                    {
                        RoutedEventArgs a = (RoutedEventArgs)arg;
                        if (a.RoutedEvent.Name == "Loaded")
                            LoadLevel();
                        if (a.RoutedEvent.Name == "SelectionChanged")
                            FilterByLevel(wm.ComboList.SelectedItem.ToString());
                    }

                }
                if(sender is ListBox)
                {
                    ListBox lb = (ListBox)sender;
                    if(lb == wm.ReListBox)
                    {
                        RoutedEventArgs a = (RoutedEventArgs)arg;
                        if (a.RoutedEvent.Name == "Loaded")
                        {
                            LoadResearchers();
                            wm.PublicationSearch.IsEnabled = false;
                        }
                        if (a.RoutedEvent.Name == "SelectionChanged")
                        {
                            LoadResearcherDetails();
                            wm.PublicationSearch.IsEnabled = true;
                        }
                    }
                    if(lb == wm.ReportList)
                    {
                        RoutedEventArgs a = (RoutedEventArgs)arg;
                        if (a.RoutedEvent.Name == "SelectionChanged")
                            LoadReport();
                    }
                }
                if(sender is TextBox)
                {
                    TextBox tb = (TextBox)sender;
                    if(tb == wm.SearchBox)
                    {
                        FilterByName(wm.SearchBox.Text);
                    }
                }
                if(sender is Button)
                {
                    Button b = (Button)sender;
                    if(b == wm.Position)
                    {
                        LoadPositionList();
                    }
                    if(b == wm.Count)
                    {
                        LoadCumCount();
                    }
                    if(b == wm.ShowName)
                    {
                        LoadSupervisionList();
                    }
                    if(b == wm.CopyButton)
                    {
                        copyemail();
                    }
                    if(b == wm.PublicationSearch && details !=null)
                    {
                        try
                        {
                            int x = Convert.ToInt32(wm.start_year.Text);
                            int y = Convert.ToInt32(wm.end_year.Text);
                            if (x != 0 && y != 0 && x <= y)
                            {
                                SearchPublication(x, y);
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Search!");
                                wm.dataGridforpublication.DataContext = details.PublicationList;
                            }
                        }catch(Exception e)
                        {
                            MessageBox.Show("Input need to be digit!");
                            wm.dataGridforpublication.DataContext = details.PublicationList;
                        }
                        
                    }
                }

            }
            /// <summary>
            /// to load researcher list in the list box
            /// </summary>
            public void LoadResearchers()
            {
                listname = dbaconnect.FetchReasearcherList();
                listname = listname.OrderBy(p => p.Name).ToList();      //sort the researcher list base on alphabetical order
                showlist = listname;
                wm.ReListBox.ItemsSource = showlist;  //show the reseacher list in the listbox
            }
            /// <summary>
            /// to load researcher employment level
            /// </summary>
            private void LoadLevel()
            {
                List<Research.EmploymentLevel> employlevel = new List<EmploymentLevel>();
                employlevel.Add(EmploymentLevel.ALL);  //ALL is to show all the researcher
                employlevel.Add(EmploymentLevel.A);
                employlevel.Add(EmploymentLevel.B);
                employlevel.Add(EmploymentLevel.C);
                employlevel.Add(EmploymentLevel.D);
                employlevel.Add(EmploymentLevel.E);
                employlevel.Add(EmploymentLevel.Student);
                wm.ComboList.ItemsSource = employlevel;   //show employment level in the combobox
            }
            /// <summary>
            /// to filter researcher list base on the employment level
            /// </summary>
            /// <param name="level"></param>
            public void FilterByLevel(string level)
            {
                if (level != "ALL")
                {
                    var filtered = from filter in listname
                                   where filter.Level == level
                                   select filter;

                    showlist = filtered.OrderBy(p=>p.Name).ToList();  
                    wm.ReListBox.ItemsSource = showlist;
                    //wm.ReListBox.ItemsSource = filtered;
                }
                else
                {
                    showlist = listname;
                    wm.ReListBox.ItemsSource = showlist;
                }
                    //wm.ReListBox.ItemsSource = listname;

            }
            /// <summary>
            /// to filter the researcher list base on name 
            /// </summary>
            /// <param name="name"></param>
            public void FilterByName(string name)
            {
                wm.ComboList.SelectedIndex = 0;
                if(name !="")
                {
                    var filtered = from filter in listname
                                   where filter.Name.ToLower().Contains(name.ToLower())
                                   select filter;

                    showlist = filtered.OrderBy(p=>p.Name).ToList();
                    wm.ReListBox.ItemsSource = showlist;
                    //wm.ReListBox.ItemsSource = filtered;
                }
                else
                {
                    showlist = listname;
                    wm.ReListBox.ItemsSource = showlist;
                }
                    //wm.ReListBox.ItemsSource = listname;
            }
            /// <summary>
            /// to load the researcher details 
            /// </summary>
            public void LoadResearcherDetails()
            {   //get the index of the name which is selected by the user
                int index = wm.ReListBox.SelectedIndex;
                if (index >= 0)
                {   //get the corresponding researcher ID
                    string id = ((Researcher)wm.ReListBox.SelectedItem).ID;
                    if (dbaconnect.FetchResearcherType(id) == ResearcherType.Staff)
                    {
                        details = dbaconnect.FetchStaffDetails(id);
                    }
                    else
                    {
                        details = dbaconnect.FetchStudentDetails(id);

                    }
                    List<object> temporarylist = new List<object>(); //declare a new temporary object
                    temporarylist.Add(details);
                    wm.StackDetail.DataContext = temporarylist;     //binding details resoures with the stackpanel
                    wm.EarliestPositionTime.DataContext = details;  //binding details resoures with the stackpanel
                    wm.CurrentPositionTime.DataContext = details;

                    details.PublicationList = details.PublicationList.OrderByDescending(o => o.Year).ThenBy(o => o.Title).ToList();     //get the publication list
                    wm.dataGridforpublication.DataContext = details.PublicationList;        //binding publication list with the DateGrid

                    BitmapImage bi = LoadUrlPhoto();
                    wm.Photo.Source = bi;       //show the photo
                }
            }
            /// <summary>
            /// to load researcher photo
            /// </summary>
            /// <returns></returns>
            private BitmapImage LoadUrlPhoto()
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(details.Photo);
                WebResponse response = request.GetResponse();
                System.Drawing.Image img = System.Drawing.Image.FromStream(response.GetResponseStream());

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                MemoryStream ms = new MemoryStream();
                // Save to a memory stream...
                img.Save(ms, ImageFormat.Bmp);
                // Rewind the stream...
                ms.Seek(0, SeekOrigin.Begin);
                // Tell the WPF image to use this stream...
                bi.StreamSource = ms;    
                bi.EndInit();

                return bi;
            }
            /// <summary>
            /// to count the number of publication every year
            /// </summary>
            public void LoadCumCount()
            {
                CumulativePublicationsWindow cw = new CumulativePublicationsWindow();
                Dictionary<int, int> cumCount = new Dictionary<int, int>();
                int current_year = DateTime.Now.Year;
                int start_year = details.EarliestStart().Year;
                for(int i = start_year; i<=current_year;  i++)
                {
                    var filtered = from filter in details.PublicationList
                                   where filter.Year <= i
                                   select filter;
                    cumCount.Add(i, filtered.Count());
                }
                cw.DataContext = cumCount;          //data binding
                cw.ShowDialog();                //to show dialog
            }
            /// <summary>
            /// to load previous position and time for the researcher
            /// </summary>
            public void LoadPositionList()
            {
                // ps.Table.ItemsSource = ((Research.Staff)details).SupervisionList;    
                PositionsWindow pw = new PositionsWindow();  //to open a new window
                pw.positionList.DataContext = details.PositionList;     //data binding
                pw.ShowDialog();        //to show dialog
            }
            /// <summary>
            /// to load the supervision list
            /// </summary>
            public void LoadSupervisionList()
            {
                Staff temp = details as Staff;
                if (temp == null)
                { 
                    MessageBox.Show("Students do not have supervisions.");
                    return; 
                }
                //to validate the researcher has supervison list or not
                if (temp.Supervisions > 0)
                {
                    supervision showname = new supervision();

                    List<Researcher> snl = new List<Researcher>();
                    foreach (var item in temp.SupervisionList)
                    {
                        Researcher r = new Researcher();
                        r.ID = item.Key;
                        r.Name = item.Value;
                        snl.Add(r);

                    }
                    showname.Tablesuper.DataContext = snl;    //data binding
                    showname.ShowDialog();      //show new a dialog
                }
                else
                    MessageBox.Show("No supervision exists.");
            }
            /// <summary>
            /// to load researcher performance report
            /// </summary>
            public void LoadReport()
            {
                List<Staff> sl = new List<Staff>();
                foreach(Researcher item in listname)
                {
                    Staff staff = new Staff();
                    if(item.Level != "" && item.Level != "Student")
                    {
                        staff = dbaconnect.FetchStaffDetails(item.ID);
                        sl.Add(staff);
                    }
                }

                int index = wm.ReportList.SelectedIndex;
                switch(index)
                {
                    case 0:
                        {
                            var filtered = from filter in sl
                                           where filter.Performance <= 70.0
                                           orderby filter.Performance ascending
                                           select filter;

                            staffperformancelist = filtered.ToList();
                            
                            break;
                        }
                    case 1:
                        {
                            var filtered = from filter in sl
                                           where filter.Performance <= 100.0 && filter.Performance >70.0
                                           orderby filter.Performance ascending
                                           select filter;

                            staffperformancelist = filtered.ToList();
                            break;
                        }
                    case 2:
                        {
                            var filtered = from filter in sl
                                           where filter.Performance <= 200.0 && filter.Performance >100.0
                                           orderby filter.Performance descending
                                           select filter;

                            staffperformancelist = filtered.ToList();
                            break;
                        }
                    case 3:
                        {
                            var filtered = from filter in sl
                                           where filter.Performance > 200.0
                                           orderby filter.Performance descending
                                           select filter;

                            staffperformancelist = filtered.ToList();
                            break;
                        }
                    default:
                        break;
                }
                wm.performance.DataContext = staffperformancelist;      //binding data
            }
            /// <summary>
            /// to copy the email
            /// </summary>
            public void copyemail()
            {
                var filtered = from filter in staffperformancelist
                              select filter.Email;

                Clipboard.SetText(String.Join(";", filtered.ToList()));
                MessageBox.Show("Copy data to Clipboard!");
            }
            /// <summary>
            /// to filter publication list base on the time the user type in
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            public void SearchPublication(int start, int end)
            {
                var filtered = from filter in details.PublicationList
                               where filter.Year >= start && filter.Year <= end
                               orderby filter.Year
                               select filter;

                
                wm.dataGridforpublication.DataContext = filtered.ToList(); ;        //data binding
            }
        } 
    }
}
