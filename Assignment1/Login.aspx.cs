using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;


namespace Assignment1
{
    public partial class Login : System.Web.UI.Page
    {
        string DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        private static System.Timers.Timer aTimer;

        public string success { get; set; }
        
        public List<string> ErrorMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string pwd = tb_password.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);

            System.Diagnostics.Debug.WriteLine(dbHash, "dbhash");
            System.Diagnostics.Debug.WriteLine(dbSalt, "dbsalt");

            
            if (Session["FailLogin"] == null)
            {
                Session["FailLogin"] = 0;
            }

           // if (!ValidateCaptcha())
            //{
              //  System.Diagnostics.Debug.WriteLine(ValidateCaptcha(), "captcha");
                //return;
            //}
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    
                    if (userHash.Equals(dbHash))
                    {
                        Session["Email"] = email;

                        // create new GUID and save to session
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        // create a new cookie with this guid value
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        System.Diagnostics.Debug.WriteLine("successful ", "login");
                        Response.Redirect("Home.aspx", false);
                        
                    }

                    else
                    {
                        System.Diagnostics.Debug.WriteLine("login ", "unsuccessful");
                        lbl_passworderror.Text = "Email or password is not valid. Please try again.";
                        lbl_passworderror.ForeColor = Color.Red;

                        int failed = (int)Session["FailLogin"];
                        failed += 1;
                        System.Diagnostics.Debug.WriteLine(failed," lockout fails: ");
                        Session["FailLogin"] = failed;

                        if (failed > 2)
                        {
                            System.Diagnostics.Debug.WriteLine("Account lockout");
                            aTimer = new System.Timers.Timer(5000);
                            aTimer.Elapsed += new ElapsedEventHandler(RecoverAccount);
                            aTimer.Enabled = true;
                            aTimer.AutoReset = false;

                            lbl_passworderror.Text = "Your account has been locked. Please try again later";
                            lbl_passworderror.ForeColor = Color.Red;

                            btn_login.Enabled = false;
                        }
                        return;
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally {
            
            }

        }

        protected void RecoverAccount(Object source, ElapsedEventArgs e)
        {
            Session["FailLogin"] = -1;
            System.Diagnostics.Debug.WriteLine("Can login already");
            lbl_passworderror.Text = "";
            btn_login.Enabled = true;
            
        }

        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("login ", "unsuccessful");
                                lbl_passworderror.Text = "Email or password is not valid. Please try again.";
                                lbl_passworderror.ForeColor = Color.Red;
                                return null;
                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { 
                connection.Close(); 
            }

            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "select PasswordSalt FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("login ", "unsuccessful");
                                lbl_passworderror.Text = "Email or password is not valid. Please try again.";
                                lbl_passworderror.ForeColor = Color.Red;
                                return null;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { 
                connection.Close(); 
            }

            return s;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6Lfw028eAAAAAHkf8GpzO--XRxqMDxd_px_fzGag &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        lbl_captcha.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        Login jsonObject = js.Deserialize<Login>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}