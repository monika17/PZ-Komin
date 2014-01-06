using System;
using System.Windows.Forms;

namespace Komin
{
    public partial class ArchiveForm : Form
    {
        public ArchiveForm(string archivePath)
        {
            InitializeComponent();

            try
            {
                var archiveContent = System.IO.File.ReadAllText(archivePath);
                ArchiveContainer.Text += archiveContent;
            }
            catch (Exception)
            {
                ArchiveContainer.Text += "Brak danych w archiwum.";
            }

        }
    }
}
