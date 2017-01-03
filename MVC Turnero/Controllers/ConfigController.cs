using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MVC_Turnero.Controllers
{
    public class ConfigController : Controller
    {
        // GET: Confi
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetConfigConsultorioJsonResult(string sistema, int? csId)
        {

            SqlConnection cx = null;

            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "select ctc.id,ctc.consultorio as ConsultorioNombre, ctc.numero as identificador, ctc.activo as Activo, (p.Apellido + ', ' + p.Nombre) as Profesional, p.ProfesionalID  from catTurneroConsultorio ctc " +
                           " inner join catTurnero ct on ct.csId = ctc.id_catTurnero " +
                           " left join Profesional p on p.ProfesionalID = ctc.PerConId " +
                           " where ct.csId = @column ";
            }

            SqlDataAdapter da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", csId);

            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch(Exception err)
            {
                return Json(err.Message);
            }
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return Json(rows);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetProfesionalesJsonResult(string sistema, int? csId) {


            var result = "";


            SqlConnection cx = null;

            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "select p.ProfesionalID, (p.Apellido + ', ' + p.Nombre) as Profesional from profesional p "+
                           " order by p.Apellido, p.Nombre asc";
            }

            SqlDataAdapter da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", csId);

            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch (Exception err)
            {
                return Json(err.Message);
            }
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return Json(rows);

        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveProfesionalesJsonResult(string sistema, int? csId, int EstadoConsultorio, string ConsultorioNombre, int ProfesionalID, int ConsultorioNumero)
        {

            if (!CheckProfesionalesJsonResult(sistema, csId, ConsultorioNumero,EstadoConsultorio))
            {
                return Json("REPEATEDID");
            }
            var result = "";
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "insert into catTurneroConsultorio (id_catTurnero, consultorio, numero, activo, PerConId) values " +
                           "('" + csId + "', '" + ConsultorioNombre + "','" + ConsultorioNumero + "','" + EstadoConsultorio + "','" + ProfesionalID + "')";
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json(result);
            }

            return Json(result);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public bool CheckProfesionalesJsonResult(string sistema, int? csId, int ConsultorioNumero, int EstadoConsultorio)
        {
            bool result = true;
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "select activo from catTurneroConsultorio where numero = " + ConsultorioNumero + " and activo=1 and activo=" + EstadoConsultorio ;
            }


            SqlDataAdapter da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", ConsultorioNumero);
            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["activo"].ToString() == "1")
                    {
                        result = false;
                    }
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return result;

        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult EditProfesionalesJsonResult(string sistema, int? csId, int EstadoConsultorio, string ConsultorioNombre, int ProfesionalID, int ConsultorioNumero, int id)
        {

            var result = "";
            SqlConnection cx = null;
            string consulta = "";

            if (!CheckProfesionalesJsonResult(sistema, csId, ConsultorioNumero,EstadoConsultorio))
            {
                return Json("REPEATEDID");
            }

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "update catTurneroConsultorio set consultorio ='" + ConsultorioNombre + "', numero = '" + ConsultorioNumero + "', activo = '" + EstadoConsultorio + "', PerConId='" + ProfesionalID + "' where id='" + id + "'";
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json(result);
            }

            return Json(result);
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DelProfesionalesJsonResult(string sistema, int? csId, int EstadoConsultorio, string ConsultorioNombre, int ProfesionalID, int ConsultorioNumero,int id)
        {

            var result = "";
            SqlConnection cx = null;
            string consulta = "";


            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "delete from catTurneroConsultorio where id = " + id;
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json(result);
            }

            return Json(result);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveMesajesJsonResult(string sistema, int? csId, string mensaje)
        {

            var result = "false";
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "insert into catTurneroMensaje (id_catTurnero, mensaje) values " +
                           "('" + csId + "', '" + mensaje + "')";
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json("false");
            }

            return Json(result);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetMensajeJsonResult(string sistema, int? csId)
        {

            var result = "";

            SqlConnection cx = null;

            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "select id,mensaje from catTurneroMensaje where id_catTurnero =  " + csId;
            }

            SqlDataAdapter da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", csId);

            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch (Exception err)
            {
                return Json(err.Message);
            }
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return Json(rows);

        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DelMensajesJsonResult(string sistema, int? csId, int idMensaje)
        {
            var result = "";
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "delete from catTurneroMensaje where id =  " + idMensaje + " and  id_catTurnero =" + csId ;
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json(result);
            }

            return Json(result);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult EditMensajeJsonResult(string sistema, int? csId, int idMensaje, string mensaje)
        {
            var result = "";
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "mho")
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                consulta = "update catTurneroMensaje set mensaje ='" + mensaje + "' where id=" + idMensaje + " and id_catTurnero=" + csId ;
            }

            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cx.Open();
                da.InsertCommand = new SqlCommand(consulta, cx);
                da.InsertCommand.ExecuteNonQuery();
                result = "true";
            }
            catch (Exception err)
            {
                result = err.Message;
                return Json(result);
            }

            return Json(result);
        }
    }
}