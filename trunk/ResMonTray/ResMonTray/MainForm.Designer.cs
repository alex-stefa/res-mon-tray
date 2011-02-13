namespace ResMonTray
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.resTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.resTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.resSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.physicalMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuTotalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuMaxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.resShowIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resShowResToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resShowCpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.res1SecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.res2SecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.res3SecToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.res5SecToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.res10SecToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.res30SecToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.resOtherSecToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.resAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.resQuitMonitoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.cpuShowIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuShowResToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuShowCpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.cpuQuitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.cpuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.cpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cpuTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cpuTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu1SecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu2SecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu3SecToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu5SecToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu10SecToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cpu30SecToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuOtherSecToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.resTrayMenu.SuspendLayout();
            this.cpuTrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // resTrayIcon
            // 
            this.resTrayIcon.ContextMenuStrip = this.resTrayMenu;
            this.resTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("resTrayIcon.Icon")));
            this.resTrayIcon.Visible = true;
            this.resTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.resTrayIcon_MouseClick);
            // 
            // resTrayMenu
            // 
            this.resTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resTitleToolStripMenuItem,
            this.resSep1,
            this.resSep2,
            this.physicalMemoryToolStripMenuItem,
            this.pageFileToolStripMenuItem,
            this.cpuTotalToolStripMenuItem,
            this.cpuMaxToolStripMenuItem,
            this.resSep3,
            this.resShowIconsToolStripMenuItem,
            this.resRefreshToolStripMenuItem,
            this.resAboutToolStripMenuItem,
            this.resSep4,
            this.resQuitMonitoringToolStripMenuItem});
            this.resTrayMenu.Name = "resTrayMenu";
            this.resTrayMenu.Size = new System.Drawing.Size(186, 226);
            this.resTrayMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.resTrayMenu_Closing);
            this.resTrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.resTrayMenu_Opening);
            // 
            // resTitleToolStripMenuItem
            // 
            this.resTitleToolStripMenuItem.Enabled = false;
            this.resTitleToolStripMenuItem.Name = "resTitleToolStripMenuItem";
            this.resTitleToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.resTitleToolStripMenuItem.Text = "RAM && CPU Monitor";
            // 
            // resSep1
            // 
            this.resSep1.Name = "resSep1";
            this.resSep1.Size = new System.Drawing.Size(182, 6);
            // 
            // resSep2
            // 
            this.resSep2.Name = "resSep2";
            this.resSep2.Size = new System.Drawing.Size(182, 6);
            // 
            // physicalMemoryToolStripMenuItem
            // 
            this.physicalMemoryToolStripMenuItem.Name = "physicalMemoryToolStripMenuItem";
            this.physicalMemoryToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.physicalMemoryToolStripMenuItem.Text = "Physical Memory:";
            // 
            // pageFileToolStripMenuItem
            // 
            this.pageFileToolStripMenuItem.Name = "pageFileToolStripMenuItem";
            this.pageFileToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.pageFileToolStripMenuItem.Text = "Page File:";
            // 
            // cpuTotalToolStripMenuItem
            // 
            this.cpuTotalToolStripMenuItem.Name = "cpuTotalToolStripMenuItem";
            this.cpuTotalToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.cpuTotalToolStripMenuItem.Text = "CPU Total:";
            // 
            // cpuMaxToolStripMenuItem
            // 
            this.cpuMaxToolStripMenuItem.Name = "cpuMaxToolStripMenuItem";
            this.cpuMaxToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.cpuMaxToolStripMenuItem.Text = "CPU Max:";
            // 
            // resSep3
            // 
            this.resSep3.Name = "resSep3";
            this.resSep3.Size = new System.Drawing.Size(182, 6);
            // 
            // resShowIconsToolStripMenuItem
            // 
            this.resShowIconsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resShowResToolStripMenuItem,
            this.resShowCpuToolStripMenuItem});
            this.resShowIconsToolStripMenuItem.Name = "resShowIconsToolStripMenuItem";
            this.resShowIconsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.resShowIconsToolStripMenuItem.Text = "Show &Monitors";
            // 
            // resShowResToolStripMenuItem
            // 
            this.resShowResToolStripMenuItem.Checked = true;
            this.resShowResToolStripMenuItem.CheckOnClick = true;
            this.resShowResToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resShowResToolStripMenuItem.Name = "resShowResToolStripMenuItem";
            this.resShowResToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.resShowResToolStripMenuItem.Text = "RAM && CPU";
            this.resShowResToolStripMenuItem.Click += new System.EventHandler(this.resShowResToolStripMenuItem_Click);
            // 
            // resShowCpuToolStripMenuItem
            // 
            this.resShowCpuToolStripMenuItem.Checked = true;
            this.resShowCpuToolStripMenuItem.CheckOnClick = true;
            this.resShowCpuToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resShowCpuToolStripMenuItem.Name = "resShowCpuToolStripMenuItem";
            this.resShowCpuToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.resShowCpuToolStripMenuItem.Text = "CPU Cores";
            this.resShowCpuToolStripMenuItem.Click += new System.EventHandler(this.resShowCpuToolStripMenuItem_Click);
            // 
            // resRefreshToolStripMenuItem
            // 
            this.resRefreshToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.res1SecToolStripMenuItem,
            this.res2SecToolStripMenuItem,
            this.res3SecToolStripMenuItem1,
            this.res5SecToolStripMenuItem2,
            this.res10SecToolStripMenuItem3,
            this.res30SecToolStripMenuItem4,
            this.resOtherSecToolStripMenuItem});
            this.resRefreshToolStripMenuItem.Name = "resRefreshToolStripMenuItem";
            this.resRefreshToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.resRefreshToolStripMenuItem.Text = "&Refresh Interval";
            // 
            // res1SecToolStripMenuItem
            // 
            this.res1SecToolStripMenuItem.Checked = true;
            this.res1SecToolStripMenuItem.CheckOnClick = true;
            this.res1SecToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.res1SecToolStripMenuItem.Name = "res1SecToolStripMenuItem";
            this.res1SecToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.res1SecToolStripMenuItem.Tag = "1";
            this.res1SecToolStripMenuItem.Text = "1 second";
            this.res1SecToolStripMenuItem.Click += new System.EventHandler(this.res1SecToolStripMenuItem_Click);
            // 
            // res2SecToolStripMenuItem
            // 
            this.res2SecToolStripMenuItem.CheckOnClick = true;
            this.res2SecToolStripMenuItem.Name = "res2SecToolStripMenuItem";
            this.res2SecToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.res2SecToolStripMenuItem.Tag = "2";
            this.res2SecToolStripMenuItem.Text = "2 seconds";
            this.res2SecToolStripMenuItem.Click += new System.EventHandler(this.res2SecToolStripMenuItem_Click);
            // 
            // res3SecToolStripMenuItem1
            // 
            this.res3SecToolStripMenuItem1.CheckOnClick = true;
            this.res3SecToolStripMenuItem1.Name = "res3SecToolStripMenuItem1";
            this.res3SecToolStripMenuItem1.Size = new System.Drawing.Size(212, 22);
            this.res3SecToolStripMenuItem1.Tag = "3";
            this.res3SecToolStripMenuItem1.Text = "3 seconds";
            this.res3SecToolStripMenuItem1.Click += new System.EventHandler(this.res3SecToolStripMenuItem1_Click);
            // 
            // res5SecToolStripMenuItem2
            // 
            this.res5SecToolStripMenuItem2.CheckOnClick = true;
            this.res5SecToolStripMenuItem2.Name = "res5SecToolStripMenuItem2";
            this.res5SecToolStripMenuItem2.Size = new System.Drawing.Size(212, 22);
            this.res5SecToolStripMenuItem2.Tag = "5";
            this.res5SecToolStripMenuItem2.Text = "5 seconds";
            this.res5SecToolStripMenuItem2.Click += new System.EventHandler(this.res5SecToolStripMenuItem2_Click);
            // 
            // res10SecToolStripMenuItem3
            // 
            this.res10SecToolStripMenuItem3.CheckOnClick = true;
            this.res10SecToolStripMenuItem3.Name = "res10SecToolStripMenuItem3";
            this.res10SecToolStripMenuItem3.Size = new System.Drawing.Size(212, 22);
            this.res10SecToolStripMenuItem3.Tag = "10";
            this.res10SecToolStripMenuItem3.Text = "10 seconds";
            this.res10SecToolStripMenuItem3.Click += new System.EventHandler(this.res10SecToolStripMenuItem3_Click);
            // 
            // res30SecToolStripMenuItem4
            // 
            this.res30SecToolStripMenuItem4.CheckOnClick = true;
            this.res30SecToolStripMenuItem4.Name = "res30SecToolStripMenuItem4";
            this.res30SecToolStripMenuItem4.Size = new System.Drawing.Size(212, 22);
            this.res30SecToolStripMenuItem4.Tag = "30";
            this.res30SecToolStripMenuItem4.Text = "30 seconds";
            this.res30SecToolStripMenuItem4.Click += new System.EventHandler(this.res30SecToolStripMenuItem4_Click);
            // 
            // resOtherSecToolStripMenuItem
            // 
            this.resOtherSecToolStripMenuItem.Name = "resOtherSecToolStripMenuItem";
            this.resOtherSecToolStripMenuItem.Size = new System.Drawing.Size(152, 23);
            this.resOtherSecToolStripMenuItem.Text = "Other";
            this.resOtherSecToolStripMenuItem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.resOtherSecToolStripMenuItem_KeyPress);
            this.resOtherSecToolStripMenuItem.Click += new System.EventHandler(this.resOtherSecToolStripMenuItem_Click);
            // 
            // resAboutToolStripMenuItem
            // 
            this.resAboutToolStripMenuItem.Image = global::ResMonTray.Properties.Resources.Info;
            this.resAboutToolStripMenuItem.Name = "resAboutToolStripMenuItem";
            this.resAboutToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.resAboutToolStripMenuItem.Text = "&About";
            this.resAboutToolStripMenuItem.Click += new System.EventHandler(this.resAboutToolStripMenuItem_Click);
            // 
            // resSep4
            // 
            this.resSep4.Name = "resSep4";
            this.resSep4.Size = new System.Drawing.Size(182, 6);
            // 
            // resQuitMonitoringToolStripMenuItem
            // 
            this.resQuitMonitoringToolStripMenuItem.Image = global::ResMonTray.Properties.Resources.quit;
            this.resQuitMonitoringToolStripMenuItem.Name = "resQuitMonitoringToolStripMenuItem";
            this.resQuitMonitoringToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.resQuitMonitoringToolStripMenuItem.Text = "&Quit Monitoring";
            this.resQuitMonitoringToolStripMenuItem.Click += new System.EventHandler(this.resQuitMonitoringToolStripMenuItem_Click);
            // 
            // cpuSep3
            // 
            this.cpuSep3.Name = "cpuSep3";
            this.cpuSep3.Size = new System.Drawing.Size(173, 6);
            // 
            // cpuShowIconsToolStripMenuItem
            // 
            this.cpuShowIconsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cpuShowResToolStripMenuItem,
            this.cpuShowCpuToolStripMenuItem});
            this.cpuShowIconsToolStripMenuItem.Name = "cpuShowIconsToolStripMenuItem";
            this.cpuShowIconsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cpuShowIconsToolStripMenuItem.Text = "Show &Monitors";
            // 
            // cpuShowResToolStripMenuItem
            // 
            this.cpuShowResToolStripMenuItem.Checked = true;
            this.cpuShowResToolStripMenuItem.CheckOnClick = true;
            this.cpuShowResToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cpuShowResToolStripMenuItem.Name = "cpuShowResToolStripMenuItem";
            this.cpuShowResToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.cpuShowResToolStripMenuItem.Text = "RAM && CPU";
            this.cpuShowResToolStripMenuItem.Click += new System.EventHandler(this.cpuShowResToolStripMenuItem_Click);
            // 
            // cpuShowCpuToolStripMenuItem
            // 
            this.cpuShowCpuToolStripMenuItem.Checked = true;
            this.cpuShowCpuToolStripMenuItem.CheckOnClick = true;
            this.cpuShowCpuToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cpuShowCpuToolStripMenuItem.Name = "cpuShowCpuToolStripMenuItem";
            this.cpuShowCpuToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.cpuShowCpuToolStripMenuItem.Text = "CPU Cores";
            this.cpuShowCpuToolStripMenuItem.Click += new System.EventHandler(this.cpuShowCpuToolStripMenuItem_Click);
            // 
            // cpuAboutToolStripMenuItem
            // 
            this.cpuAboutToolStripMenuItem.Image = global::ResMonTray.Properties.Resources.Info;
            this.cpuAboutToolStripMenuItem.Name = "cpuAboutToolStripMenuItem";
            this.cpuAboutToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cpuAboutToolStripMenuItem.Text = "&About";
            this.cpuAboutToolStripMenuItem.Click += new System.EventHandler(this.cpuAboutToolStripMenuItem_Click);
            // 
            // cpuSep4
            // 
            this.cpuSep4.Name = "cpuSep4";
            this.cpuSep4.Size = new System.Drawing.Size(173, 6);
            // 
            // cpuQuitToolStripMenuItem
            // 
            this.cpuQuitToolStripMenuItem.Image = global::ResMonTray.Properties.Resources.quit;
            this.cpuQuitToolStripMenuItem.Name = "cpuQuitToolStripMenuItem";
            this.cpuQuitToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cpuQuitToolStripMenuItem.Text = "&Quit Monitoring";
            this.cpuQuitToolStripMenuItem.Click += new System.EventHandler(this.cpuQuitToolStripMenuItem_Click);
            // 
            // cpuSep1
            // 
            this.cpuSep1.Name = "cpuSep1";
            this.cpuSep1.Size = new System.Drawing.Size(173, 6);
            // 
            // cpuSep2
            // 
            this.cpuSep2.Name = "cpuSep2";
            this.cpuSep2.Size = new System.Drawing.Size(173, 6);
            // 
            // cpuTrayIcon
            // 
            this.cpuTrayIcon.ContextMenuStrip = this.cpuTrayMenu;
            this.cpuTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("cpuTrayIcon.Icon")));
            this.cpuTrayIcon.Visible = true;
            this.cpuTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cpuTrayIcon_MouseClick);
            // 
            // cpuTrayMenu
            // 
            this.cpuTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cpuTitleToolStripMenuItem,
            this.cpuSep1,
            this.cpuSep2,
            this.cpuSep3,
            this.cpuShowIconsToolStripMenuItem,
            this.cpuRefreshToolStripMenuItem,
            this.cpuAboutToolStripMenuItem,
            this.cpuSep4,
            this.cpuQuitToolStripMenuItem});
            this.cpuTrayMenu.Name = "cpuTrayMenu";
            this.cpuTrayMenu.Size = new System.Drawing.Size(177, 160);
            this.cpuTrayMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.cpuTrayMenu_Closing);
            this.cpuTrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.cpuTrayMenu_Opening);
            // 
            // cpuTitleToolStripMenuItem
            // 
            this.cpuTitleToolStripMenuItem.Enabled = false;
            this.cpuTitleToolStripMenuItem.Name = "cpuTitleToolStripMenuItem";
            this.cpuTitleToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cpuTitleToolStripMenuItem.Text = "CPU Cores Monitor";
            // 
            // cpuRefreshToolStripMenuItem
            // 
            this.cpuRefreshToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cpu1SecToolStripMenuItem,
            this.cpu2SecToolStripMenuItem,
            this.cpu3SecToolStripMenuItem1,
            this.cpu5SecToolStripMenuItem2,
            this.cpu10SecToolStripMenuItem3,
            this.cpu30SecToolStripMenuItem4,
            this.cpuOtherSecToolStripMenuItem});
            this.cpuRefreshToolStripMenuItem.Name = "cpuRefreshToolStripMenuItem";
            this.cpuRefreshToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cpuRefreshToolStripMenuItem.Text = "&Refresh Interval";
            // 
            // cpu1SecToolStripMenuItem
            // 
            this.cpu1SecToolStripMenuItem.Checked = true;
            this.cpu1SecToolStripMenuItem.CheckOnClick = true;
            this.cpu1SecToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cpu1SecToolStripMenuItem.Name = "cpu1SecToolStripMenuItem";
            this.cpu1SecToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.cpu1SecToolStripMenuItem.Tag = "1";
            this.cpu1SecToolStripMenuItem.Text = "1 second";
            this.cpu1SecToolStripMenuItem.Click += new System.EventHandler(this.cpu1SecToolStripMenuItem_Click);
            // 
            // cpu2SecToolStripMenuItem
            // 
            this.cpu2SecToolStripMenuItem.CheckOnClick = true;
            this.cpu2SecToolStripMenuItem.Name = "cpu2SecToolStripMenuItem";
            this.cpu2SecToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.cpu2SecToolStripMenuItem.Tag = "2";
            this.cpu2SecToolStripMenuItem.Text = "2 seconds";
            this.cpu2SecToolStripMenuItem.Click += new System.EventHandler(this.cpu2SecToolStripMenuItem_Click);
            // 
            // cpu3SecToolStripMenuItem1
            // 
            this.cpu3SecToolStripMenuItem1.CheckOnClick = true;
            this.cpu3SecToolStripMenuItem1.Name = "cpu3SecToolStripMenuItem1";
            this.cpu3SecToolStripMenuItem1.Size = new System.Drawing.Size(212, 22);
            this.cpu3SecToolStripMenuItem1.Tag = "3";
            this.cpu3SecToolStripMenuItem1.Text = "3 seconds";
            this.cpu3SecToolStripMenuItem1.Click += new System.EventHandler(this.cpu3SecToolStripMenuItem1_Click);
            // 
            // cpu5SecToolStripMenuItem2
            // 
            this.cpu5SecToolStripMenuItem2.CheckOnClick = true;
            this.cpu5SecToolStripMenuItem2.Name = "cpu5SecToolStripMenuItem2";
            this.cpu5SecToolStripMenuItem2.Size = new System.Drawing.Size(212, 22);
            this.cpu5SecToolStripMenuItem2.Tag = "5";
            this.cpu5SecToolStripMenuItem2.Text = "5 seconds";
            this.cpu5SecToolStripMenuItem2.Click += new System.EventHandler(this.cpu5SecToolStripMenuItem2_Click);
            // 
            // cpu10SecToolStripMenuItem3
            // 
            this.cpu10SecToolStripMenuItem3.CheckOnClick = true;
            this.cpu10SecToolStripMenuItem3.Name = "cpu10SecToolStripMenuItem3";
            this.cpu10SecToolStripMenuItem3.Size = new System.Drawing.Size(212, 22);
            this.cpu10SecToolStripMenuItem3.Tag = "10";
            this.cpu10SecToolStripMenuItem3.Text = "10 seconds";
            this.cpu10SecToolStripMenuItem3.Click += new System.EventHandler(this.cpu10SecToolStripMenuItem3_Click);
            // 
            // cpu30SecToolStripMenuItem4
            // 
            this.cpu30SecToolStripMenuItem4.CheckOnClick = true;
            this.cpu30SecToolStripMenuItem4.Name = "cpu30SecToolStripMenuItem4";
            this.cpu30SecToolStripMenuItem4.Size = new System.Drawing.Size(212, 22);
            this.cpu30SecToolStripMenuItem4.Tag = "30";
            this.cpu30SecToolStripMenuItem4.Text = "30 seconds";
            this.cpu30SecToolStripMenuItem4.Click += new System.EventHandler(this.cpu30SecToolStripMenuItem4_Click);
            // 
            // cpuOtherSecToolStripMenuItem
            // 
            this.cpuOtherSecToolStripMenuItem.Name = "cpuOtherSecToolStripMenuItem";
            this.cpuOtherSecToolStripMenuItem.Size = new System.Drawing.Size(152, 23);
            this.cpuOtherSecToolStripMenuItem.Text = "Other";
            this.cpuOtherSecToolStripMenuItem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cpuOtherSecToolStripMenuItem_KeyPress);
            this.cpuOtherSecToolStripMenuItem.Click += new System.EventHandler(this.cpuOtherSecToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 219);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.resTrayMenu.ResumeLayout(false);
            this.cpuTrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon resTrayIcon;
        private System.Windows.Forms.NotifyIcon cpuTrayIcon;
        private System.Windows.Forms.ContextMenuStrip resTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem resTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator cpuSep1;
        private System.Windows.Forms.ToolStripMenuItem physicalMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pageFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuTotalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuMaxToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator cpuSep3;
        private System.Windows.Forms.ToolStripMenuItem cpuShowIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuShowResToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuShowCpuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator cpuSep4;
        private System.Windows.Forms.ToolStripMenuItem cpuQuitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cpuTrayMenu;
        private System.Windows.Forms.ToolStripSeparator cpuSep2;
        private System.Windows.Forms.ToolStripMenuItem cpuTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator resSep1;
        private System.Windows.Forms.ToolStripSeparator resSep2;
        private System.Windows.Forms.ToolStripSeparator resSep3;
        private System.Windows.Forms.ToolStripMenuItem resShowIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resShowResToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resShowCpuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resQuitMonitoringToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator resSep4;
        private System.Windows.Forms.ToolStripMenuItem resRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem res1SecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem res2SecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem res3SecToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem res5SecToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem res10SecToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem res30SecToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cpuRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpu1SecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpu2SecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpu3SecToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cpu5SecToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cpu10SecToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem cpu30SecToolStripMenuItem4;
        private System.Windows.Forms.ToolStripTextBox resOtherSecToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox cpuOtherSecToolStripMenuItem;
    }
}

