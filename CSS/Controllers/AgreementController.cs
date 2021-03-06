﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSS.Models;
using CSS.ViewModels;
using System.Net.Mail;
using System.Net;

namespace CSS.Controllers
{
    public class AgreementController : Controller
    {
        private CSSEntities1 db = new CSSEntities1();
        //
        // GET: /Agreement/

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult ParticialStatics()
        {
            if (Request.IsAuthenticated)
            {
                return View(db.AgreementStatus.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public ActionResult Statics()
        {
            if (Request.IsAuthenticated)
            {
                List<Agreement> recentAgreement = db.Agreements.Where(x => (x.CreatedBy == @User.Identity.Name)).OrderBy(x=>x.LastUpdatedDate).Take(3).ToList() ;
                ViewBag.recentAgreement = recentAgreement;
                return View(db.AgreementStatus.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Statics(string Select)
        {
            if (Request.IsAuthenticated)
            {
                if (Select != null)
                {
                    string[] part = Select.Split('/');
                    return RedirectToAction("View", new { agreementNumber = part[0], variant = part[1] });
                }
                else
                {
                    return RedirectToAction("Statics");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Copy(int agreementNumber = 0, int variant = 0)
        {
            if (Request.IsAuthenticated)
            {
                Agreement agreement = db.Agreements.Find(agreementNumber, variant);
                if (agreement == null)
                {
                    return HttpNotFound();
                }
                //new Variant = highest variant +1
                ViewBag.NewVariant = db.Agreements.Where(x => x.AgreementNumber == agreement.AgreementNumber).OrderByDescending(x => x.VariantNumber).First().VariantNumber + 1;
                return View(agreement);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Copy(Agreement agreement, int choice)
        {
            if (Request.IsAuthenticated)
            {
                Agreement NewAgreement = db.Agreements.AsNoTracking().Single(x => x.AgreementNumber == agreement.AgreementNumber && x.VariantNumber == agreement.VariantNumber);
                NewAgreement.CreatedBy = User.Identity.Name;
                NewAgreement.LastUpdatedDate = DateTime.Now;
                NewAgreement.CreatedDate = DateTime.Now;
                NewAgreement.RFOUsers = db.Agreements.Find(agreement.AgreementNumber, agreement.VariantNumber).RFOUsers;
                switch (choice)
                {
                    //Duplicate this agreement with a new variant
                    case 1:
                        NewAgreement.VariantNumber = db.Agreements.Where(x => x.AgreementNumber == NewAgreement.AgreementNumber).OrderByDescending(x => x.VariantNumber).First().VariantNumber + 1; 
                        NewAgreement.RFONumbers = db.Agreements.Find(agreement.AgreementNumber, agreement.VariantNumber).RFONumbers;
                        db.Agreements.Add(NewAgreement);
                        db.SaveChanges();
                        return RedirectToAction("Edit", new { NewAgreement = agreement.AgreementNumber, variant = NewAgreement.VariantNumber });
                    //Duplicate this agreement agreement for a new agreement with...
                    case 2:
                        NewAgreement.AgreementNumber = db.Agreements.OrderBy(x => x.AgreementNumber).First().AgreementNumber + 1;
                        NewAgreement.VariantNumber = 1;
                        db.Agreements.Add(NewAgreement);
                        db.SaveChanges();
                        return RedirectToAction("Edit", new { agreementNumber = NewAgreement.AgreementNumber, variant = NewAgreement.VariantNumber });
                    //Duplicate this agreement for a new customer
                    case 3:
                        NewAgreement.RFONumbers.Clear();
                        return RedirectToAction("Add", NewAgreement);
                    //Something went wrong
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchAgreement()
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.ListCustomerType = db.CustomerTypes.ToList();
                ViewBag.ListStatus = db.AgreementStatus.ToList();

                ViewBag.ListCSM = (from usertype in db.UserTypes
                                   from username in db.RFOUsers
                                   where usertype.UserType1 == "CSM" && usertype.UserTypeId == username.UserTypeId
                                   select username);


                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SearchAgreement(string select, int choice)
        {
            if (Request.IsAuthenticated)
            {
                if (select != null)
                {
                    string[] part = select.Split('/');
                    switch (choice)
                    {
                        case 1:
                            //Audit
                            break;
                        case 2:
                            //Copy
                            RedirectToAction("Copy", new { agreementNumber = part[0], variant = part[1] });
                            break;
                        case 3:
                            //View
                            break;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("SearchAgreement");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddAgreement()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SelectCustomer()
        {
            if (Request.IsAuthenticated)
            {
                SearchCustomer searchModel = new SearchCustomer();
                ViewBag.CustomerTypes = db.CustomerTypes.ToArray<CustomerType>();
                return PartialView("SelectCustomer", searchModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SearchCustomer()
        {
            SearchCustomer searchModel = new SearchCustomer();
            if (TryUpdateModel(searchModel))
            {
                SearchCustomer[] searchResult = null;
                if (searchModel.RFONumber != null)
                {
                    var results = from rfo in db.RFONumbers.Where(r => (r.RFONumber1 == searchModel.RFONumber))
                                  join com in db.Companies
                                  on rfo.CompanyId equals com.CompanyId
                                  join csm in db.CustomerTypes
                                  on rfo.CustomerTypeId equals csm.CustomerTypeId
                                  select new SearchCustomer
                                  {
                                      RFONumber = rfo.RFONumber1,
                                      SelectedCustomerType = rfo.CustomerType,
                                      Name = com.Name,
                                      PostCode = rfo.PostCode,
                                      BusinessArea = com.BusinessArea
                                  };
                    if (results != null)
                        searchResult = results.ToArray<SearchCustomer>();
                }
                else
                {
                    var results = from rfo in db.RFONumbers.Where(r => ((searchModel.RFONumber == null || r.RFONumber1 == searchModel.RFONumber)
                                        && r.CustomerTypeId == searchModel.SelectedCustomerType.CustomerTypeId
                                        && (string.IsNullOrEmpty(searchModel.PostCode) || r.PostCode.Contains(searchModel.PostCode))))

                                  join com in db.Companies.Where(co => ((string.IsNullOrEmpty(searchModel.Name) || co.Name.Contains(searchModel.Name))
                                        && (string.IsNullOrEmpty(searchModel.BusinessArea) || co.BusinessArea.Contains(searchModel.BusinessArea))))
                                    on rfo.CompanyId equals com.CompanyId

                                  join csm in db.CustomerTypes
                                    on rfo.CustomerTypeId equals csm.CustomerTypeId
                                  select new SearchCustomer
                                  {
                                      RFONumber = rfo.RFONumber1,
                                      SelectedCustomerType = rfo.CustomerType,
                                      Name = com.Name,
                                      PostCode = rfo.PostCode,
                                      BusinessArea = com.BusinessArea
                                  };
                    if (results != null)
                        searchResult = results.ToArray<SearchCustomer>();
                }
                if (searchResult != null)
                    return PartialView("SearchCustomerResults", searchResult);
                else
                    return PartialView("SelectCustomer", new SearchCustomer[0]);
            }
            else
                return PartialView("SearchCustomerResults", new SearchCustomer[0]);
        }

        [HttpPost]
        public ActionResult CreateAgreement(int rfoNumber)
        {
            return PartialView("BasicDetails");
        }

        public ActionResult RejectAgreement()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Homepage()
        {
            return View(db.Agreements);
        }

        public ActionResult ExtendAgreement(int id, int ivar)
        {
            ExtendAgreement extendModel = new ExtendAgreement();
            Agreement iAgr = db.Agreements.Single(x => x.AgreementNumber == id && x.VariantNumber == ivar);
            
            if (iAgr.RFONumbers.Count > 0)
                extendModel.RFONumber = iAgr.RFONumbers.First().RFONumber1;
            extendModel.Name = iAgr.Name;
            extendModel.AgreementNumber = iAgr.AgreementNumber;
            extendModel.VariantNumber = iAgr.VariantNumber;
            extendModel.StartDate = (DateTime)iAgr.StartDate;
            extendModel.EndDate = (DateTime)iAgr.EndDate;
            extendModel.StatusId = (int)iAgr.StatusId;

            return View(extendModel);
        }

        //------------send email------------
        private void SendEmail(string iEmailTo, string iSubject, string iBody)
        {
            if (iEmailTo != null)
            {
                string smtpAddress = "smtp.mail.yahoo.com";
                int portNumber = 587;
                bool enableSSL = true;

                string emailFrom = "phuong_css@yahoo.com.vn";
                string password = "taikhoancss";
                string emailTo = iEmailTo;

                string subject = iSubject;
                string body = iBody;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
        }

        public ActionResult DisposeExtendAgreement()
        {
            ExtendAgreement extendModel = new ExtendAgreement();
            if (TryUpdateModel(extendModel))
            {
                //Agreement NewAgreement = db.Agreements.AsNoTracking().Single(x => x.AgreementNumber == extendModel.AgreementNumber && x.VariantNumber == extendModel.VariantNumber);

                ////------------Add new agreement------------
                //NewAgreement.VariantNumber = db.Agreements.Where(x => x.AgreementNumber == extendModel.AgreementNumber).OrderByDescending(x => x.VariantNumber).First().VariantNumber + 1;
                //NewAgreement.StatusId = 1;
                //NewAgreement.StartDate = extendModel.StartDate;
                //NewAgreement.EndDate = extendModel.EndDate;
                ////add agreementRFO
                //NewAgreement.RFONumbers = db.Agreements.Find(extendModel.AgreementNumber, extendModel.VariantNumber).RFONumbers;


                ////------------send email------------
                //Company CompanySendEmail = db.Companies.Find(db.Agreements.Find(extendModel.AgreementNumber, extendModel.VariantNumber).RFONumbers.First().CompanyId);

                //string subject = "Hello, " + CompanySendEmail.Name + ".";
                //string body = "We are System Administrator. We wanted inform with you.\n" + "The system been create new variant based on the previous agreement and add new entry to audit trail."
                //    + " New variant  have AgreementNumber = " + NewAgreement.AgreementNumber + ", VariantNumber = " + NewAgreement.VariantNumber;
                //SendEmail(CompanySendEmail.Emailaddress, subject, body);

                ////------------discounts: Replace UC11------------
                //if (extendModel.EndDate < DateTime.Now)
                //{
                //    int afterCharge = (int)NewAgreement.HandlingCharge - (int)NewAgreement.DiscountUnit;
                //    if (afterCharge < 0)
                //        afterCharge = 0;
                //    NewAgreement.HandlingCharge = afterCharge;
                //}

                ////------------save------------
                //db.Agreements.Add(NewAgreement);
                //db.SaveChanges();
            }
            //RedirectToAction: trả về hàm index-> để show ra trang chính
            return RedirectToAction("HomePage");
        }

        // UC 06
        public ActionResult TerminateAgreement(int id, int ivar)
        {
            Agreement iAgr = db.Agreements.Single(x => x.AgreementNumber == id && x.VariantNumber == ivar);

            //if (iAgr.StatusId == 4)
            //{
            //    iAgr.StatusId = 6;//"Discontinued"
            //    db.SaveChanges();

            //    //------------send email------------
            //    Company CompanySendEmail = db.Companies.Find(db.Agreements.Find(iAgreementNumber, iVariantNumber).RFONumbers.First().CompanyId);

            //    string subject = "Hello, " + CompanySendEmail.Name + ".";
            //    string body = "We are System Administrator. We wanted inform with you. System been change agreement status to 'Discontinued'. The agreement"
            //        + " have AgreementNumber = " + iAgreementNumber + " and VariantNumber = " + iVariantNumber;

            //    SendEmail(CompanySendEmail.Emailaddress, subject, body);
            //}

            return View(iAgr);
        }

        //UC 07
        public ActionResult CompleteAgreement(int iAgreementNumber, int iVariantNumber)
        {
            Agreement iAgr = db.Agreements.Single(x => x.AgreementNumber == iAgreementNumber && x.VariantNumber == iVariantNumber);
            Company CompanySendEmail = db.Companies.Find(db.Agreements.Find(iAgreementNumber, iVariantNumber).RFONumbers.First().CompanyId);

            //if (iAgr.StatusId == 1)
            //{
            //    iAgr.StatusId = 2;//"Draft"
            //    db.SaveChanges();
            //}

            //------------send email------------
            //

            //string subject = "Hello, " + CompanySendEmail.Name + ".";
            //string body = "We are System Administrator. We wanted inform with you. System been change agreement status to 'Discontinued'. The agreement"
            //    + " have AgreementNumber = " + iAgreementNumber + " and VariantNumber = " + iVariantNumber;

            //SendEmail(CompanySendEmail.Emailaddress, subject, body);

            return View(CompanySendEmail);
        }
    }
}