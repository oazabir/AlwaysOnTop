using AlwaysOnTop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AlwaysOnTop
{
    public partial class AlwaysOnTopMain : Form
    {
        bool firstLaunch = true;

        public AlwaysOnTopMain()
        {
            InitializeComponent();

            foreach (string s in Settings.Default.Filters)
                lstFilters.Items.Add(s);

            RefreshWindows();

            mnuRunAtStartup.Checked = RunAtStartup.IsRunAtStartup();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtFilter.Text.Length == 0)
                return;

            if (Settings.Default.Filters == null)
                Settings.Default.Filters = new System.Collections.Specialized.StringCollection();

            Settings.Default.Filters.Add(txtFilter.Text);
            Settings.Default.Save();

            lstFilters.Items.Add(txtFilter.Text);
            txtFilter.Text = "";
            RefreshWindows();
            User32.RemoveAlwaysOnTop();
            MakeWindowsAlwaysOnTop();
        }

        private void lstFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = (lstFilters.SelectedIndex >= 0);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstFilters.SelectedIndex >= 0)
            {
                var index = lstFilters.SelectedIndex;
                lstFilters.Items.RemoveAt(index);
                Settings.Default.Filters.RemoveAt(index);
                Settings.Default.Save();
                RefreshWindows();
                User32.RemoveAlwaysOnTop();
                MakeWindowsAlwaysOnTop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            MessageBox.Show(this, "Saved", "Always on Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void RefreshWindows()
        {
            List<string> windows = User32.GetWindowTitles();
            lstWindows.Items.Clear();
            foreach (string s in windows)
                lstWindows.Items.Add(s);

            foreach(string w in windows)
            {
                foreach (string f in Settings.Default.Filters)
                    if (w.Contains(f))
                    {
                        lstWindows.SelectedItems.Add(w);
                    }
            }
        }

        private void tmrCheckWindows_Tick(object sender, EventArgs e)
        {
            MakeWindowsAlwaysOnTop();
        }

        private void MakeWindowsAlwaysOnTop()
        {
            foreach (string f in Settings.Default.Filters)
                User32.AlwaysOnTop(f);

            User32.AlwaysOnTop(this.Text);
        }

        private void AlwaysOnTopMain_Shown(object sender, EventArgs e)
        {
            tmrCheckWindows.Enabled = false;

            if (firstLaunch)
            {
                this.Hide();
                this.notifyIcon1.ShowBalloonTip(3000, "Always on Top", "Don't worry. I am running here, working hard to make your windows appear always on top.", ToolTipIcon.Info);
                firstLaunch = false;
            }
        }

        private void AlwaysOnTopMain_VisibleChanged(object sender, EventArgs e)
        {
            tmrCheckWindows.Enabled = !this.Visible;

            MakeWindowsAlwaysOnTop();

            if (this.Visible) 
                this.Activate();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "mnuExit")
                Exit();
            else if (e.ClickedItem.Name == "mnuShow")
                this.Show();
            else if (e.ClickedItem.Name == "mnuRunAtStartup")
            {
                mnuRunAtStartup.Checked = !mnuRunAtStartup.Checked;
                RunAtStartup.SetStartup(mnuRunAtStartup.Checked);
            }
        }

        private void Exit()
        {
            if (MessageBox.Show(this, "Are you sure you want to stop making windows always on top?", "Always on top", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                tmrCheckWindows.Enabled = false;
                this.Close();
                User32.RemoveAlwaysOnTop();
                Application.Exit();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
