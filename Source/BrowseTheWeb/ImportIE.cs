using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MediaPortal.Configuration;

namespace BrowseTheWeb
{
  public partial class ImportIE : Form
  {
    public List<Bookmark> EntryList = new List<Bookmark>();

    public ImportIE()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }
    private void ImportIE_Load(object sender, EventArgs e)
    {
      string favPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
      string[] favFiles;

      listBox1.Items.Add("Import folder is " + favPath);
      listBox1.Items.Add("Reading folder");

      if (Directory.Exists(favPath))
      {
        string[] favDirs = Directory.GetDirectories(favPath);
        listBox1.Items.Add("Found " + favDirs.Length.ToString() + " folder");

        foreach (string folder in favDirs)
        {
          listBox1.Items.Add("Work on folder '" + Path.GetFileName(folder) + "'");

          favFiles = Directory.GetFiles(folder, "*.url", SearchOption.TopDirectoryOnly);
          listBox1.Items.Add(favFiles.Length.ToString() + " files to import");

          foreach (string s in favFiles)
          {
            FileInfo f = new FileInfo(s);
            string name = Path.GetFileNameWithoutExtension(f.Name);

            listBox1.Items.Add("Found '" + name + "'");
            string url = GetUrlFile(s);

            if (url != null)
            {
              Bookmark bkm = new Bookmark();
              bkm.Url = url;
              bkm.Name = name;

              EntryList.Add(bkm);
            }
          }
        }

        listBox1.Items.Add("Reading root folder");

        favFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Favorites), "*.url", SearchOption.TopDirectoryOnly);
        listBox1.Items.Add(favFiles.Length.ToString() + " files to import");

        foreach (string s in favFiles)
        {
          FileInfo f = new FileInfo(s);
          string name = Path.GetFileNameWithoutExtension(f.Name);

          listBox1.Items.Add("Found '" + name + "'");
          string url = GetUrlFile(s);

          if (url != null)
          {
            Bookmark bkm = new Bookmark();
            bkm.Url = url;
            bkm.Name = name;

            EntryList.Add(bkm);
          }
        }
        listBox1.Items.Add("Reading finished. Found " + EntryList.Count + " bookmarks");
      }
      else
      {
        listBox1.Items.Add("Directory does not exist.");
      }
    }

    private string GetUrlFile(string File)
    {
      using (StreamReader sr = new StreamReader(File))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          if (line.StartsWith("URL="))
            return line.Substring(4);
        }
      }
      return null;
    }
  }
}
