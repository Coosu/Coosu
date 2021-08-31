using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string solutionUrl = "../../../../../Coosu.sln";
            string outputDir = "./output";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            bool success = CompileSolution(new FileInfo(Path.Combine(Environment.CurrentDirectory, solutionUrl)).FullName, outputDir);

            if (success)
            {
                Console.WriteLine("Compilation completed successfully.");
                Console.WriteLine("Output directory:");
                Console.WriteLine(outputDir);
            }
            else
            {
                Console.WriteLine("Compilation failed.");
            }

            Console.WriteLine("Press the any key to exit.");
            Console.ReadKey();
        }

        private static bool CompileSolution(string solutionUrl, string outputDir)
        {
            bool success = true;
            
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solution = workspace.OpenSolutionAsync(solutionUrl).Result;
            ProjectDependencyGraph projectGraph = solution.GetProjectDependencyGraph();
            Dictionary<string, Stream> assemblies = new Dictionary<string, Stream>();

            foreach (ProjectId projectId in projectGraph.GetTopologicallySortedProjects())
            {
                Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;
                if (null != projectCompilation && !string.IsNullOrEmpty(projectCompilation.AssemblyName))
                {
                    using (var stream = new MemoryStream())
                    {
                        EmitResult result = projectCompilation.Emit(stream);
                        if (result.Success)
                        {
                            string fileName = string.Format("{0}.dll", projectCompilation.AssemblyName);

                            using (FileStream file = File.Create(outputDir + '\\' + fileName))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(file);
                            }
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }
    }
}
