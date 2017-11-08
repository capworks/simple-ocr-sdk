using System;
using System.Drawing;
using System.Linq;

namespace PreProcessing
{
    public interface IImagePreProcessingUtils
    {
        Bitmap RemoveTransparency(Bitmap bitmap);
        bool AdjustImageOrientation(Bitmap src);
    }

    public class ImagePreProcessingUtils : IImagePreProcessingUtils
    {
        public Bitmap RemoveTransparency(Bitmap bitmap)
        {
            var temp = new Bitmap(bitmap.Width, bitmap.Height);
            temp.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            var g = Graphics.FromImage(temp);
            g.Clear(Color.White);
            g.DrawImage(bitmap, 0, 0);
            return temp;
        }

        public bool AdjustImageOrientation(Bitmap src)
        {
            const int ExifOrientationId = 0x112;

            try
            {
                // Read orientation tag
                if (!src.PropertyIdList.ToList().Contains(ExifOrientationId)) return false;

                var prop = src.GetPropertyItem(ExifOrientationId);
                var orient = BitConverter.ToInt16(prop.Value, 0);
                // Force value to 1
                prop.Value = BitConverter.GetBytes((short)1);
                src.SetPropertyItem(prop);

                // Rotate/flip image according to <orient>
                switch (orient)
                {
                    case 2:
                        src.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        src.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        src.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        src.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        src.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        src.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        src.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 1:
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}