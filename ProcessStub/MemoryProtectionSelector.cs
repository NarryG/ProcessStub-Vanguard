﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.NetCore.StaticTools;
using RTCV.ProcessCorrupt;
using RTCV.UI;

namespace ProcessStub
{
	public partial class MemoryProtectionSelector : Form, IAutoColorize
	{
        public MemoryProtectionSelector()
		{
			InitializeComponent();
			FormClosing += MemoryProtectionSelector_Closing;
            Load += MemoryProtectionSelector_Load;
            UICore.SetRTCColor(Color.FromArgb(149, 120, 161), this);
        }

        private void MemoryProtectionSelector_Load(object sender, EventArgs e)
        {
            tablePanel.Controls.Clear();
            foreach (var t in Enum.GetNames(typeof(ProcessExtensions.MemoryProtection)).Where(x => x != ProcessExtensions.MemoryProtection.NoAccess.ToString() && x != ProcessExtensions.MemoryProtection.ZeroAccess.ToString() && x != ProcessExtensions.MemoryProtection.Empty.ToString()))
            {
                CheckBox cb = new CheckBox
                {
                    AutoSize = true,
                    Text = t,
                    Name = t,
                    Checked = (ProcessWatch.ProtectMode & (ProcessExtensions.MemoryProtection)Enum.Parse(typeof(ProcessExtensions.MemoryProtection), t)) >= (ProcessExtensions.MemoryProtection)Enum.Parse(typeof(ProcessExtensions.MemoryProtection), t)
                };
                tablePanel.Controls.Add(cb);
            }
            Show();
        }

        private void MemoryProtectionSelector_Closing(object sender, FormClosingEventArgs e)
		{
			if (tablePanel.Controls.Cast<CheckBox>().Count(item => item.Checked) == 0)
			{
				e.Cancel = true;
				MessageBox.Show("Select at least one type of memory protection");
				return;
            }

            ProcessExtensions.MemoryProtection a = ProcessExtensions.MemoryProtection.ZeroAccess;
            foreach (CheckBox cb in tablePanel.Controls.Cast<CheckBox>().Where(item => item.Checked)) 
                a = a | (ProcessExtensions.MemoryProtection)Enum.Parse(typeof(ProcessExtensions.MemoryProtection), cb.Text);

            ProcessWatch.ProtectMode = a;
            Params.SetParam("PROTECTIONMODE", ((uint)a).ToString());
            ProcessWatch.UpdateDomains();
        }
    }
}
