using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using QRCoder;
using WebApplication1.Dataset;
using WebApplication1.Models;
using WebApplication1.Reports;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        private sysKabugwasonEntities db = new sysKabugwasonEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult GenerateApplicationFormFront(int applicationId)//https://localhost:44357/Report/GenerateApplicationFormFront?applicationId=1100
        {
            try
            {
                CRApplicationFormFront rpt = new CRApplicationFormFront();
                dsReports dsApplicationForm = new dsReports();

                using (var sqlCmd = db.Database.Connection.CreateCommand())
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add(new SqlParameter("@afid", applicationId));
                    using (DbDataAdapter da = new SqlDataAdapter())
                    {
                        sqlCmd.CommandText = "LoadReportApplicationForm";
                        da.SelectCommand = sqlCmd;
                        da.Fill(dsApplicationForm, "tblApplicationForm");

                        foreach (DataRow row in dsApplicationForm.Tables["tblApplicationForm"].Rows)
                        {
                            string qr_value = $"{row["af_ref_no"]} | {DateTime.Now}";
                            byte[] qrCode = GenerateQRCode(qr_value);
                            row["qr_code"] = qrCode;
                        }

                        sqlCmd.CommandText = "LoadReportSiblings";
                        da.SelectCommand = sqlCmd;
                        da.Fill(dsApplicationForm, "tblSiblings");

                        rpt.Subreports[0].SetDataSource(dsApplicationForm);
                        rpt.SetDataSource(dsApplicationForm);
                    }
                }

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

        public ActionResult GenerateApplicationFormBack(int applicationId)//https://localhost:44357/Report/GenerateApplicationFormBack?applicationId=1100
        {
            try
            {
                CRApplicationFormBack rpt = new CRApplicationFormBack();
                dsReports dsApplicationForm = new dsReports();

                using (var sqlCmd = db.Database.Connection.CreateCommand())
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add(new SqlParameter("@afid", applicationId));
                    using (DbDataAdapter da = new SqlDataAdapter())
                    {
                        sqlCmd.CommandText = "LoadReportApplicationForm";
                        da.SelectCommand = sqlCmd;
                        da.Fill(dsApplicationForm, "tblApplicationForm");

                        sqlCmd.CommandText = "LoadReportSiblings";
                        da.SelectCommand = sqlCmd;
                        da.Fill(dsApplicationForm, "tblSiblings");



                        foreach (DataRow row in dsApplicationForm.Tables["tblApplicationForm"].Rows)
                        {
                            string qr_value = $"{row["af_ref_no"]} | {DateTime.Now}";
                            byte[] qrCode = GenerateQRCode(qr_value);
                            row["qr_code"] = qrCode;
                        }

                        rpt.SetDataSource(dsApplicationForm);
                    }
                }

                Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                rpt.Close();
                rpt.Dispose();
                GC.Collect();

                return File(stream, "application/pdf", $"ApplicationForm_Back_{applicationId}.pdf");
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

        private byte[] GenerateQRCode(string qrValue)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrValue, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public ActionResult ApplicantsByMunAndBrgy() // https://localhost:44357/Report/ApplicantsByMunAndBrgy
        {
            ReportClass rpt = new CRApplicantsByMunAndBrgy();
            rpt.SetParameterValue("@batch_id", null);

            ApplyConnectionInfo(rpt);
            Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            rpt.Close();
            rpt.Dispose();
            GC.Collect();

            stream.Position = 0; // Reset stream
            return File(stream, "application/pdf");
        }

        public ActionResult ListOfIPStudents() // https://localhost:44357/Report/ListOfIPStudents
        {
            ReportClass rpt  = new CRListofIPStudents();
            rpt.SetParameterValue("@batch_id", null);

            ApplyConnectionInfo(rpt);
            Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            rpt.Close();
            rpt.Dispose();
            GC.Collect();

            stream.Position = 0; // Reset stream
            return File(stream, "application/pdf");
        }

        public ActionResult ScholarsByProgramCategory() // https://localhost:44357/Report/ScholarsByProgramCategory
        {
            ReportClass rpt = new CRLoadReportsScholarsByProgramCategory();
            rpt.SetParameterValue("@batch_id", null);

            ApplyConnectionInfo(rpt);
            Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            rpt.Close();
            rpt.Dispose();
            GC.Collect();

            stream.Position = 0; // Reset stream
            return File(stream, "application/pdf");
        }

        public static void ApplyConnectionInfo(ReportDocument report)
        {
            // Connection info for OLE DB (ADO)
            ConnectionInfo connectionInfo = new ConnectionInfo
            {
                ServerName = "188.180.66.231",
                DatabaseName = "sysKabugwason",
                UserID = "ojt",
                Password = "ojt123",
                IntegratedSecurity = false // Set to true if using Windows Authentication
            };

            // Apply to main report tables
            foreach (Table table in report.Database.Tables)
            {
                TableLogOnInfo logOnInfo = table.LogOnInfo;
                logOnInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(logOnInfo);
            }

            // Apply to subreports if applicable (optional)
            foreach (Section section in report.ReportDefinition.Sections)
            {
                foreach (ReportObject reportObject in section.ReportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReport = subreportObject.OpenSubreport(subreportObject.SubreportName);

                        foreach (Table table in subReport.Database.Tables)
                        {
                            TableLogOnInfo logOnInfo = table.LogOnInfo;
                            logOnInfo.ConnectionInfo = connectionInfo;
                            table.ApplyLogOnInfo(logOnInfo);
                        }
                    }
                }
            }
        }
    }
}

