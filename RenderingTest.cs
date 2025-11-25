using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PipelineRenderingTest
{
    using Silk.NET.Maths;
    using Silk.NET.Windowing;

    [TestClass]
    public sealed class RenderingTest
    {
        private static Process? xvfb;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var display = Environment.GetEnvironmentVariable("DISPLAY");
                if (string.IsNullOrEmpty(display))
                {
                    throw new InvalidOperationException($"DISPLAY environment variable is required by Xvfb for running tests under Linux.");
                }

                var info = new ProcessStartInfo { FileName = "/usr/bin/Xvfb", Arguments = $"{display} -ac -screen 0 1024x768x24", CreateNoWindow = true, };
                xvfb = new Process();
                xvfb.StartInfo = info;
                xvfb.Start();
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (xvfb != null)
            {
                xvfb?.Kill();
                xvfb?.Dispose();
            }
        }

        [TestMethod]
        public void RenderTestWindow()
        {
            foreach (var p in Window.Platforms)
                Console.WriteLine($"{p.GetType().Name} | Applicable={p.IsApplicable}");
            
            Console.WriteLine($"Selected={Window.GetWindowPlatform()?.GetType().Name}");
            
            Console.WriteLine($"DISPLAY={Environment.GetEnvironmentVariable("DISPLAY")}");
            Console.WriteLine($"LD_LIBRARY_PATH={Environment.GetEnvironmentVariable("LD_LIBRARY_PATH")}");
            
            try
            {
                NativeLibrary.Load("libglfw.so.3");
                Console.WriteLine("dlopen OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"dlopen FAIL: {ex.Message}");
            }
            
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string nuget = Path.Combine(home, ".nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native/libglfw.so.3");
            Console.WriteLine($"Exists={File.Exists(nuget)}: {nuget}");

            var options = WindowOptions.Default with
                                        {
                                            Size = new Vector2D<int>(800, 600),
                                            Title = "My first Silk.NET application!"
                                        };

            var window = Window.Create(options);
            window.Render += (_) =>
            {
                window.Close();
            };
            window.Run();
        }
    }
}

