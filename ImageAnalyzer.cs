using System;
using System.Drawing;
using System.Collections;
using _5laba;
using static _5laba.DataStorageModule;

public class ImageAnalyzer
{
    private PerceptualHash _perceptualHash = new PerceptualHash();
    private DataStorageModule _dataStorage = new DataStorageModule();

    public (Dictionary<string, Bitmap>, Bitmap, string, string) Analyze(Bitmap image, string semanticInfo, Dictionary<Bitmap, DataRecord> existhashes)
    {
        var binaryImage = ConvertToBinary(image);
        var labels = LabelComponents(binaryImage);
        var connectedComponents = ExtractConnectedComponents(labels, image);
        var hashes = new Dictionary<string, Bitmap>();
        Bitmap img = null;
        string mosthash = "";
        var distance = int.MaxValue;
        var s = "";
       
        foreach (var connectedComponent in connectedComponents)
        {
            var (hash, bitmap) = _perceptualHash.GetHash(connectedComponent);
            hashes[hash] = bitmap;
          
            var  (img1, mosthash1, distance1, s1) = FindMostSimilarImage(hash, existhashes);
            if(distance1 < distance)
            {
                distance = distance1;
                img = img1;
                mosthash = mosthash1;
                s = s1;
            }
            
        }
        //var h = _perceptualHash.GetHash(image);
        //img = FindMostSimilarImage(h.hash, existedimgInfo);
        return (hashes, img, mosthash, s);
    }
    public Dictionary<string, Bitmap> AnalyzeAddImg(Bitmap image, string semanticInfo)
    {
        var binaryImage = ConvertToBinary(image);
        var labels = LabelComponents(binaryImage);
        var connectedComponents = ExtractConnectedComponents(labels, image);
        var hashes = new Dictionary<string, Bitmap>();
        var existedimgInfo = _dataStorage.GetRecords().ToDictionary(x => x.Key, X => X.Value.Img);
        foreach (var connectedComponent in connectedComponents)
        {
            var (hash, bitmap) = _perceptualHash.GetHash(connectedComponent);
            hashes[hash] = bitmap;
        }

        return hashes;
    }
    private int[,] ConvertToBinary(Bitmap image)
    {
        var binaryImage = new int[image.Width, image.Height];
        for (int i = 0; i < image.Width; i++)
        {
            for (int j = 0; j < image.Height; j++)
            {
                binaryImage[i, j] = image.GetPixel(i, j).R > 128 ? 1 : 0;
            }
        }

        return binaryImage;
    }

    private int[,] LabelComponents(int[,] binaryImage)
    {
        int labelCount = 1;
        int width = binaryImage.GetLength(0);
        int height = binaryImage.GetLength(1);
        int[,] labels = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (binaryImage[x, y] != 0 && labels[x, y] == 0)
                {
                    FloodFill(binaryImage, labels, x, y, labelCount);
                    labelCount++;
                }
            }
        }

        return labels;
    }

    private void FloodFill(int[,] binaryImage, int[,] labels, int x, int y, int label)
    {
        if (x >= 0 && y >= 0 && x < binaryImage.GetLength(0) && y < binaryImage.GetLength(1) && binaryImage[x, y] != 0 && labels[x, y] == 0)
        {
            labels[x, y] = label;

            FloodFill(binaryImage, labels, x - 1, y, label);
            FloodFill(binaryImage, labels, x + 1, y, label);
            FloodFill(binaryImage, labels, x, y - 1, label);
            FloodFill(binaryImage, labels, x, y + 1, label);
        }
    }

    private List<Bitmap> ExtractConnectedComponents(int[,] labels, Bitmap image)
    {
        var connectedComponents = new List<Bitmap>();
        var labelCount = labels.Cast<int>().Max();

        for (int i = 1; i <= labelCount; i++)
        {
            var minX = labels.GetLength(0);
            var minY = labels.GetLength(1);
            var maxX = 0;
            var maxY = 0;

            for (int x = 0; x < labels.GetLength(0); x++)
            {
                for (int y = 0; y < labels.GetLength(1); y++)
                {
                    if (labels[x, y] == i)
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            var width = maxX - minX + 1;
            var height = maxY - minY + 1;
            var component = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    component.SetPixel(x, y, image.GetPixel(minX + x, minY + y));
                }
            }

            connectedComponents.Add(component);
        }

        return connectedComponents;
    }

    public (Bitmap, string, int, string) FindMostSimilarImage(string hash, Dictionary<Bitmap, DataRecord> hashes)
    {
        int minDistance = int.MaxValue;
        Bitmap mostSimilarImage = null;
        string mosthash = "";
        string s = "";
        foreach (var pair in hashes)
        {
            int distance = HammingDistance(hash, pair.Value.Hash);
            if (distance < minDistance)
            {
                minDistance = distance;
                mostSimilarImage = pair.Key;
                mosthash = pair.Value.Hash;
                s = pair.Value.SemanticInformation;
            }
        }
        return (mostSimilarImage, mosthash, minDistance, s);
    }

    private int HammingDistance(string hash1, string hash2)
    {
        int distance = 0;
        for (int i = 0; i < hash1.Length; i++)
        {
            if (hash1[i] != hash2[i])
            {
                distance++;
            }
        }
        return distance;
    }

}
//public Bitmap AnalyzeWithRetry(Bitmap image, int maxAttempts)
//{
//    for (int attempt = 0; attempt < maxAttempts; attempt++)
//    {
//        var hashes = Analyze(image);
//        var mostSimilarImage = FindMostSimilarImage(targetHash, hashes);
//        if (mostSimilarImage != null)
//        {
//            return mostSimilarImage;
//        }
//        // Измените параметры анализа здесь, например:
//        // image = ApplySomeTransformation(image);
//    }
//    return null;
//}