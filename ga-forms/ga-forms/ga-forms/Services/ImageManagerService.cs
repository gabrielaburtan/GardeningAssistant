using ga_forms.Common;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ga_forms.Services
{
    public class ImageManagerService : IImageManagerService
    {
        // Health
        public SKBitmap HealthInitialImageBitmap { get; set; }
        public SKBitmap HealthInitialImageStretchedBitmap { get; set; }
        public SKBitmap HealthSelectionImageBitmap { get; set; }
        public SKPath HealthSelectionPath { get; set; }

        // Decorate
        public SKBitmap DecorateInitialImageBitmap { get; set; }
        public SKBitmap DecorateInitialImageStretchedBitmap { get; set; }
        public SKBitmap DecorateSelectionImageBitmap { get; set; }
        public SKPath DecorateSelectionPath { get; set; }

        public SKBitmap GetHealthSelectedBitmap()
        {
            SKBitmap initialImageStretched = new SKBitmap(HealthSelectionImageBitmap.Width, HealthSelectionImageBitmap.Height);
            using (SKCanvas canvas = new SKCanvas(initialImageStretched))
            {
                canvas.DrawBitmap(
                    HealthInitialImageBitmap,
                    new SKRect(0, 0, HealthSelectionImageBitmap.Width, HealthSelectionImageBitmap.Height),
                    BitmapStretch.Uniform);
            }

            HealthInitialImageStretchedBitmap = initialImageStretched;

            SKBitmap selectedBitmap = new SKBitmap(HealthSelectionImageBitmap.Width, HealthSelectionImageBitmap.Height);

            for (int x = 0; x < HealthSelectionImageBitmap.Width; ++x)
            {
                for (int y = 0; y < HealthSelectionImageBitmap.Height; ++y)
                {
                    selectedBitmap.SetPixel(x, y,
                        HealthSelectionImageBitmap.GetPixel(x, y).Red == 0
                            ? new SKColor(0, 0, 0, 0)
                            : initialImageStretched.GetPixel(x, y));
                }
            }

            return selectedBitmap;
        }

        public double GetDiseasePercentage(SKBitmap croppedImage, SKBitmap resultImage)
        {
            int totalNumberOfPixels = 0;
            int numberOfBlackPixels = 0;

            for (int x = 0; x < resultImage.Width; ++x)
            {
                for (int y = 0; y < resultImage.Height; ++y)
                {
                    if (croppedImage.GetPixel(x, y).Alpha != 0)
                    {
                        totalNumberOfPixels++;
                        if (resultImage.GetPixel(x, y).Red == 0)
                        {
                            numberOfBlackPixels++;
                        }
                    }
                }
            }
            return (double)numberOfBlackPixels * 100 / totalNumberOfPixels;
        }

        public SKBitmap GetDecorateSelectedBitmap()
        {
            SKBitmap initialImageStretched = new SKBitmap(DecorateSelectionImageBitmap.Width, DecorateSelectionImageBitmap.Height);
            using (SKCanvas canvas = new SKCanvas(initialImageStretched))
            {
                canvas.DrawBitmap(
                    DecorateInitialImageBitmap,
                    new SKRect(0, 0, DecorateSelectionImageBitmap.Width, DecorateSelectionImageBitmap.Height),
                    BitmapStretch.Uniform);
            }

            DecorateInitialImageStretchedBitmap = initialImageStretched;

            SKBitmap selectedBitmap = new SKBitmap(DecorateSelectionImageBitmap.Width, DecorateSelectionImageBitmap.Height);

            for (int x = 0; x < DecorateSelectionImageBitmap.Width; ++x)
            {
                for (int y = 0; y < DecorateSelectionImageBitmap.Height; ++y)
                {
                    selectedBitmap.SetPixel(x, y,
                        DecorateSelectionImageBitmap.GetPixel(x, y).Red == 0
                            ? new SKColor(0, 0, 0, 0)
                            : initialImageStretched.GetPixel(x, y));
                }
            }

            return selectedBitmap;
        }

        public Tuple<SKBitmap, SKBitmap, SKBitmap> GetDecorateImages(SKBitmap croppedImage, List<SKColor> dominantColors)
        {
            SKBitmap originalImage = DecorateInitialImageStretchedBitmap;
            SKBitmap firstImage = new SKBitmap(originalImage.Width, originalImage.Height);
            SKBitmap secondImage = new SKBitmap(originalImage.Width, originalImage.Height);
            SKBitmap thirdImage = new SKBitmap(originalImage.Width, originalImage.Height);

            // Complementary
            SKColor firstComplementary = new SKColor((byte)(255 - dominantColors[0].Red), (byte)(255 - dominantColors[0].Green), (byte)(255 - dominantColors[0].Blue));
            SKColor secondComplementary = new SKColor((byte)(255 - dominantColors[1].Red), (byte)(255 - dominantColors[1].Green), (byte)(255 - dominantColors[1].Blue));
            SKColor thirdComplementary = new SKColor((byte)(255 - dominantColors[2].Red), (byte)(255 - dominantColors[2].Green), (byte)(255 - dominantColors[2].Blue));

            // Hues
            int firstColorHue = BitmapExtensions.RgbToHsv(firstComplementary).Hue;
            int secondColorHue = BitmapExtensions.RgbToHsv(secondComplementary).Hue;
            int thirdColorHue = BitmapExtensions.RgbToHsv(thirdComplementary).Hue;

            // Triad
            //if ((predominantHue + 120) < 360 && (predominantHue - 120) > 0)
            //{
            //    firstColorHue = predominantHue + 120;
            //    secondColorHue = predominantHue - 120;
            //}
            //else if ((predominantHue + 120 > 360) && (predominantHue - 120) > 0)
            //{
            //    firstColorHue = 120 - 360 - predominantHue;
            //    secondColorHue = predominantHue - 120;
            //}
            //else if ((predominantHue + 120) < 360 && (predominantHue - 120) < 0)
            //{
            //    firstColorHue = predominantHue + 120;
            //    secondColorHue = 360 - 120 + predominantHue;
            //}

            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    SKColor color = originalImage.GetPixel(x, y);

                    if (croppedImage.GetPixel(x, y).Alpha != 0)
                    {
                        // Get HSV
                        HsvColor hsv = BitmapExtensions.RgbToHsv(color);

                        // Determine modified colors
                        SKColor firstColor = BitmapExtensions.HsvToRgb(new HsvColor(firstColorHue, hsv.Saturation, hsv.Value));
                        SKColor secondColor = BitmapExtensions.HsvToRgb(new HsvColor(secondColorHue, hsv.Saturation, hsv.Value));
                        SKColor thirdColor = BitmapExtensions.HsvToRgb(new HsvColor(thirdColorHue, hsv.Saturation, hsv.Value));

                        // Set modified colors
                        firstImage.SetPixel(x, y, firstColor);
                        secondImage.SetPixel(x, y, secondColor);
                        thirdImage.SetPixel(x, y, thirdColor);
                    }
                    else
                    {
                        // Set original colors
                        firstImage.SetPixel(x, y, color);
                        secondImage.SetPixel(x, y, color);
                        thirdImage.SetPixel(x, y, color);
                    }
                }
            }
            return new Tuple<SKBitmap, SKBitmap, SKBitmap>(firstImage, secondImage, thirdImage);
        }

    }
}
