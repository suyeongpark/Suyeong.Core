using System;

namespace Suyeong.Core.Util
{
    public static class MathUtil
    {
        public static float Round(float Num, float Den)
        {
            return IsZero(Den) ? 0f : (float)Math.Round((double)Num / (double)Den) * 1000f * 0.001f;
        }
        public static double Round(double Num, double Den)
        {
            return IsZero(Den) ? 0f : Math.Round(Num / Den) * 1000d * 0.001d;
        }

        public static bool IsEqual(float val1, float val2)
        {
            return IsZero(num: val1 - val2);
        }

        public static bool IsEqual(double val1, double val2)
        {
            return IsZero(num: val1 - val2);
        }

        public static bool IsZero(float num)
        {
            return (num < float.Epsilon && num > -float.Epsilon);
        }

        public static bool IsZero(double num)
        {
            return (num < double.Epsilon && num > -double.Epsilon);
        }

        public static bool IsPointBetweenLine(double lineStartX, double lineStartY, double lineEndX, double lineEndY, double x, double y)
        {
            double crossProduct = (y - lineStartY) * (lineEndX - lineStartX) - (x - lineStartX) * (lineEndY - lineStartY);

            if (Math.Abs(crossProduct) > double.Epsilon)
            {
                return false;
            }

            double dotProduct = (x - lineStartX) * (lineEndX - lineStartX) + (y - lineStartY) * (lineEndY - lineStartY);

            if (dotProduct < 0)
            {
                return false;
            }

            double deltaX = lineEndX - lineStartX;
            double deltaY = lineEndX - lineStartX;

            double length = deltaX * deltaX + deltaY * deltaY;

            if (dotProduct > length)
            {
                return false;
            }

            return true;
        }

        public static double RadianToDegree(double radian)
        {
            return radian * 180d / Math.PI;
        }

        public static double DegreeToRadian(double angle)
        {
            return angle * Math.PI / 180d;
        }

        public static double PixelToPoint(double dpi, double pointPerInch, double value)
        {
            return value * pointPerInch / dpi;
        }

        public static double PointToPixel(double dpi, double pointPerInch, double value)
        {
            return value * dpi / pointPerInch;
        }
    }
}
