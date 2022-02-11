using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment1
{
    public partial class SuccessfulLogin : System.Web.UI.Page
    {

        string DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        
        byte[] Key;
        byte[] IV;
        byte[] ccInfo = null;
        string email = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    email = (string)Session["Email"];
                    displayUserProfile(email);
                    btn_logout.Visible = true;
                }
                else
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

            string datetime = getDate(email);
            DateTime timeToChange = Convert.ToDateTime(datetime).AddMinutes(1);
            DateTime now = DateTime.Now;
            System.TimeSpan diff = timeToChange.Subtract(now);

            if (timeToChange > now)
            {
                lbl_alert.Text = "You need to change your password in " + diff.Seconds + " seconds!";
                lbl_alert.ForeColor = Color.Blue;
            }
            else
            {
                lbl_alert.Text = "CHANGE YOUR PASSWORD NOW!!";
                lbl_alert.ForeColor = Color.Red;
            }
        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void btn_feedback_Click(object sender, EventArgs e)
        {
            Response.Redirect("Feedback.aspx", true);
        }

        protected void btn_changepwd_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx", false);
        }

        protected void displayUserProfile(string email)
        {
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "select * FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            lbl_email.Text = reader["Email"].ToString();
                        }

                        if (reader["CreditCardInfo"] != DBNull.Value)
                        {
                            ccInfo = Convert.FromBase64String(reader["CreditCardInfo"].ToString());
                        }

                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }

                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }

                    lbl_credit.Text = DecryptData(ccInfo);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }
        }

        protected string getDate(string email)
        {
            string time = null;
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "SELECT ChangeDateTime FROM PasswordHistory WHERE Email=@Email AND PasswordCount=1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ChangeDateTime"] != null)
                        {
                            if (reader["ChangeDateTime"] != DBNull.Value)
                            {
                                time = reader["ChangeDateTime"].ToString();
                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }

            return time;
        }

        protected string DecryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // read the decrypted bytes from the decrypting steam
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            catch (Exception ex) { 
                throw new Exception(ex.ToString()); 
            }

            finally {

            }

            return plainText;
        }

        
    }


}