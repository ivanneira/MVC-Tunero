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
    public class ConsultorioController : Controller
    {
        // GET: Consultorio
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getConsultorioData(string sistema,int? csId, int? consultorioID)
        {
            SqlConnection cx = null;
            string consulta = "";

            if (sistema.ToLower() == "gedoc")
            {
                 cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_GEDOC"].ConnectionString);
                consulta = "select (cpa.pacApellido + ', ' + cpa.pacNombre) as paciente, (cpe.perApellidoyNombre) as profesional, cta.estado,cta.urgencia,cta.prioridad , cta.id_catTurneroConsultorio as consultorioId" +
                           " from catTurneroAtencion cta" +
                           "     left   join catPaciente cpa on cpa.pacId = cta.pacId" +
                           "     left   join catPersona cpe on cpe.perId = cta.PerConId" +
                           " where cta.csId = @column and cast(cta.fecha as date) = cast(GETDATE() as date) order by estado desc, urgencia desc ,id asc";

            }
            else
            {
                cx = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_MHO"].ConnectionString);
                /*
                consulta = "select (cpa.Apellido + ', ' + cpa.Nombre) as paciente, (cpe.Apellido + ', '+ cpe.Nombre) as profesional, cta.estado,cta.urgencia,cta.prioridad , cta.id_catTurneroConsultorio as consultorioId  "+
                           " from catTurneroAtencion cta "+
                           "     left join Paciente cpa on cpa.PacienteID = cta.pacId "+
                           "     left join Profesional cpe on cpe.ProfesionalID = cta.PerConId "+
                           " where cta.csId = @column and cast(cta.fecha as date) = cast(GETDATE() as date) and id_catTurneroConsultorio = 1 order by estado desc, urgencia desc, id asc";
                */
                consulta = "select cta.pacId, (cpa.Apellido + ', ' + cpa.Nombre) as paciente, (cpe.Apellido + ', '+ cpe.Nombre) as profesional, cta.estado,cta.urgencia,cta.prioridad , cta.id_catTurneroConsultorio as consultorioId  " +
                           " from catTurneroAtencion cta " +
                           "     left join Paciente cpa on cpa.PacienteID = cta.pacId " +
                           "     left join Profesional cpe on cpe.ProfesionalID = cta.PerConId " +
                           "     inner join catturneroconsultorio ctc on ctc.PerConId = cta.PerConId " +
                           " where cta.csId = @column and ctc.id = @column2  order by cta.estado desc, cta.urgencia desc, cta.id asc";
            }



            SqlDataAdapter da = new SqlDataAdapter(consulta, cx);
            da.SelectCommand.Parameters.AddWithValue("@column", csId);
            da.SelectCommand.Parameters.AddWithValue("@column2", consultorioID);

            DataTable dt = new DataTable();

            var list = new List<string[]>();
            List<datosJson> y = new List<datosJson>();

            try
            {
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    var x = new datosJson
                    {
                        pacId = row["pacId"].ToString(),
                        consultorioId = row["consultorioId"].ToString(),
                        paciente = row["paciente"].ToString(),
                        profesional = row["profesional"].ToString(),
                        estado = row["estado"].ToString(),
                        prioridad = row["prioridad"].ToString(),
                        urgencia = row["urgencia"].ToString(),
                        FechaHora = DateTime.Now.ToString("HH:mm")
                    };

                    y.Add(x);
                }
            }
            catch (Exception err)
            {
                return View();
            }

            return Json(y);

        }

        public class  datosJson
        {
            public string paciente { get; set; }
            public string profesional { get; set; }
            public string prioridad { get; set; }
            public string urgencia { get; set; }
            public string estado { get; set; }
            public string consultorioId { get; set; }
            public string FechaHora { get; set; }
            public string pacId { get; set; }
        }


    }
}