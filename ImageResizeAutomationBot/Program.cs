using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Drawing.Processing;


namespace ImageResizeAutomationBot
{
    internal class Program
    {
        static void Main()
        {
            string rawRoot = "RawProductImages";
            string outputRoot = "FinalProductImages";
            string progressFile = "progressFile.csv";

            var progressTracking = new ProgressTracking(progressFile);

            Directory.CreateDirectory(outputRoot);

            foreach (var partFolder in Directory.GetDirectories(rawRoot))
            {
                string partNumber = new DirectoryInfo(partFolder).Name;
                string outputPath = Path.Combine(outputRoot, partNumber + ".jpg");

                // Get first image file (supports jpg, png, webp, gif)
                var imageFile = Directory.GetFiles(partFolder)
                    .FirstOrDefault(f => new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".tif" }
                    .Contains(Path.GetExtension(f).ToLower()));

                if (imageFile == null)
                {
                    Console.WriteLine($"No image found for: {partNumber}");
                    continue;
                }
                
                if (progressTracking.IsModified(partNumber))
                {
                    Console.WriteLine("Already Modified");
                    continue;
                }

                using (Image image = Image.Load(imageFile))
                {
                    // Step 1: Enlarge by 1.3x
                    float scale = 1.3f;
                    int enlargedWidth = (int)(image.Width * scale);
                    int enlargedHeight = (int)(image.Height * scale);

                    // Step 2: Set max allowed dimensions inside canvas (with padding)
                    int maxCanvasWidth = 1000;
                    int maxCanvasHeight = 750;
                    int maxImageWidth = 900;  // leave 100px padding (50px each side)
                    int maxImageHeight = 650; // leave 100px padding (50px top/bottom)

                    // Step 3: If enlarged image exceeds max allowed size, scale it down proportionally
                    if (enlargedWidth > maxImageWidth || enlargedHeight > maxImageHeight)
                    {
                        float widthRatio = (float)maxImageWidth / enlargedWidth;
                        float heightRatio = (float)maxImageHeight / enlargedHeight;
                        float fitRatio = Math.Min(widthRatio, heightRatio);

                        enlargedWidth = (int)(enlargedWidth * fitRatio);
                        enlargedHeight = (int)(enlargedHeight * fitRatio);
                    }

                    // Step 4: Resize image to final dimensions
                    image.Mutate(x => x.Resize(enlargedWidth, enlargedHeight));


                    using (var canvas = new Image<Rgba32>(maxCanvasWidth, maxCanvasHeight, Color.White))
                    {
                        //Centering
                        int x = (maxCanvasWidth - enlargedWidth) / 2;
                        int y = (maxCanvasHeight - enlargedHeight) / 2;

                        canvas.Mutate(ctx =>
                        {
                            ctx.DrawImage(image, new Point(x, y), 1);
                            // Draw 1px black border
                            ctx.DrawPolygon(Color.Black, 1, new PointF[]
                            {
                            new PointF(0,0),
                            new PointF(maxCanvasWidth - 1, 0),
                            new PointF(maxCanvasWidth - 1, maxCanvasHeight - 1),
                            new PointF(0, maxCanvasHeight - 1),
                            new PointF(0,0)
                            });
                        });

                        // Save as JPG
                        canvas.Save(outputPath, new JpegEncoder { Quality = 100 });
                    }
                }
                progressTracking.MarkDone(partNumber);
                Console.WriteLine($"Processed: {partNumber}");
            }

            Console.WriteLine("Done!");
        }
    }
}
