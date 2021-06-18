// This code was built using Visual Studio 2005
using System;
using System.IO;
using System.Web.Services.Protocols;
using ShipWebServiceClient.ShipServiceWebReference;

// Sample code to call the FedEx Ship Service - GDE India Shipment
// Tested with Microsoft Visual Studio 2005 Professional Edition

namespace ShipWebServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set this to true to process a COD shipment and print a COD return Label
            Boolean isCodShipment = true;

            ProcessShipmentRequest request = CreateShipmentRequest();
            //
            ShipService service = new ShipService();
			if (usePropertyFile())
            {
                service.Url = getProperty("endpoint");
            }
            //
            try
            {
                // Call the ship web service passing in a shipment request and returning a shipment reply
                ProcessShipmentReply reply = service.processShipment(request);
                if ((reply.HighestSeverity != NotificationSeverityType.ERROR) && (reply.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    ShowShipmentReply(reply, isCodShipment);
                }
                ShowNotifications(reply);
            }
            catch (SoapException e)
            {
                Console.WriteLine(e.Detail.InnerText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press any key to quit !");
            Console.ReadKey();
        }

        private static WebAuthenticationDetail SetWebAuthenticationDetail()
        {
            WebAuthenticationDetail wad = new WebAuthenticationDetail();
            wad.UserCredential = new WebAuthenticationCredential();
            wad.ParentCredential = new WebAuthenticationCredential();
            wad.UserCredential.Key = "XXX"; // Replace "XXX" with the Key
            wad.UserCredential.Password = "XXX"; // Replace "XXX" with the Password
            wad.ParentCredential = new WebAuthenticationCredential();
            wad.ParentCredential.Key = "XXX"; // Replace "XXX" with the Key
            wad.ParentCredential.Password = "XXX"; // Replace "XXX"
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                wad.UserCredential.Key = getProperty("key");
                wad.UserCredential.Password = getProperty("password");
                wad.ParentCredential.Key = getProperty("parentkey");
                wad.ParentCredential.Password = getProperty("parentpassword");
            }
            return wad;
        }

        private static ProcessShipmentRequest CreateShipmentRequest()
        {
            // Build the ShipmentRequest
            ProcessShipmentRequest request = new ProcessShipmentRequest();
            //
            request.WebAuthenticationDetail = SetWebAuthenticationDetail();
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = "XXX"; // Replace "XXX" with the client's account number
            request.ClientDetail.MeterNumber = "XXX"; // Replace "XXX" with the client's meter number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.ClientDetail.AccountNumber = getProperty("accountnumber");
                request.ClientDetail.MeterNumber = getProperty("meternumber");
            }
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***GDE India Shipment Request using VC#***"; // The client will get the same value back in the response
            //
            request.Version = new VersionId();
            //
            SetShipmentDetails(request);
            //
            SetSender(request);
            //
            SetRecipient(request);
            //
            SetPayment(request);
            //
            SetShipmentSpecialServicesRequested(request);
            //
            SetLabelDetails(request);
            //
            SetPackageLineItems(request);
            //
            SetCustomsClearanceDetails(request);
            //
            return request;
        }
       
        private static void SetShipmentDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Ship date and time
            //
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.ServiceType = ServiceType.STANDARD_OVERNIGHT;
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type YOUR_PACKAGING, ...
            //
            request.RequestedShipment.PackageCount = "1";            
        }

        private static void SetSender(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Contact = new Contact();
            request.RequestedShipment.Shipper.Contact.PersonName = "Sender Name";
            request.RequestedShipment.Shipper.Contact.CompanyName = "Sender Company Name";
            request.RequestedShipment.Shipper.Contact.PhoneNumber = "0805522713";
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new String[1] { "1 SENDER STREET" };
            request.RequestedShipment.Shipper.Address.City = "PUNE";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "MH";
            request.RequestedShipment.Shipper.Address.PostalCode = "411011";
            request.RequestedShipment.Shipper.Address.CountryCode = "IN";
            request.RequestedShipment.Shipper.Address.CountryName = "INDIA";
        }

        private static void SetRecipient(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Contact = new Contact();
            request.RequestedShipment.Recipient.Contact.PersonName = "Recipient Name";
            request.RequestedShipment.Recipient.Contact.CompanyName = "Recipient Company Name";
            request.RequestedShipment.Recipient.Contact.PhoneNumber = "9012637906";
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new String[1] {"1 RECIPIENT STREET"};
            request.RequestedShipment.Recipient.Address.City = "NEWDELHI";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "DL";
            request.RequestedShipment.Recipient.Address.PostalCode = "110010";
            request.RequestedShipment.Recipient.Address.CountryCode = "IN";
            request.RequestedShipment.Recipient.Address.CountryName = "INDIA";
            request.RequestedShipment.Recipient.Address.Residential = false;
            request.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
        }

        private static void SetPayment(ProcessShipmentRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = "XXX"; // Replace "XXX" with client's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = getProperty("payoraccount");
            }
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Contact = new Contact();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address = new Address();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address.CountryCode = "IN";
        }

        private static void SetShipmentSpecialServicesRequested(ProcessShipmentRequest request)
        {
            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested();
            request.RequestedShipment.SpecialServicesRequested.SpecialServiceTypes = new ShipmentSpecialServiceType[1] { ShipmentSpecialServiceType.COD };
            request.RequestedShipment.SpecialServicesRequested.CodDetail = new CodDetail();
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount = new Money();
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount.Currency = "INR";
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount.Amount = 400M;
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CollectionType = CodCollectionType.GUARANTEED_FUNDS;
            SetFinancialInstitionContactAndAddress(request.RequestedShipment.SpecialServicesRequested.CodDetail);
            request.RequestedShipment.SpecialServicesRequested.CodDetail.RemitToName = "Remitter";
        }

        private static void SetFinancialInstitionContactAndAddress(CodDetail codDetail)
        {
            codDetail.FinancialInstitutionContactAndAddress = new ContactAndAddress();
            codDetail.FinancialInstitutionContactAndAddress.Contact = new Contact();
            codDetail.FinancialInstitutionContactAndAddress.Contact.PersonName = "Financial Contact";
            codDetail.FinancialInstitutionContactAndAddress.Contact.CompanyName = "Financial Company";
            codDetail.FinancialInstitutionContactAndAddress.Contact.PhoneNumber = "8888888888";
            codDetail.FinancialInstitutionContactAndAddress.Address = new Address();
            codDetail.FinancialInstitutionContactAndAddress.Address.StreetLines = new String[1] {"1 FINANCIAL STREET"};
            codDetail.FinancialInstitutionContactAndAddress.Address.City = "NEWDELHI";
            codDetail.FinancialInstitutionContactAndAddress.Address.StateOrProvinceCode = "DL";
            codDetail.FinancialInstitutionContactAndAddress.Address.PostalCode = "110010";
            codDetail.FinancialInstitutionContactAndAddress.Address.CountryCode = "IN";
            codDetail.FinancialInstitutionContactAndAddress.Address.CountryName = "INDIA";
        }

        private static void SetLabelDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification();
            request.RequestedShipment.LabelSpecification.ImageType = ShippingDocumentImageType.PDF; // Image types PDF, PNG, DPL, ...
            request.RequestedShipment.LabelSpecification.ImageTypeSpecified = true;
            request.RequestedShipment.LabelSpecification.LabelFormatType = LabelFormatType.COMMON2D;
            request.RequestedShipment.LabelSpecification.LabelStockType = LabelStockType.PAPER_7X475;
            request.RequestedShipment.LabelSpecification.LabelStockTypeSpecified = true;
        }

        private static void SetPackageLineItems(ProcessShipmentRequest request)
        {
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
            request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = "1";
            // Package weight information
            request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight();
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = 20.0M;
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.LB;
            // package dimensions
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions();
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "12";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "13";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "14";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
            // insured value
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money();
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Amount = 100;
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = "INR";
            // Reference details
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences = new CustomerReference[1] { new CustomerReference() };
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE;
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].Value = "testreference";
        }

        private static void SetCustomsClearanceDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor = new Payor();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.AccountNumber = "XXX"; // Replace "XXX" with payor's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.AccountNumber = getProperty("dutiesaccount");
            }
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Contact = new Contact();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Address = new Address();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Address.CountryCode = "IN";
            request.RequestedShipment.CustomsClearanceDetail.DocumentContent = InternationalDocumentContentType.NON_DOCUMENTS;
            request.RequestedShipment.CustomsClearanceDetail.DocumentContentSpecified = true;
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue = new Money();
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue.Amount = 400M;
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue.Currency = "INR";
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice = new CommercialInvoice();
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.Purpose = PurposeOfShipmentType.SOLD;
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.PurposeSpecified = true;
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.CustomerReferences = new CustomerReference[1] { new CustomerReference() };
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.CustomerReferences[0].CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE;
            request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.CustomerReferences[0].Value = "1234";
            SetCommodityDetails(request);
        }

        private static void SetCommodityDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.CustomsClearanceDetail.Commodities = new Commodity[1] { new Commodity() };
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].NumberOfPieces = "1";
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Description = "Books";
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CountryOfManufacture = "IN";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight = new Weight();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight.Value = 1M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight.Units = WeightUnits.LB;
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Quantity = 1M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].QuantitySpecified = true;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].QuantityUnits = "EA";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice = new Money();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice.Amount = 100M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice.Currency = "INR";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue = new Money();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue.Amount = 400M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue.Currency = "INR";
        }

        private static void ShowShipmentReply(ProcessShipmentReply reply, Boolean isCodShipment)
        {
            Console.WriteLine("Shipment Reply details:");
            // Details for each package
            foreach (CompletedPackageDetail packageDetail in reply.CompletedShipmentDetail.CompletedPackageDetails)
            {
                ShowTrackingDetails(packageDetail.TrackingIds);
                if (null != reply.CompletedShipmentDetail && null != reply.CompletedShipmentDetail.ShipmentRating)
                {
                    ShowPackageRateDetails(reply.CompletedShipmentDetail.ShipmentRating);
                }
                else
                {
                    Console.WriteLine("No rate information returned.");
                }
                if (null != packageDetail.Label.Parts[0].Image)
                {
                    // Save outbound shipping label
                    string LabelPath = "c:\\";
                    if (usePropertyFile())
                    {
                        LabelPath = getProperty("labelpath");
                    }
                    string LabelFileName = LabelPath + packageDetail.TrackingIds[0].TrackingNumber + ".pdf";
                    SaveLabel(LabelFileName, packageDetail.Label.Parts[0].Image);
                    ShowShipmentLabels(reply.CompletedShipmentDetail, packageDetail, isCodShipment);
                }
                ShowBarcodeDetails(packageDetail.OperationalDetail.Barcodes);
            }
            ShowPackageRouteDetails(reply.CompletedShipmentDetail.OperationalDetail);
        }

        private static void ShowTrackingDetails(TrackingId[] TrackingIds)
        {
            // Tracking information for each package
            Console.WriteLine("Tracking details");
            if (TrackingIds != null)
            {
                for (int i = 0; i < TrackingIds.Length; i++)
                {
                    Console.WriteLine("Tracking # {0} Form ID {1}", TrackingIds[i].TrackingNumber, TrackingIds[i].FormId);
                }
            }
        }

        private static void ShowPackageRateDetails(ShipmentRating ShipmentRating)
        {
            Console.WriteLine("\nRate details");
            for (int i = 0; i < ShipmentRating.ShipmentRateDetails.Length; i++)
            {
                Console.WriteLine("RateType: " + ShipmentRating.ActualRateType);
                Console.WriteLine("Total Billing Weight: " + ShipmentRating.ShipmentRateDetails[i].TotalBillingWeight.Value);
                Console.WriteLine("Total Base Charge: " + ShipmentRating.ShipmentRateDetails[i].TotalBaseCharge.Amount);
                Console.WriteLine("Total Freight Discount: " + ShipmentRating.ShipmentRateDetails[i].TotalFreightDiscounts.Amount);
                Console.WriteLine("Total Surcharges: " + ShipmentRating.ShipmentRateDetails[i].TotalSurcharges.Amount);
                Console.WriteLine("Total Net Charge: " + ShipmentRating.ShipmentRateDetails[i].TotalNetCharge.Amount);
                Console.WriteLine("**********************************************************");
            }
        }

        private static void ShowBarcodeDetails(PackageBarcodes barcodes)
        {
            // Barcode information for each package
            Console.WriteLine("\nBarcode details");
            if (barcodes != null)
            {
                if (barcodes.StringBarcodes != null)
                {
                    for (int i = 0; i < barcodes.StringBarcodes.Length; i++)
                    {
                        Console.WriteLine("String barcode {0} Type {1}", barcodes.StringBarcodes[i].Value, barcodes.StringBarcodes[i].Type);
                    }
                }
                if (barcodes.BinaryBarcodes != null)
                {
                    for (int i = 0; i < barcodes.BinaryBarcodes.Length; i++)
                    {
                        Console.WriteLine("Binary barcode Type {0}", barcodes.BinaryBarcodes[i].Type);
                    }
                }
            }
        }

        private static void ShowPackageRouteDetails(ShipmentOperationalDetail routingDetail)
        {
            Console.WriteLine("\nRouting details");
            Console.WriteLine("URSA prefix {0} suffix {1}", routingDetail.UrsaPrefixCode, routingDetail.UrsaSuffixCode);
            Console.WriteLine("Service commitment {0} Airport ID {1}", routingDetail.DestinationLocationId, routingDetail.AirportId);

            if (routingDetail.DeliveryDaySpecified)
            {
                Console.WriteLine("Delivery day " + routingDetail.DeliveryDay);
            }
            if (routingDetail.DeliveryDateSpecified)
            {
                Console.WriteLine("Delivery date " + routingDetail.DeliveryDate.ToShortDateString());
            }
            if (routingDetail.TransitTimeSpecified)
            {
                Console.WriteLine("Transit time " + routingDetail.TransitTime);
            }
        }

        private static void ShowShipmentLabels(CompletedShipmentDetail completedShipmentDetail, CompletedPackageDetail packageDetail, Boolean isCodShipment)
        {
            if (packageDetail.Label.Parts[0].Image != null)
            {
                // Save outbound shipping label
                string LabelPath = "c:\\";
                if (usePropertyFile())
                {
                    LabelPath = getProperty("labelpath");
                }
                string FileName = LabelPath + packageDetail.TrackingIds[0].TrackingNumber + ".pdf";
                SaveLabel(FileName, packageDetail.Label.Parts[0].Image);
                // Save COD Return label
                if (isCodShipment)
                {
                    foreach(AssociatedShipmentDetail associatedShipment in completedShipmentDetail.AssociatedShipments)
                    {
                        if (associatedShipment.Type == AssociatedShipmentType.COD_RETURN)
                        {
                            if (associatedShipment.Label.Parts[0].Image != null)
                            {
                                string CodFileName = LabelPath + associatedShipment.TrackingId.TrackingNumber + "CR.pdf";
                                SaveLabel(CodFileName, associatedShipment.Label.Parts[0].Image);
                            }
                        }
                    }
                }
            }
        }

        private static void SaveLabel(string labelFileName, byte[] labelBuffer)
        {
            // Save label buffer to file
            FileStream LabelFile = new FileStream(labelFileName, FileMode.Create);
            LabelFile.Write(labelBuffer, 0, labelBuffer.Length);
            LabelFile.Close();
            // Display label in Acrobat
            DisplayLabel(labelFileName);
        }

        private static void DisplayLabel(string labelFileName)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(labelFileName);
            info.UseShellExecute = true;
            info.Verb = "open";
            System.Diagnostics.Process.Start(info);
        }

        private static void ShowNotifications(ProcessShipmentReply reply)
        {
            Console.WriteLine("Notifications");
            for (int i = 0; i < reply.Notifications.Length; i++)
            {
                Notification notification = reply.Notifications[i];
                Console.WriteLine("Notification no. {0}", i);
                Console.WriteLine(" Severity: {0}", notification.Severity);
                Console.WriteLine(" Code: {0}", notification.Code);
                Console.WriteLine(" Message: {0}", notification.Message);
                Console.WriteLine(" Source: {0}", notification.Source);
            }
        }
        private static bool usePropertyFile() //Set to true for common properties to be set with getProperty function.
        {
            return getProperty("usefile").Equals("True");
        }
        private static String getProperty(String propertyname) //Sets common properties for testing purposes.
        {
            try
            {
                String filename = "C:\\filepath\\filename.txt";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(filename);
                    do
                    {
                        String[] parts = sr.ReadLine().Split(',');
                        if (parts[0].Equals(propertyname) && parts.Length == 2)
                        {
                            return parts[1];
                        }
                    }
                    while (!sr.EndOfStream);
                }
                Console.WriteLine("Property {0} set to default 'XXX'", propertyname);
                return "XXX";
            }
            catch (Exception e)
            {
                Console.WriteLine("Property {0} set to default 'XXX'", propertyname);
                return "XXX";
            }
        }
    }
}