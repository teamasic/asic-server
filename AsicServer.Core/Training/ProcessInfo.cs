using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsicServer.Core.Training
{
    public class ProcessStarter
    {
        public static ResponsePython Start(ProcessStartInfo startInfo, Action action = null)
        {
            var responsePython = new ResponsePython();
            using (var myProcess = Process.Start(startInfo))
            {
                action?.Invoke();
                responsePython.Errors = myProcess.StandardError.ReadToEnd();
                responsePython.Success = string.IsNullOrEmpty(responsePython.Errors);
                // responsePython.Results = myProcess.StandardOutput.ReadToEnd();
            }
            return responsePython;
        }
    }
    public class ProcessStartInfoFactory
    {
        private readonly MyConfiguration configuration;

        public ProcessStartInfoFactory(MyConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public ProcessStartInfo Create(ProcessInfo processInfo)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            var pythonFullPath = configuration.PythonExeFullPath;
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            startInfo.FileName = pythonFullPath;
            startInfo.Arguments = string.Format("{0} {1}", processInfo.Path, processInfo.GetArgs());
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WorkingDirectory = parentDirectory + "\\" + configuration.RecognitionServiceName;
            return startInfo;
        }
    }
    public abstract class ProcessInfo
    {
        public string Path { get; set; }
        public abstract string GetArgs();
    }

    public class AugmentProcessInfo : ProcessInfo
    {
        public string Dataset { get; set; } = "dataset";
        public string Output { get; set; } = "augmented";
        public ICollection<string> Names { get; set; } = new List<string>();

        public override string GetArgs()
        {
            var args = "";
            args += string.Format(@"--dataset {0} ", Dataset);
            if (Names.Count > 0)
            {
                args += string.Format(@"--names {0} ", String.Join(",", Names)); // has whitespace
            }
            return args;
        }
    }
    public class ExtractEmbeddingsProcessInfo : ProcessInfo
    {
        public string Dataset { get; set; } = "augmented";
        public string Output { get; set; } = "output_dlib";

        public override string GetArgs()
        {
            var args = "";
            args += string.Format(@"--dataset {0} ", Dataset);
            args += string.Format(@"--output {0} ", Output);
            return args;
        }
    }
    public class TrainModelProcessInfo : ProcessInfo
    {
        public string Output { get; set; } = "output_dlib";

        public override string GetArgs()
        {
            var args = "";
            args += string.Format(@"--output {0} ", Output);
            return args;
        }
    }
    public class ModifyEmbeddingProcessInfo : ProcessInfo
    {
        public string Dataset { get; set; } = "augmented";
        public string Output { get; set; } = "output_dlib";
        public string Input { get; set; } = "output_dlib";
        public ICollection<string> Names { get; set; } = new List<string>();

        public override string GetArgs()
        {
            var args = "";
            args += string.Format(@"--dataset {0} ", Dataset);
            args += string.Format(@"--output {0} ", Output);
            args += string.Format(@"--input {0} ", Input);
            if (Names.Count > 0)
            {
                args += string.Format(@"--names {0} ", String.Join(",", Names)); // has whitespace
            }
            return args;
        }
    }
}
