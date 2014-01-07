using System;
using System.Windows.Forms;

namespace Komin
{
    public partial class ArchiveForm : Form
    {
        public ArchiveForm(string archivePath)
        {
            InitializeComponent();
            textMessageContainer.DocumentText = "";
            try
            {
                var archiveContent = System.IO.File.ReadAllText(archivePath);
                textMessageContainer.Document.Write(archiveContent);

            }
            catch (Exception)
            {
                textMessageContainer.Document.Write("<div style='width:100%; height:100%; " +
                                                    "font-family: Verdana, Tahoma, Arial; " +
                                                    "background-color: #EEEEEE; " +
                                                    "text-align: center; padding: 100px'>" +
                                                    "Brak danych w archiwum." +
                                                    "</div>");
            }

        }
    }
}
