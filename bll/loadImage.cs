using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll
{
    /// <summary>
    /// 图片加载、图片文件验证、每个步骤名字的追加。
    /// </summary>
    public class loadImage
    {
        public static Bitmap file2img(string filepath)//Tuple<Bitmap, bool>
        {
           
            Bitmap b = null;
            //bool a = true;
           // Tuple<Bitmap, bool> tuple = null;           
            try
            {
               b = new Bitmap(filepath); 
            }
            catch(ArgumentException ag)
            {
                Console.WriteLine(ag.Message);
                b = new Bitmap("D:\\C# exercise\\web\\web\\image\\error.jpg");
                //a = false;
            }
           // tuple = new Tuple<Bitmap, bool>(b, a);
            // return tuple;
            return b;

        }
        /// <summary>
        /// 图片储存
        /// </summary>
        /// <param name="b">图片对象</param>
        /// <param name="filepath">存储地址</param>
        public static void img2file(Bitmap b, string filepath)
        {
            b.Save(filepath);
        }
        /// <summary>
        /// 判断一个文件名是否是JPG格式的
        /// </summary>
        /// <param name="fileName">图片名</param>
        /// <returns></returns>
        public static bool isImage(String fileName)
        {
            String temp = null;
            temp = fileName.Substring(fileName.LastIndexOf("."));
            if (temp.Equals(".jpg"))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 给一个文件名的末尾追加名字，会用"_"间隔
        /// </summary>
        /// <param name="previous">文件名</param>
        /// <param name="addpart">最佳的部分</param>
        /// <returns></returns>
        public static String imgNameadd(String previous,String addpart)
        {
            addpart = "_" + addpart;
            return  previous.Insert(previous.LastIndexOf('.'), addpart);
        }
    }
}
