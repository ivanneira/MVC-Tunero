using System.Web.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.Data;
using System;


namespace MVC_Turnero.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index(string sistema,  int ? csId, string  consultorioId)
        {

            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);  // HTTP 1.1.
            Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AppendHeader("Expires", "0"); // Proxies.

            string consulta = "";
            SqlDataAdapter da = null;
            SqlConnection cx = null;
            if (sistema.ToLower() == "gedoc")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_GEDOC"].ConnectionString);
                consulta = "select ct.csId, cs.csNombre  as CsNombre, ct.id from catTurnero ct inner join catCentroDeSalud cs on cs.csId = ct.csId  where ct.csId=@column";

            }
            else
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "select ct.csId, cs.Descripcion as CsNombre, ct.id from catTurnero ct inner join Empresa cs on cs.EmpresaID = ct.csId  where ct.csId=@column";
            }

            //Centro de Salud
            da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", csId);

            DataTable dt = new DataTable();

            
            var catTurneroId = "";
            var catTurnerocsId = "";
            int cantSalas = 0;
            var csNombre = "";

            try
            {
                da.Fill(dt);
                catTurneroId = dt.Rows[0]["id"].ToString();
                catTurnerocsId = dt.Rows[0]["csId"].ToString();
                csNombre = dt.Rows[0]["csNombre"].ToString();
            }
            catch(Exception err)
            {
                return View();
            }

            //Cantidad de Consultorios

            if (consultorioId == "0")
            {
                return View("~/Views/Config/index.cshtml");
            }

            if (consultorioId.Length > 1)
            {
                if (consultorioId.Substring(0, 2) == "c=" )
                {
                    var id = consultorioId.Substring(2, consultorioId.Length - 2);
                    
                    ViewBag.idConsultorio = (id == "") ? "0" : id;
                    return View("~/Views/Config/index2.cshtml");
                }
            }
            if (consultorioId == "")
            {
                consulta = "select id,consultorio from catTurneroConsultorio where id_catTurnero = @column and activo = 1";
                da = new SqlDataAdapter(consulta, cx);
                da.SelectCommand.Parameters.AddWithValue("@column", csId);
            }
            else
            {

                string[] j = consultorioId.Split('-');

                consulta = "select  id,consultorio from catTurneroConsultorio where ( id_catTurnero = "+ csId + " and ( numero = @column";

                for(int i=1;i<j.Length;i++)
                {
                    consulta += " OR numero=@column_n" + i ;
                }

                consulta += ") and activo = 1)";

                da = new SqlDataAdapter(consulta, cx);
                da.SelectCommand.Parameters.AddWithValue("@column", j[0]);

                for (int i = 1; i < j.Length; i++)
                {
                    da.SelectCommand.Parameters.AddWithValue("@column_n"+i, j[i]);
                }

            }

            dt.Rows.Clear();
            try
            {
                da.Fill(dt);
                cantSalas = (int)dt.Rows.Count;
                List<string> modes = new List<string>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    modes.Add(dt.Rows[i]["consultorio"].ToString());
                }

                List<string> modes2 = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    
                    modes2.Add(dt.Rows[i]["id"].ToString());
                }

                ViewBag.ConsultorioId = modes2;
                ViewBag.ConsultorioNombre = modes;

            }
            catch (Exception err)
            {
                return View();
            }

            

            ViewBag.csId = catTurnerocsId;
            ViewBag.cantSalas = cantSalas;
            ViewBag.csNombre = csNombre;

            return View();
        }


    }




}