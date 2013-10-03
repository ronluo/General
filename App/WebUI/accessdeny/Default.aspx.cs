using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AccessDeniedDesign_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.UserLabel.Text = Request.LogonUserIdentity.Name;
        this.DisablePageCaching();
    }

    private void DisablePageCaching()
    {
        Response.Expires = 0;
        Response.Cache.SetNoStore();
        Response.AppendHeader("Pragma", "no-cache");
    }
    protected void btnAccD_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccessDenied.aspx");
    }
}