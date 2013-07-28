//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

//Just for the record: this project was started 12.03.2009
//Version 1.6 released 02.07.2010 - first full version of Krento
//Version 2.0 released 27.01.2011
//Version 2.1 released 01.07.2011

// Command line parameters:
// ns - no splash screen
// nd - do not load docklets
// nt - do not load toys
// low - low memory

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using Laugris.Sage;
using System.Security;
using System.Reflection;
using System.IO;
using System.Security.Permissions;
using System.Configuration;
using Krento.Properties;

namespace Krento
{
    /// <summary>
    /// Main class of Krento, startup of the application
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    internal sealed class Startup : IServiceProvider
    {
        private string[] args;
        internal static Startup instance;
        private MainForm mainForm;
        private bool showSplashScreen;
        private SplashScreen splashScreen;
        private KrentoContext context;
        private ApplicationContext appContext;
        private ResolveEventHandler assemblyResolver;

        private static int messageID;
        private static bool busyException;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        private Startup(string[] args)
        {
            this.args = args;
        }

        /// <summary>
        /// Gets the main form of the Krento.
        /// </summary>
        /// <value>The main form of the Krento. This is the real Form, not Layered Window</value>
        internal static MainForm MainForm
        {
            get { return instance.mainForm; }
        }

        internal static KrentoContext Context
        {
            get { return instance.context; }
        }

        /// <summary>
        /// Stops this instance of Krento and dispose Krento context.
        /// </summary>
        internal void Stop()
        {
            TraceDebug.Trace("Stop Krento");
            if (context != null)
            {
                try
                {
                    if (splashScreen != null)
                    {
                        splashScreen.Dispose();
                        splashScreen = null;
                    }
                    context.StopServer();
                    context.UnloadKrentoPlugins();
                    context.UnloadKrentoToys();
                    context.UnloadKrentoDocklets();
                    context.UnloadRingImage();
                    context.Dispose();
                    context = null;
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace("Exception in Startup.Stop: " + ex.Message);
                    context = null;
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private void PrepareStandardProperties()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(HandleApplicationException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleDomainException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain domain = AppDomain.CurrentDomain;
            this.assemblyResolver = new ResolveEventHandler(this.ResolveAssembly);
            domain.AssemblyResolve += new ResolveEventHandler(this.assemblyResolver);

        }

        /// <summary>
        /// Resolves the assembly reference.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>Reference to Assembly</returns>
        [SecurityCritical, SecurityTreatAsSafe]
        internal Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            Assembly assembly = InternalHelper.FindLoadedAssembly(name);
            return assembly;
        }

        internal static void BroadcastShutdownMessage()
        {
            InteropHelper.BroadcastApplicationMessage(messageID);
        }

        private static void ApplyGUILanguage()
        {
            string cultureName;
            string lngFileName;

            cultureName = NativeMethods.ReadString(GlobalConfig.KrentoSettingsFileName, "General", "Language", CultureInfo.CurrentCulture.Name);

            Language.Culture = new CultureInfo(cultureName);
            lngFileName = cultureName + ".lng";

            string cultureFile = Path.Combine(GlobalConfig.LanguagesFolder, lngFileName);
            if (FileOperations.FileExists(cultureFile))
            {
                Language.CultureFile = cultureFile;

                string[] lngFiles = Directory.GetFiles(GlobalConfig.StoneClasses, lngFileName, SearchOption.AllDirectories);
                foreach (string lngFile in lngFiles)
                {
                    Language.Merge(lngFile);
                }

            }

        }

        /// <summary>
        /// Starts this instance of Krento.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        internal void Start()
        {

            string ApplicationPrefix = GlobalConfig.ProductName.ToUpperInvariant();

            TraceDebug.Trace("Start {0}", GlobalConfig.ProductName);
#if PORTABLE
            Environment.SetEnvironmentVariable(ApplicationPrefix + "_PORTABLE", GlobalConfig.MainFolder, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(ApplicationPrefix + "_DRIVE", GlobalConfig.ApplicationDrive, EnvironmentVariableTarget.Process);
#endif

            Environment.SetEnvironmentVariable(ApplicationPrefix + "_DATA", GlobalConfig.MainFolder, EnvironmentVariableTarget.Process);

            messageID = NativeMethods.RegisterWindowMessage("KrentoStop"); //Do not localize!!!
            NativeMethods.AddRemoveMessageFilter(messageID, ChangeWindowMessageFilterFlags.Add);


            PrepareStandardProperties();

            showSplashScreen = true;

            ApplyGUILanguage();

            GlobalSettings.LoadGlobalSettings();
            GlobalSettings.ManagerLeft = Settings.Default.ManagerLeft;
            GlobalSettings.ManagerTop = Settings.Default.ManagerTop;



            #region Parse command line
            foreach (string param in args)
            {
                if (TextHelper.SameText(param, @"/low"))
                {
                    GlobalConfig.LowMemory = true;
                }

                if (TextHelper.SameText(param, @"/ns"))
                {
                    showSplashScreen = false;
                }

                if (TextHelper.SameText(param, @"/nd"))
                {
                    KrentoContext.SkipDocklets = true;
                }

                if (TextHelper.SameText(param, @"/nt"))
                {
                    KrentoContext.SkipToys = true;
                }

                if (TextHelper.SameText(param, @"/desktop"))
                {
                    KrentoContext.CreateDesktopCircle = true;
                }

                if (param.IndexOf(@".circle", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    KrentoContext.CircleParameter = param;
                }

                if (FileOperations.IsKrentoPackage(param))
                {
                    KrentoContext.PackageParameter = param;
                }
            }
            #endregion

            this.context = new KrentoContext();

            #region FirstRun
            if (NativeMethods.GlobalCheckAtom("krento_first_run"))
            {
                KrentoContext.FirstRun = true;
                NativeMethods.GlobalKillAtom("krento_first_run");
            }
#if PORTABLE
            string firstRunFile = Path.Combine(GlobalConfig.ApplicationFolder, "first.run");
            if (NativeMethods.FileExists(firstRunFile))
            {
                KrentoContext.FirstRun = true;
                NativeMethods.FileDelete(firstRunFile);
            }
#endif
            if (KrentoContext.FirstRun)
            {
                GlobalSettings.SaveGlobalSettings();
            }
            #endregion

            #region First Instance
            if (context.FirstInstance)
            {

                if ((showSplashScreen) && GlobalSettings.ShowSplashScreen)
                {
                    splashScreen = new SplashScreen();
                }
                else
                    splashScreen = null;



                this.mainForm = new MainForm(context);
                mainForm.Text = GlobalConfig.ProductName;
                InteropHelper.MainWindow = mainForm.Handle;

                if (showSplashScreen)
                {
                    if (splashScreen != null)
                    {
                        mainForm.SplashScreen = splashScreen;
                    }
                }


                if (NativeMethods.StartEngineEx(mainForm.Handle))
                {
                    if (GlobalSettings.ActivateServer)
                    {
                        if (NetworkOperations.PortAvailable(GlobalSettings.PortNumber))
                            context.StartServer();
                    }


                    appContext = new ApplicationContext();

                    //Load plugins. If the instance is the first instance and 
                    //engine is activated, then plugins are loaded

                    try
                    {
                        context.LoadKrentoPlugins();
                    }
                    catch (Exception)
                    {
                        //do not stop on wrong plugins
                        TraceDebug.Trace("Exception on loading Krento plugin");
                    }

                    try
                    {
                        context.LoadKrentoToys();
                    }
                    catch (Exception)
                    {
                        //do not stop on wrong plugins
                        TraceDebug.Trace("Exception on loading Krento toy");
                    }
                    try
                    {
                        context.LoadKrentoDocklets();
                    }
                    catch
                    {
                        //do not stop on wrong docklet
                        TraceDebug.Trace("Exception on loading docklet");
                    }

                    NativeThemeManager.MakeSound("#110");
                    Application.Run(appContext);
                }
            }
            #endregion
        }


        [SecurityCritical]
        private static void HandleDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            TraceDebug.Trace("DEBUG: Current Domain unhandled exception");
            UnhandledException((Exception)e.ExceptionObject);
        }

        [SecurityCritical]
        private static void HandleApplicationException(object sender, ThreadExceptionEventArgs e)
        {
            TraceDebug.Trace("DEBUG: Application thread exception");
            UnhandledException(e.Exception);

        }

        internal static ThreadWindows DisableTaskWindows()
        {
            ThreadWindows threadWindows = new ThreadWindows();
            threadWindows.Enable(false);
            return threadWindows;
        }

        internal static void EnableTaskWindows(ThreadWindows threadWindows)
        {
            threadWindows.Enable(true);
            threadWindows.Dispose();
            threadWindows = null;
        }

        [SecurityCritical]
        internal static void UnhandledException(Exception ex)
        {
            if (!busyException)
                try
                {
                    busyException = true;

#if !DEBUG
            try
            {
#endif
                    InteropHelper.OutputDebugString(ex.Message);

                    BroadcastShutdownMessage();
                    string message = string.Empty;

                    ErrorHandleDialog ehd = new ErrorHandleDialog();
                    try
                    {
                        if (ex is HookException)
                        {
                            ehd.ErrorText.Text = SR.KrentoHookMissing;
                        }
                        else
                        {
                            while (ex.InnerException != null)
                            {
                                message = message + ex.Message + Environment.NewLine;
                                ex = ex.InnerException;
                            }
                            message = message + ex.Message + Environment.NewLine;

                            StackTrace st = new StackTrace(ex);
                            message = message + "Call stack:";
                            ehd.ErrorText.Text = message + Environment.NewLine + st.ToString();
                            InteropHelper.OutputDebugString(ehd.ErrorText.Text);
                        }

                        DialogResult ok = DialogResult.Cancel;

                        ThreadWindows threadWindows;
                        threadWindows = DisableTaskWindows();
                        try
                        {
                            ok = ehd.ShowDialog(MainForm);
                        }
                        finally
                        {
                            EnableTaskWindows(threadWindows);
                        }
                        if (ok == DialogResult.OK)
                        {
#if DEBUG
                            if (Debugger.IsAttached)
                                throw ex;
                            else
                                Killer.KillSelf();
#else
                        Killer.KillSelf();
#endif
                        }
                        else
                        {
#if DEBUG
                            if (Debugger.IsAttached)
                                throw ex;
                            else
                                Killer.KillSelf();
#else
                        Application.Restart();
                        Killer.KillSelf();
#endif
                        }

                    }
                    finally
                    {
                        ehd.Dispose();
                    }
#if !DEBUG
            }

            catch
            {
                //Exception happens in exception handler
                //Can't do anything now
                TraceDebug.Trace(ex.Message);
                MessageBox.Show(ex.ToString());
                Killer.KillSelf();
            }
#endif
                }
                finally
                {
                    busyException = false;
                }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[DebuggerNonUserCode]
        [LoaderOptimization(LoaderOptimization.SingleDomain)]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static int Main(string[] args)
        {

            if (!InteropHelper.IsTrueColorMonitor)
            {
                RtlAwareMessageBox.Show("This application requires 32 bit colors monitor to run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            if (!InteropHelper.RunningOnXP)
            {
                RtlAwareMessageBox.Show("This application requires at least Windows XP installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            IntPtr handle = FastBitmap.LoadThemeLibrary(Path.Combine(GlobalConfig.ApplicationFolder, "Laugris.Standard.dll"));
            if (handle == IntPtr.Zero)
            {
                RtlAwareMessageBox.Show("The standard theme library is not found. Please reinstall Krento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }


#if !DEBUG
            try
            {
#endif
            instance = new Startup(args);
            instance.Start();
#if !DEBUG
            }

            catch (Exception ex)
            {
                try
                {
                    UnhandledException(ex);
                    Killer.KillSelf();
                }

                catch (Exception)
                {
                    MessageBox.Show(ex.ToString());
                    Killer.KillSelf();
                }
            }
#endif

            //running ...

            //finished in normal way...
#if !DEBUG
            try
            {
#endif
            instance.Stop();
#if !DEBUG
            }
            catch
            {
                Killer.KillSelf();
            }
#endif
            return 0;
        }

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type. This method is used by Krento to
        /// load additional modules that implement IPackage interface
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            try
            {
                if (this.context == null)
                    return null;
                else
                    return context.GetService(serviceType);
            }
            catch
            {
                TraceDebug.Trace("Exception on Startup.GetService");
                return null;
            }
        }

        #endregion

    }
}
