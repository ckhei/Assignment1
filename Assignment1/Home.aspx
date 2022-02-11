<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Assignment1.SuccessfulLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">
    <h1>Login was successful!</h1>
    <h4><asp:Label ID="lbl_alert" runat="server" Text=" "></asp:Label></h4>
    <h3>User profile:</h3>
    <table>
        <tr>
            <td class="auto-style1">Email :</td>
            <td class="auto-style1"><asp:Label ID="lbl_email" runat="server" Text=" "></asp:Label></td>
        </tr>
        <tr>
            <td>Credit Card Info : </td>
            <td><asp:Label ID="lbl_credit" runat="server" Text=" "></asp:Label></td>

        </tr>
    </table>
        <p>
            <asp:Button ID="btn_logout" runat="server" Text="Logout" OnClick="btn_logout_Click" />
        </p>
        <p>
            <asp:Button ID="btn_changepwd" runat="server" Text="Change password" OnClick="btn_changepwd_Click" />
        </p>
         <p>
            <asp:Button ID="btn_feedback" runat="server" Text="Add feedback" OnClick="btn_feedback_Click"/>
        </p>
    </form>
</body>
</html>
