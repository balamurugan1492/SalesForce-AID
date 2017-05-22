namespace StatTickerFive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Windows;

    using log4net;

    using StatTickerFive.ViewModels;

    [RunInstaller(true)]
    public partial class InstallerClass1 : System.Configuration.Install.Installer
    {
        #region Fields

        private static ILog logger = LogManager.GetLogger(typeof(InstallerClass1));

        #endregion Fields

        #region Constructors

        public InstallerClass1()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception occurred : StatTickerFive Installer : " + ex.Message);
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Handles the AfterInstall event of the InstallerClass1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InstallEventArgs" /> instance containing the event data.</param>
        private void InstallerClass1_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                //MessageBox.Show("Stat InstallerClass1_AfterInstall Started ");

                try
                {
                    //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Started ");

                    string fileName = "app_config.xml";
                    string fileName1 = "app_db_config.xml";

                    string sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\PHS\AgentInteractionDesktop";

                    if (!System.IO.Directory.Exists(targetPath))
                    {
                        //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Creating  Directory :  " + targetPath);
                        System.IO.Directory.CreateDirectory(targetPath);
                        //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Created  Directory :  " + targetPath);
                    }

                    string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(sourceFile, destFile, true);

                    //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Sourcefile :  "+sourceFile+" , Destinations File : "+destFile );

                    sourceFile = System.IO.Path.Combine(sourcePath, fileName1);
                    destFile = System.IO.Path.Combine(targetPath, fileName1);
                    System.IO.File.Copy(sourceFile, destFile, true);

                    //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Sourcefile :  " + sourceFile + " , Destinations File : " + destFile);

                    //MessageBox.Show("InstallerClass1_AfterInstall : Copying XML Files : Ended");

                }
                catch (Exception GeneralException)
                {
                    logger.Error("InstallerClass1_AfterInstall Method: Error occured while after installing the setup " +
                        GeneralException.ToString());
                    //MessageBox.Show("Exception InstallerClass1_AfterInstall : " + GeneralException.Message);
                }

                //Commented on 06/07/2015, by Elango.T for Preventing Application start in OS boot and Run for the first time
                //Directory.SetCurrentDirectory(Path.GetDirectoryName
                //   (Assembly.GetExecutingAssembly().Location));

                //Process.Start(Path.GetDirectoryName(
                //  Assembly.GetExecutingAssembly().Location) + "\\StatTickerFive.exe");

                //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                //Assembly curAssembly = Assembly.GetExecutingAssembly();
                //key.SetValue(curAssembly.GetName().Name, curAssembly.Location);

                //MessageBox.Show("InstallerClass1_AfterInstall Ended ");
            }
            catch (Exception GeneralException)
            {
                MessageBox.Show(" Exception occurred after Installing StatTickerFive : " + GeneralException.InnerException != null ? GeneralException.InnerException.Message : GeneralException.Message);
            }
        }

        /// <summary>
        /// Handles the AfterUninstall event of the InstallerClass1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InstallEventArgs" /> instance containing the event data.</param>
        private void InstallerClass1_AfterUninstall(object sender, InstallEventArgs e)
        {
            //try
            //{
            //    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\StatTickerFive";

            //    if (Directory.Exists(path))
            //    {
            //        Directory.Delete(path, true);
            //    }
            //}
            //catch (Exception generalException)
            //{
            //    logger.Error("InstallerClass_AfterUninstall Method: Error occured while after uninstall the setup " +
            //        generalException.ToString());
            //}
        }

        /// <summary>
        /// Handles the BeforeInstall event of the InstallerClass1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InstallEventArgs" /> instance containing the event data.</param>
        private void InstallerClass1_BeforeInstall(object sender, InstallEventArgs e)
        {
            //try
            //{

            //    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\StatTickerFive";

            //    if (Directory.Exists(path))
            //    {
            //        Directory.Delete(path, true);
            //    }
            //}
            //catch (Exception generalException)
            //{
            //    logger.Error("InstallerClass_BeforeInstall Method: Error occured while before install the setup " +
            //        generalException.ToString());
            //}
        }

        #endregion Methods
    }
}