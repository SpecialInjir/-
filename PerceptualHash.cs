using System;
using System.Drawing;
using System.Linq;
namespace _5laba
{


    public class PerceptualHash
    {
        private const int Size = 8; // Размер изображения для хэша

        public (string hash, Bitmap connectedComponent) GetHash(Bitmap connectedComponent)
        {
            // 1. Изменить размер изображения до 8x8
            var resizedImage = ResizeImage(connectedComponent, Size, Size);

            // 3. Вычислить среднее значение
            var mean = CalculateMean(resizedImage);

            // 4. Бинаризовать изображение
            var binaryImage = Binarize(resizedImage, mean);

            // 5. Преобразовать бинарное изображение в хэш-строку
            var hash = ConvertToHash(binaryImage);

            return (hash, connectedComponent);
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var resizedImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return resizedImage;
        }

        //private Bitmap ConvertToGrayscale(Bitmap image)
        //{
        //    var grayImage = new Bitmap(image.Width, image.Height);
        //    for (int i = 0; i < image.Width; i++)
        //    {
        //        for (int j = 0; j < image.Height; j++)
        //        {
        //            var pixel = image.GetPixel(i, j);
        //            var grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
        //            grayImage.SetPixel(i, j, Color.FromArgb(grayScale, grayScale, grayScale));
        //        }
        //    }

        //    return grayImage;
        //}

        private double CalculateMean(Bitmap image)
        {
            long sum = 0;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    sum += image.GetPixel(i, j).R;
                }
            }

            return sum / (double)(image.Width * image.Height);
        }

        private int[,] Binarize(Bitmap image, double threshold)
        {
            var binaryImage = new int[image.Width, image.Height];
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    binaryImage[i, j] = image.GetPixel(i, j).R >= threshold ? 1 : 0;
                }
            }

            return binaryImage;
        }

        private string ConvertToHash(int[,] binaryImage)
        {
            return string.Join("", binaryImage.Cast<int>());
        }
    }

}
