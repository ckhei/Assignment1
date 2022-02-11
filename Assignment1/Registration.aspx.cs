using System;
using System.Drawing;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

using System.Data.SqlClient;
using System.IO;

namespace Assignment1
{
    public partial class Registration : System.Web.UI.Page
    {

        string DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_register_Click(object sender, EventArgs e)
        {
            bool pass = true;

            // check if fields is empty 
            if (tb_email.Text == "")
            {
                lbl_email.Text = "This field cannot be empty!";
                lbl_email.ForeColor = Color.Red;
                pass = false;
            }
            // check email format
            else if (!Regex.IsMatch(tb_email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                lbl_email.Text = "Wrong email format!";
                lbl_email.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_email.Text = " ";

                // check if email in database
                string email = tb_email.Text.Trim();
                string sql = "select Email FROM Account WHERE Email=@Email";

                SqlConnection con = new SqlConnection(DBConnectionString);
                SqlCommand command = new SqlCommand(sql, con);

                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            if (reader["Email"] != null)
                            {
                                lbl_email.Text = "Email has already been used";
                                lbl_email.ForeColor = Color.Red;
                                return;
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
                    con.Close();
                }
            }

            if (tb_firstname.Text == "")
            {
                lbl_firstname.Text = "This field cannot be empty!";
                lbl_firstname.ForeColor = Color.Red;
                pass = false;
            }
            else if (Regex.IsMatch(tb_firstname.Text, "[^a-zA-Z0-9]") || Regex.IsMatch(tb_firstname.Text, "[0-9]"))
            {
                lbl_firstname.Text = "Invalid first name!";
                lbl_firstname.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_firstname.Text = " ";
            }

            if (tb_lastname.Text == "")
            {
                lbl_lastname.Text = "This field cannot be empty!";
                lbl_lastname.ForeColor = Color.Red;
                pass = false;
            }
            else if (Regex.IsMatch(tb_lastname.Text, "[^a-zA-Z0-9]") || Regex.IsMatch(tb_lastname.Text, "[0-9]"))
            {
                lbl_lastname.Text = "Invalid last name!";
                lbl_lastname.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_lastname.Text = " ";
            }

            if (tb_dob.Text == "")
            {
                lbl_dob.Text = "This field cannot be empty!";
                lbl_dob.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_dob.Text = " ";
            }

            if (tb_credit.Text == "")
            {
                lbl_credit.Text = "This field cannot be empty!";
                lbl_credit.ForeColor = Color.Red;
                pass = false;
            }
            else if (tb_credit.Text.Length > 17)
            {
                lbl_credit.Text = "Invalid credit card info";
                lbl_credit.ForeColor = Color.Red;
                pass = false;
            }
            else
            {
                lbl_credit.Text = " ";
            }


            if (tb_password.Text == "")
            {
                lbl_password.Text = "This field cannot be empty!";
                lbl_password.ForeColor = Color.Red;
                pass = false;
            }
            else
            {


                // check password
                int scores = checkPassword(tb_password.Text);
                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Password length must be at least 12 characters";
                        break;
                    case 2:
                        status = "Password require at least 1 upper case and lower case";
                        break;
                    case 3:
                        status = "Password require at least 1 number";
                        break;
                    case 4:
                        status = "Password require at least 1 special character";
                        break;
                    case 5:
                        status = "";
                        break;
                    default:
                        break;
                }

                lbl_passwordstatus.Text = status;

                if (scores < 5)
                {
                    lbl_passwordstatus.ForeColor = Color.Red;
                    pass = false;
                }
                else
                {
                    lbl_passwordstatus.ForeColor = Color.Green;
                    string pwd = tb_password.Text.ToString().Trim();

                    //Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + salt;

                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);

                    System.Diagnostics.Debug.WriteLine("hashWithSalt ", pwdWithSalt);
                }
            }


            if (pass)
            {

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                createAccount();
                Response.Redirect("Login.aspx", true);
            }
            else
            {
                return;
            }
        }

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            //score 2 very weak- contains lowercase letters
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            //score 3 weak - contains uppercase letters
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            //score 4 medium - contains numerals
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            //score 5 strong- contains special characters
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }


            return score;
        }

        protected byte[] encryptData(string data)
        {

            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error at encrypt data ");
                throw new Exception(ex.ToString());
            }

            finally
            {
            }

            return cipherText;
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName, @LastName, @Email, @PasswordHash, @PasswordSalt, @DoB, @Photo, @CreditCardInfo, @IV, @Key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstname.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastname.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());

                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);

                            cmd.Parameters.AddWithValue("@DoB", tb_dob.Text.Trim());
                            // photo not done yet
                            cmd.Parameters.AddWithValue("@Photo", tb_photo.ToString());

                            // encrypt credit card info
                            cmd.Parameters.AddWithValue("@CreditCardInfo", Convert.ToBase64String(encryptData(tb_credit.Text.Trim())));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            
                            // add to password history
                            SqlCommand passwordCmd = new SqlCommand("INSERT INTO PasswordHistory VALUES(@Email, @PasswordCount, @ChangeDateTime, @PasswordHash, @PasswordSalt)");

                            passwordCmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            passwordCmd.Parameters.AddWithValue("@passwordCount", 1);

                            DateTime passwordCreated = DateTime.Now;
                            passwordCmd.Parameters.AddWithValue("@ChangeDateTime", passwordCreated);
                            System.Diagnostics.Debug.WriteLine(passwordCreated, "date of password created");

                            passwordCmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            passwordCmd.Parameters.AddWithValue("@PasswordSalt", salt);

                            cmd.Connection = con;
                            passwordCmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            passwordCmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Here");
                throw new Exception(ex.ToString());
            }
        }
    }
}