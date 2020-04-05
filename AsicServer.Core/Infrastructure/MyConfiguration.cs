using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AppSettingConfiguration
{
    public class MyConfiguration
    {
        public string PythonExeFullPath { get; set; }
        public string RecognitionProgramPathOpenCV { get; set; }
        public string RecognitionServiceName { get; set; }
        public string ModelOutputFolderName { get; set; }
        public string TemporaryOutputFolderName { get; set; }
        public string AugmentProgramPath { get; set; }
        public string ExtractEmbeddingsProgramPath { get; set; }
        public string SVMTrainProgramPath { get; set; }
        public string TrainModelProgramPath { get; set; }
        public string AddEmbeddingsProgramPath { get; set; }
        public string RemoveEmbeddingsProgramPath { get; set; }
    }
}
