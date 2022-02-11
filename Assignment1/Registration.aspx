<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Assignment1.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration</title>
    <script type="text/javascript">

        function checkpassword(password) {
            var score = 1;

            if (password.length < 12) {
                 return score;
            }

            if (password.search(/[0-9]/) != -1) {
                score++;
            }

            // at least 1 upper case
            if (password.search(/(?=.*[A-Z])/) != -1) {
                score++;
            }

            // at least 1 lower case
            if (password.search(/(?=.*[a-z])/) != -1) {
                score++;
            }

            // at least 1 special char
            if (password.search(/(?=.*[^a-zA-Z0-9])/) != -1) {
                score++;
            }

            return score;
        }

        function check() {
            var password = document.getElementById('<%=tb_password.ClientID %>').value;
            var score = checkpassword(password);
            var status = "";
            if (score == 1) {
                status = "Extremely weak";
            }
            else if (score == 2) {
                status = "Very weak";
            }

            else if (score == 3) {
                status = "Weak";
            }

            else if (score == 4) {
                status = "Medium";
            }

            else if (score == 5) {
                status = "Strong!";
            }

            document.getElementById("lbl_password").textContent = status;
            if (status == "Strong!") {
                document.getElementById("lbl_password").style.color = "Green";
            }
            else
            {
                document.getElementById("lbl_password").style.color = "Red";
            }
        }

        // not using
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("lbl_password").innerHTML = "Password length must be at least 12 characters.";
                document.getElementById("lbl_password").style.color = "Red";
                return ("too_short");
            }
            
            else if (str.search(/[0-9]/) == -1) { 
                document.getElementById("lbl_password").innerHTML = "Password require at least 1 number.";
                document.getElementById("lbl_password").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/(?=.*[A-Z])/) == -1) { 
                document.getElementById("lbl_password").innerHTML = "Password require at least 1 letter in upper case.";
                document.getElementById("lbl_password").style.color = "Red";
                return ("no_upper");
            }

            else if (str.search(/(?=.*[a-z])/) == -1) {
                document.getElementById("lbl_password").innerHTML = "Password require at least 1 letter in lower case.";
                document.getElementById("lbl_password").style.color = "Red";
                return ("no_lower");
            }

            else if (str.search(/(?=.*[^a-zA-Z0-9])/) == -1) {
                document.getElementById("lbl_password").innerHTML = "Password require at least 1 special character.";
                document.getElementById("lbl_password").style.color = "Red";
                return ("no_specchar");
            }   

        }
    </script>
    <style type="text/css">
        .auto-style1 {
            height: 29px;
        }
        .auto-style2 {
            height: 25px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <h1>Registration</h1>
        <table>
            <tr>
                <td>First Name</td>
                <td><asp:TextBox ID="tb_firstname" runat="server"></asp:TextBox></td>
                <td><asp:Label ID="lbl_firstname" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Last Name</td>
                <td><asp:TextBox ID="tb_lastname" runat="server"></asp:TextBox></td>
                <td><asp:Label ID="lbl_lastname" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td class="auto-style1">Email Address</td>
                <td class="auto-style1"><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                <td class="auto-style1"><asp:Label ID="lbl_email" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Date of Birth</td>
                 <td>
                     <asp:TextBox ID="tb_dob" runat="server" TextMode="Date"></asp:TextBox>
                </td>
                <td><asp:Label ID="lbl_dob" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Credit Card info</td>
                <td><asp:TextBox ID="tb_credit" runat="server"></asp:TextBox></td>
                <td><asp:Label ID="lbl_credit" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Photo</td>
                <td>
                    <asp:FileUpload ID="tb_photo" runat="server" />
                </td>
                <td><asp:Label ID="lbl_photo" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><asp:TextBox ID="tb_password" runat="server" TextMode="Password" onkeyup="javascript:check()"></asp:TextBox></td>
                <td><asp:Label ID="lbl_password" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td class="auto-style2"></td>
                <td class="auto-style2"><asp:Label ID="lbl_passwordstatus" runat="server" Text=" "></asp:Label></td>
            </tr>
            <tr>
                <td><asp:Button ID="btn_register" runat="server" Text="Register" OnClick="btn_register_Click" /></td>
            </tr>
            
            
        </table>
    </form>

    </body>
</html>
