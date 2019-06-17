using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageRemove
{
    class Program
    {
        private static IEnumerable<IPackage> GetListedPackages(string packageID)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            var packages = from package in repo.FindPackagesById(packageID)
                           where package.IsListed()
                           select package;
            return packages;
        }

        private static string UnlistPackage(IPackage package, string apiKey)
        {
            var arguments = $"delete {package.Id} {package.Version} -Source https://api.nuget.org/v3/index.json -ApiKey {apiKey} -NonInteractive";
            var psi = new ProcessStartInfo("nuget.exe", arguments)
            {
                RedirectStandardOutput = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false
            };
            var process = Process.Start(psi);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }
        static void Main(string[] args)
        {
            var packageID = "yourPackageName";
            var apiKey = "yourKeyFromNugetOrg";

            var packages = GetListedPackages(packageID);

            foreach (var package in packages)
            {
                Console.WriteLine("Unlisting package " + package);
                var output = UnlistPackage(package, apiKey);
                Console.WriteLine(output);
            }

            Console.Write("Completed. Press ENTER to quit.");
            Console.ReadLine();
        }
    }
}
