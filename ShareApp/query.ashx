<%@ WebHandler Language="C#"Debug="true" Class="query" %>

using System;
using System.Web;
using System;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;

using System.Configuration;

public class query : IHttpHandler {

    static string ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString.Replace("~", HttpRuntime.AppDomainAppPath);

    public void ProcessRequest (HttpContext context) {

        switch(context.Request.QueryString["action"]) {
            case "add":
                AddComment(context);
                break;
            case "get":
                GetComments(context);
                break;
            default:
                context.Response.ContentType = "text/plain";
                context.Response.Write("Status OK");
                break;
        }
        
    }

    static void Jsonp(HttpContext context, string data) {
        context.Response.ContentType = "text/javascript";
        context.Response.Write(context.Request.QueryString["callback"] + "(");
        context.Response.Write(data);
        context.Response.Write(");");
    }

    static void AddComment(HttpContext context) {

        if(context.Request.QueryString["content"] == null) {
            Jsonp(context, "\"content is empty\"");
        }

        using(OleDbConnection conn = new OleDbConnection(ConnectionString)) {
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = "INSERT INTO [Comments]([Url], [Contact], [Content], [DateTime], [IP]) VALUES(@Url, @Contact, @Content, @DateTime, @IP)";
            command.Parameters.AddWithValue("@Url", context.Request.QueryString["url"] ?? String.Empty);
            command.Parameters.AddWithValue("@Contact", context.Request.QueryString["contact"] ?? String.Empty);
            command.Parameters.AddWithValue("@Content", context.Request.QueryString["content"]);
            command.Parameters.AddWithValue("@DateTime", DateTime.Now.ToOADate());
            command.Parameters.AddWithValue("@IP", context.Request.UserHostAddress ?? String.Empty);
            
            int r = command.ExecuteNonQuery();
            if(r > 0) {
                Jsonp(context, "\"ok\"");
            } else {
                Jsonp(context, "\"error\"");
            }
        }
    }

    static string EncodeJs(string s) {
        return s.Replace("\"", "\\\"")
            .Replace("\'", "\\\'")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }

    static void GetComments(HttpContext context) {

        using(OleDbConnection conn = new OleDbConnection(ConnectionString)) {
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM [Comments] WHERE [Url]=@Url ORDER BY [DateTime]";
            command.Parameters.AddWithValue("@Url", context.Request.QueryString["url"] ?? String.Empty);
            var reader = command.ExecuteReader();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append("[");
            while(reader.Read()) {
                sb.Append("{\"url\":\"");
                sb.Append(EncodeJs((string)reader["url"]));
                sb.Append("\", \"contact\":\"");
                sb.Append(EncodeJs((string)reader["contact"]));
                sb.Append("\", \"content\":\"");
                sb.Append(EncodeJs((string)reader["content"]));
                sb.Append("\", \"date\":\"");
                sb.Append(((DateTime)reader["datetime"]).ToString("yyyy/MM/dd HH:mm:ss"));
                sb.Append("\", \"ip\":\"");
                sb.Append(EncodeJs((string)reader["ip"]));
                sb.Append("\"},");
            }
			
			if(sb.Length > 1)
				sb[sb.Length - 1] = ']';
			else
				sb.Append(']');

            Jsonp(context, sb.ToString());
        }
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}