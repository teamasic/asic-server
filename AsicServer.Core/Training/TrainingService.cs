using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsicServer.Core.Training
{
    public interface ITrainingService
    {
        public ResponsePython Train();
        public ResponsePython AddEmbeddings(ICollection<string> names);
        public ResponsePython RemoveEmbeddings(ICollection<string> names);
        public TrainResultViewModel GetLastTrainingResult();
        public bool HasExistingModel();
    }
    public class TrainingService: ITrainingService
    {
        private readonly ProcessStartInfoFactory processStartInfoFactory;
        private readonly MyConfiguration myConfiguration;
        private readonly FilesConfiguration filesConfig;

        public TrainingService(ProcessStartInfoFactory processStartInfoFactory, 
            MyConfiguration myConfiguration, FilesConfiguration filesConfig)
        {
            this.processStartInfoFactory = processStartInfoFactory;
            this.myConfiguration = myConfiguration;
            this.filesConfig = filesConfig;
        }
        public bool HasExistingModel()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fileDest = Path.Join(parentDirectory,
                myConfiguration.RecognitionServiceName,
                myConfiguration.ModelOutputFolderName,
                filesConfig.RecognizerModelFile);
            return File.Exists(fileDest);
        }
        public ResponsePython Train()
        {
            Go(Augment());
            Go(ExtractEmbeddings());
            var response = TrainModelToFile();
            return response;
        }
        public ResponsePython AddEmbeddings(ICollection<string> names)
        {
            Go(Augment(names));
            Go(AddEmbeddingsHelper(names));
            return TrainModelToFile();
        }
        public ResponsePython RemoveEmbeddings(ICollection<string> names)
        {
            Go(Augment(names));
            Go(RemoveEmbeddingsHelper(names));
            return TrainModelToFile();
        }

        private ResponsePython TrainModelToFile()
        {
            var response = Go(TrainModel());
            var sourceDir = GetPath(myConfiguration.TemporaryOutputFolderName);
            var targetDir = GetPath(myConfiguration.ModelOutputFolderName);
            Copy(sourceDir, targetDir);
            return response;
        }

        #region Train model utility methods
        private string GetPath(string folder) // relative to the recognition service
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            return Path.Join(parentDirectory, myConfiguration.RecognitionServiceName, folder);
        }
        private ResponsePython Go(ResponsePython responsePython)
        {
            if (!responsePython.Success)
            {
                throw new BaseException(responsePython.Errors);
            }
            return responsePython;
        }
        private void Copy(string sourceDir, string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);

                foreach (var file in Directory.GetFiles(sourceDir))
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

                foreach (var file in Directory.GetFiles(sourceDir))
                    File.Delete(file);
                Directory.Delete(sourceDir);
            }
        }
        private ResponsePython Augment()
        {
            var startInfo = processStartInfoFactory.Create(new AugmentProcessInfo
            {
                Path = myConfiguration.AugmentProgramPath
            });
            return ProcessStarter.Start(startInfo);
        }
        private ResponsePython Augment(ICollection<string> names)
        {
            var startInfo = processStartInfoFactory.Create(new AugmentProcessInfo
            {
                Path = myConfiguration.AugmentProgramPath,
                Names = names
            });
            return ProcessStarter.Start(startInfo);
        }
        private ResponsePython ExtractEmbeddings()
        {
            var startInfo = processStartInfoFactory.Create(new ExtractEmbeddingsProcessInfo
            {
                Path = myConfiguration.ExtractEmbeddingsProgramPath,
                Output = myConfiguration.TemporaryOutputFolderName
            });
            return ProcessStarter.Start(startInfo);
        }
        private ResponsePython TrainModel()
        {
            var startInfo = processStartInfoFactory.Create(new TrainModelProcessInfo
            {
                Path = myConfiguration.TrainModelProgramPath,
                Output = myConfiguration.TemporaryOutputFolderName
            });
            return ProcessStarter.Start(startInfo);
        }
        private ResponsePython AddEmbeddingsHelper(ICollection<string> names)
        {
            var startInfo = processStartInfoFactory.Create(new ModifyEmbeddingProcessInfo
            {
                Path = myConfiguration.AddEmbeddingsProgramPath,
                Output = myConfiguration.TemporaryOutputFolderName,
                Input = myConfiguration.ModelOutputFolderName,
                Names = names
            });
            return ProcessStarter.Start(startInfo);
        }
        private ResponsePython RemoveEmbeddingsHelper(ICollection<string> names)
        {
            var startInfo = processStartInfoFactory.Create(new ModifyEmbeddingProcessInfo
            {
                Path = myConfiguration.RemoveEmbeddingsProgramPath,
                Output = myConfiguration.TemporaryOutputFolderName,
                Input = myConfiguration.ModelOutputFolderName,
                Names = names
            });
            return ProcessStarter.Start(startInfo);
        }
        public TrainResultViewModel GetLastTrainingResult()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fileDest = Path.Join(parentDirectory,
                myConfiguration.RecognitionServiceName, myConfiguration.LastTrainedFilePath);
            using StreamReader file = File.OpenText(fileDest);
            JsonSerializer serializer = new JsonSerializer();
            return (TrainResultViewModel) serializer.Deserialize(file, typeof(TrainResultViewModel));
        }
        #endregion
    }
}
