# ImageResizeAutomationBot

A **.NET 8 console application** for automating product image formatting.  
This tool enlarges, resizes, and standardizes images into a **1000 × 750 px white canvas** with a **1px black border**, ensuring they meet e-commerce template requirements.

The application was designed to process thousands of Agilent product images scraped in a previous project:  
👉 [WebScraperBot](https://github.com/alihassani133/WebScraperBot)

---

## ✨ Features

- **Enlarges images by 1.3×** before fitting them into a standardized canvas
- **Maintains proportions** by scaling down if the enlarged size exceeds safe bounds
- Places images on a **1000 × 750 px white background**
- Adds a **1px black frame** to match employer website templates
- Saves processed images as **high-quality JPGs**
- **Progress tracking** with `progressFile.csv`:
  - Skips already-processed images
  - Allows resuming after failures without repeating work

---

## 📂 Project Structure

- **Program.cs**  
  Main entry point. Handles resizing, centering, background creation, and saving images.

- **ProgressTracking.cs**  
  Logs processed part numbers in `progressFile.csv` and prevents duplicate work.

- **RawProductImages/**  
  Input directory containing folders named after product part numbers, each with at least one source image.

- **FinalProductImages/**  
  Output directory where processed images are saved, named `<PartNumber>.jpg`.

- **progressFile.csv**  
  Progress log. Ensures each part number is processed only once.

---

## 📦 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or any IDE with C# support
- [ImageSharp](https://github.com/SixLabors/ImageSharp) NuGet package
