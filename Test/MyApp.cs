using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using Bango;
using Bango.Base;
using Bango.Base.List;
using Bango.Helpers;

namespace Test
{
    public static class MyApp
    {
        public static void init()
        {
            //if (Myro.Db.FetchConnectionCmd == null)
            //    Myro.Db.FetchConnectionCmd = new GetNpgsqlConnect();
            //IRegisterType registerType = new Myro.Npgsql.RegisterTypes();
            //Myro.Helpers.IRegisterType registerType = new Myro.RegisterTypes();
            IRegisterType registerType = new RegisterTypes();
            //registerType.Container.RegisterApiControllers();
            //registerType.Container.EnableWebApi(GlobalConfiguration.Configuration);
            App.init(registerType);
            //TaxMigrations taxMigrations = new TaxMigrations();
            //taxMigrations.Load_Migrations()

            //MakeSymlink();
        }

        private static void MakeSymlink()
        {
            var isProduction = WebConfigurationManager.AppSettings["MyroConfig:IsProduction"];
            if (isProduction == "false")
            {
                Process proc = null;
                try
                {
                    var symlinkPath = AppDomain.CurrentDomain.BaseDirectory;

                    proc = new Process
                    {
                        StartInfo =
                        {
                            WorkingDirectory = symlinkPath,
                            FileName = "create-symlink.bat",
                            CreateNoWindow = false
                        }
                    };
                    proc.Start();
                    proc.WaitForExit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
      
    }
}