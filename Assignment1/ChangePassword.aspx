<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Assignment1.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Change password</h1>
        <h4 class="auto-style1"><asp:Label ID="lbl_info" runat="server" Text=" "></asp:Label></h4>
        <br />
        <table>
            <tr>
                <td class="auto-style1">Old Password:</td>
                <td class="auto-style1"><asp:TextBox ID="tb_oldpwd" runat="server" TextMode="Password"></asp:TextBox></td>
                <td class="auto-style1"><asp:Label ID="lbl_oldpwd" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td class="auto-style1"><asp:Label ID="lbl_oldpwd2" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>New Password:</td>
                <td><asp:TextBox ID="tb_newpws" runat="server" TextMode="Password"></asp:TextBox></td>
                <td><asp:Label ID="lbl_newpwd" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Confirm new Password:</td>
                <td><asp:TextBox ID="tb_cfmNewPwd" runat="server" TextMode="Password"></asp:TextBox></td>
                <td><asp:Label ID="lbl_cfmNewPwd" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td class="auto-style1"><asp:Label ID="lbl_cfmNewPwd2" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td><asp:Button ID="btn_homepage" runat="server" Text="Home" OnClick="btn_homepage_Click" /></td>
                <td><asp:Button ID="btn_change" runat="server" Text="Change Password" OnClick="btn_change_Click" /></td>
            </tr>
        </table>
    </form>
</body>
</html>
