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
using System.Windows.Shapes;
using VF.ViewModel;

namespace VF
{
    /// <summary>
    /// Interaction logic for BlockScreen.xaml
    /// </summary>
    public partial class BlockScreen : Window
    {
        public BlockScreen(DateTime overTime)
        {
            InitializeComponent();
            BlockScrVM vm = new BlockScrVM(overTime);
            this.DataContext = vm;
            Closing += vm.OnWindowClosing;
            Deactivated += vm.OnDeactivated;
        }
    }
}
