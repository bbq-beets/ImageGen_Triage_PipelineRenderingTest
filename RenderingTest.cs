using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace PipelineRenderingTest
{
    [TestClass]
    public sealed class RenderingTest
    {
        private static Process? xvfb;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext c)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var display = Environment.GetEnvironmentVariable("DISPLAY");
                if (string.IsNullOrEmpty(display))
                {
                    throw new InvalidOperationException($"DISPLAY environment variable is required by Xvfb for running tests under Linux.");
                }

                var info = new ProcessStartInfo
                {
                    FileName = "/usr/bin/Xvfb",
                    Arguments = $"{display} -ac -screen 0 1024x768x24",
                    CreateNoWindow = true
                };

                xvfb = new Process();
                xvfb.StartInfo = info;
                xvfb.Start();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (xvfb != null)
            {
                xvfb.Kill();
                xvfb.Dispose();
            }
        }

        [TestMethod]
        public void RenderTestWindow()
        {
            RenderDebug.Dump();

            var options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "My first Silk.NET application!"
            };

            var window = Window.Create(options);
            window.Render += _ => { window.Close(); };
            window.Run();
        }
    }

    internal static class RenderDebug
    {
        public static void Dump()
        {
            foreach (var p in Window.Platforms)
                Console.WriteLine($"Platform={p.GetType().Name}, Applicable={p.IsApplicable}");

            Console.WriteLine($"Selected={Window.GetWindowPlatform(viewOnly: true)?.GetType().Name}");
            Console.WriteLine($"DISPLAY={Environment.GetEnvironmentVariable("DISPLAY")}");
            Console.WriteLine($"LD_LIBRARY_PATH={Environment.GetEnvironmentVariable("LD_LIBRARY_PATH")}");

            try
            {
                NativeLibrary.Load("libglfw.so.3");
                Console.WriteLine("dlopen(libglfw.so.3) OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("dlopen(libglfw.so.3) FAILED: " + ex.Message);
            }

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var nuget = Path.Combine(home, ".nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native/libglfw.so.3");
            Console.WriteLine($"NuGet libglfw exists={File.Exists(nuget)}: {nuget}");
        }
    }
}
