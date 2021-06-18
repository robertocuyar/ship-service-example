// This code was built using Visual Studio 2017
using System;
using System.IO;
using System.Web.Services.Protocols;
using ShipWebServiceClient.ShipServiceWebReference;

// Sample code to call the FedEx Ground Domestic MPS Shipping Web Service
// Tested with Microsoft Visual Studio 2017 Professional Edition

namespace ShipWebServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set this to true to process a COD shipment and COD return Label
            bool isCodShipment = true;
            //
            ProcessShipmentRequest masterRequest = CreateMasterShipmentRequest(isCodShipment);
            //
            ShipService service = new ShipService();
			if (usePropertyFile())
            {
                service.Url = getProperty("endpoint");
            }
            //
            try
            {
                // Call the ship web service passing in a master shipment request and returning a master shipment reply
                ProcessShipmentReply masterReply = service.processShipment(masterRequest);
                //
                if ((masterReply.HighestSeverity != NotificationSeverityType.ERROR) && (masterReply.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    ProcessShipmentRequest childRequest = CreateChildShipmentRequest(masterReply, isCodShipment);
                    //
                    // Call the ship web service passing in a child shipment request and returning a child shipment reply
                    ProcessShipmentReply childReply = service.processShipment(childRequest);
                    //
                    if ((childReply.HighestSeverity != NotificationSeverityType.ERROR) && (childReply.HighestSeverity != NotificationSeverityType.FAILURE))
                    {
                        Console.WriteLine("Master Package details\n");
                        ShowShipmentReply(isCodShipment, masterReply);
                        Console.WriteLine("\nChild Package details\n");
                        ShowShipmentReply(isCodShipment, childReply);
                    }
                    else
                    {
                        Console.WriteLine(childReply.Notifications[0].Message);
                    }
                }
                else
                {
                    Console.WriteLine(masterReply.Notifications[0].Message);
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
            Console.WriteLine("Press any key to quit !");
            Console.ReadKey();
        }

        private static ProcessShipmentRequest CreateMasterShipmentRequest(bool isCodShipment)
        {
            // Build the Master ShipmentRequest
            ProcessShipmentRequest masterRequest = new ProcessShipmentRequest();
            //
            SetShipmentDetails(masterRequest, "***Ground Domestic MPS v13 Shipment Request - Master using VC#***");
            //
            SetPackageLineItems(masterRequest, "1", isCodShipment);
            //
            return masterRequest;
        }

        private static ProcessShipmentRequest CreateChildShipmentRequest(ProcessShipmentReply masterReply, bool isCodShipment)
        {
            // Build the Child ShipmentRequest
            ProcessShipmentRequest childRequest = new ProcessShipmentRequest();
            //
            SetShipmentDetails(childRequest, "***Ground Domestic MPS Shipment Request - Child using VC# ***");
            //
            childRequest.RequestedShipment.MasterTrackingId = new TrackingId();
            childRequest.RequestedShipment.MasterTrackingId.TrackingNumber = masterReply.CompletedShipmentDetail.CompletedPackageDetails[0].TrackingIds[0].TrackingNumber;
            childRequest.RequestedShipment.MasterTrackingId.TrackingIdType = TrackingIdType.GROUND;
            childRequest.RequestedShipment.MasterTrackingId.TrackingIdTypeSpecified = true;
            //
            SetPackageLineItems(childRequest, "2", isCodShipment);
            //
            return childRequest;
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

        private static void SetShipmentDetails(ProcessShipmentRequest request, string transactionId)
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
            request.TransactionDetail.CustomerTransactionId = transactionId;
            //
            request.Version = new VersionId();
            //
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now;
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.ServiceType = "FEDEX_GROUND"; // Service types are FEDEX_GROUND ...
            request.RequestedShipment.PackagingType = "YOUR_PACKAGING"; // Packaging type YOUR_PACKAGING, ...
            request.RequestedShipment.PackageCount = "2"; // Number of packages
            //
            SetSender(request);
            //
            SetRecipient(request);
            //
            SetPayment(request);
            //
            SetLabelDetails(request);
        }

        private static void SetSender(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Contact = new Contact();
            request.RequestedShipment.Shipper.Contact.PersonName = "Shipper Name";
            request.RequestedShipment.Shipper.Contact.CompanyName = "Shipper Company Name";
            request.RequestedShipment.Shipper.Contact.PhoneNumber = "9012639431";
            //
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "Address Line 1" };
            request.RequestedShipment.Shipper.Address.City = "Memphis";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "TN";
            request.RequestedShipment.Shipper.Address.PostalCode = "38119";
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
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { " Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = "Barre";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "MA";
            request.RequestedShipment.Recipient.Address.PostalCode = "01006";
            request.RequestedShipment.Recipient.Address.CountryCode = "US";
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
            request.RequestedShipment.LabelSpecification.ImageType = ShippingDocumentImageType.PDF;
            request.RequestedShipment.LabelSpecification.ImageTypeSpecified = true;
            request.RequestedShipment.LabelSpecification.LabelFormatType = LabelFormatType.COMMON2D;
        }

        private static void SetPackageLineItems(ProcessShipmentRequest request, string sequenceNumber, bool isCodShipment)
        {
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
            //
            request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight();
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = 40.0M;
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.LB;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions();
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "12";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "13";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "14";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = sequenceNumber;
            // Customer reference information
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences = new CustomerReference[3] { new CustomerReference(), new CustomerReference(), new CustomerReference() };
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE;
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[0].Value = "CR345678";
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[1].CustomerReferenceType = CustomerReferenceType.P_O_NUMBER;
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[1].Value = "PO345678";
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[2].CustomerReferenceType = CustomerReferenceType.INVOICE_NUMBER;
            request.RequestedShipment.RequestedPackageLineItems[0].CustomerReferences[2].Value = "INV345678";
            //
            if (isCodShipment)
            {
                SetCOD(request);
            }
        }

        private static void SetCOD(ProcessShipmentRequest request)
        {
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested = new PackageSpecialServicesRequested();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.SpecialServiceTypes = new String[1];
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.SpecialServiceTypes[0] = "COD";
            //
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail = new CodDetail();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CollectionType = CodCollectionType.GUARANTEED_FUNDS;
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount = new Money();
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount.Amount = 250.00M;
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail.CodCollectionAmount.Currency = "USD";
        }

        private static void ShowShipmentReply(bool isCodShipment, ProcessShipmentReply reply)
        {
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
                ShowBarcodeDetails(packageDetail.OperationalDetail.Barcodes);
                ShowMPSChildShipmentLabels(isCodShipment, reply, packageDetail);
            }
            ShowPackageRouteDetails(reply.CompletedShipmentDetail.OperationalDetail);
        }

        private static void ShowMPSChildShipmentLabels(bool isCodShipment, ProcessShipmentReply reply, CompletedPackageDetail packageDetail)
        {
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
                // Save COD label
                LabelFileName = LabelPath + packageDetail.TrackingIds[0].TrackingNumber + "CR.pdf";
                if (isCodShipment) SaveLabel(LabelFileName, reply.CompletedShipmentDetail.CompletedPackageDetails[0].CodReturnDetail.Label.Parts[0].Image);
            }
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
            foreach (PackageRateDetail ratedPackage in PackageRateDetails)
            {
                Console.WriteLine("\nRate details");
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

        private static void SaveLabel( string labelFileName, byte[] labelBuffer)
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
        private static bool usePropertyFile() //Set to true for common properties to be set with getProperty function.
        {
            return getProperty("usefile").Equals("True");
        }
        private static String getProperty(String propertyname) //Sets common properties for testing purposes.
        {
            try
            {
                String filename = "D:\\CS_SHGW_Properties.txt";
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