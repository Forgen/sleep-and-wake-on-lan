﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Threading;

namespace SleepOnLan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkClass _doWork = new WorkClass();
        private const string SettingsPath = "Settings.ini";
        private Thread workingThread;

        public MainWindow()
        {
            InitializeComponent();

            List<string> list = Settings.Load(SettingsPath);

            if (list.Count == 0)
            {
                list.Add("0");
                list.Add(checkBox1.IsChecked.ToString());
                Settings.Save(SettingsPath, list);
            }

            // Load the id of action which we will do.
            comboBox1.SelectedIndex = Int32.Parse(list[0]);
            checkBox1.IsChecked = Boolean.Parse(list[1]);

            // This thread will be waiting "antimagic" packet.
            workingThread = new Thread(_doWork.DoWork);
            workingThread.Start();
        }

        /// <summary>
        /// If we change the action we should save the changes.
        /// </summary>
        private void comboBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _doWork.State = comboBox1.SelectedIndex;

            SaveSettings();
        }

        private void SaveSettings()
        {
            var list = new List<string>();
            list.Add(comboBox1.SelectedIndex.ToString());
            list.Add(checkBox1.IsChecked.ToString());
            Settings.Save(SettingsPath, list);
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            comboBox1.IsEnabled = false;
            _doWork.ComboBoxIsChecked = Boolean.Parse(checkBox1.IsChecked.ToString());
            SaveSettings();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox1.IsEnabled = true;
            _doWork.ComboBoxIsChecked = Boolean.Parse(checkBox1.IsChecked.ToString());
            SaveSettings();
        }

        /// <summary>
        /// That's our tray icon. We can show and hide main window of our program by clicking on it.
        /// </summary>
        private NotifyIcon TrayIcon = new NotifyIcon();

        /// <summary>
        /// Set parameters and create icon in tray.
        /// </summary>
        private void CreateTrayIcon()
        {
            // Set picture of icon.
            TrayIcon.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.Icon.GetHicon());
            // Set tooltip text on tray icon.
            TrayIcon.Text = "Click to Show/Hide";


            // We will catch left mouse button click.
            TrayIcon.Click += delegate(object sender, EventArgs e)
            {
                if (((MouseEventArgs)e).Button == MouseButtons.Left)
                {
                    // If user clicked on icon we will call this function.
                    ShowHideMainWindow(sender, null);
                }
            };

            // Let's make our icon visible.
            TrayIcon.Visible = true;
        }

        /// <summary>
        /// We will show programs main window if it's minimized or hide it if it's in normal state.
        /// </summary>
        private void ShowHideMainWindow(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                Show();
                WindowState = WindowState.Normal;
                Activate(); // Set focus to program window.
            }
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            CreateTrayIcon(); // Create our tray icon.
            WindowState = WindowState.Minimized; // Minimize app on start.
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _doWork.CloseConnection(); // Close UDP client.
            workingThread.Abort(); // Close child thread.
            TrayIcon.Dispose(); // Dispose tray icon.
            base.OnClosing(e); // Do original closing event.
        }
    }
}
