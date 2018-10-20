using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
//

namespace bll
{
    /// <summary>
    /// 过波
    /// </summary>
    public class waveFiltering
    {
        ///<summary>
        ///均值滤波
        ///Mean filter process.
        ///</summary>
        ///<param
        ///name="src">Source image</param>
        ///<returns>WriteableBitmap</returns>
        public static WriteableBitmap MeanFilterProcess(WriteableBitmap src)////9均值滤波处理
        {
            if (src != null)
            {
                int w = src.PixelWidth;
                int h = src.PixelHeight;
                WriteableBitmap filterImage = new WriteableBitmap(w, h);
                byte[] temp = src.PixelBuffer.ToArray();
                byte[] tempMask = (byte[])temp.Clone();
                for (int j = 1; j < h - 1; j++)
                {
                    for (int i = 4; i < w * 4 - 4; i += 4)
                    {
                        temp[i + j * w * 4] = (byte)((tempMask[i - 4 + (j - 1) * w * 4] + tempMask[i + (j - 1) * w * 4] + tempMask[i + 4 + (j - 1) * w * 4] + tempMask[i - 4 + j * w * 4] + tempMask[i + 4 + j * w * 4] + tempMask[i - 4 + (j + 1) * w * 4] + tempMask[i + (j + 1) * w * 4] + tempMask[i + 4 + (j + 1) * w * 4]) / 8);
                        temp[i + 1 + j * w * 4] = (byte)((tempMask[i - 4 + 1 + (j - 1) * w * 4] + tempMask[i + 1 + (j - 1) * w * 4] + tempMask[i + 1 + 4 + (j - 1) * w * 4] + tempMask[i + 1 - 4 + j * w * 4] + tempMask[i + 1 + 4 + j * w * 4] + tempMask[i + 1 - 4 + (j + 1) * w * 4] + tempMask[i + 1 + (j + 1) * w * 4] + tempMask[i + 1 + 4 + (j + 1) * w * 4]) / 8);
                        temp[i + 2 + j * w * 4] = (byte)((tempMask[i + 2 - 4 + (j - 1) * w * 4] + tempMask[i + 2 + (j - 1) * w * 4] + tempMask[i + 2 + 4 + (j - 1) * w * 4] + tempMask[i + 2 - 4 + j * w * 4] + tempMask[i + 2 + 4 + j * w * 4] + tempMask[i + 2 - 4 + (j + 1) * w * 4] + tempMask[i + 2 + (j + 1) * w * 4] + tempMask[i + 2 + 4 + (j + 1) * w * 4]) / 8);

                    }
                }
                Stream sTemp = filterImage.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, w * 4 * h);
                return filterImage;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 中值滤波算法处理
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="bmp">是否是彩色位图</param>
        /// <param name="windowRadius">过滤半径</param>
        public Bitmap ColorfulBitmapMedianFilterFunction(Bitmap srcBmp, int windowRadius, bool IsColorfulBitmap)
        {
            if (windowRadius < 1)
            {
                throw new Exception("过滤半径小于1没有意义");
            }
            //创建一个新的位图对象
            Bitmap bmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            //存储该图片所有点的RGB值
            byte[,] mR, mG, mB;
            mR = new byte[srcBmp.Width, srcBmp.Height];
            if (IsColorfulBitmap)
            {
                mG = new byte[srcBmp.Width, srcBmp.Height];
                mB = new byte[srcBmp.Width, srcBmp.Height];
            }
            else
            {
                mG = mR;
                mB = mR;
            }

            for (int i = 0; i <= srcBmp.Width - 1; i++)
            {
                for (int j = 0; j <= srcBmp.Height - 1; j++)
                {
                    mR[i, j] = srcBmp.GetPixel(i, j).R;
                    if (IsColorfulBitmap)
                    {
                        mG[i, j] = srcBmp.GetPixel(i, j).G;
                        mB[i, j] = srcBmp.GetPixel(i, j).B;
                    }
                }
            }

            mR = MedianFilterFunction(mR, windowRadius);
            if (IsColorfulBitmap)
            {
                mG = MedianFilterFunction(mG, windowRadius);
                mB = MedianFilterFunction(mB, windowRadius);
            }
            else
            {
                mG = mR;
                mB = mR;
            }
            for (int i = 0; i <= bmp.Width - 1; i++)
            {
                for (int j = 0; j <= bmp.Height - 1; j++)
                {
                    bmp.SetPixel(i, j, Color.FromArgb(mR[i, j], mG[i, j], mB[i, j]));
                }
            }
            return bmp;
        }




        /// <summary>
        /// 对矩阵M进行中值滤波
        /// </summary>
        /// <param name="m">矩阵M</param>
        /// <param name="windowRadius">过滤半径</param>
        /// <returns>结果矩阵</returns>
        private byte[,] MedianFilterFunction(byte[,] m, int windowRadius)
        {
            int width = m.GetLength(0);
            int height = m.GetLength(1);

            byte[,] lightArray = new byte[width, height];

            //开始滤波
            for (int i = 0; i <= width - 1; i++)
            {
                for (int j = 0; j <= height - 1; j++)
                {
                    //得到过滤窗口矩形
                    Rectangle rectWindow = new Rectangle(i - windowRadius, j - windowRadius, 2 * windowRadius + 1, 2 * windowRadius + 1);
                    if (rectWindow.Left < 0) rectWindow.X = 0;
                    if (rectWindow.Top < 0) rectWindow.Y = 0;
                    if (rectWindow.Right > width - 1) rectWindow.Width = width - 1 - rectWindow.Left;
                    if (rectWindow.Bottom > height - 1) rectWindow.Height = height - 1 - rectWindow.Top;
                    //将窗口中的颜色取到列表中
                    List<byte> windowPixelColorList = new List<byte>();
                    for (int oi = rectWindow.Left; oi <= rectWindow.Right - 1; oi++)
                    {
                        for (int oj = rectWindow.Top; oj <= rectWindow.Bottom - 1; oj++)
                        {
                            windowPixelColorList.Add(m[oi, oj]);
                        }
                    }
                    //排序
                    windowPixelColorList.Sort();
                    //取中值
                    byte middleValue = 0;
                    if ((windowRadius * windowRadius) % 2 == 0)
                    {
                        //如果是偶数
                        middleValue = Convert.ToByte((windowPixelColorList[windowPixelColorList.Count / 2] + windowPixelColorList[windowPixelColorList.Count / 2 - 1]) / 2);
                    }
                    else
                    {
                        //如果是奇数
                        middleValue = windowPixelColorList[(windowPixelColorList.Count - 1) / 2];
                    }
                    //设置为中值
                    lightArray[i, j] = middleValue;
                }
            }
            return lightArray;
        }



        /// <summary>
        ///  去掉杂点（适合杂点/杂线粗为1）
        /// </summary>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>        
        public void ClearNoise(Bitmap bmpobj,int dgGrayValue, int MaxNearPoints)
        {
            Color piexl;
            int nearDots = 0;
            int XSpan, YSpan, tmpX, tmpY;
            //逐点判断            
            for (int i = 0; i < bmpobj.Width; i++)
                for (int j = 0; j<bmpobj.Height; j++)
                {
                    piexl = bmpobj.GetPixel(i, j);
                    if (piexl.R<dgGrayValue)
                    {
                        nearDots = 0;
                        //判断周围8个点是否全为空                        
                        if (i == 0 || i == bmpobj.Width - 1 || j == 0 || j == bmpobj.Height - 1)  //边框全去掉                        {
                            bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        if (bmpobj.GetPixel(i - 1, j - 1).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i, j - 1).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i + 1, j - 1).R<dgGrayValue) nearDots++;    
                        if (bmpobj.GetPixel(i - 1, j).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i + 1, j).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i - 1, j + 1).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i, j + 1).R<dgGrayValue) nearDots++;
                        if (bmpobj.GetPixel(i + 1, j + 1).R<dgGrayValue) nearDots++;
                    }
                    if (nearDots<MaxNearPoints)
                        bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));   //去掉单点 && 粗细小3邻边点                    }
                    else  //背景                        
                        bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
        }

    }


    //一个处理类
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                //gauss Gauss = new gauss();

                //if (MessageBox.Show("执行高斯模糊？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    double sigma = Convert.ToDouble(this.textBox1.Text);//标准方差

                    int mask = Convert.ToInt32(Math.Ceiling(3.0 * sigma));

                    Bitmap bm1 = new Bitmap(pictureBox1.Image);
                    Bitmap bmGauss = new Bitmap(bm1.Width, bm1.Height);

                    double[,] GaussSmooth = smooth(bm1, sigma);

                    for (int i = 0; i < bmGauss.Width; i++)
                    {
                        for (int j = 0; j < bmGauss.Height; j++)
                        {
                            int pix = (int)(GaussSmooth[i, j] * 255);

                            if (pix < 0)
                                pix = 0;
                            else if (pix > 255)
                                pix = 255;

                            bmGauss.SetPixel(i, j, Color.FromArgb(pix, pix, pix));
                        }
                    }

                    pictureBox2.Refresh();
                    pictureBox2.Image = bmGauss;
                    //label2.Text = "高斯滤波";
                }
            }
        }

        public double[,] ImagedateRGB(Bitmap bm) //图像的值
        {
            Color c = new Color();

            int BmIw = bm.Width;
            int BmIh = bm.Height;

            double[,] Imagedate = new double[BmIw, BmIh];

            for (int i = 0; i < BmIw; i++)
            {
                for (int j = 0; j < BmIh; j++)
                {
                    c = bm.GetPixel(i, j);
                    Imagedate[i, j] = (double)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B); //(double)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                }
            }

            return Imagedate;
        }

        public double[,] fliter(double sigma) //高斯滤波模板数值
        {
            int mask = Convert.ToInt32(Math.Ceiling(3.0 * sigma));

            double[,] Gaussfliter = new double[2 * mask + 1, 2 * mask + 1];

            double a = 1.0 / (2 * sigma * sigma);

            double sum = 0.0;

            for (int i = 0; i <= 2 * mask; i++)
            {
                for (int j = 0; j <= 2 * mask; j++)
                {
                    Gaussfliter[i, j] = Math.Exp(-1.0 * ((i - mask) * (i - mask) + (j - mask) * (j - mask)) * a);
                    sum += Gaussfliter[i, j];
                }
            }

            for (int i = 0; i <= 2 * mask; i++)
            {
                for (int j = 0; j <= 2 * mask; j++)
                {
                    Gaussfliter[i, j] /= sum;
                }
            }

            return Gaussfliter;
        }

        public double[,] newImage(Bitmap bm, int mask)  //图像扩边
        {
            double[,] data = ImagedateRGB(bm);
            double[,] newImagedata = new double[bm.Width + 2 * mask, bm.Height + 2 * mask];

            for (int i = mask; i < bm.Width + mask; i++)  //输入图像的数据拷贝到新图像的中心位置
            {
                for (int j = mask; j < bm.Height + mask; j++)
                {
                    newImagedata[i, j] = data[i - mask, j - mask];
                }
            }

            for (int i = mask; i < bm.Width + mask; i++)  //上边界
            {
                for (int j = 0; j < mask; j++)
                {
                    newImagedata[i, j] = newImagedata[i, j + mask];
                }
            }

            for (int i = mask; i < bm.Width + mask; i++)  //下边界
            {
                for (int j = bm.Height + mask; j < bm.Height + 2 * mask; j++)
                {
                    newImagedata[i, j] = newImagedata[i, j - mask];
                }
            }

            for (int i = 0; i < mask; i++)  //左边界
            {
                for (int j = 0; j < bm.Height + 2 * mask; j++)
                {
                    newImagedata[i, j] = newImagedata[i + mask, j];
                }
            }

            for (int i = bm.Width + mask; i < bm.Width + 2 * mask; i++)  //右边界
            {
                for (int j = 0; j < bm.Height + 2 * mask; j++)
                {
                    newImagedata[i, j] = newImagedata[i - mask, j];
                }
            }

            return newImagedata;
        }

        public double[,] smooth(Bitmap bm, double sigma)  //高斯滤波
        {
            int mask = Convert.ToInt32(Math.Ceiling(3.0 * sigma));

            double[,] NewImage = newImage(bm, mask);
            double[,] image = new double[bm.Width, bm.Height];

            double[,] Gaussfliter = fliter(sigma);

            for (int i = mask; i < bm.Width + mask; i++) //滤波相乘
            {
                for (int j = mask; j < bm.Height + mask; j++)
                {
                    for (int u = 0; u <= 2 * mask; u++)
                    {
                        for (int v = 0; v <= 2 * mask; v++)
                        {
                            NewImage[i, j] += NewImage[i - mask + u, j - mask + v] * Gaussfliter[u, v];
                        }
                    }
                }
            }

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    image[i, j] = NewImage[i + mask, j + mask];
                }
            }

            double max = image[0, 0];
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    if (image[i, j] > max)
                        max = image[i, j];
                }
            }

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    image[i, j] /= max;
                }
            }

            return image;
        }

    }

}
