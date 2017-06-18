using System;
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

namespace RSFJ.View
{
    /// <summary>
    /// Interaction logic for RojmelPage.xaml
    /// </summary>
    public partial class RojmelPage : Page
    {
        private bool isManualEditCommit;

        public RojmelPage()
        {
            InitializeComponent();
        }

        private void V_DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column == V_TypeColumn)
            {
                //if (!isManualEditCommit)
                //{
                //    isManualEditCommit = true;
                //    DataGrid grid = (DataGrid)sender;
                //    grid.CommitEdit(DataGridEditingUnit.Row, true);
                //    isManualEditCommit = false;
                //}
            }
        }
    }
}
