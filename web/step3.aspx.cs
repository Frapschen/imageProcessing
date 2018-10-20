using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web
{
    public partial class step3 : System.Web.UI.Page
    {
        public static String fileName = null;
        public static String filePath = null;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void FileUploadButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                filePath = Server.MapPath("~/image/");
                fileName = FileUpload1.FileName;
                //判断图片
                String temp = null;
                temp = fileName.Substring(fileName.LastIndexOf("."));
                if (temp.Equals(".jpg"))
                {
                    FileUpload1.SaveAs(filePath + fileName);
                    Response.Write("<p >上传成功!</p>");
                    Image1.ImageUrl = "~/image/" + fileName;
                }
                else
                {
                    Response.Write("<p >请选择正确的文件!</p>");
                }

            }
            else
            {
                Response.Write("<p >请选择要上传的文件!</p>");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String newfileName = "";

            //加载图片
            Bitmap b = bll.loadImage.file2img(filePath + fileName);
            Bitmap target = null;
            
            switch (RadioButtonList1.SelectedIndex)
            {
                case 0://算法一
                    newfileName += "均值滤波_";
                    //target = bll.thresholding.ConvertTo1Bpp1(b);
                    break;
                case 1://算法二
                    newfileName += "中值滤波_";
                    //target = bll.thresholding.ConvertTo1Bpp2(b);
                    break;
                case 2://算法三
                    newfileName += "高斯滤波_";
                    //target = bll.thresholding.ConvertTo1Bpp3(b);
                    break;
            }
            
            newfileName = fileName + newfileName;
            //存放图片
            bll.loadImage.img2file(target, filePath + newfileName);
            Image2.ImageUrl = "~/image/" + newfileName;
        }
    }
}