using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StockDataChooser
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> industry = new Dictionary<string, string>(),
            codeName = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent(); 
            lblStockDir.Text = Properties.Settings.Default.DataDir;

            foreach (string line in File.ReadAllLines("industry.txt", tfidf.EncodingType.GetType("industry.txt"))) 
            {
                string key = line.Substring(0, line.IndexOf('\t')),
                    value = line.Substring(line.IndexOf('\t') + 1);
                industry.Add(key, value);
            }

            foreach (string line in Properties.Settings.Default.StockNames.Split(Environment.NewLine.ToCharArray()))
            {
                int comma = line.IndexOf(',');
                if (comma < 0) continue;
                codeName.Add(line.Substring(0, comma), line.Substring(comma + 1));
            }

            updateStockList();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void updateStockList()
        {
            try
            {
                Regex reg = new Regex("[0-9]{6}");
                dateTimePicker1.Value = Properties.Settings.Default.DateFrom;
                dateTimePicker2.Value = Properties.Settings.Default.DateTill;
                foreach (string id in Properties.Settings.Default.Columns)
                {
                    if (String.IsNullOrEmpty(id)) continue;
                    int index = int.Parse(id);
                    lvwColumns.Items[index].Checked = true;
                }
                lvwStocks.Items.Clear();
                string path = Properties.Settings.Default.DataDir;
                foreach (string fn in System.IO.Directory.GetFiles(path, "*.txt"))
                {
                    string code = fn.Substring(fn.LastIndexOf('\\') + 1);
                    if (reg.IsMatch(code))
                    {
                        lvwStocks.Items.Add(code);
                        string realcode = reg.Match(code).Groups[0].Value;
                        if (codeName.ContainsKey(realcode))
                        {
                            lvwStocks.Items[lvwStocks.Items.Count - 1].Name = codeName[realcode];
                        }
                    }
                    if (Properties.Settings.Default.Stocks.Contains(code)) lvwStocks.Items[lvwStocks.Items.Count - 1].Checked = true;
                }
            }
            catch (Exception) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            datafolder.SelectedPath = lblStockDir.Text;
            if (datafolder.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            lblStockDir.Text = Properties.Settings.Default.DataDir = datafolder.SelectedPath;
            updateStockList();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwStocks.Items)
            {
                item.Checked = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwStocks.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private string getColumnsString()
        {
            string tmp = "";
            foreach (int index in lvwColumns.CheckedIndices)
            {
                tmp += "_" + index;
            }
            return tmp;
        }

        private string[] buildEverydayData(DateTime since, DateTime until, string file)
        {
            int total = (int)Math.Ceiling(until.Subtract(since).TotalDays);
            string[] lines = new string[total];
            lines[0] = "";
            foreach (string line in File.ReadAllLines(file))
            {
                string date = line.Split(',')[0];
                DateTime dateVal = DateTime.ParseExact(date, "yyyyMMdd", null);
                if (dateVal < since || dateVal > until) continue;
                int index = (int)(dateVal.Subtract(since).TotalDays);
                lines[index] = line;
            }
            for (int i = 1; i < total; ++i)
                if (string.IsNullOrEmpty(lines[i])) lines[i] = lines[i - 1];

            for (int i = total - 2; i >= 0; --i)
                if (string.IsNullOrEmpty(lines[i])) lines[i] = lines[i + 1];
            
            return lines;
        }

        public void Generate(string workingDir, string[] data, string columns, DateTime sinceDate, DateTime untilDate, string output1, string output2, bool singleFile)
        {
            string dirname = "", since =sinceDate.ToString("yyyyMMdd"), till = untilDate.ToString("yyyyMMdd");
            dirname = "sel" + columns + since + "_" + till;
            if (output1 == "") output1 = workingDir + "\\" + dirname + ".txt";
            if (output2 == "") output2 = workingDir + "\\" + dirname + "_industry.txt";

            List<int> columnIndicies = new List<int>();
            foreach (string s in columns.Split('_'))
            {
                if (s == "") continue;
                int id = 0; if (int.TryParse(s, out id)) columnIndicies.Add(id);
            }
            
            StreamWriter sw = null;
            if (singleFile)
                sw = new StreamWriter(output1);
            else
            {
                if (!Directory.Exists(workingDir + "\\" + dirname))
                    Directory.CreateDirectory(workingDir + "\\" + dirname);
            }

            StreamWriter swIndustry = new StreamWriter(output2);

            string title = this.Text;

            foreach (string file in data)
            {
                Application.DoEvents();
                this.Text = "正在处理: " + file;

                string code = file.Substring(0, file.LastIndexOf('.'));
                if (code.IndexOf('-') > 0) code = code.Substring(code.IndexOf('-') + 1);
                if (industry.ContainsKey(code))
                    swIndustry.WriteLine("{0}\t{1}", code, industry[code]);
                else
                    swIndustry.WriteLine("{0}\t{1}", code, "没有 经营 范围 描述 信息");

                double lastEnd = 0;

                if (!singleFile) sw = new StreamWriter(workingDir + "\\" + dirname + "\\" + file);

                foreach (string line in buildEverydayData(sinceDate, untilDate, workingDir + "\\" + file))
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    string[] cols = line.Split(',');
                    if (cols[0].CompareTo(since) < 0 || cols[0].CompareTo(till) > 0) continue;

                    bool first = true;
                    foreach (int index in columnIndicies)
                    {
                        if (!first) sw.Write(","); else first = false;
                        if (index >= 0 && index < cols.Length)
                            sw.Write(cols[index]);
                        if (index >= cols.Length)
                        {
                            switch (index - cols.Length)
                            {
                                case 0:
                                    int rise = 0, drop = 0;
                                    if (lastEnd != 0)
                                    {
                                        double rate = double.Parse(cols[4]) / lastEnd - 1;
                                        if (rate > 0) rise = (int)Math.Round(rate / 0.01);
                                        else drop = -(int)Math.Round(rate / 0.01);
                                    }
                                    sw.Write(rise + "," + drop);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    lastEnd = double.Parse(cols[4]);
                    if (!singleFile) sw.WriteLine(); else sw.Write(",");
                }

                sw.Flush();
                if (singleFile) sw.WriteLine("0"); else sw.Close();
            }
            if (sw != null) sw.Close();
            swIndustry.Flush(); swIndustry.Close();
            this.Text = title;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Properties.Settings.Default.DateFrom = dateTimePicker1.Value; Properties.Settings.Default.DateTill = dateTimePicker2.Value;
            Properties.Settings.Default.Columns.Clear(); Properties.Settings.Default.Columns.AddRange(getColumnsString().Split('_'));
            Properties.Settings.Default.Stocks.Clear();

            List<string> filedata = new List<string>();
            foreach (ListViewItem lvi in lvwStocks.SelectedItems)
            {
                filedata.Add(lvi.Text);
                Properties.Settings.Default.Stocks.Add(lvi.Text);
            }

            button4.Enabled = false;
            Generate(Properties.Settings.Default.DataDir, filedata.ToArray(), getColumnsString(), dateTimePicker1.Value, dateTimePicker2.Value, "", "", checkBox1.Checked);
            button4.Enabled = true;
            MessageBox.Show("生成完毕。");
        }

        private void txtFilter_Enter(object sender, EventArgs e)
        {
            if (txtFilter.Text == "筛选...") txtFilter.Text = "";
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            Regex reg = new Regex(txtFilter.Text);
            if (txtFilter.Text == "") return;
            foreach (ListViewItem lvi in lvwStocks.Items)
            {
                if (reg.IsMatch(lvi.Text) || reg.IsMatch(lvi.Name))
                {
                    lvi.Selected = true;
                }
                else
                {
                    lvi.Selected = false;
                }
            }
        }

        private void txtFilter_Leave(object sender, EventArgs e)
        {
            lvwStocks.Focus();
        }

        private void lvwStocks_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            lblSelectedCount.Text = "共选择 " + lvwStocks.SelectedItems.Count + " 项";
        }
    }
}
