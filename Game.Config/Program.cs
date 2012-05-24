using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using Game.Server;
using Game.Base;

namespace DOLConfig
{
	static class Program
	{
		static Form1 MainForm = null;
		static GameServerConfiguration Config = null;
		static FileInfo ConfigFile = null;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "." + Path.DirectorySeparatorChar + "lib";
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			MainForm = new Form1();
			LoadExistingSettings();

			Application.Run(MainForm);
		}

		static void LoadExistingSettings()
		{
			try
			{
				if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + "serverconfig.xml"))
					return;

				ConfigFile = new FileInfo(Application.StartupPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + "serverconfig.xml");
				Config = new GameServerConfiguration();

				if (ConfigFile.Exists)
				{
					Config.LoadFromXMLFile(ConfigFile);
					MainForm.serverTypeComboBox.Text = Config.ServerType.ToString().Replace("GST_", "");
					MainForm.shortNameTextBox.Text = Config.ServerNameShort;
					MainForm.FullNameTextBox.Text = Config.ServerName;
				}
				else
				{
					Config.SaveToXMLFile(ConfigFile);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		public static void SaveSettings()
		{
			try
			{
				switch (MainForm.serverTypeComboBox.Text)
				{
					case "Normal": Config.ServerType = eGameServerType.GST_Normal; break;
				}
				Config.ServerNameShort = MainForm.shortNameTextBox.Text;
				Config.ServerName = MainForm.FullNameTextBox.Text;
                Config.DBConnectionString = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Asynchronous Processing=true",MainForm.mysqlHostTextBox.Text,MainForm.mysqlDatabaseTextBox.Text,MainForm.mysqlUsernameTextBox.Text,MainForm.mysqlPasswordTextBox.Text);
				Config.SaveToXMLFile(ConfigFile);
				MessageBox.Show("Settings saved successfully");
			}
			catch (Exception e)
			{
				MessageBox.Show("Cannot save settings: " + e.Message);
			}
		}
	}
}