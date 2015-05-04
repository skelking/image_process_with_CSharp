using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;


namespace image_process
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\paul\\Desktop\\image\\rectangle.jpg";
            string color_path="C:\\Users\\paul\\Desktop\\image\\color.jpg";
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(path);       //获取源图像
            Image<Gray, Byte> imageGray = imageSource.Convert<Gray, Byte>(); //将源图像转换成灰度图像
            Image<Bgr,Byte> color_image=new Image<Bgr, byte>(color_path); 

            IntPtr Dyncontour = new IntPtr();
            IntPtr Dynstorage = CvInvoke.cvCreateMemStorage(0);//开辟内存区域  

            int m = 88;//在不安全处理下获取的数据  
            
            int n = CvInvoke.cvFindContours(imageGray, Dynstorage, ref Dyncontour, m, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, new Point(1, 1));
            Seq<Point> temp = new Seq<Point>(Dyncontour, null);
            int w=CvInvoke.cvGetSize(imageSource).Width;
            int h=CvInvoke.cvGetSize(imageSource).Height;
            Image<Gray,Byte> image_draw=new Image<Gray,Byte>(w,h);
            image_draw.SetZero();
            for (; temp != null && temp.Ptr.ToInt32() != 0; temp = temp.HNext)
            {
                double x_all = 0, y_all = 0;
                
                for (int i = 0; i < temp.Total; i++)
                {
                    x_all += temp[i].X;
                    y_all += temp[i].Y; 
                }
                //中心点
                int x_ave = (int)x_all / temp.Total;
                int y_ave = (int)y_all / temp.Total;

                int roi_w = 0, roi_h = 0;
                while (CvInvoke.cvPointPolygonTest(temp, new Point(x_ave + roi_w, y_ave + roi_h), true) > 0)
                {
                    roi_h++;
                    roi_w++;
                }

                roi_h -= 3;
                roi_w -= 3;
                Rectangle rect = new Rectangle(new Point(x_ave, y_ave), new Size(roi_w , roi_w));
                CvInvoke.cvSetImageROI(color_image, rect);
                Bgr bgr = color_image.GetAverage();
                /////

                CvInvoke.cvCircle(image_draw, new Point(x_ave, y_ave), 2, new MCvScalar(255, 255, 255), 1, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, 0);
                CvInvoke.cvDrawContours(image_draw, temp, new MCvScalar(128, 128, 128), new MCvScalar(255, 255, 255), 0, 1, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, new Point(0, 0));
                CvInvoke.cvResetImageROI(color_image);
            }

            CvInvoke.cvNamedWindow("drawing");
            CvInvoke.cvShowImage("drawing", image_draw);
            CvInvoke.cvWaitKey(0);

            

        }
    }
}
