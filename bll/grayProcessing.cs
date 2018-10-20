using System.Drawing;
using System.Drawing.Imaging;

namespace bll
{
    /// <summary>
    /// 灰度化
    /// </summary>
    public class grayProcessing  
    {
       // static void Main()
       // {
            //Bitmap b = file2img("C:\\Users\\msi\\Pictures\\aaa.jpg");
            //Bitmap bb = Img_gray(b);
            //img2file(bb, "C:\\Users\\msi\\Pictures\\new_aaa.jpg");
       // }


        //图片灰度化实现函数
        //图片灰度化
        public static unsafe Bitmap Img_gray(Bitmap curBitmap)
        {
            int width = curBitmap.Width;
            int height = curBitmap.Height;
            Bitmap back = new Bitmap(width, height);
            byte temp;
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            //这种速度最快
            BitmapData bmpData = curBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//24位rgb显示一个像素，即一个像素点3个字节，每个字节是BGR分量。Format32bppRgb是用4个字节表示一个像素
            byte* ptr = (byte*)(bmpData.Scan0);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    //ptr[2]为r值，ptr[1]为g值，ptr[0]为b值
                    temp = (byte)(0.299 * ptr[2] + 0.587 * ptr[1] + 0.114 * ptr[0]);
                    back.SetPixel(i, j, Color.FromArgb(temp, temp, temp));
                    ptr += 3; //Format24bppRgb格式每个像素占3字节
                }
                ptr += bmpData.Stride - bmpData.Width * 3;//每行读取到最后“有用”数据时，跳过未使用空间XX
            }
            curBitmap.UnlockBits(bmpData);
            return back;
        }


        

    }
}
