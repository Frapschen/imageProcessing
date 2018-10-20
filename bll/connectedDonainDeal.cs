using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace bll
{
    /// <summary>
    /// 连通域及其质心标记
    /// </summary>
    public  class connectedDonainDeal
    {
        //构造了两个结构体，EqualRun用于记录两个等价run，centroid用于存储与质心计算相关的参数

        public struct EqualRun
        {
            public int firstRun;
            public int secondRun;
        };

        public struct Centroid
        {
            public int sumx;
            public int sumy;
            public int area;
        };

        /**
         * 此三个函数：fillRunVectors、firstpass、replaceSameLabel是仿照博客写的。
         * 函数centroidCalculate用于计算连通域的质心。函数drawCaliResult用于在图中标记出质心位置。
         */

        //fillRunVectors函数完成所有团的查找与记录
        public unsafe void fillRunVectors(Bitmap bwBitmap, ref int NumberOfRuns, ref List<int> stRun,
            ref List<int> enRun, ref List<int> rowRun, out List<Point> stPoint, out List<Point> endPoint)
        {
            stPoint = new List<Point>();
            endPoint = new List<Point>();
            BitmapData bwBmpData = bwBitmap.LockBits(new Rectangle(0, 0, bwBitmap.Width, bwBitmap.Height), ImageLockMode.ReadWrite, bwBitmap.PixelFormat);
            byte* sorPtr = (byte*)bwBmpData.Scan0;

            for (int i = 0; i < bwBitmap.Height; i++)//行
            {
                if (sorPtr[i * bwBmpData.Stride] == 255)//每行的第一个元素
                {
                    NumberOfRuns++;
                    stRun.Add(0);
                    rowRun.Add(i);
                    stPoint.Add(new Point(0, i));
                }
                for (int j = 1; j < bwBitmap.Width; j++)//列
                {
                    if (sorPtr[i * bwBmpData.Stride + j - 1] == 0 && sorPtr[i * bwBmpData.Stride + j] == 255)//新增一个团
                    {
                        NumberOfRuns++;
                        stRun.Add(j);
                        rowRun.Add(i);
                        stPoint.Add(new Point(j, i));
                    }
                    else if (sorPtr[i * bwBmpData.Stride + j - 1] == 255 && sorPtr[i * bwBmpData.Stride + j] == 0)//结束一个团
                    {
                        enRun.Add(j - 1);
                        endPoint.Add(new Point(j - 1, i));
                    }
                }
                if (sorPtr[i * bwBmpData.Stride + bwBmpData.Width - 1] == 255)//每行的最后一个元素
                {
                    enRun.Add(bwBitmap.Width - 1);
                    endPoint.Add(new Point(bwBitmap.Width - 1, i));
                }
            }
            bwBitmap.UnlockBits(bwBmpData);
        }

        //firstPass函数完成团的标记与等价对列表的生成 offset=1,8邻域，offset=0，四邻域
        public void firstPass(ref List<int> stRun, ref List<int> enRun, ref List<int> rowRun, ref int NumberOfRuns,
            ref List<int> runLabels, ref List<EqualRun> equivalences, int offset)
        {
            //runLabels = new List<int>(NumberOfRuns);//可能会出问题
            for (int i = 0; i < NumberOfRuns; i++)
            {
                runLabels.Add(0);
            }
            int idxLabel = 1;
            int curRowIdx = 0;
            int firstRunOnCur = 0;
            int firstRunOnPre = 0;
            int lastRunOnPre = -1;
            for (int i = 0; i < NumberOfRuns; i++)
            {
                // 如果是该行的第一个run，则更新上一行第一个run第最后一个run的序号
                if (rowRun[i] != curRowIdx)
                {
                    curRowIdx = rowRun[i]; // 更新行的序号
                    firstRunOnPre = firstRunOnCur;
                    lastRunOnPre = i - 1;
                    firstRunOnCur = i;
                }
                // 遍历上一行的所有run，判断是否与当前run有重合的区域
                for (int j = firstRunOnPre; j <= lastRunOnPre; j++)
                {
                    // 区域重合 且 处于相邻的两行
                    if (stRun[i] <= enRun[j] + offset && enRun[i] >= stRun[j] - offset && rowRun[i] == rowRun[j] + 1)
                    {
                        if (runLabels[i] == 0) // 没有被标号过
                            runLabels[i] = runLabels[j];
                        else if (runLabels[i] != runLabels[j])// 已经被标号            
                        {
                            EqualRun equalRun;
                            equalRun.firstRun = runLabels[i];
                            equalRun.secondRun = runLabels[j];
                            equivalences.Add(equalRun); // 保存等价对
                        }
                    }
                }
                if (runLabels[i] == 0) // 没有与前一列的任何run重合
                {
                    runLabels[i] = idxLabel++;
                }
            }
        }

        //每个等价表用一个vector<int>来保存，等价对列表保存在map<pair<int,int>>里
        public void replaceSameLabel(ref List<int> runLabels, ref List<EqualRun> equivalence, out List<List<int>> equaList)
        {
            int maxLabel = runLabels.Max();//找到最大的标号
            bool[,] eqTab = new bool[maxLabel + 1, maxLabel + 1];
            for (int i = 0; i < maxLabel; i++)
            {
                for (int j = 0; j < maxLabel; j++)
                {
                    eqTab[i, j] = false;
                }
            }
            foreach (var item in equivalence)
            {
                eqTab[item.firstRun, item.secondRun] = true;
                eqTab[item.secondRun, item.firstRun] = true;
            }
            List<int> labelFlag = new List<int>();
            for (int i = 0; i < maxLabel + 1; i++)
            {
                labelFlag.Add(0);
            }

            equaList = new List<List<int>>();//等价链表
            /*
             遍历所有的团标号，来看每个标号应该加入哪个连通域，并赋连通域标号（labelFlag）。
             若当前的标号不属于任何一个连通域，则新给它一个连通域标号（labelFlag），并把它加入当前的连通域（tempList.push_back()），
             遍历当前连通域内的所有标号，将包含其中标号的所有等价对都加入到当前连通域内。
            */
            int labelFlagNum = 1;
            for (int i = 1; i <= maxLabel; i++)
            {
                if (labelFlag[i] != 0)
                {
                    continue;
                }
                labelFlag[i] = labelFlagNum;
                List<int> tempList = new List<int>();
                tempList.Add(i);
                for (int j = 0; j < tempList.Count; j++)
                {
                    for (int k = 0; k != maxLabel; k++)
                    {
                        if (eqTab[tempList[j], k] && labelFlag[k] == 0) //说明标号k+1与j连通，并且k+1没有被表示过，不属于任何连通域
                        {
                            tempList.Add(k);
                            labelFlag[k] = labelFlagNum;
                        }
                    }
                }
                equaList.Add(tempList);
                labelFlagNum++;
            }
            for (int i = 0; i != runLabels.Count; i++)
            {
                runLabels[i] = labelFlag[runLabels[i]];
            }
        }

        //函数centroidCalculate用于计算连通域的质心。
        public void centroidCalculate(ref List<Point> stPoint, ref List<Point> endPoint, ref List<int> runLabels, out List<Point> centroidPointList)
        {
            int maxLabel = runLabels.Max();//找到最大的标号
            List<Centroid> centroidList = new List<Centroid>();//保存与质心计算有关的参数，总个数为连通域个数
            for (int i = 0; i < maxLabel; i++)
            {
                Centroid tempCentroid;
                tempCentroid.sumx = 0;
                tempCentroid.sumy = 0;
                tempCentroid.area = 0;
                centroidList.Add(tempCentroid);
            }
            for (int i = 0; i < runLabels.Count; i++)
            {
                int index = runLabels[i];
                int tempSumx = (stPoint[i].X + endPoint[i].X) * (endPoint[i].X - stPoint[i].X + 1) / 2;
                int tempSumy = (endPoint[i].X - stPoint[i].X + 1) * stPoint[i].Y;
                int tempArea = endPoint[i].X - stPoint[i].X + 1;
                int newSumx = centroidList[index - 1].sumx + tempSumx;
                int newSumy = centroidList[index - 1].sumy + tempSumy;
                int newArea = centroidList[index - 1].area + tempArea;
                Centroid newCentroid;
                newCentroid.sumx = newSumx; newCentroid.sumy = newSumy; newCentroid.area = newArea;
                centroidList[index - 1] = newCentroid;
            }
            centroidPointList = new List<Point>();
            for (int i = 0; i < maxLabel; i++)
            {
                Point tempPoint = new Point(centroidList[i].sumx / centroidList[i].area, centroidList[i].sumy / centroidList[i].area);
                centroidPointList.Add(tempPoint);
            }
        }



        /// <summary>
        /// 画出连通域的质心，函数drawCaliResult用于在图中标记出质心位置。
        /// </summary>
        /// <param name="oriBitmap">二值图像</param>
        /// <param name="centroidPointList">质心点集</param>
        /// <returns></returns>
        public Bitmap drawCaliResult(Bitmap oriBitmap, List<Point> centroidPointList)
        {
            Bitmap result = (Bitmap)oriBitmap.Clone();
            Graphics g;
            Color[] color = new Color[9];
            color[0] = Color.FromArgb(255, 0, 0);
            color[1] = Color.FromArgb(0, 255, 0);
            color[2] = Color.FromArgb(0, 0, 255);
            color[3] = Color.FromArgb(255, 255, 0);
            color[4] = Color.FromArgb(0, 255, 255);
            color[5] = Color.FromArgb(255, 0, 255);
            color[6] = Color.FromArgb(125, 0, 125);
            color[7] = Color.FromArgb(125, 125, 0);
            color[8] = Color.FromArgb(0, 125, 125);


            using (Image img = result)
            {
                //如果原图片是索引像素格式之列的，则需要转换格式
                if (IsPixelFormatIndexed(img.PixelFormat))
                {
                    result = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                    using (g = Graphics.FromImage(result))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.DrawImage(img, 0, 0);
                        if (centroidPointList != null || centroidPointList.Count > 0)
                        {
                            for (int i = 0; i < centroidPointList.Count; ++i)
                            {
                                Brush brush = new SolidBrush(color[0]);
                                g.FillEllipse(brush, centroidPointList[i].X, centroidPointList[i].Y, 3, 3);
                            }
                        }
                        g.Dispose();
                    }
                }
                else //否则直接操作
                {
                    g = Graphics.FromImage(result);
                    if (centroidPointList != null || centroidPointList.Count > 0)
                    {
                        for (int i = 0; i < centroidPointList.Count; ++i)
                        {
                            Brush brush = new SolidBrush(color[0]);
                            g.FillEllipse(brush, centroidPointList[i].X, centroidPointList[i].Y, 3, 3);
                        }
                    }

                    g.Dispose();                                     
                }
                
            }
            return result;

        }


        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare,PixelFormat.Format16bppArgb1555,
                                             PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed};


        /// <summary>
        /// 判断图片的PixelFormat 是否在引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">原图片的PixelFormat</param>
        /// <returns></returns>
        private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }
            return false;
        }

        /// <summary>
        /// 其中参数openImage应为二值图像。
        /// </summary>
        /// <param name="openImage">二值图像。</param>
        /// <returns>处理后的图片</returns>
        public  Bitmap imageDeal(Bitmap openImage)
        {        
            int numberOfRuns = 0;
            List<int> stRun = new List<int>();
            List<int> enRun = new List<int>();
            List<int> rowRun = new List<int>();
            List<Point> stPoint; List<Point> endPoint;
            List<List<int>> equaList;
            List<Point> centroidPointList;
            List<int> runLabels = new List<int>();
            List<EqualRun> equivalences = new List<EqualRun>();
            this.fillRunVectors(openImage, ref numberOfRuns, ref stRun, ref enRun, ref rowRun, out stPoint, out endPoint);
            this.firstPass(ref stRun, ref enRun, ref rowRun, ref numberOfRuns, ref runLabels, ref equivalences, 1);
            this.replaceSameLabel(ref runLabels, ref equivalences, out equaList);
            this.centroidCalculate(ref stPoint, ref endPoint, ref runLabels, out centroidPointList);
            Bitmap resBitmap = this.drawCaliResult(openImage, centroidPointList);
            return resBitmap;
        }
        
        
        

    }
}
