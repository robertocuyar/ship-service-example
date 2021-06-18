// This code was built using Visual Studio 2005
using System;
using System.IO;
using System.Web.Services.Protocols;
using ShipWebServiceClient.ShipServiceWebReference;

// Sample code to call the FedEx International Ground Shipping Web Service
// Tested with Microsoft Visual Studio 2005 Professional Edition

namespace ShipWebServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set this to true to process a COD shipment and print a COD return Label
            bool isCodShipment = true;
            ProcessShipmentRequest request = CreateShipmentRequest(isCodShipment);
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

        private static ProcessShipmentRequest CreateShipmentRequest(bool isCodShipment)
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
            request.TransactionDetail.CustomerTransactionId = "***Ground International Shipment Request using VC#***"; // The client will get the same value back in the response
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
            SetLabelDetails(request);
            //
            SetPackageLineItems(request, isCodShipment);
            //
            SetCustomsClearanceDetails(request);
            //
            return request;
        }
       
        private static void SetShipmentDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Ship date and time
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.ServiceType = ServiceType.FEDEX_GROUND; // Service types are FEDEX_GROUND, ...
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
            //
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "Address Line 1" };
            request.RequestedShipment.Shipper.Address.City = "Irving";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "TX";
            request.RequestedShipment.Shipper.Address.PostalCode = "75063";
            request.RequestedShipment.Shipper.Address.CountryCode = "US";
        }

        private static void SetRecipient(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Contact = new Contact();
            request.RequestedShipment.Recipient.Contact.PersonName = "Recipient Name";
            request.RequestedShipment.Recipient.Contact.CompanyName = "Recipient Company Name";
            request.RequestedShipment.Recipient.Contact.PhoneNumber = "9012637906";
            //
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = "Richmond";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "BC";
            request.RequestedShipment.Recipient.Address.PostalCode = "V7C4V4";
            request.RequestedShipment.Recipient.Address.CountryCode = "CA";
            request.RequestedShipment.Recipient.Address.Residential = false;
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
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address.CountryCode = "US";
        }

        private static void SetLabelDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification();
            request.RequestedShipment.LabelSpecification.ImageType = ShippingDocumentImageType.PDF; // Image types PDF, PNG, DPL, ...
            request.RequestedShipment.LabelSpecification.ImageTypeSpecified = true;
            request.RequestedShipment.LabelSpecification.LabelFormatType = LabelFormatType.COMMON2D;
        }

        private static void SetPackageLineItems(ProcessShipmentRequest request, bool isCodShipment)
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
            // Reference details
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences = new CustomerReference[1] { new CustomerReference() };
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE;
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].Value = "testreference";
            //
            if (isCodShipment)
                SetCOD(request);
        }

        private static void SetCOD(ProcessShipmentRequest request)
        {
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested = new PackageSpecialServicesRequested();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.SpecialServiceTypes = new PackageSpecialServiceType[1];
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.SpecialServiceTypes[0] = PackageSpecialServiceType.COD;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail = new CodDetail();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CollectionType = CodCollectionType.GUARANTEED_FUNDS;
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount = new Money();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount.Amount = 250;
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount.Currency = "CAD";
        }

        private static void SetCustomsClearanceDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor = new Payor();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.AccountNumber = "XXX"; // Replace "XXX" with the payor account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.AccountNumber = getProperty("dutiesaccount");
            }
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Contact = new Contact();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Address = new Address();
            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment.Payor.ResponsibleParty.Address.CountryCode = "CA";
            request.RequestedShipment.CustomsClearanceDetail.DocumentContent = InternationalDocumentContentType.NON_DOCUMENTS;
            //
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue = new Money();
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue.Amount = 100.0M;
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue.Currency = "USD";
            //
            SetCommodityDetails(request);
        }

        private static void SetCommodityDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.CustomsClearanceDetail.Commodities = new Commodity[1] { new Commodity() };
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].NumberOfPieces = "1";
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Description = "Books";
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CountryOfManufacture = "US";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight = new Weight();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight.Value = 1.0M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Weight.Units = WeightUnits.LB;
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].Quantity = 1.0M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].QuantitySpecified = true;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].QuantityUnits = "EA";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice = new Money();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice.Amount = 1.000000M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].UnitPrice.Currency = "USD";
            //
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue = new Money();
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue.Amount = 100.000000M;
            request.RequestedShipment.CustomsClearanceDetail.Commodities[0].CustomsValue.Currency = "USD";
        }

        private static void ShowShipmentReply(ProcessShipmentReply reply, bool isCodShipment)
        {
            Console.WriteLine("Shipment Reply details:");
            // Details for each package
            foreach (CompletedPackageDetail packageDetail in reply.CompletedShipmentDetail.CompletedPackageDetails)
            {
                ShowTrackingDetails(packageDetail.TrackingIds);
                if (null != packageDetail.PackageRating && null != packageDetail.PackageRating.PackageRateDetails)
                {
                    ShowPackageRateDetails(packageDetail.PackageRating.PackageRateDetails);
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
                    if (isCodShipment)
                    {
                        if (null != packageDetail.CodReturnDetail.Label.Parts[0].Image)
                        {
                            // Save outbound shipping label
                            LabelFileName = LabelPath + packageDetail.TrackingIds[0].TrackingNumber + "_COD.pdf";
                            SaveLabel(LabelFileName, packageDetail.CodReturnDetail.Label.Parts[0].Image);
                        }
                    }
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

        private static void ShowPackageRateDetails(PackageRateDetail[] PackageRateDetails)
        {
            Console.WriteLine("\nRate details");
            foreach (PackageRateDetail ratedPackage in PackageRateDetails)
            {
                if (ratedPackage.BillingWeight != null)
                    Console.WriteLine("Billing weight {0} {1}", ratedPackage.BillingWeight.Value, ratedPackage.BillingWeight.Units);
                if (ratedPackage.BaseCharge != null)
                    Console.WriteLine("Base charge {0} {1}", ratedPackage.BaseCharge.Amount, ratedPackage.BaseCharge.Currency);
                if (ratedPackage.TotalSurcharges != null)
                    Console.WriteLine("Total surcharge {0} {1}", ratedPackage.TotalSurcharges.Amount, ratedPackage.TotalSurcharges.Currency);
                if (ratedPackage.Surcharges != null)
                {
                    // Individual surcharge for each package
                    foreach (Surcharge surcharge in ratedPackage.Surcharges)
                        Console.WriteLine(" {0} surcharge {1} {2}", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency);
                }
                if (ratedPackage.NetCharge != null)
                    Console.WriteLine("Net charge {0} {1}", ratedPackage.NetCharge.Amount, ratedPackage.NetCharge.Currency);
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