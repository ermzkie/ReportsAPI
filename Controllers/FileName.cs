using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.MVC;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        private sysBCSEntities db = new sysBCSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }


        public ActionResult GenerateApplicationFormFront(int applicationId)
        {
            try
            {
                var collections = db.LoadReportApplicationForm(applicationId).Select(a => new
                {
                    a.afid,
                    a.af_ref_no,
                    a.student_id,
                    a.af_gwa,
                    a.progid,
                    a.prog_abb,
                    a.catid,
                    a.cat_code,
                    a.course_abb,
                    a.year_level,
                    a.school_id,
                    a.school_abb,
                    a.school_name,
                    a.last_name,
                    a.first_name,
                    a.middle_name,
                    a.name_ext_code,
                    a.birthdate,
                    a.place_of_birth,
                    a.sex,
                    a.religion_id,
                    a.religion_name,
                    a.civil_status_id,
                    a.civil_status_name,
                    a.email_add,
                    a.fb_acct_name,
                    a.house_no,
                    a.lot_no,
                    a.purok,
                    a.street,
                    a.barangay_id,
                    a.barangay_name,
                    a.cm_name,
                    a.father_last_name,
                    a.father_first_name,
                    a.father_middle_name,
                    a.father_income,
                    a.father_occupation,
                    a.mother_last_name,
                    a.mother_first_name,
                    a.mother_middle_name,
                    a.mother_income,
                    a.mother_occupation
                }).ToList();

                ReportDocument rpt = new ReportDocument();
                rpt.Load(Path.Combine(Server.MapPath("~/Reports"), "CRApplicationFormFront.rpt"));
                rpt.SetDataSource(collections);
                rpt.SetParameterValue("header", "Provincial Government of South Cotabato");

                Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                rpt.Close();
                rpt.Dispose();
                GC.Collect();

                return File(stream, "application/pdf", $"ApplicationForm_Front_{applicationId}.pdf");
            }
            catch (Exception ex)
            {

                return Content($"Error generating report: {ex.Message}");
            }
        }


        public ActionResult GenerateApplicationFormBack(int remitId)
        {
            try
            {
                var collections = db.LoadReportApplicationForm(remitId).Select(a => new
                {

                    a.afid,
                    a.af_ref_no,
                    a.student_id,
                    a.af_gwa,
                    a.progid,
                    a.prog_abb,
                    a.catid,
                    a.cat_code,
                    a.course_abb,
                    a.year_level,
                    a.school_id,
                    a.school_abb,
                    a.school_name,
                    a.last_name,
                    a.first_name,
                    a.middle_name,
                    a.name_ext_code,
                    a.birthdate,
                    a.place_of_birth,
                    a.sex,
                    a.religion_id,
                    a.religion_name,
                    a.civil_status_id,
                    a.civil_status_name,
                    a.email_add,
                    a.fb_acct_name,
                    a.house_no,
                    a.lot_no,
                    a.purok,
                    a.street,
                    a.barangay_id,
                    a.barangay_name,
                    a.cm_name,
                    a.father_last_name,
                    a.father_first_name,
                    a.father_middle_name,
                    a.father_income,
                    a.father_occupation,
                    a.mother_last_name,
                    a.mother_first_name,
                    a.mother_middle_name,
                    a.mother_income,
                    a.mother_occupation

                }).ToList();

                ReportDocument rpt = new ReportDocument();
                rpt.Load(Path.Combine(Server.MapPath("~/Reports"), "CRApplicationFormBack.rpt"));
                rpt.SetDataSource(collections);
                rpt.SetParameterValue("header", "Provincial Government of South Cotabato");

                Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                rpt.Close();
                rpt.Dispose();
                GC.Collect();

                return File(stream, "application/pdf", $"ApplicationForm_Back_{remitId}.pdf");
            }
            catch (Exception ex)
            {

                return Content($"Error generating report: {ex.Message}");
            }
        }


        public ActionResult GenerateCompleteApplication(int remitId)
        {
            try
            {
                var collections = db.LoadReportApplicationForm(remitId).Select(a => new
                {

                    a.afid,
                    a.af_ref_no,
                    a.student_id,
                    a.af_gwa,
                    a.progid,
                    a.prog_abb,
                    a.catid,
                    a.cat_code,
                    a.course_abb,
                    a.year_level,
                    a.school_id,
                    a.school_abb,
                    a.school_name,
                    a.last_name,
                    a.first_name,
                    a.middle_name,
                    a.name_ext_code,
                    a.birthdate,
                    a.place_of_birth,
                    a.sex,
                    a.religion_id,
                    a.religion_name,
                    a.civil_status_id,
                    a.civil_status_name,
                    a.email_add,
                    a.fb_acct_name,
                    a.house_no,
                    a.lot_no,
                    a.purok,
                    a.street,
                    a.barangay_id,
                    a.barangay_name,
                    a.cm_name,
                    a.father_last_name,
                    a.father_first_name,
                    a.father_middle_name,
                    a.father_income,
                    a.father_occupation,
                    a.mother_last_name,
                    a.mother_first_name,
                    a.mother_middle_name,
                    a.mother_income,
                    a.mother_occupation
                }).ToList();


                MemoryStream combinedPdf = new MemoryStream();


                ReportDocument frontRpt = new ReportDocument();
                frontRpt.Load(Path.Combine(Server.MapPath("~/Reports"), "CRApplicationFormFront.rpt"));
                frontRpt.SetDataSource(collections);
                frontRpt.SetParameterValue("header", "Provincial Government of South Cotabato");


                ReportDocument backRpt = new ReportDocument();
                backRpt.Load(Path.Combine(Server.MapPath("~/Reports"), "CRApplicationFormBack.rpt"));
                backRpt.SetDataSource(collections);
                backRpt.SetParameterValue("header", "Provincial Government of South Cotabato");

                Stream frontStream = frontRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                Stream backStream = backRpt.ExportToStream(ExportFormatType.PortableDocFormat);


                frontRpt.Close();
                frontRpt.Dispose();
                backRpt.Close();
                backRpt.Dispose();
                GC.Collect();

                return File(frontStream, "application/pdf", $"CompleteApplication_{remitId}.pdf");
            }
            catch (Exception ex)
            {

                return Content($"Error generating complete report: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}