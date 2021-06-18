// This code was built using Visual Studio 2005
using System;
using System.IO;
using System.Web.Services.Protocols;
using ShipWebServiceClient.ShipServiceWebReference;

// Sample code to call the FedEx Ship Service - Express Domestic MPS
// Tested with Microsoft Visual Studio 2005 Professional Edition

namespace ShipWebServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set this to true to process a COD shipment and COD return Label.
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
                // Call the ship web service passing in a masterRequest and returning a masterReply
                ProcessShipmentReply masterReply = service.processShipment(masterRequest);
                if ((masterReply.HighestSeverity != NotificationSeverityType.ERROR) && (masterReply.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    ProcessShipmentRequest childRequest = CreateChildShipmentRequest(masterRequest, masterReply, isCodShipment);
                    //
                    try
                    {
                        // Call the ship web service passing in a childRequest and returning a childReply
                        ProcessShipmentReply childReply = service.processShipment(childRequest);
                        if ((childReply.HighestSeverity != NotificationSeverityType.ERROR) && (childReply.HighestSeverity != NotificationSeverityType.FAILURE))
                        {
                            Console.WriteLine("Master Package details\n");
                            ShowShipmentReply(isCodShipment, masterReply);
                            Console.WriteLine("\nChild Package details\n");
                            ShowShipmentReply(isCodShipment, childReply);
                            ShowMPSChildShipmentLabels(isCodShipment, masterReply, childReply);
                        }
                        else
                        {
                            Console.WriteLine(childReply.Notifications[0].Message);
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

        private static ProcessShipmentRequest CreateMasterShipmentRequest(bool isCodShipment)
        {
            // Build a masterRequest
            ProcessShipmentRequest masterRequest = new ProcessShipmentRequest();
            //
            masterRequest.WebAuthenticationDetail = SetWebAuthenticationDetail();
            //
            masterRequest.ClientDetail = new ClientDetail();
            masterRequest.ClientDetail.AccountNumber = "XXX"; // Replace "XXX" with client's account number
            masterRequest.ClientDetail.MeterNumber = "XXX"; //Replace "XXX" with client's meter number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                masterRequest.ClientDetail.AccountNumber = getProperty("accountnumber");
                masterRequest.ClientDetail.MeterNumber = getProperty("meternumber");
            }
            //
            masterRequest.TransactionDetail = new TransactionDetail();
            masterRequest.TransactionDetail.CustomerTransactionId = "***Domestic Express MPS Ship Request - Master using VC#***"; // The client will get the same value back in the response
            //
            masterRequest.Version = new VersionId();
            //
            SetShipmentDetails(masterRequest, false, null);
            //
            SetSender(masterRequest);
            //
            SetRecipient(masterRequest);
            //
            SetPayment(masterRequest);
            //
            SetLabelDetails(masterRequest);
            //
            SetPackageLineItems(masterRequest, 50.0M, "1", isCodShipment, null);
            //
            return masterRequest;
        }

        private static ProcessShipmentRequest CreateChildShipmentRequest(ProcessShipmentRequest masterRequest, ProcessShipmentReply masterReply, bool isCodShipment)
        {
            // Build a childRequest
            ProcessShipmentRequest childRequest = new ProcessShipmentRequest();
            //
            childRequest.WebAuthenticationDetail = SetWebAuthenticationDetail();
            //
            childRequest.ClientDetail = masterRequest.ClientDetail;
            //
            childRequest.TransactionDetail = new TransactionDetail();
            childRequest.TransactionDetail.CustomerTransactionId = "***Domestic Express MPS Ship Request - Child using VC#***"; // The client will get the same value back in the response
            //
            childRequest.Version = masterRequest.Version;
            //
            SetShipmentDetails(childRequest, true, masterReply);
            //
            childRequest.RequestedShipment.Shipper = masterRequest.RequestedShipment.Shipper;
            //
            childRequest.RequestedShipment.Recipient = masterRequest.RequestedShipment.Recipient;
            //
            childRequest.RequestedShipment.ShippingChargesPayment = masterRequest.RequestedShipment.ShippingChargesPayment;
            //
            childRequest.RequestedShipment.LabelSpecification = masterRequest.RequestedShipment.LabelSpecification;
            //
            SetPackageLineItems(childRequest, 30.0M, "2", isCodShipment, masterReply);
            //
            return childRequest;
        }

        private static void SetShipmentDetails(ProcessShipmentRequest request, bool isChild, ProcessShipmentReply masterReply)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Ship date and time
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.ServiceType = ServiceType.PRIORITY_OVERNIGHT; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
            //
            request.RequestedShipment.PackageCount = "2";
            if (isChild)
            {
                request.RequestedShipment.MasterTrackingId = new TrackingId(); // Master Tracking Number details
                request.RequestedShipment.MasterTrackingId.TrackingNumber = masterReply.CompletedShipmentDetail.CompletedPackageDetails[0].TrackingIds[0].TrackingNumber;
                request.RequestedShipment.MasterTrackingId.TrackingIdType = TrackingIdType.EXPRESS;
                request.RequestedShipment.MasterTrackingId.TrackingIdTypeSpecified = true;
            }
        }

        private static void SetSender(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Contact = new Contact();
            request.RequestedShipment.Shipper.Contact.PersonName = "Sender Name";
            request.RequestedShipment.Shipper.Contact.CompanyName = "Sender Company Name";
            request.RequestedShipment.Shipper.Contact.PhoneNumber = "0805522713";
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "Address Line 1" };
            request.RequestedShipment.Shipper.Address.City = "Austin";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "TX";
            request.RequestedShipment.Shipper.Address.PostalCode = "73301";
            request.RequestedShipment.Shipper.Address.CountryCode = "US";
        }

        private static void SetRecipient(ProcessShipmentRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Contact = new Contact();
            request.RequestedShipment.Recipient.Contact.PersonName = "Recipient Name";
            request.RequestedShipment.Recipient.Contact.CompanyName = "Recipient Company Name";
            request.RequestedShipment.Recipient.Contact.PhoneNumber = "9012637906";
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = "Windsor";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "CT";
            request.RequestedShipment.Recipient.Address.PostalCode = "06006";
            request.RequestedShipment.Recipient.Address.CountryCode = "US";
            request.RequestedShipment.Recipient.Address.Residential = true;
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

        private static void SetPackageLineItems(ProcessShipmentRequest request, decimal weightValue, string sequenceNumber, bool isCodShipment, ProcessShipmentReply masterReply)
        {
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
            request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = sequenceNumber;
            // Package weight information
            request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight();
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = weightValue;
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.LB;
            if (isCodShipment)
            {
                SetCOD(request);
                if (masterReply != null)
                {
                    request.RequestedShipment.SpecialServicesRequested.CodDetail.ReturnTrackingId = new TrackingId(); // Master Tracking Number details
                    request.RequestedShipment.SpecialServicesRequested.CodDetail.ReturnTrackingId.TrackingNumber = masterReply.CompletedShipmentDetail.AssociatedShipments[0].TrackingId.TrackingNumber;
                    request.RequestedShipment.SpecialServicesRequested.CodDetail.ReturnTrackingId.TrackingIdType = masterReply.CompletedShipmentDetail.AssociatedShipments[0].TrackingId.TrackingIdType;
                    request.RequestedShipment.SpecialServicesRequested.CodDetail.ReturnTrackingId.TrackingIdTypeSpecified = true;
                }
            }
        }

        private static void SetCOD(ProcessShipmentRequest request)
        {
            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested();
            request.RequestedShipment.SpecialServicesRequested.SpecialServiceTypes = new ShipmentSpecialServiceType[1];
            request.RequestedShipment.SpecialServicesRequested.SpecialServiceTypes[0] = ShipmentSpecialServiceType.COD;
            //
            request.RequestedShipment.SpecialServicesRequested.CodDetail = new CodDetail();
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CollectionType = CodCollectionType.ANY;
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount = new Money();
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount.Amount = 250.00M;
            request.RequestedShipment.SpecialServicesRequested.CodDetail.CodCollectionAmount.Currency = "USD";
        }

        private static void ShowShipmentReply(bool isCodShipment, ProcessShipmentReply reply)
        {
            // Details for each package
            foreach (CompletedPackageDetail packageDetail in reply.CompletedShipmentDetail.CompletedPackageDetails)
            {
                ShowTrackingDetails(packageDetail.TrackingIds);
                if (packageDetail.PackageRating != null && packageDetail.PackageRating.PackageRateDetails != null)
                {
                    ShowPackageRateDetails(packageDetail.PackageRating.PackageRateDetails);
                }
                ShowBarcodeDetails(packageDetail.OperationalDetail.Barcodes);
            }
            ShowPackageRouteDetails(reply.CompletedShipmentDetail.OperationalDetail);
        }

        private static void ShowMPSChildShipmentLabels(bool isCodShipment, ProcessShipmentReply masterReply, ProcessShipmentReply childReply)
        {
            // Save Master label buffer to file
            string LabelPath = "c:\\";
            if (usePropertyFile())
            {
                LabelPath = getProperty("labelpath");
            }
            string LabelFileName = LabelPath + masterReply.CompletedShipmentDetail.CompletedPackageDetails[0].TrackingIds[0].TrackingNumber + ".pdf";
            SaveLabel(LabelFileName, masterReply.CompletedShipmentDetail.CompletedPackageDetails[0].Label.Parts[0].Image);

            // Save Child label buffer to file

            LabelFileName = LabelPath + childReply.CompletedShipmentDetail.CompletedPackageDetails[0].TrackingIds[0].TrackingNumber + ".pdf";
            SaveLabel(LabelFileName, childReply.CompletedShipmentDetail.CompletedPackageDetails[0].Label.Parts[0].Image);

            if (isCodShipment)
            {
                // Save COD Return label buffer to file
                LabelFileName = LabelPath + childReply.CompletedShipmentDetail.AssociatedShipments[0].TrackingId.TrackingNumber + "CR" + ".pdf";
                SaveLabel(LabelFileName, childReply.CompletedShipmentDetail.AssociatedShipments[0].Label.Parts[0].Image);
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

        private static void SaveLabel(string labelFileName, byte[] labelBuffer)
        {
            // Save label buffer to file
            FileStream LabelFile = new FileStream(labelFileName, FileMode.Create);
            LabelFile.Write(labelBuffer, 0, labelBuffer.Length);
            LabelFile.Close();

            // Display label in Acrobat
            DisplayLabel(labelFileName);
        }

        private static void DisplayLabel(string LabelFileName)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(LabelFileName);
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