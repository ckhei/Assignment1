<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Assignment1.Feedback" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>
            Feedback</h1>
        <h3>
            Your comments</h3>
        <p>
            <asp:TextBox ID="tb_comments" runat="server" Height="151px" Width="395px"></asp:TextBox>
        </p>
        <asp:Button ID="Submitbtn" runat="server" OnClick="Submitbtn_Click" Text="Submit" />
        
        <p>
            <asp:Label ID="lbl_comments" runat="server" Text="Comments will display here"></asp:Label>
        </p>
    </form>
</body>
</html>
