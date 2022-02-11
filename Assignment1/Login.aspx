<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Assignment1.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Lfw028eAAAAAD_f8WXAeJS0zo4c0Hjxl_41J8R6"></script>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <h1>Login</h1>
        <table>
            <tr>
                <td class="auto-style1">Email Address</td>
                <td class="auto-style1"><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                <td class="auto-style1"><asp:Label ID="lbl_email" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><asp:TextBox ID="tb_password" runat="server" TextMode="Password"></asp:TextBox></td>
                <td><asp:Label ID="lbl_password" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td class="auto-style2"></td>
                <td class="auto-style2"><asp:Label ID="lbl_passworderror" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td><asp:Button ID="btn_login" runat="server" Text="Login" OnClick="btn_login_Click" /></td>
            </tr>
            
        </table>

        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
        <asp:Label ID="lbl_captcha" runat="server" Text=" "></asp:Label>

        <script>
            grecaptcha.ready(function () {
                grecaptcha.execute('6Lfw028eAAAAAD_f8WXAeJS0zo4c0Hjxl_41J8R6', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
        </script>
    </form>
</body>
</html>
