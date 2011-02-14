using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ResMonTray
{
    public class MainApp : ApplicationContext
    {
        private const int maxAllowedCores = 8;

        private int refreshInterval;
        private int historySize;
        private int plottedCores;
        private bool showResTray, showCpuTray;
        private bool shuttingDown;
        private bool keepResMenuOpen, keepCpuMenuOpen;

        private LinkedList<PerformanceDataItem> history;
        private PerformanceDataProvider dataProvider;
        private GraphicsProvider graphicsProvider;
        private BackgroundWorker worker;
        private ImageToolStripMenuItem resGraphItem, cpuGraphItem;
        private List<ToolStripMenuItem> coresMenuItems;
        private Icon resIcon, cpuIcon;

        public MainApp()
        {
            InitializeComponent();

            InitializeTrays();

            shuttingDown = false;
            keepResMenuOpen = false;
            keepCpuMenuOpen = false;
            dataProvider = new PerformanceDataProvider();
            plottedCores = Math.Min(maxAllowedCores, dataProvider.coreCount);
            graphicsProvider = new GraphicsProvider(plottedCores);
            history = new LinkedList<PerformanceDataItem>();
            historySize = Properties.Settings.Default.PlotItems;
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerSupportsCancellation = false;
            refreshInterval = Properties.Settings.Default.RefreshInterval;
            SetRefreshInterval(refreshInterval);

            InitializeMenus();

            worker.RunWorkerAsync();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (shuttingDown)
            {
                DoShutdown();
                return;
            }

            UpdateLabels(false, false);
            UpdateIcons();

            if (resTrayMenu.Visible) resGraphItem.Invalidate();
            if (cpuTrayMenu.Visible) cpuGraphItem.Invalidate();

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            long lastUpdate = DateTime.Now.Ticks;

            history.AddFirst(dataProvider.GetNextItem());
            if (history.Count > historySize) history.RemoveLast();

            if (showResTray) resIcon = graphicsProvider.GetResTrayIcon(dataProvider, history);
            if (showCpuTray) cpuIcon = graphicsProvider.GetCpuTrayIcon(dataProvider, history);

            if (resTrayMenu.Visible) resGraphItem.DisplayImage = graphicsProvider.GetResGraph(dataProvider, history);
            if (cpuTrayMenu.Visible) cpuGraphItem.DisplayImage = graphicsProvider.GetCpuGraph(dataProvider, history);

            long now = DateTime.Now.Ticks;

            //Console.WriteLine((now - lastUpdate) / 10000);

            int sleepInterval = (int)(refreshInterval * 1000 - (now - lastUpdate) / 10000);
            if (sleepInterval < 500) sleepInterval = 500;
            Thread.Sleep(sleepInterval);
        }

        private void InitializeMenus()
        {
            resTrayIcon.BalloonTipTitle = "RAM & CPU Monitor";
            cpuTrayIcon.BalloonTipTitle = "CPU Cores Monitor";
            resTrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            cpuTrayIcon.BalloonTipIcon = ToolTipIcon.Info;

            resGraphItem = new ImageToolStripMenuItem();
            cpuGraphItem = new ImageToolStripMenuItem();
            resTrayMenu.Items.Insert(2, resGraphItem);
            cpuTrayMenu.Items.Insert(2, cpuGraphItem);

            if (dataProvider.coreCount == 1) cpuMaxToolStripMenuItem.Visible = false;

            physicalMemoryToolStripMenuItem.Image = GraphicsProvider.GetSolidColorImage(Properties.Settings.Default.RamColor);
            pageFileToolStripMenuItem.Image = GraphicsProvider.GetSolidColorImage(Properties.Settings.Default.SwapColor);
            cpuTotalToolStripMenuItem.Image = GraphicsProvider.GetSolidColorImage(Properties.Settings.Default.CpuTotalColor);
            cpuMaxToolStripMenuItem.Image = GraphicsProvider.GetSolidColorImage(Properties.Settings.Default.CpuMaxColor);

            resGraphItem.MouseDown += PreventResMenuClosingOnClickHandler;
            cpuGraphItem.MouseDown += PreventCpuMenuClosingOnClickHandler;

            physicalMemoryToolStripMenuItem.MouseDown += PreventResMenuClosingOnClickHandler;
            pageFileToolStripMenuItem.MouseDown += PreventResMenuClosingOnClickHandler;
            cpuTotalToolStripMenuItem.MouseDown += PreventResMenuClosingOnClickHandler;
            cpuMaxToolStripMenuItem.MouseDown += PreventResMenuClosingOnClickHandler;

            coresMenuItems = new List<ToolStripMenuItem>(plottedCores);
            for (int i = 0; i < plottedCores; i++)
            {
                //ToolStripMenuItem menuItem = new ToolStripMenuItem("CPU " + (i + 1));
                ToolStripMenuItem menuItem = new ProgressToolStripMenuItem();
                menuItem.MouseDown += PreventCpuMenuClosingOnClickHandler;
                menuItem.Image = GraphicsProvider.GetSolidColorImage((Color)Properties.Settings.Default["Cpu" + (i + 1) + "Color"]);
                menuItem.Height = 22;
                cpuTrayMenu.Items.Insert(4 + i, menuItem);
                coresMenuItems.Add(menuItem);
            }

            resGraphItem.AutoSize = false;
            resGraphItem.Height = Properties.Settings.Default.PlotHeight;
            resGraphItem.Width = Properties.Settings.Default.PlotWidth;
            resTrayMenu.AutoSize = false;
            int resHeight = 4;
            foreach (ToolStripItem item in resTrayMenu.Items)
            {
                item.AutoSize = false;
                item.Width = Properties.Settings.Default.PlotWidth;
                //item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                //item.Dock = DockStyle.Fill;
                resHeight += item.Height;
            }
            resTrayMenu.Width = Properties.Settings.Default.PlotWidth;
            if (dataProvider.coreCount == 1) resHeight -= cpuMaxToolStripMenuItem.Height;
            resTrayMenu.Height = resHeight;

            cpuGraphItem.AutoSize = false;
            cpuGraphItem.Height = Properties.Settings.Default.PlotHeight;
            cpuGraphItem.Width = Properties.Settings.Default.PlotWidth;
            cpuTrayMenu.AutoSize = false;
            int cpuHeight = 4;
            foreach (ToolStripItem item in cpuTrayMenu.Items)
            {
                item.AutoSize = false;
                item.Width = Properties.Settings.Default.PlotWidth;
                //item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                //item.Dock = DockStyle.Fill;
                cpuHeight += item.Height;
            }
            cpuTrayMenu.Width = Properties.Settings.Default.PlotWidth;
            cpuTrayMenu.Height = cpuHeight;
        }

        private void UpdateLabels(bool forceResUpdate, bool forceCpuUpdate)
        {
            if (history.Count == 0) return;

            PerformanceDataItem data = history.First();

            if (resTrayMenu.Visible || forceResUpdate)
            {
                physicalMemoryToolStripMenuItem.Text = "RAM free: " + data.availableRam + "/" + dataProvider.ramSize + "MB (" +
                    (int)(data.availableRam * 100.0f / dataProvider.ramSize) + "%)";
                pageFileToolStripMenuItem.Text = "Swap free: " + (data.totalPageFile - data.usedPageFile) + "/" + data.totalPageFile + "MB (" +
                    (int)((data.totalPageFile - data.usedPageFile) * 100.0f / data.totalPageFile) + "%)";
                cpuTotalToolStripMenuItem.Text = "CPU Total Load: " + data.cpuTotal + "% @ " + dataProvider.cpuFrequency + "MHz";
                cpuMaxToolStripMenuItem.Text = "CPU Max Load: " + data.cores[data.GetMaxIndex()] + "% Core #" + (data.GetMaxIndex() + 1) + " of " + dataProvider.coreCount;
            }

            resTrayIcon.BalloonTipText = "\nCPU Total " + data.cpuTotal + "%\nRAM free: " + data.availableRam + "MB / " + dataProvider.ramSize + "MB\nSwap free: " +
                (data.totalPageFile - data.usedPageFile) + "MB / " + data.totalPageFile + "MB\n\n Right-click for details..";
            resTrayIcon.Text = "ResMonTray " + data.cpuTotal + "%\nRAM fr: " + data.availableRam + "MB / " + dataProvider.ramSize + "MB\nSwap fr: " +
                (data.totalPageFile - data.usedPageFile) + "MB / " + data.totalPageFile + "MB";

            if (cpuTrayMenu.Visible || forceCpuUpdate)
            {
                for (int i = 0; i < plottedCores; i++)
                    coresMenuItems[i].Text = "CPU Core #" + (i + 1) + " Load: " + data.cores[i] + "%";
            }

            string text = "\n@ " + dataProvider.cpuFrequency + "MHz";
            for (int i = 0; i < plottedCores; i++)
                text += "\nCPU #" + (i + 1) + " " + data.cores[i] + "%";
            text += "\n\n Right-click for details..";
            cpuTrayIcon.BalloonTipText = text;

            if (dataProvider.coreCount == 1)
                cpuTrayIcon.Text = "ResMonTray @ " + dataProvider.cpuFrequency + "MHz\nCPU load: " + data.cores[0] + "%";
            else
                cpuTrayIcon.Text = "ResMonTray @ " + dataProvider.cpuFrequency + "MHz x " + dataProvider.coreCount +
                    "\nCPU max load " + data.cores[data.GetMaxIndex()] + "% core #" + (data.GetMaxIndex() + 1);
        }

        private void UpdateIcons()
        {
            if (showResTray && resIcon != null)
            {
                resTrayIcon.Icon = resIcon;
                DestroyIcon(resIcon.Handle);
            }
            if (showCpuTray && cpuIcon != null)
            {
                cpuTrayIcon.Icon = cpuIcon;
                DestroyIcon(cpuIcon.Handle);
            }
        }

        private void InitializeTrays()
        {
            this.showResTray = Properties.Settings.Default.ShowResIcon;
            this.showCpuTray = Properties.Settings.Default.ShowCpuIcon;
            if (!showResTray && !showCpuTray) InitializeShutdown();
            this.resTrayIcon.Visible = showResTray;
            this.cpuTrayIcon.Visible = showCpuTray;
            this.resShowResToolStripMenuItem.Checked = showResTray;
            this.cpuShowResToolStripMenuItem.Checked = showResTray;
            this.resShowCpuToolStripMenuItem.Checked = showCpuTray;
            this.cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
        }

        private void SetResTrayVisible(bool isVisible)
        {
            if (showResTray == isVisible) return;

            if (isVisible)
            {
                showResTray = true;
                resTrayIcon.Visible = true;
                Properties.Settings.Default.ShowResIcon = true;
                cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                cpuShowCpuToolStripMenuItem.Enabled = true;
                resShowCpuToolStripMenuItem.Checked = showCpuTray;
                resShowCpuToolStripMenuItem.Enabled = true;
                cpuShowResToolStripMenuItem.Checked = showResTray;
                cpuShowResToolStripMenuItem.Enabled = true;
                resShowResToolStripMenuItem.Checked = showResTray;
                resShowResToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (showCpuTray)
                {
                    showResTray = false;
                    resTrayIcon.Visible = false;
                    Properties.Settings.Default.ShowResIcon = false;
                    cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                    cpuShowCpuToolStripMenuItem.Enabled = false;
                    resShowCpuToolStripMenuItem.Checked = showCpuTray;
                    resShowCpuToolStripMenuItem.Enabled = false;
                    cpuShowResToolStripMenuItem.Checked = showResTray;
                    cpuShowResToolStripMenuItem.Enabled = true;
                    resShowResToolStripMenuItem.Checked = showResTray;
                    resShowResToolStripMenuItem.Enabled = true;
                }
                else
                {
                    showResTray = true;
                    resTrayIcon.Visible = true;
                    Properties.Settings.Default.ShowResIcon = true;
                    cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                    cpuShowCpuToolStripMenuItem.Enabled = true;
                    resShowCpuToolStripMenuItem.Checked = showCpuTray;
                    resShowCpuToolStripMenuItem.Enabled = true;
                    cpuShowResToolStripMenuItem.Checked = showResTray;
                    cpuShowResToolStripMenuItem.Enabled = false;
                    resShowResToolStripMenuItem.Checked = showResTray;
                    resShowResToolStripMenuItem.Enabled = false;
                }
            }

            Properties.Settings.Default.Save();
        }

        private void SetCpuTrayVisible(bool isVisible)
        {
            if (showCpuTray == isVisible) return;

            if (isVisible)
            {
                showCpuTray = true;
                cpuTrayIcon.Visible = true;
                Properties.Settings.Default.ShowCpuIcon = true;
                cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                cpuShowCpuToolStripMenuItem.Enabled = true;
                resShowCpuToolStripMenuItem.Checked = showCpuTray;
                resShowCpuToolStripMenuItem.Enabled = true;
                cpuShowResToolStripMenuItem.Checked = showResTray;
                cpuShowResToolStripMenuItem.Enabled = true;
                resShowResToolStripMenuItem.Checked = showResTray;
                resShowResToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (showResTray)
                {
                    showCpuTray = false;
                    cpuTrayIcon.Visible = false;
                    Properties.Settings.Default.ShowCpuIcon = false;
                    cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                    cpuShowCpuToolStripMenuItem.Enabled = true;
                    resShowCpuToolStripMenuItem.Checked = showCpuTray;
                    resShowCpuToolStripMenuItem.Enabled = true;
                    cpuShowResToolStripMenuItem.Checked = showResTray;
                    cpuShowResToolStripMenuItem.Enabled = false;
                    resShowResToolStripMenuItem.Checked = showResTray;
                    resShowResToolStripMenuItem.Enabled = false;
                }
                else
                {
                    showCpuTray = true;
                    cpuTrayIcon.Visible = true;
                    Properties.Settings.Default.ShowCpuIcon = true;
                    cpuShowCpuToolStripMenuItem.Checked = showCpuTray;
                    cpuShowCpuToolStripMenuItem.Enabled = false;
                    resShowCpuToolStripMenuItem.Checked = showCpuTray;
                    resShowCpuToolStripMenuItem.Enabled = false;
                    cpuShowResToolStripMenuItem.Checked = showResTray;
                    cpuShowResToolStripMenuItem.Enabled = true;
                    resShowResToolStripMenuItem.Checked = showResTray;
                    resShowResToolStripMenuItem.Enabled = true;
                }
            }

            Properties.Settings.Default.Save();
        }

        private void PreventResMenuClosingOnClickHandler(object sender, EventArgs e)
        {
            keepResMenuOpen = true;
        }

        private void PreventCpuMenuClosingOnClickHandler(object sender, EventArgs e)
        {
            keepCpuMenuOpen = true;
        }

        private void ValidateRefreshInterval(string input)
        {
            int seconds = -1;

            try { seconds = Convert.ToInt32(input); }
            catch (Exception ex) { }

            if (seconds > 0 && seconds <= 3600)
                SetRefreshInterval(seconds);
            else
            {
                MessageBox.Show("Invalid refresh interval!\nPlease specify a number of seconds between 1 and 3600.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetRefreshInterval(1);
            }
        }

        private void SetRefreshInterval(int seconds)
        {
            bool isOther = true;
            foreach (Object item in resRefreshToolStripMenuItem.DropDownItems)
            {
                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem == null) continue;
                menuItem.Checked = (menuItem.Tag.ToString().Equals(seconds.ToString()));
                if (menuItem.Checked) isOther = false;
            }
            if (isOther)
                resOtherSecToolStripMenuItem.Text = "Other: " + seconds + " seconds";
            else
                resOtherSecToolStripMenuItem.Text = "Other..";

            isOther = true;
            foreach (Object item in cpuRefreshToolStripMenuItem.DropDownItems)
            {
                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem == null) continue;
                menuItem.Checked = (menuItem.Tag.ToString().Equals(seconds.ToString()));
                if (menuItem.Checked) isOther = false;
            }
            if (isOther)
                cpuOtherSecToolStripMenuItem.Text = "Other: " + seconds + " seconds";
            else
                cpuOtherSecToolStripMenuItem.Text = "Other..";

            Properties.Settings.Default.RefreshInterval = seconds;
            Properties.Settings.Default.Save();

            refreshInterval = seconds;
            history.Clear();
        }

        private void InitializeShutdown()
        {
            shuttingDown = true;
        }

        private void DoShutdown()
        {
            Properties.Settings.Default.Save();
            graphicsProvider.Dispose();
            this.Dispose(true);
            Application.Exit();
        }

        private void PopAboutBox()
        {
            MessageBox.Show("Memory & CPU Activity Monitor\nv 1.0 beta\n\n(c) 2011\nAlexandru Stefanescu\nalex.stefa@gmail.com",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region event handlers

        private void cpuQuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeShutdown();
        }

        private void resQuitMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeShutdown();
        }

        private void resAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopAboutBox();
        }

        private void cpuAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopAboutBox();
        }

        private void cpu1SecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(1);
        }

        private void cpu2SecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(2);
        }

        private void cpu3SecToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(3);
        }

        private void cpu5SecToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(5);
        }

        private void cpu10SecToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(10);
        }

        private void cpu30SecToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(30);
        }

        private void res1SecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(1);
        }

        private void res2SecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(2);
        }

        private void res3SecToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(3);
        }

        private void res5SecToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(5);
        }

        private void res10SecToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(10);
        }

        private void res30SecToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetRefreshInterval(30);
        }

        private void resShowResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cpuShowResToolStripMenuItem.Checked = resShowResToolStripMenuItem.Checked;
            SetResTrayVisible(resShowResToolStripMenuItem.Checked);
        }

        private void resShowCpuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cpuShowCpuToolStripMenuItem.Checked = resShowCpuToolStripMenuItem.Checked;
            SetCpuTrayVisible(resShowCpuToolStripMenuItem.Checked);
        }

        private void cpuShowResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resShowResToolStripMenuItem.Checked = cpuShowResToolStripMenuItem.Checked;
            SetResTrayVisible(cpuShowResToolStripMenuItem.Checked);
        }

        private void cpuShowCpuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resShowCpuToolStripMenuItem.Checked = cpuShowCpuToolStripMenuItem.Checked;
            SetCpuTrayVisible(cpuShowCpuToolStripMenuItem.Checked);
        }

        private void resOtherSecToolStripMenuItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ValidateRefreshInterval(resOtherSecToolStripMenuItem.Text);
                e.Handled = true;
            }
        }

        private void cpuOtherSecToolStripMenuItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ValidateRefreshInterval(cpuOtherSecToolStripMenuItem.Text);
                e.Handled = true;
            }
        }

        private void cpuOtherSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cpuOtherSecToolStripMenuItem.Text = "";
        }

        private void resOtherSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resOtherSecToolStripMenuItem.Text = "";
        }

        private void resTrayMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateLabels(true, false);
            resGraphItem.DisplayImage = graphicsProvider.GetResGraph(dataProvider, history);
        }

        private void cpuTrayMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateLabels(false, true);
            cpuGraphItem.DisplayImage = graphicsProvider.GetCpuGraph(dataProvider, history);
        }

        private void resTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                resTrayIcon.ShowBalloonTip(3000);
        }

        private void cpuTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                cpuTrayIcon.ShowBalloonTip(3000);
        }

        private void resTrayMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            e.Cancel = keepResMenuOpen;
            keepResMenuOpen = false;
        }

        private void cpuTrayMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            e.Cancel = keepCpuMenuOpen;
            keepCpuMenuOpen = false;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private IContainer components;
        
        #region automagically generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            // 
            // resTrayIcon
            // 
            this.resTrayIcon.ContextMenuStrip = this.resTrayMenu;
            this.resTrayIcon.Icon = Properties.Resources.icon;
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
            this.cpuTrayIcon.Icon = Properties.Resources.icon;
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
            // Main
            // 
            this.resTrayMenu.ResumeLayout(false);
            this.cpuTrayMenu.ResumeLayout(false);
        }

        #endregion

        #region components definitions

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

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class PerformanceDataProvider
    {
        public readonly int ramSize;        // MBytes
        public readonly int cpuFrequency;   // MHz
        public readonly int coreCount;      // logical processors

        public PerformanceDataProvider()
        {
            coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select NumberOfLogicalProcessors from Win32_ComputerSystem").Get())
                coreCount = Convert.ToInt32(item["NumberOfLogicalProcessors"]);

            ramSize = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select Capacity from Win32_PhysicalMemory").Get())
                ramSize += (int)(Convert.ToInt64(item["Capacity"]) >> 20);

            cpuFrequency = 0;
            int freqCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select MaxClockSpeed from Win32_Processor").Get())
            {
                freqCount++;
                cpuFrequency += Convert.ToInt32(item["MaxClockSpeed"]);
            }
            if (freqCount != 0) cpuFrequency /= freqCount;
        }

        public PerformanceDataItem GetNextItem()
        {
            PerformanceDataItem dataItem = new PerformanceDataItem(coreCount);

            dataItem.totalPageFile = 0;
            dataItem.usedPageFile = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select AllocatedBaseSize, CurrentUsage from Win32_PageFileUsage").Get())
            {
                dataItem.totalPageFile += Convert.ToInt32(item["AllocatedBaseSize"]);
                dataItem.usedPageFile += Convert.ToInt32(item["CurrentUsage"]);
            }

            dataItem.availableRam = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select AvailableMBytes from Win32_PerfFormattedData_PerfOS_Memory").Get())
                dataItem.availableRam += Convert.ToInt32(item["AvailableMBytes"]);

            dataItem.cpuTotal = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select Name, PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor").Get())
            {
                if (item["Name"].Equals("_Total"))
                    dataItem.cpuTotal = Convert.ToInt32(item["PercentProcessorTime"]);
                else
                    dataItem.cores.Add(Convert.ToInt32(item["PercentProcessorTime"]));
            }

            return dataItem;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class PerformanceDataItem
    {
        public int availableRam;    // MBytes
        public int usedPageFile;    // MBytes
        public int totalPageFile;   // MBytes
        public int cpuTotal;        // %
        public List<int> cores;     // %

        public PerformanceDataItem(int coreCount)
        {
            cores = new List<int>(coreCount);
        }

        public int GetMaxIndex()
        {
            if (cores.Count < 1)
                return -1;
            int index = 0;
            for (int i = 1; i < cores.Count; i++)
                if (cores[i] > cores[index])
                    index = i;
            return index;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class GraphicsProvider
    {
        private static int menuIconSize = 32;
        private static int trayIconSize = Properties.Settings.Default.IconSize;
        private static int graphWidth = Properties.Settings.Default.PlotWidth;
        private static int graphHeight = Properties.Settings.Default.PlotHeight;

        private static int resBarWidth = 2;
        private static int resBarSpace = 1;
        private static int graphPaddingV = 3;
        private static int graphPaddingH = 7;

        private Bitmap resIcon, cpuIcon;
        private Bitmap resGraph, cpuGraph;

        private Font markerFont;
        private Brush bgBrush, graphAreaBrush, swapBrush, ramBrush, cpuTotalBrush, cpuMaxBrush, markerBrush;
        private Pen gridPen, rectPen, swapPen, ramPen, cpuTotalPen, cpuMaxPen;
        private List<Brush> coreBrushes;
        private List<Pen> corePens;

        private int maxCores;

        public GraphicsProvider(int maxCores)
        {
            this.maxCores = maxCores;

            resIcon = new Bitmap(trayIconSize, trayIconSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            cpuIcon = new Bitmap(trayIconSize, trayIconSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            resGraph = new Bitmap(graphWidth, graphHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            cpuGraph = new Bitmap(graphWidth, graphHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bgBrush = Brushes.Black;
            graphAreaBrush = new SolidBrush(Properties.Settings.Default.IconBackgroundColor);
            swapBrush = new SolidBrush(Properties.Settings.Default.SwapColor);
            ramBrush = new SolidBrush(Properties.Settings.Default.RamColor);
            cpuTotalBrush = new SolidBrush(Properties.Settings.Default.CpuTotalColor);
            cpuMaxBrush = new SolidBrush(Properties.Settings.Default.CpuMaxColor);
            markerBrush = new SolidBrush(Properties.Settings.Default.PlotMarkerColor);

            swapPen = new Pen(swapBrush, 1);
            ramPen = new Pen(ramBrush, 1);
            cpuMaxPen = new Pen(cpuMaxBrush, 1);
            cpuTotalPen = new Pen(cpuTotalBrush, 1);
            gridPen = new Pen(Properties.Settings.Default.PlotGridColor, 1);
            rectPen = new Pen(Properties.Settings.Default.PlotRectColor, 1);

            coreBrushes = new List<Brush>(maxCores);
            corePens = new List<Pen>(maxCores);
            for (int i = 0; i < maxCores; i++)
            {
                Brush brush = new SolidBrush((Color)Properties.Settings.Default["Cpu" + (i + 1) + "Color"]);
                coreBrushes.Add(brush);
                corePens.Add(new Pen(brush, 1));
            }

            markerFont = new Font("Arial", 5);
        }

        public static Image GetSolidColorImage(Color color)
        {
            Bitmap image = new Bitmap(menuIconSize, menuIconSize);
            Graphics graphics = Graphics.FromImage(image);
            Brush brush = new SolidBrush(color);
            graphics.FillRectangle(brush, 0, 0, menuIconSize, menuIconSize);
            graphics.Flush();
            brush.Dispose();
            graphics.Dispose();
            return image;
        }

        public Icon GetResTrayIcon(PerformanceDataProvider provider, LinkedList<PerformanceDataItem> history)
        {
            if (history.Count == 0) return null;
            PerformanceDataItem data = history.First();

            Graphics graphics = Graphics.FromImage(resIcon);

            int halfSize = 2 * resBarWidth + 2 * resBarSpace;
            graphics.FillRectangle(bgBrush, 0, 0, halfSize, trayIconSize);
            graphics.FillRectangle(graphAreaBrush, halfSize, 0, trayIconSize - halfSize, trayIconSize);

            int pageHeight = (int)((float)trayIconSize * (float)data.usedPageFile / data.totalPageFile);
            int ramHeight = (int)((float)trayIconSize * (float)(provider.ramSize - data.availableRam) / provider.ramSize);

            graphics.FillRectangle(swapBrush, 0, trayIconSize - pageHeight, resBarWidth, pageHeight);
            graphics.FillRectangle(ramBrush, resBarSpace + resBarWidth, trayIconSize - ramHeight, resBarWidth, ramHeight);

            int samples = Math.Min(trayIconSize - halfSize, history.Count);
            int index = 0;
            foreach (PerformanceDataItem item in history)
            {
                if (provider.coreCount != 1)
                {
                    int maxBarHeight = (int)((float)trayIconSize * (float)item.cores[item.GetMaxIndex()] / 100.0f + 0.5f);
                    graphics.FillRectangle(cpuMaxBrush, trayIconSize - index - 1, trayIconSize - maxBarHeight, 1, maxBarHeight);
                }

                int totalBarHeight = (int)((float)trayIconSize * (float)item.cpuTotal / 100.0f + 0.5f);
                graphics.FillRectangle(cpuTotalBrush, trayIconSize - index - 1, trayIconSize - totalBarHeight, 1, totalBarHeight);

                if (++index >= samples) break;
            }

            graphics.Flush();
            graphics.Dispose();

            return Icon.FromHandle(resIcon.GetHicon());
        }

        public Icon GetCpuTrayIcon(PerformanceDataProvider provider, LinkedList<PerformanceDataItem> history)
        {
            if (history.Count == 0) return null;
            PerformanceDataItem data = history.First();

            Graphics graphics = Graphics.FromImage(cpuIcon);

            int barWidth = trayIconSize / maxCores;

            graphics.FillRectangle(bgBrush, 0, 0, trayIconSize, trayIconSize);

            for (int i = 0; i < maxCores; i++)
            {
                int barHeight = (int)((float)trayIconSize * (float)data.cores[i] / 100.0f + 0.5f);
                graphics.FillRectangle(coreBrushes[i], barWidth * i, trayIconSize - barHeight, barWidth, barHeight);
            }

            graphics.Flush();
            graphics.Dispose();

            return Icon.FromHandle(cpuIcon.GetHicon());
        }

        public void Dispose()
        {
            graphAreaBrush.Dispose();
            cpuMaxBrush.Dispose();
            cpuTotalBrush.Dispose();
            ramBrush.Dispose();
            swapBrush.Dispose();
            markerBrush.Dispose();

            ramPen.Dispose();
            swapPen.Dispose();
            cpuMaxPen.Dispose();
            cpuTotalPen.Dispose();
            gridPen.Dispose();
            rectPen.Dispose();

            foreach (Brush brush in coreBrushes) brush.Dispose();
            foreach (Pen pen in corePens) pen.Dispose();

            markerFont.Dispose();

            resIcon.Dispose();
            cpuIcon.Dispose();
            resGraph.Dispose();
            cpuGraph.Dispose();
        }

        private RectangleF DrawGrid(Graphics graphics)
        {
            int fullTime = Properties.Settings.Default.PlotItems * Properties.Settings.Default.RefreshInterval;
            int halfTime = fullTime / 2;

            SizeF size100 = graphics.MeasureString("100%", markerFont);
            SizeF size50 = graphics.MeasureString("50%", markerFont);
            SizeF size0 = graphics.MeasureString("0", markerFont);
            SizeF sizeFull = graphics.MeasureString(fullTime + "s", markerFont);
            SizeF sizeHalf = graphics.MeasureString(halfTime + "s", markerFont);

            float gridLeft = graphPaddingH;
            float gridTop = graphPaddingV;
            float gridRight = graphWidth - graphPaddingH - size100.Width - 1;
            float gridBottom = graphHeight - graphPaddingV - size0.Height - 1;

            graphics.DrawString("100%", markerFont, markerBrush, gridRight + 1, graphPaddingV);
            graphics.DrawString("50%", markerFont, markerBrush, gridRight + 1, (gridTop + gridBottom - size50.Height) / 2);
            graphics.DrawString("0", markerFont, markerBrush, gridRight + 1, graphHeight - graphPaddingV - size0.Height);
            graphics.DrawString(halfTime + "s", markerFont, markerBrush, (gridLeft + gridRight - sizeHalf.Width) / 2, graphHeight - graphPaddingV - size0.Height);
            graphics.DrawString(fullTime + "s", markerFont, markerBrush, graphPaddingH, graphHeight - graphPaddingV - size0.Height);

            float gridGapV = (gridBottom - gridTop) / 4;
            for (int i = 1; i < 4; i++)
                graphics.DrawLine(gridPen, gridLeft, gridTop + i * gridGapV, gridRight, gridTop + i * gridGapV);

            float gridGapH = (gridRight - gridLeft) / 6;
            for (int i = 1; i < 6; i++)
                graphics.DrawLine(gridPen, gridLeft + i * gridGapH, gridTop, gridLeft + i * gridGapH, gridBottom);

            graphics.DrawRectangle(rectPen, gridLeft, gridTop, gridRight - gridLeft, gridBottom - gridTop);

            return new RectangleF(gridLeft, gridTop, gridRight - gridLeft, gridBottom - gridTop);
        }

        public Bitmap GetResGraph(PerformanceDataProvider provider, LinkedList<PerformanceDataItem> history)
        {
            Graphics graphics = Graphics.FromImage(resGraph);

            graphics.Clear(Color.White);
            graphics.DrawImage(Properties.Resources.background, 0, 0, graphWidth, graphHeight);

            RectangleF grid = DrawGrid(graphics);

            if (history.Count == 0) goto EndDraw;

            float unitH = (float)grid.Width / Properties.Settings.Default.PlotItems;
            float unitV = (float)grid.Height / Properties.Settings.Default.PlotItems;

            PointF[] ramPoints = new PointF[history.Count];
            PointF[] swapPoints = new PointF[history.Count];
            PointF[] cpuTotalPoints = new PointF[history.Count];
            PointF[] cpuMaxPoints = new PointF[history.Count];

            int index = 0;
            foreach (PerformanceDataItem data in history)
            {
                float ramPerc = (float)(provider.ramSize - data.availableRam) / provider.ramSize * 100.0f;
                float swapPerc = (float)data.usedPageFile / data.totalPageFile * 100.0f;

                ramPoints[index] = new PointF(grid.Right - index * unitH, grid.Bottom - ramPerc * unitV);
                swapPoints[index] = new PointF(grid.Right - index * unitH, grid.Bottom - swapPerc * unitV);
                cpuTotalPoints[index] = new PointF(grid.Right - index * unitH, grid.Bottom - data.cpuTotal * unitV);
                cpuMaxPoints[index] = new PointF(grid.Right - index * unitH, grid.Bottom - data.cores[data.GetMaxIndex()] * unitV);
                index++;
            }

            if (history.Count == 1)
            {
                graphics.DrawRectangle(ramPen, ramPoints[0].X, ramPoints[0].Y, 1, 1);
                graphics.DrawRectangle(swapPen, swapPoints[0].X, swapPoints[0].Y, 1, 1);
                if (provider.coreCount != 1) graphics.DrawRectangle(cpuMaxPen, cpuMaxPoints[0].X, cpuMaxPoints[0].Y, 1, 1);
                graphics.DrawRectangle(cpuTotalPen, cpuTotalPoints[0].X, cpuTotalPoints[0].Y, 1, 1);
            }
            else
            {
                graphics.DrawLines(ramPen, ramPoints);
                graphics.DrawLines(swapPen, swapPoints);
                if (provider.coreCount != 1) graphics.DrawLines(cpuMaxPen, cpuMaxPoints);
                graphics.DrawLines(cpuTotalPen, cpuTotalPoints);
            }

        EndDraw:

            graphics.Flush();
            graphics.Dispose();

            return resGraph;
        }

        public Bitmap GetCpuGraph(PerformanceDataProvider provider, LinkedList<PerformanceDataItem> history)
        {
            if (history.Count == 0) return null;

            Graphics graphics = Graphics.FromImage(cpuGraph);

            graphics.Clear(Color.White);
            graphics.DrawImage(Properties.Resources.background, 0, 0, graphWidth, graphHeight);

            RectangleF grid = DrawGrid(graphics);

            if (history.Count == 0) goto EndDraw;

            float unitH = (float)grid.Width / Properties.Settings.Default.PlotItems;
            float unitV = (float)grid.Height / Properties.Settings.Default.PlotItems;

            PointF[][] points = new PointF[maxCores][];
            for (int i = 0; i < maxCores; i++) points[i] = new PointF[history.Count];

            int index = 0;
            foreach (PerformanceDataItem data in history)
            {
                for (int core = 0; core < maxCores; core++)
                    points[core][index] = new PointF(grid.Right - index * unitH, grid.Bottom - data.cores[core] * unitV);
                index++;
            }

            if (history.Count == 1)
            {
                for (int core = maxCores - 1; core >= 0; core--)
                    graphics.DrawRectangle(corePens[core], points[core][0].X, points[core][0].Y, 1, 1);
            }
            else
            {
                for (int core = maxCores - 1; core >= 0; core--)
                    graphics.DrawLines(corePens[core], points[core]);
            }

        EndDraw:

            graphics.Flush();
            graphics.Dispose();

            return cpuGraph;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class ImageToolStripMenuItem : ToolStripMenuItem
    {
        public Bitmap DisplayImage { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DisplayImage != null)
                e.Graphics.DrawImage(DisplayImage, 0, 0);
            else
                e.Graphics.FillRectangle(Brushes.Transparent, 0, 0, this.Width, this.Height);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class ProgressToolStripMenuItem : ToolStripMenuItem
    {
        public Color ProgressBarColor { get; set; }
        private float ProgressStatus { get; set; }

        public ProgressToolStripMenuItem(string text, Color color) : base(text)
        {
            this.ProgressBarColor = color;
            this.ProgressStatus = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Red, 0, 0, this.Width / 2, this.Height);

            base.OnPaint(e);
        }
    }

}
