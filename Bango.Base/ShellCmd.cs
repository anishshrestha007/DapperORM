using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base
{
    public static class ShellCmd
    {
        /*
           
        string retMessage = String.Empty;
        ProcessStartInfo startInfo = new ProcessStartInfo();
        Process p = new Process();

        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardInput = true;

        startInfo.UseShellExecute = false;
        startInfo.Arguments = command;
        startInfo.FileName = exec;

        p.StartInfo = startInfo;
        p.Start();

        p.OutputDataReceived += new DataReceivedEventHandler
        (
            delegate(object sender, DataReceivedEventArgs e)
            {                   
                using (StreamReader output = p.StandardOutput)
                {
                    retMessage = output.ReadToEnd();
                }
            }
        );

        p.WaitForExit();

        return retMessage;
         */

        public static string ExecuteCommand(string command, string argument)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = command;
            //startInfo.FileName = "cmd.exe";
            startInfo.Arguments = argument;
            //startInfo.Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        public static string ExecuteExportCommand(string argument)
        {
            return ExecuteAndWait(string.Empty, argument);
            //return ExecuteAndWait(FileBox.PmisExportAppPath, argument);
        }

        public static string ExecuteAndWait(string command, string argument)
        {
            string retMessage = String.Empty;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;

            startInfo.UseShellExecute = false;
            startInfo.Arguments = argument;
            startInfo.FileName = command;

            //test
            p.StartInfo = startInfo;
            p.Start();

            //p.OutputDataReceived += new DataReceivedEventHandler
            //(
            //    delegate(object sender, DataReceivedEventArgs e)
            //    {
            //        using (StreamReader output = p.StandardOutput)
            //        {
            //            retMessage = output.ReadToEnd();
            //            return retMessage;
            //        }
            //    }
            //);
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd();
        }

        public static string CallExport(string command, string json_data_file, Dictionary<string, object> parameters)
        {
            //prepare the argument
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("command=={0}", command);
            sb.AppendFormat(" user_id=={0}", 0);
            //sb.AppendFormat(" user_id=={0}", App.GetCurrentUserId());
            sb.AppendFormat(" json_data_file=={0}", json_data_file);
            //converting param to json string without opening & closing curly brackets {, }
            //string param = JsonConvert.SerializeObject(parameters);
            //param = param.TrimStart(new char[] { '{' });
            //param = param.TrimEnd(new char[] { '}' });
            //sb.AppendFormat(" query_param=={0}", param);
            //sb.Replace("\"", "\\\"");
            //return ExecuteCommand(FileBox.PmisExportAppPath, sb.ToString());


            //return string.Empty;
            return ExecuteExportCommand(sb.ToString());
        }

        public static string CallWkHtmlToPdf(string htmlFilePath, string destFilePath)
        {
            string argument = string.Format("--load-error-handling ignore \"{0}\" \"{1}\"", htmlFilePath, destFilePath);
            string command = "DLL/wkhtmltopdf.exe";
            //string command = FileBox.BinPath + "DLL/wkhtmltopdf.exe";

            return ExecuteAndWait(command, argument);
        }
    }
}

/**
 * TODO
 * 1) root path of application (line 64 && 126)
 *      need to replace FileBox static class usage
 * 2) Need to remove the usage App.GetCurrentUserId() 
**/