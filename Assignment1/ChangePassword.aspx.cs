using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment1
{
    public partial class ChangePassword : System.Web.UI.Page
    {

        string DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        static string finalHash;
        static string salt;
        string email = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    email = (string)Session["Email"];
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
        }

        protected void btn_change_Click(object sender, EventArgs e)
        {
            string datetime = getDate(email);
            DateTime changePwEnabled = Convert.ToDateTime(datetime).AddSeconds(30);
            DateTime now = DateTime.Now;
            System.TimeSpan diff = changePwEnabled.Subtract(now);

            if (changePwEnabled > now)
            {
                lbl_info.Text = "Please wait for " + diff.Seconds + " seconds to change your password again!";
                return;
            }
            else
            {
                lbl_info.Text = " ";
            }

            try
            {
                if (FieldIsEmpty())
                {
                    
                    return;
                }

                string oldPwd = tb_oldpwd.Text;
                string newPwd = tb_cfmNewPwd.Text;

                SHA512Managed hashing = new SHA512Managed();
                string olddbHash = getAccountDBHash(email);
                string olddbSalt = getAccountDBSalt(email);

                string pwHistdbHash = getPwHistDBHash(email);
                string pwHistdbSalt = getPwHistDBSalt(email);

                if (olddbSalt != null && olddbSalt.Length > 0 && olddbHash != null && olddbHash.Length > 0) 
                {
                    string oldpwdWithSalt = oldPwd + olddbSalt;
                    byte[] oldhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(oldpwdWithSalt));
                    string oldUserHash = Convert.ToBase64String(oldhashWithSalt);

                    System.Diagnostics.Debug.WriteLine(oldUserHash, "old uservpw hash");
                    System.Diagnostics.Debug.WriteLine(olddbHash, "old pw hash");
                    

                    // check if user original password is the same as account database password
                    if (oldUserHash.Equals(olddbHash))
                    {
                        
                        // check if new password == confirm password
                        if (tb_newpws.Text == tb_cfmNewPwd.Text)
                        {
                            // to check with current pw
                            string newpwdWithSalt = newPwd + olddbSalt; // add new pw to old salt (alrdy in db) to generate a hash value
                            byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));
                            string newUserHash = Convert.ToBase64String(newhashWithSalt);

                            // to check with prev pw
                            string new2pwdWithSalt = newPwd + pwHistdbSalt; // add new pw to old salt (alrdy in db) to generate a hash value
                            byte[] new2hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(new2pwdWithSalt));
                            string new2UserHash = Convert.ToBase64String(new2hashWithSalt);

                            System.Diagnostics.Debug.WriteLine(new2UserHash, "new2userhash");
                            System.Diagnostics.Debug.WriteLine(pwHistdbHash, "pwHistdbHash");
                            

                            // check if new hash value == to old hash value (if equal, inside db --> cannot use) 
                            // old hash value needs to be retrieved from pw hist database

                            if (newUserHash.Equals(olddbHash)) // --> check if password is the same as the current password
                            {
                                // inside password history db
                                System.Diagnostics.Debug.WriteLine("Used current Password");
                                lbl_cfmNewPwd.Text = "Cannot reuse last 2 password!";
                                lbl_cfmNewPwd.ForeColor = Color.Red;
                                return;
                            }

                            else if (new2UserHash.Equals(pwHistdbHash)) // --> check if password is the same as the previous password
                            {
                                System.Diagnostics.Debug.WriteLine("Used prev Password");
                                lbl_cfmNewPwd.Text = "Cannot reuse last 2 password!";
                                lbl_cfmNewPwd.ForeColor = Color.Red;
                                return;
                            }

                            else
                            {
                                // change password here
                                System.Diagnostics.Debug.WriteLine("Password NOT in passwordhistory");

                                SqlConnection con = new SqlConnection(DBConnectionString);
                                string allpwdsql = "SELECT * FROM PasswordHistory WHERE Email=@Email";
                                SqlCommand allpwdcommand = new SqlCommand(allpwdsql, con);
                                allpwdcommand.Parameters.AddWithValue("@Email", email);

                                //Generate random "salt"
                                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                byte[] saltByte = new byte[8];

                                //Fills array of bytes with a cryptographically strong sequence of random values.
                                rng.GetBytes(saltByte);
                                salt = Convert.ToBase64String(saltByte);

                                string pwdandSalt = newPwd + salt;
                                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPwd));
                                byte[] hashandSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdandSalt));
                                finalHash = Convert.ToBase64String(hashandSalt);

                                System.Diagnostics.Debug.WriteLine(finalHash,"final hash");

                                updatePwHistoryDB(); // --> this needs to be first
                                updateAccountDB();
                                addToPwHistory();

                                System.Diagnostics.Debug.WriteLine("Password changed");
                                lbl_info.Text = "Password changed! :)";
                                lbl_info.ForeColor = Color.Blue;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Password does not match");
                            lbl_cfmNewPwd2.Text = "Password does not match";
                            lbl_cfmNewPwd2.ForeColor = Color.Red;
                            return;
                        }
                    }

                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Wrong Password");
                        lbl_oldpwd2.Text = "Wrong Password";
                        lbl_oldpwd2.ForeColor = Color.Red;
                        return;
                    }

                }

            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        protected string getDate(string email)
        {
            string time = null;
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "select ChangeDateTime FROM PasswordHistory WHERE Email=@Email AND PasswordCount=1";
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

        protected bool FieldIsEmpty()
        {
            bool pass = true;
            if (tb_oldpwd.Text.Length == 0)
            {
                lbl_oldpwd.Text = "This field cannot be empty!";
                lbl_oldpwd.ForeColor = Color.Red;
                pass = false;
            }
            else
            {

                lbl_oldpwd.Text = " ";

            }

            if (tb_newpws.Text.Length == 0)
            {
                lbl_newpwd.Text = "This field cannot be empty!";
                lbl_newpwd.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_newpwd.Text = " ";
            }

            if (tb_cfmNewPwd.Text.Length == 0)
            {
                lbl_cfmNewPwd.Text = "This field cannot be empty!";
                lbl_cfmNewPwd.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_cfmNewPwd.Text = " ";
            }


            if (!pass)
            {
                return true;
            }

            return false;
        }


        // AccountDB check password (if old pw is correct)
        protected string getAccountDBHash(string email)
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

            return h;
        }

        protected string getAccountDBSalt(string email)
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

            return s;
        }



        // PwHistDB check password (if password keyed in is old pw 2 --> prev password not current)
        protected string getPwHistDBHash(string email)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "SELECT PasswordHash FROM PasswordHistory WHERE Email=@Email AND PasswordCount=2";
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

            return h;
        }


        protected string getPwHistDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(DBConnectionString);
            string sql = "select PasswordSalt FROM PasswordHistory WHERE Email=@Email AND PasswordCount=2";
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

            return s;
        }


        protected void updatePwHistoryDB()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand deletecmd = new SqlCommand("DELETE FROM PasswordHistory WHERE Email=@Email AND PasswordCount=2"))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE PasswordHistory SET PasswordCount=2 WHERE Email=@Email"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                deletecmd.Parameters.AddWithValue("@Email", email);
                                cmd.Parameters.AddWithValue("@Email", email);

                                deletecmd.Connection = con;
                                cmd.Connection = con;
                                
                                con.Open();

                                deletecmd.ExecuteNonQuery();
                                cmd.ExecuteNonQuery();
                                
                                con.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void addToPwHistory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand pwdcommand = new SqlCommand("INSERT INTO PasswordHistory VALUES(@Email, @Count, @Date, @Hash, @Salt)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            DateTime date = DateTime.Now;
                            pwdcommand.Parameters.AddWithValue("@Email", email);
                            pwdcommand.Parameters.AddWithValue("@Count", 1);
                            pwdcommand.Parameters.AddWithValue("@Date", date);
                            pwdcommand.Parameters.AddWithValue("@Hash", finalHash);
                            pwdcommand.Parameters.AddWithValue("@Salt", salt);

                            pwdcommand.Connection = con;
                            con.Open();
                            pwdcommand.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void updateAccountDB()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash=@Hash, PasswordSalt=@Salt WHERE Email=@Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            DateTime date = DateTime.Now;
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Hash", finalHash);
                            cmd.Parameters.AddWithValue("@Salt", salt);

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void btn_homepage_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx", true);
        }
    }
}