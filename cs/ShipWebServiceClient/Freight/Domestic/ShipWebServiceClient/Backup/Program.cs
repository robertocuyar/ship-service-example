// This code was built using Visual Studio 2005
using System;
using System.IO;
using ShipWebServiceClient.ShipServiceWebReference;
using System.Web.Services.Protocols;

// Sample code to call the FedEx Freight Domestic Ship Service
// Tested with Microsoft Visual Studio 2005 Professional Edition

namespace ShipWebServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
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
                // Call the ship web service passing in a ProcessShipmentRequest and returning a ProcessShipmentReply
                ProcessShipmentReply reply = service.processShipment(request);
                //
                if ((reply.HighestSeverity != NotificationSeverityType.ERROR) && (reply.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    ShowShipmentReply(reply);
                }
                else
                {
                    ShowNotifications(reply);
                }
            }
            catch (SoapException e)
            {
                Console.WriteLine(e.Detail.InnerText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press any key to quit!");
            Console.ReadKey();
        }

        private static ProcessShipmentRequest CreateShipmentRequest()
        {
            // Build the ShipmentRequest
            ProcessShipmentRequest request = new ProcessShipmentRequest();
            //
            SetShipmentDetails(request);
            //
            return request;
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

        private static void SetShipmentDetails(ProcessShipmentRequest request)
        {
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
            request.TransactionDetail.CustomerTransactionId = "***Freight Domestic Ship Request using VC#***"; // The client will get the same value back in the response
            //
            request.Version = new VersionId();
            //
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Ship date and time
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.ServiceType = ServiceType.FEDEX_FREIGHT_PRIORITY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, ...
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
            //
            SetSender(request);
            //
            SetRecipient(request);
            //
            SetPayment(request);
            //
            SetFreightShipmentDetail(request);
            //
            request.RequestedShipment.DeliveryInstructions = "FreightDeliveryInstructions";
            //
            SetLabelDetails(request);
            //
            SetShippingDocumentDetails(request);
            //
            request.RequestedShipment.PackageCount = "1";
        }

        private static void SetSender(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Contact = new Contact();
            request.RequestedShipment.Shipper.Contact.PersonName = "Shipper Contact";
            request.RequestedShipment.Shipper.Contact.CompanyName = "Shipper Company";
            request.RequestedShipment.Shipper.Contact.PhoneNumber = "1234567890";
            //
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "SHIPPER ADDRESS LINE 1" };
            request.RequestedShipment.Shipper.Address.City = "Harrison";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "AR";
            request.RequestedShipment.Shipper.Address.PostalCode = "72601";
            request.RequestedShipment.Shipper.Address.CountryCode = "US";
        }

        private static void SetRecipient(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Contact = new Contact();
            request.RequestedShipment.Recipient.Contact.PersonName = "Recipient Contact";
            request.RequestedShipment.Recipient.Contact.CompanyName = "Recipient Company";
            request.RequestedShipment.Recipient.Contact.PhoneNumber = "1234567890";
            //
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "RECIPIENT ADDRESS LINE 1" };
            request.RequestedShipment.Recipient.Address.City = "COLORADO SPRINGS";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "CO";
            request.RequestedShipment.Recipient.Address.PostalCode = "80915";
            request.RequestedShipment.Recipient.Address.CountryCode = "US";
        }

        private static void SetPayment(ProcessShipmentRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.RECIPIENT;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = "XXX"; // Replace "XXX" with the client's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = getProperty("payoraccount");
            }
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Contact = new Contact();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address = new Address();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address.CountryCode = "US";
        }

        private static void SetFreightShipmentDetail(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail = new FreightShipmentDetail();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightAccountNumber = "XXX"; // Replace "XXX" with the client's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.FreightShipmentDetail.FedExFreightAccountNumber = getProperty("freightaccount");
            }
            SetFreightBillingContactAddress(request);
            SetFreightPrintedReferences(request);
            request.RequestedShipment.FreightShipmentDetail.Role = FreightShipmentRoleType.SHIPPER;
            request.RequestedShipment.FreightShipmentDetail.RoleSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.CollectTermsType = FreightCollectTermsType.STANDARD;
            request.RequestedShipment.FreightShipmentDetail.CollectTermsTypeSpecified = true;
            SetFreightDeclaredValue(request);
            SetFreightLiabilityCoverageDetail(request);
            request.RequestedShipment.FreightShipmentDetail.TotalHandlingUnits = "15";
            SetFreightShipmentDimensions(request);
            SetFreightShipmentLineItems(request);
        }

        private static void SetFreightBillingContactAddress(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress = new ContactAndAddress();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact = new Contact();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.PersonName = "Freight Billing Contact";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.CompanyName = "Freight Billing Company";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.PhoneNumber = "1234567890";
            //
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address = new Address();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StreetLines = new string[1] { "FREIGHT SHIPPER ADDRESS LINE 1" };
            // Replace "XXX" with Freight billing address
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.City = "Harrison";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StateOrProvinceCode = "AR";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.PostalCode = "72601";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.CountryCode = "US";
        }

        private static void SetFreightPrintedReferences(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.PrintedReferences = new PrintedReference[1];
            request.RequestedShipment.FreightShipmentDetail.PrintedReferences[0] = new PrintedReference();
            request.RequestedShipment.FreightShipmentDetail.PrintedReferences[0].Type = PrintedReferenceType.SHIPPER_ID_NUMBER;
            request.RequestedShipment.FreightShipmentDetail.PrintedReferences[0].TypeSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.PrintedReferences[0].Value = "SHIPPERIDNUMBER";
        }

        private static void SetFreightDeclaredValue(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit = new Money();
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit.Currency = "USD";
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit.Amount = 50.0M;
        }

        private static void SetFreightLiabilityCoverageDetail(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail = new LiabilityCoverageDetail();
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageType = LiabilityCoverageType.NEW;
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageTypeSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount = new Money();
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount.Currency = "USD";
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount.Amount = 50.0M;
        }

        private static void SetFreightShipmentDimensions(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions = new Dimensions();
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Length = "180";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Width = "93";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Height = "106";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Units = LinearUnits.IN;
        }

        private static void SetFreightShipmentLineItems(ProcessShipmentRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.LineItems = new FreightShipmentLineItem[1];
            request.RequestedShipment.FreightShipmentDetail.LineItems[0] = new FreightShipmentLineItem();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClass = FreightClassType.CLASS_050;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClassSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].ClassProvidedByCustomer = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].ClassProvidedByCustomerSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].HandlingUnits = "15";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Packaging = PhysicalPackagingType.BOX;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].PackagingSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Pieces = "15";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].PurchaseOrderNumber = "PurchaseOrderNumber";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Description = "Freight line item description";
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight = new Weight();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Units = WeightUnits.LB;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Value = 1000.0M;
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions = new Dimensions();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Length = "180";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Width = "93";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Height = "106";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Units = LinearUnits.IN;
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume = new Volume();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.Units = VolumeUnits.CUBIC_FT;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.UnitsSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.Value = 30M;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.ValueSpecified = true;
        }

        private static void SetLabelDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification();
            request.RequestedShipment.LabelSpecification.LabelFormatType = LabelFormatType.FEDEX_FREIGHT_STRAIGHT_BILL_OF_LADING;
            request.RequestedShipment.LabelSpecification.ImageType = ShippingDocumentImageType.PDF; // Use this line for a PDF label
            request.RequestedShipment.LabelSpecification.ImageTypeSpecified = true;
            request.RequestedShipment.LabelSpecification.LabelStockType = LabelStockType.PAPER_LETTER;
            request.RequestedShipment.LabelSpecification.LabelStockTypeSpecified = true;
            request.RequestedShipment.LabelSpecification.LabelPrintingOrientation = LabelPrintingOrientationType.TOP_EDGE_OF_TEXT_FIRST;
            request.RequestedShipment.LabelSpecification.LabelPrintingOrientationSpecified = true;
        }

        private static void SetShippingDocumentDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.ShippingDocumentSpecification = new ShippingDocumentSpecification();
            request.RequestedShipment.ShippingDocumentSpecification.ShippingDocumentTypes = new RequestedShippingDocumentType[1];
            request.RequestedShipment.ShippingDocumentSpecification.ShippingDocumentTypes[0] = RequestedShippingDocumentType.FREIGHT_ADDRESS_LABEL;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail = new FreightAddressLabelDetail();
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format = new ShippingDocumentFormat();
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.ImageType = ShippingDocumentImageType.PDF;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.ImageTypeSpecified = true;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.StockType = ShippingDocumentStockType.PAPER_4X6;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.StockTypeSpecified = true;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.ProvideInstructions = true;
            request.RequestedShipment.ShippingDocumentSpecification.FreightAddressLabelDetail.Format.ProvideInstructionsSpecified = true;
        }

        private static void ShowShipmentReply(ProcessShipmentReply reply)
        {
            Console.WriteLine("Shipment Reply details:");
            ShowTrackingDetails(reply.CompletedShipmentDetail.MasterTrackingId);
            if (null != reply.CompletedShipmentDetail)
            {
                ShowShipmentRateDetails(reply.CompletedShipmentDetail);
            }
            else
            {
                Console.WriteLine("No rate information returned.");
            }
            //
            Console.WriteLine("Shipment details:");
            ShowShipmentLabels(reply.CompletedShipmentDetail.ShipmentDocuments);
            //
            Console.WriteLine();
            ShowNotifications(reply);
        }

        private static void ShowTrackingDetails(TrackingId MasterTrackingId)
        {
            if (MasterTrackingId == null) return;
            // Tracking information for the package
            Console.WriteLine("Tracking details");
            Console.Write("Tracking # " + MasterTrackingId.TrackingNumber);
            if (MasterTrackingId.TrackingIdTypeSpecified)
            {
                Console.Write(" Track ID Type " + MasterTrackingId.TrackingIdType + "\n");
            }
        }

        private static void ShowShipmentRateDetails(CompletedShipmentDetail replyShipmentDetail)
        {
            if (replyShipmentDetail == null) return;
            if (replyShipmentDetail.ShipmentRating == null) return;
            if (replyShipmentDetail.ShipmentRating.ShipmentRateDetails == null) return;
            foreach (ShipmentRateDetail ratedShipment in replyShipmentDetail.ShipmentRating.ShipmentRateDetails)
            {
                Console.WriteLine();
                Console.WriteLine("Shipment Rate details");
                if (ratedShipment.TotalBillingWeight != null)
                    Console.WriteLine("Total Billing weight: {0} {1}", ratedShipment.TotalBillingWeight.Value, ratedShipment.TotalBillingWeight.Units);
                if (ratedShipment.TotalBaseCharge != null)
                    Console.WriteLine("Total Base charge: {0} {1}", ratedShipment.TotalBaseCharge.Amount, ratedShipment.TotalBaseCharge.Currency);
                if (ratedShipment.TotalFreightDiscounts != null)
                    Console.WriteLine("Total Freight Discounts: {0} {1}", ratedShipment.TotalFreightDiscounts.Amount, ratedShipment.TotalFreightDiscounts.Currency);
                if (ratedShipment.TotalSurcharges != null)
                    Console.WriteLine("Total surcharges: {0} {1}", ratedShipment.TotalSurcharges.Amount, ratedShipment.TotalSurcharges.Currency);
                if (ratedShipment.Surcharges != null)
                {
                    // Individual surcharge for each package
                    foreach (Surcharge surcharge in ratedShipment.Surcharges)
                        Console.WriteLine(" {0} surcharge {1} {2}", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency);
                }
                if (ratedShipment.TotalNetCharge != null)
                    Console.WriteLine("Total Net Charge: {0} {1}", ratedShipment.TotalNetCharge.Amount, ratedShipment.TotalNetCharge.Currency);
                ShowFreightRateDetail(ratedShipment.FreightRateDetail);
            }
        }

        private static void ShowFreightRateDetail(FreightRateDetail freightRateDetail)
        {
            if (freightRateDetail == null) return;
            Console.WriteLine();
            Console.WriteLine("Freight Rate details");
            if (freightRateDetail.QuoteNumber != null)
                Console.WriteLine("Quote number {0} ", freightRateDetail.QuoteNumber);

            // Individual FreightBaseCharge for each shipment
            foreach (FreightBaseCharge freightBaseCharge in freightRateDetail.BaseCharges)
            {
                if (freightBaseCharge.Description != null)
                    Console.WriteLine("Description " + freightBaseCharge.Description);
                if (freightBaseCharge.Weight != null)
                    Console.WriteLine("Weight {0} {1} ", freightBaseCharge.Weight.Value, freightBaseCharge.Weight.Units);
                if (freightBaseCharge.ChargeRate != null)
                    Console.WriteLine("Charge rate {0} {1} ", freightBaseCharge.ChargeRate.Amount, freightBaseCharge.ChargeRate.Currency);
                if (freightBaseCharge.ExtendedAmount != null)
                    Console.WriteLine("Extended amount {0} {1} ", freightBaseCharge.ExtendedAmount.Amount, freightBaseCharge.ExtendedAmount.Currency);
                Console.WriteLine();
            }
        }

        private static void ShowShipmentLabels(ShippingDocument[] shipmentDocuments)
        {
            if (shipmentDocuments == null) return;
            Console.WriteLine();
            Console.WriteLine("ShippingDocument details");
            foreach (ShippingDocument shipmentDocument in shipmentDocuments)
            {
                if (shipmentDocument.TypeSpecified)
                {
                    Console.WriteLine("Shipping Document Type {0}", shipmentDocument.Type);
                }
                if (shipmentDocument.ShippingDocumentDispositionSpecified)
                {
                    Console.WriteLine("Shipping Document Disposition Type {0}", shipmentDocument.ShippingDocumentDisposition);
                }

                foreach (ShippingDocumentPart part in shipmentDocument.Parts)
                {
                    // Save each shipping document image.
                    string LabelPath = "c:\\";
                    if (usePropertyFile())
                    {
                        LabelPath = getProperty("labelpath");
                    }
                    string labelFileName = LabelPath + shipmentDocument.Type + ".pdf";
                    SaveLabel(labelFileName, part.Image);
                }
                Console.WriteLine();
            }
        }

        private static void SaveLabel(string labelFileName, byte[] labelBuffer)
        {
            // Save label buffer to file
            FileStream labelFile = new FileStream(labelFileName, FileMode.Create);
            labelFile.Write(labelBuffer, 0, labelBuffer.Length);
            labelFile.Close();

            // Display label in Acrobat
            DisplayLabel(labelFileName);
        }

        private static void DisplayLabel(string labelFileName)
        {
            Console.WriteLine("Label file name {0}", labelFileName);
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