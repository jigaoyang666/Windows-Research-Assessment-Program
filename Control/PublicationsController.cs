using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAP.Research;
using RAP.View;

namespace RAP
{
    namespace Control
    {
        public class PublicationController : AbstractController
        {
            private Publication pub;        //declare a object for publication
            public MainWindow wm { get; }   //declare a object for mainwindow

            public PublicationController(MainWindow w)
            {
                pub = new Publication();
                wm = w;
            }
            /// <summary>
            /// to update the publication details
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="arg"></param>
            public override void Update(object sender, EventArgs arg)
            {
                System.Windows.Controls.DataGrid dg = (System.Windows.Controls.DataGrid)sender;
                if (dg == wm.dataGridforpublication)
                {
                    if (wm.dataGridforpublication.SelectedIndex != -1)
                    {
                        pub = (Publication)wm.dataGridforpublication.SelectedItem;
                    }
                    else
                        pub = new Publication();
                    LoadPublicationDetails();
                }
            }

            public void LoadPublicationDetails()
            {
                wm.PublicationDetail.DataContext = pub;       // data binding
            }
        }
    }
}
