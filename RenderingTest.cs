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
        public void RenderListPlatforms()
        {
            string allPlatforms = "All platforms: ";
            foreach (var platform in Window.Platforms)
            {
                allPlatforms += platform.GetType().Name + ", ";
            }

            throw new Exception(allPlatforms);
        }

        [TestMethod]
        public void RenderTestWindow()
        {
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





