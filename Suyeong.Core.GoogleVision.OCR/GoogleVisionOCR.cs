using System.Collections.Generic;
using Google.Cloud.Vision.V1;

namespace Suyeong.Core.GoogleVision.OCR
{
    public static class GoogleVisionOCR
    {
        public static OcrTextCollection DetectTextByGoogleVision(string imagePath)
        {
            OcrTextCollection textBlocks = new OcrTextCollection();

            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> response = client.DetectText(Image.FromFile(imagePath));

            int minX, maxX, minY, maxY, centerX, centerY, rotate, index = 0;

            foreach (EntityAnnotation annotation in response)
            {
                if (index > 0)
                {
                    minX = minY = int.MaxValue;
                    maxX = maxY = 0;

                    foreach (Vertex vertex in annotation.BoundingPoly.Vertices)
                    {
                        minX = vertex.X < minX ? vertex.X : minX;
                        maxX = vertex.X > maxX ? vertex.X : maxX;
                        minY = vertex.Y < minY ? vertex.Y : minY;
                        maxY = vertex.Y > maxY ? vertex.Y : maxY;
                    }

                    centerX = (int)((minX + maxX) * 0.5d);
                    centerY = (int)((minY + maxY) * 0.5d);
                    rotate = GetRotate(centerX: centerX, centerY: centerY, firstVertex: annotation.BoundingPoly.Vertices[0]);

                    textBlocks.Add(new OcrText(index: index, rotate: rotate, leftX: minX, rightX: maxX, topY: minY, bottomY: maxY, text: annotation.Description));
                }

                index++;
            }

            return textBlocks;
        }

        static int GetRotate(Vertex firstVertex, int centerX, int centerY)
        {
            if (firstVertex.X < centerX)
            {
                if (firstVertex.Y < centerY)
                {
                    return 0;
                }
                else
                {
                    return 270;
                }
            }
            else
            {
                if (firstVertex.Y < centerY)
                {
                    return 90;
                }
                else
                {
                    return 180;
                }
            }
        }
    }
}
