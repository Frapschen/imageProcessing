using System;
using System.Drawing;

namespace web
{
    public partial class index : System.Web.UI.Page
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
                if (bll.loadImage.isImage(fileName))
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
            //加载图片
            Bitmap b = bll.loadImage.file2img(filePath + fileName);
            //使用灰度算法 
            Bitmap bb = bll.grayProcessing.Img_gray(b);
            

            //图片重命名：加上处理算法的名字
            fileName = bll.loadImage.imgNameadd(fileName, "灰度化");

            //存放图片
            bll.loadImage.img2file(bb, filePath + fileName);


            Image2.ImageUrl = "~/image/" + fileName;
        }

    }
}