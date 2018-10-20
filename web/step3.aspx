<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="step3.aspx.cs" Inherits="web.step3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>3、滤波</title>
<style type="text/css">
        .auto-style1 {
            width: 800px;
            height: 398px;
            border-collapse: collapse;
        }
        .auto-style2 {
            width: 100px;
        }
        .auto-style3 {
            width: 300px;
        }
        .auto-style4 {
            height: 40px;
        }
    </style>
</head>
<body>
     <form id="form1" runat="server">
        <table class="auto-style1" border="1">
            <tr>
                <td colspan="3" class="auto-style4">
                    <asp:Button ID="Button2" runat="server" OnClick="FileUploadButton_Click" Text="上传图片" />
                    <asp:FileUpload ID="FileUpload1" runat="server" onclick="FileUploadButton_Click"/>
                </td>
            </tr>
            <tr>
                <td class="auto-style3">
                    <asp:Image ID="Image1" runat="server" Height="531px" Width="415px" AlternateText="图片" />
                </td>
                <td class="auto-style2">

                &nbsp;<asp:RadioButtonList ID="RadioButtonList1" runat="server" Width="115px">
                        <asp:ListItem Selected="True">均值滤波</asp:ListItem>
                        <asp:ListItem>中值滤波</asp:ListItem>
                        <asp:ListItem>高斯滤波</asp:ListItem>
                    </asp:RadioButtonList>
                    <br />
&nbsp;<asp:Button ID="Button1" runat="server" Text="二值化" OnClick="Button1_Click" />

                    <br />
                    <br />

                </td>
                <td>
                    <asp:Image ID="Image2" runat="server" Height="531px" Width="473px" AlternateText="图片" />
                </td>
            </tr>
        </table>   
          <p><asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/step2.aspx">上一步：二值化</asp:LinkButton>
       <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/step4.aspx">下一步：边缘检测</asp:LinkButton>
        只支持JPG格式的图片

    </p>
    </form>
    
</body>
</html>
