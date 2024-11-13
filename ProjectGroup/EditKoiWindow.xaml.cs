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

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for EditKoiWindow.xaml
    /// </summary>
    public partial class EditKoiWindow : Window
    {
        public KoiDtosResponse UpdatedKoi { get; set; }

        public EditKoiWindow(KoiDtosResponse koi)
        {
            InitializeComponent();
            UpdatedKoi = koi;

            txtName.Text = koi.Name;
            cmbSex.SelectedItem = koi.Sex;
            txtVariety.Text = koi.Variety;
            txtPhysique.Text = koi.Physique;
            txtNote.Text = koi.Note;
            txtOrigin.Text = koi.Origin;
            txtLength.Text = koi.Length.ToString();
            txtWeight.Text = koi.Weight.ToString();
            txtColor.Text = koi.Color;
            chkStatus.IsChecked = koi.Status;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdatedKoi.Name = txtName.Text;
            UpdatedKoi.Sex = cmbSex.SelectedItem?.ToString();
            UpdatedKoi.Variety = txtVariety.Text;
            UpdatedKoi.Physique = txtPhysique.Text;
            UpdatedKoi.Note = txtNote.Text;
            UpdatedKoi.Origin = txtOrigin.Text;

            UpdatedKoi.Length = int.TryParse(txtLength.Text, out var length) ? length : 0;
            UpdatedKoi.Weight = int.TryParse(txtWeight.Text, out var weight) ? weight : 0;
            UpdatedKoi.Color = txtColor.Text;
            UpdatedKoi.Status = chkStatus.IsChecked ?? false;

            DialogResult = true;  
            Close();
        }
    }
}
