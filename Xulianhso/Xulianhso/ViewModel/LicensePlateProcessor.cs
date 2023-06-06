using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;

namespace Xulianhso.ViewModel
{
    public class LicensePlateProcessor
    {
        private TesseractEngine _ocr;

        public LicensePlateProcessor()
        {
            _ocr = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractOnly);
            _ocr.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        }

        public string ExtractLicensePlate(string imagePath)
        {
            // Đọc ảnh từ đường dẫn
            Mat image = CvInvoke.Imread(imagePath, ImreadModes.Color);

            // Chuyển đổi ảnh sang ảnh xám
            Mat gray = new Mat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            // Áp dụng các bộ lọc và phép biến đổi để nhận diện biển số
            Mat threshold = new Mat();
            CvInvoke.Threshold(gray, threshold, 150, 255, ThresholdType.Binary);

            // Tìm các contour trong ảnh
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(threshold, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            // Tìm contour có diện tích lớn nhất (giả sử là biển số)
            double maxArea = 0;
            int maxAreaContourIndex = -1;
            for (int i = 0; i < contours.Size; i++)
            {
                double area = CvInvoke.ContourArea(contours[i]);
                if (area > maxArea)
                {
                    maxArea = area;
                    maxAreaContourIndex = i;
                }
            }

            // Tạo hình chữ nhật bao quanh biển số
            Rectangle licensePlateRect = CvInvoke.BoundingRectangle(contours[maxAreaContourIndex]);

            // Cắt hình ảnh biển số từ ảnh gốc
            Mat licensePlateImage = new Mat(image, licensePlateRect);

            // Chuyển đổi Mat thành Bitmap
            Bitmap licensePlateBitmap = new Bitmap(licensePlateImage.Width, licensePlateImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData bmpData = licensePlateBitmap.LockBits(new Rectangle(0, 0, licensePlateBitmap.Width, licensePlateBitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, licensePlateBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * licensePlateBitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(licensePlateImage.DataPointer, rgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            licensePlateBitmap.UnlockBits(bmpData);

            // Nhận dạng và trả về chuỗi biển số
            using (var page = _ocr.Process(licensePlateBitmap))
            {
                string licensePlate = page.GetText();
                return licensePlate;
            }
        }
    }

}
