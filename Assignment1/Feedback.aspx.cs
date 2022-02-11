using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment1
{
    public partial class Feedback : System.Web.UI.Page
    {
        string DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

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

        protected void Submitbtn_Click(object sender, EventArgs e)
        {
            lbl_comments.Text = HttpUtility.HtmlEncode(tb_comments.Text);
        }
    }
}