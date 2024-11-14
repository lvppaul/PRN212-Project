using ProjectGroup.Dtos;
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
using System.Xml.Linq;

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for EditPondWindow.xaml
    /// </summary>
    public partial class EditPondWindow : Window
    {
        public PondDtosResponse UpdatedPond { get; set; }
        public EditPondWindow(PondDtosResponse  pond)
        {
            InitializeComponent();
            UpdatedPond = pond;

            txtPondName.Text = pond.Name;
            txtVolume.Text = pond.Volume.ToString();
            txtDepth.Text = pond.Depth.ToString();
            txtPumpingCapacity.Text = pond.PumpingCapacity.ToString();
            txtDrain.Text = pond.Drain.ToString();
            txtSkimmer.Text = pond.Skimmer.ToString();
            txtNote.Text = pond.Note;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdatedPond.Name = txtPondName.Text;
            UpdatedPond.Volume = double.Parse(txtVolume.Text);
            UpdatedPond.Depth = int.Parse(txtDepth.Text);
            UpdatedPond.PumpingCapacity = int.Parse(txtPumpingCapacity.Text);
            UpdatedPond.Drain = int.Parse(txtDrain.Text);
            UpdatedPond.Skimmer = int.Parse(txtSkimmer.Text);
            UpdatedPond.Note = txtNote.Text;

            DialogResult = true;
            this.Close();
        }
    }
}
