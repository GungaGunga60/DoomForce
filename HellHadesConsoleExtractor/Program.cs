using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellHadesConsoleExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full path to the HellHades extractor:");
            string assemblyPath = Console.ReadLine();
            if (!assemblyPath.ToLowerInvariant().EndsWith("hellhades.artifactextractor.exe"))
            {
                assemblyPath = Path.Combine(assemblyPath, "HellHades.ArtifactExtractor.exe");
            }

            Assembly hh = Assembly.LoadFrom(assemblyPath);
            if (hh == null)
            {
                Console.WriteLine("Failed to load {0}", assemblyPath);
                return;
            }

            Type mainFormType = hh.GetType("HellHades.ArtifactExtractor.MainForm");
            if (mainFormType == null)
            {
                Console.WriteLine("Couldn't find the MainForm.  Use Ildasm to inspect {0} and see what object has the new GetDump method!", assemblyPath);
                return;
            }

            MethodInfo getDumpMethod = mainFormType.GetMethod("GetDump", BindingFlags.NonPublic | BindingFlags.Instance);
            if (getDumpMethod == null)
            {
                Console.WriteLine("Couldn't find the GetDump method on the MainForm type.  Use Ildasm to inspect {0} and see what object has the new GetDump method!", assemblyPath);
                return;
            }

            object mainForm = Activator.CreateInstance(mainFormType, string.Empty);
            if (mainForm == null)
            {
                Console.WriteLine("Failed to create an instance of the MainForm class.  Need to debug this.");
                return;
            }

            object getDumpResults = getDumpMethod.Invoke(mainForm, new object[] { });
            if (getDumpResults == null)
            {
                Console.WriteLine("Failed to invoke the GetDump method!  Need to debug this.");
                return;
            }

            Console.WriteLine("Where do you want to save the json dump file?");
            string jsonPath = Console.ReadLine();
            if (!jsonPath.ToLowerInvariant().EndsWith(".json"))
            {
                jsonPath = Path.Combine(jsonPath, "dump.json");
            }
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(getDumpResults, Formatting.Indented));
            Console.WriteLine("Wrote to {0}", jsonPath);
        }
    }
}
