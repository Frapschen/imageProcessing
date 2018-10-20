using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll
{
     public class xingTaiXue
    {
        /*
         * 1、腐蚀
            腐蚀是一种消除边界点，使边界向内部收缩的过程。可以用来消除小且无意义的物体。
         */
        /// <summary>
        /// 腐蚀（来消除小且无意义的物体）
        /// </summary>
        /// <param name="image">二值化图片</param>
        /// <returns></returns> 
        public static Bitmap GetFuShiImage(Bitmap image)
        {
            List<Point> setList = new List<Point>();
            Bitmap result = image.Clone() as Bitmap;
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    // 如果是应该设置为黑色的点，将其添加到我们要设的list中
                    if (SetPixelFuShi(result, i, j)) { setList.Add(new Point(i, j));
                    }
                }
            }
            // 遍历，设置相应的值
            foreach (var item in setList)
            {
                result.SetPixel(item.X, item.Y, Color.White);
            }
            return result;
        }
        // 判断一个点是不是应该设置为白颜色 
        protected static bool SetPixelFuShi(Bitmap image, int i, int j)
        {
            int origion; int upPoint; int leftPoint;
            if (i != 0 && j != 0)
            {
                origion = image.GetPixel(i, j).ToArgb();
                upPoint = image.GetPixel(i, j - 1).ToArgb();
                leftPoint = image.GetPixel(i - 1, j).ToArgb();
                // image.GetPixel(i,j) == Color.Black // 这样写居然没用，我还调试了老半天，也不报错！！！ 
                if (origion == Color.Black.ToArgb() && upPoint == Color.Black.ToArgb() && leftPoint == Color.Black.ToArgb())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return true;
        }

        /*2、膨胀
    思路和腐蚀算法极为相似，就是腐蚀算法的逆运算。
        如果指定的点是黑色的，那么就把它正上方和左边的像素点设置为黑色。
            */

        /// <summary>
        /// 膨胀算法 
        /// </summary> 
        /// <param name="image"></param> 
        /// <returns></returns> 
        public static Bitmap GetPengZhangImage(Bitmap image)
        {
            List<Point> setList = new List<Point>();
            Bitmap result = image.Clone() as Bitmap;
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    // 如果应该设置为黑色的
                    if (SetPixelPengZhang(result, i, j))
                    {
                        setList.Add(new Point(i, j));
                    }
                }
            }
            int x, y;
            foreach (var item in setList)
            {
                x = item.X;
                y = item.Y;
                result.SetPixel(x - 1, y, Color.Black);
                result.SetPixel(x, y - 1, Color.Black);
            }
            return result;
        }
        // 判断这个点应不应该设置为黑色
        protected static bool SetPixelPengZhang(Bitmap image, int i, int j)
        {
            Color c = image.GetPixel(i, j);
            if (i != 0 && j != 0)
            {
                if (image.GetPixel(i, j).ToArgb() == Color.Black.ToArgb())
                {
                    return true;
                }
                else
                    return false;

            }
            else return false;
        }


        ///* 
        // 1、开运算
        //    二值图像开运算的数学表达式为:
        //        g(x, y)=open[f(x, y ), B]=dilate{erode[f(x, y),B],B}
        //        二值图像的开运算事实上就是先作腐蚀运算,再作膨胀运算
        //        先腐蚀后膨胀的过程称为开运算。用来消除小物体、在纤细点处分离物体、平滑较大物体的边界的同时并不明显改变其面积。
        //        */
        //private void opening_Click(object sender, EventArgs e)
        //{
        //    if (curBitmap != null)
        //    {
        //        struction struForm = new struction();
        //        struForm.Text = "开运算结构元素";
        //        if (struForm.ShowDialog() == DialogResult.OK)
        //        {
        //            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
        //            System.Drawing.Imaging.BitmapData bmpData = curBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, curBitmap.PixelFormat);
        //            IntPtr ptr = bmpData.Scan0;
        //            int bytes = curBitmap.Width * curBitmap.Height;
        //            byte[] grayValues = new byte[bytes];
        //            Marshal.Copy(ptr, grayValues, 0, bytes);

        //            byte flagStru = struForm.GetStruction;

        //            byte[] temp1Array = new byte[bytes];
        //            byte[] tempArray = new byte[bytes];
        //            for (int i = 0; i < bytes; i++)
        //            {
        //                tempArray[i] = temp1Array[i] = 255;
        //            }

        //            switch (flagStru)
        //            {
        //                case 0x11:
        //                    //腐蚀运算
        //                    for (int i = 0; i < curBitmap.Height; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 0; i < curBitmap.Height; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x21:
        //                    //腐蚀运算
        //                    for (int i = 0; i < curBitmap.Height; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 2] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 0; i < curBitmap.Height; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 2] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x12:
        //                    //腐蚀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 0; j < curBitmap.Width; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 0; j < curBitmap.Width; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x22:
        //                    //腐蚀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 0; j < curBitmap.Width; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 0; j < curBitmap.Width; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x14:
        //                    //腐蚀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x24:
        //                    //腐蚀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 2] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 2] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x18:
        //                    //腐蚀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j + 1] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 1; i < curBitmap.Height - 1; i++)
        //                    {
        //                        for (int j = 1; j < curBitmap.Width - 1; j++)
        //                        {
        //                            if (temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j + 1] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                case 0x28:
        //                    //腐蚀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (grayValues[(i - 2) * curBitmap.Width + j - 2] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[(i - 2) * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j - 2] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[(i - 1) * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 2] == 0 &&
        //                                grayValues[i * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[i * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j - 2] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[(i + 2) * curBitmap.Width + j + 2] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j - 2] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j - 1] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j + 1] == 0 &&
        //                                grayValues[(i + 1) * curBitmap.Width + j + 2] == 0)
        //                            {
        //                                temp1Array[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    //膨胀运算
        //                    for (int i = 2; i < curBitmap.Height - 2; i++)
        //                    {
        //                        for (int j = 2; j < curBitmap.Width - 2; j++)
        //                        {
        //                            if (temp1Array[(i - 2) * curBitmap.Width + j - 2] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[(i - 2) * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j - 2] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[(i - 1) * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 2] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[i * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j - 2] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[(i + 2) * curBitmap.Width + j + 2] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j - 2] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j - 1] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j + 1] == 0 ||
        //                                temp1Array[(i + 1) * curBitmap.Width + j + 2] == 0)
        //                            {
        //                                tempArray[i * curBitmap.Width + j] = 0;
        //                            }

        //                        }
        //                    }
        //                    break;
        //                default:
        //                    MessageBox.Show("错误的结构元素！");
        //                    break;
        //            }


        //            grayValues = (byte[])tempArray.Clone();

        //            System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
        //            curBitmap.UnlockBits(bmpData);
        //        }

        //        Invalidate();
        //    }
        //}

        ///* 
        // *4、闭运算
        //    二值图像闭运算的数学表达式为:
        //    g(x, y)=close[f(x, y ), B]=erode{dilate[f(x, y),B],B}
        //    二值图像的闭运算事实上就是先作膨胀运算,再作腐蚀运算
        //    先膨胀后腐蚀的过程称为闭运算。用来填充物体内细小空洞、连接邻近物体、平滑其边界的同时并不明显改变其面积。
 
        // */



        ///*  5、击中及不中变换
        //    击中击不中变换定义
        //    击中击不中变换(HMT)需要两个结构元素B1和B2,合成一个结构元素对B=(B1,B2)
        //    一个用于探测图像内部,作为击中部分;另一个用于探测图像外部,作为击不中部分。显然,B1和B2是不应该相连接的,即B1∩B2=Φ。击中击不中变换的数学表达式为：
        //        g(x, y)=hitmiss[f(x, y), B]=erode[f(x, y), B1]AND erode[fc(x, y), B2]
        //            其中,fc(x,y)表示的是f(x,y)的补集。
        //         */
        ///// <summary>
        ///// 击中击不中：只能处理位深度为8的512*512图像
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void hitMiss_Click(object sender, EventArgs e)
        //{
        //    if (curBitmap != null)
        //    {
        //        hitmiss hitAndMiss = new hitmiss();
        //        if (hitAndMiss.ShowDialog() == DialogResult.OK)
        //        {
        //            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
        //            BitmapData bmpData = curBitmap.LockBits(rect, ImageLockMode.ReadWrite, curBitmap.PixelFormat);
        //            IntPtr ptr = bmpData.Scan0;
        //            int bytes = curBitmap.Width * curBitmap.Height;
        //            byte[] grayValues = new byte[bytes];
        //            Marshal.Copy(ptr, grayValues, 0, bytes);

        //            //得到击中结构元素
        //            bool[] hitStru = hitAndMiss.GetHitStruction;
        //            //得到击不中结构元素
        //            bool[] missStru = hitAndMiss.GetMissStruction;

        //            byte[] tempArray = new byte[bytes];
        //            byte[] temp1Array = new byte[bytes];
        //            byte[] temp2Array = new byte[bytes];
        //            for (int i = 0; i < bytes; i++)
        //            {
        //                //原图补集
        //                tempArray[i] = (byte)(255 - grayValues[i]);
        //                temp1Array[i] = 255;
        //                temp2Array[i] = 255;
        //            }

        //            //应用击中结构元素进行腐蚀运算
        //            for (int i = 1; i < curBitmap.Height - 1; i++)
        //            {
        //                for (int j = 1; j < curBitmap.Width - 1; j++)
        //                {
        //                    //当前位置是黑色或者是击中结构元素的这一位置没有选中
        //                    if ((grayValues[(i - 1) * curBitmap.Width + j - 1] == 0 || hitStru[0] == false) &&
        //                        (grayValues[(i - 1) * curBitmap.Width + j] == 0 || hitStru[1] == false) &&
        //                        (grayValues[(i - 1) * curBitmap.Width + j + 1] == 0 || hitStru[2] == false) &&
        //                        (grayValues[i * curBitmap.Width + j - 1] == 0 || hitStru[3] == false) &&
        //                        (grayValues[i * curBitmap.Width + j] == 0 || hitStru[4] == false) &&
        //                        (grayValues[i * curBitmap.Width + j + 1] == 0 || hitStru[5] == false) &&
        //                        (grayValues[(i + 1) * curBitmap.Width + j - 1] == 0 || hitStru[6] == false) &&
        //                        (grayValues[(i + 1) * curBitmap.Width + j] == 0 || hitStru[7] == false) &&
        //                        (grayValues[(i + 1) * curBitmap.Width + j + 1] == 0 || hitStru[8] == false))
        //                    {
        //                        temp1Array[i * curBitmap.Width + j] = 0;
        //                    }

        //                }
        //            }

        //            //应用击不中结构元素进行腐蚀运算
        //            for (int i = 1; i < curBitmap.Height - 1; i++)
        //            {
        //                for (int j = 1; j < curBitmap.Width - 1; j++)
        //                {
        //                    ////当前位置是黑色或者是击不中结构元素的这一位置没有选中
        //                    if ((tempArray[(i - 1) * curBitmap.Width + j - 1] == 0 || missStru[0] == false) &&
        //                        (tempArray[(i - 1) * curBitmap.Width + j] == 0 || missStru[1] == false) &&
        //                        (tempArray[(i - 1) * curBitmap.Width + j + 1] == 0 || missStru[2] == false) &&
        //                        (tempArray[i * curBitmap.Width + j - 1] == 0 || missStru[3] == false) &&
        //                        (tempArray[i * curBitmap.Width + j] == 0 || missStru[4] == false) &&
        //                        (tempArray[i * curBitmap.Width + j + 1] == 0 || missStru[5] == false) &&
        //                        (tempArray[(i + 1) * curBitmap.Width + j - 1] == 0 || missStru[6] == false) &&
        //                        (tempArray[(i + 1) * curBitmap.Width + j] == 0 || missStru[7] == false) &&
        //                        (tempArray[(i + 1) * curBitmap.Width + j + 1] == 0 || missStru[8] == false))
        //                    {
        //                        temp2Array[i * curBitmap.Width + j] = 0;
        //                    }

        //                }
        //            }

        //            //两个腐蚀运算结果再进行“与”操作
        //            for (int i = 0; i < bytes; i++)
        //            {
        //                if (temp1Array[i] == 0 && temp2Array[i] == 0)
        //                {
        //                    tempArray[i] = 0;
        //                }
        //                else
        //                {
        //                    tempArray[i] = 255;
        //                }
        //            }

        //            grayValues = (byte[])tempArray.Clone();

        //            Marshal.Copy(grayValues, 0, ptr, bytes);
        //            curBitmap.UnlockBits(bmpData);
        //        }

        //        Invalidate();
        //    }
        //}




    }
}
