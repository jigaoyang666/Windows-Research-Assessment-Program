using RAP.Research;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RAP.Control;

namespace RAP
{
    namespace View
    {
        /// <summary>
        /// MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {//declare some objects
            private Dictionary<string, AbstractController> observers;   // Observer Design Pattern
            // Subject is MainWindow, Observers is AbstractController

            public MainWindow()
            {
                observers = new Dictionary<string, AbstractController>();
                Attach("Publication",new PublicationController(this));
                Attach("Researcher",new ResearcherController(this));

                InitializeComponent();
            }

            /// <summary>
            /// add observer
            /// </summary>
            /// <param name="observer"></param>
            public void Attach(string s,AbstractController observer)
            {
                observers.Add(s, observer);
            }

            /// <summary>
            /// remove observer
            /// </summary>
            /// <param name="observer"></param>
            public void Detach(string s)
            {
                observers.Remove(s);
            }

            public void Notify(string name, object sender, EventArgs arg)
            {
                observers[name].Update(sender, arg);
            }

            private void ComboList_LoadLevel(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            /// <summary>
            /// filter the research list with the employment level
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ComboList_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }
            /// <summary>
            /// show the researcher details depend on the user selection
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ReListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                e.Handled = true;
                Notify("Researcher", sender, e);
            }

            private void TextBox_KeyUp(object sender, KeyEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            private void ShowName_Click(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            private void Position_Click(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);

            }

            private void ChoosePublication(object sender, SelectionChangedEventArgs e)
            {
                e.Handled = true;
                Notify("Publication", sender, e);
                //pc.Update();
                //pc.LoadPublicationDetails();
            }

            private void ButtonCopy_Click(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            private void ChooseReport(object sender, SelectionChangedEventArgs e)
            {
                e.Handled = true;
                Notify("Researcher", sender, e);
            }

            private void Count_Click(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            private void ReListBox_Loaded(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }

            private void SearchPublications(object sender, RoutedEventArgs e)
            {
                Notify("Researcher", sender, e);
            }
        }
    }
}
