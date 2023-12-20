using System.Drawing.Imaging;
using System.Drawing;


namespace _5laba
{
    public class ImagePreparationModule
    {
        public Bitmap PrepareImgFunc(Bitmap image, float contrast, int windowSize)
        {
           //  image = AdjustContrast(image, contrast);
            // Преобразование в полутоновое изображение
            image.Save(@"C:\проектв\5laba\database\monochromeImage.jpg", ImageFormat.Jpeg);
            Bitmap grayscaleImage = ConvertToGrayscale(image);

            grayscaleImage = ApplyMinFilter(grayscaleImage, windowSize);
            grayscaleImage = ApplyMaxFilter(grayscaleImage, windowSize);
            // Преобразование в монохромное изображение
            Bitmap monochromeImage = ConvertToMonochrome(grayscaleImage);

           // monochromeImage.Save(@"C:\проектв\5laba\database\monochromeImage.jpg", ImageFormat.Jpeg);
            return monochromeImage;
        }

        public Bitmap ConvertToGrayscale(Bitmap image)
        {
            // Реализация преобразования изображения в полутоновое
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    int grayScale = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                    Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                    image.SetPixel(i, j, grayColor);
                }
            }

            return image;
        }

        public Bitmap ConvertToMonochrome(Bitmap image)
        {
            // Преобразование в монохромное изображение
            Bitmap monoImage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    if (pixelColor.R > 128)
                        monoImage.SetPixel(i, j, Color.White);
                    else
                        monoImage.SetPixel(i, j, Color.Black);
                }
            }

            return MorphologicalDilation(image);
        }


        public Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            float transfer = (100.0f - contrast) / 100.0f;
            float shift = transfer * 128.0f;

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color oldColor = image.GetPixel(i, j);
                    float red = (oldColor.R / 255.0f - 0.5f) * transfer + shift; // изменение здесь
                    float green = (oldColor.G / 255.0f - 0.5f) * transfer + shift; // изменение здесь
                    float blue = (oldColor.B / 255.0f - 0.5f) * transfer + shift; // изменение здесь
                    int iR = red > 1 ? 255 : red < 0 ? 0 : (int)(red * 255);
                    int iG = green > 1 ? 255 : green < 0 ? 0 : (int)(green * 255);
                    int iB = blue > 1 ? 255 : blue < 0 ? 0 : (int)(blue * 255);

                    newImage.SetPixel(i, j, Color.FromArgb(oldColor.A, iR, iG, iB));
                }
            }

            return newImage;
        }

        public Bitmap ApplyMinFilter(Bitmap image, int windowSize)
        {
            // Реализация применения фильтра "минимум"
            // windowSize в оба метода.
            // Этот параметр определяет размер окна, в котором считается минимальное или максимальное значение для каждого пикселя.
            // Размер окна должен быть нечетным числом.
            Bitmap newImage = new Bitmap(image.Width, image.Height);

            int radius = windowSize / 2;

            for (int i = radius; i < image.Width - radius; i++)
            {
                for (int j = radius; j < image.Height - radius; j++)
                {
                    int minR = 255, minG = 255, minB = 255;

                    for (int k = -radius; k <= radius; k++)
                    {
                        for (int l = -radius; l <= radius; l++)
                        {
                            Color pixel = image.GetPixel(i + k, j + l);
                            minR = Math.Min(minR, pixel.R);
                            minG = Math.Min(minG, pixel.G);
                            minB = Math.Min(minB, pixel.B);
                        }
                    }

                    newImage.SetPixel(i, j, Color.FromArgb(minR, minG, minB));
                }
            }

            return newImage;
        }

        public Bitmap ApplyMaxFilter(Bitmap image, int windowSize)
        {

            // Реализация применения фильтра "максимум"
            // windowSize в оба метода.
            // Этот параметр определяет размер окна, в котором считается минимальное или максимальное значение для каждого пикселя.
            // Размер окна должен быть нечетным числом.
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            int offset = windowSize / 2;

            for (int i = offset; i < image.Width - offset; i++)
            {
                for (int j = offset; j < image.Height - offset; j++)
                {
                    int maxR = 0, maxG = 0, maxB = 0;

                    for (int k = -offset; k <= offset; k++)
                    {
                        for (int l = -offset; l <= offset; l++)
                        {
                            Color pixel = image.GetPixel(i + k, j + l);
                            maxR = Math.Max(maxR, pixel.R);
                            maxG = Math.Max(maxG, pixel.G);
                            maxB = Math.Max(maxB, pixel.B);
                        }
                    }

                    newImage.SetPixel(i, j, Color.FromArgb(maxR, maxG, maxB));
                }
            }

            return newImage;
        }

        public Bitmap MorphologicalDilation(Bitmap image)
        {
            // Реализация морфологических преобразований над монохромным изображением
            // Реализация морфологической дилатации

            Bitmap newImage = new Bitmap(image.Width, image.Height);

            for (int i = 1; i < image.Width - 1; i++)
            {
                for (int j = 1; j < image.Height - 1; j++)
                {
                    int maxR = 0, maxG = 0, maxB = 0;

                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            Color pixel = image.GetPixel(i + k, j + l);
                            maxR = Math.Max(maxR, pixel.R);
                            maxG = Math.Max(maxG, pixel.G);
                            maxB = Math.Max(maxB, pixel.B);
                        }
                    }

                    newImage.SetPixel(i, j, Color.FromArgb(maxR, maxG, maxB));
                }
            }

            return newImage;
        }
    }
}
