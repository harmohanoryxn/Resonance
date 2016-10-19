using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Packaging;
using Cloudmaster.WCS.Classes;
using System.IO;

using System.Xml;
using System.Xml.Serialization;
using Cloudmaster.WCS.IO;

namespace Cloudmaster.WCS.Packaging
{
    public class FormInstancePackager : IPackageable<FormInstance>
    {
        private static string documentPath = @"FormInstances/FormInstance.xml";

        private const string PackageRelationshipType =
            @"http://schemas.slowtrain.ie/2010/ISM/site";

        private const string ImageRelationshipType =
            @"http://schemas.slowtrain.ie/2010/ISM/image";

        private static Uri SitePartUri;

        static FormInstancePackager()
        {
            SitePartUri = PackUriHelper.CreatePartUri(
                                      new Uri(documentPath, UriKind.Relative));
        }

        #region Package

        public void Package(FormInstance formInstance, string filename)
        {
            using (Package package = System.IO.Packaging.Package.Open(filename, FileMode.Create))
            {
                PackagePart packagePartSite = package.CreatePart(SitePartUri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

                // Add Images Files To Package

                AddImagesToPackage(formInstance, package, packagePartSite);

                // Add Signature To Package

                if (formInstance.Signature != null)
                {
                    string imagePath = string.Format(@"Images/Signature.jpg");

                    AddImageToPackage(package, packagePartSite, formInstance.Signature, imagePath);
                }

                // Write Xml To Package

                using (Stream stream = packagePartSite.GetStream())
                {
                    XmlTypeSerializer<FormInstance>.SerializeAndOverwriteFile(formInstance, stream);
                }

                package.CreateRelationship(packagePartSite.Uri, TargetMode.Internal, PackageRelationshipType);
            }
        }

        private static void AddImagesToPackage(FormInstance formInstance, Package package, PackagePart packagePartSite)
        {
            int imageCount = 1;

            foreach (Section section in formInstance.Sections)
            {
                foreach (Check check in section.Checks)
                {
                    foreach (RelatedFile relatedFile in check.UserImages)
                    {
                        string imagePath = string.Format(@"Images/Image{0}.jpg", imageCount);

                        bool successfullyStoredImage = AddImageToPackage(package, packagePartSite, relatedFile, imagePath);

                        if (successfullyStoredImage)
                        {
                            imageCount += 1;
                        }
                    }
                }
            }
        }

        private static bool AddImageToPackage(Package package, PackagePart packagePartSite, RelatedFile relatedFile, string imagePath)
        {
            bool result = false;

            try
            {
                Uri partUriImage = PackUriHelper.CreatePartUri(new Uri(imagePath, UriKind.Relative));

                PackagePart packagePartImage = package.CreatePart(partUriImage, System.Net.Mime.MediaTypeNames.Image.Jpeg);

                using (FileStream fileStream = new FileStream(relatedFile.LocalFilename, FileMode.Open, FileAccess.Read))
                {
                    CopyStream(fileStream, packagePartImage.GetStream());
                }

                PackageRelationship relationship = packagePartSite.CreateRelationship(new Uri(string.Format(@"../{0}", imagePath),
                    UriKind.Relative),
                    TargetMode.Internal,
                    ImageRelationshipType);

                relatedFile.StorageFilename = relationship.TargetUri.OriginalString;

                result = true;
            }
            catch (Exception) { }

            return result;
        }

        #endregion

        #region Unpackage

        public FormInstance Unpackage(string filename)
        {
            FormInstance formInstance = null;

            using (Package package = System.IO.Packaging.Package.Open(filename))
            {
                PackagePart packagePartSite = package.GetPart(SitePartUri);

                PackageRelationshipCollection relationships = packagePartSite.GetRelationships();

                formInstance = XmlTypeSerializer<FormInstance>.Deserialize(packagePartSite.GetStream());

                RestoreImagesFromPackage(formInstance, package, relationships);
            }

            return formInstance;
        }

        private static void RestoreImagesFromPackage(FormInstance formInstance, Package package, PackageRelationshipCollection relationships)
        {
            foreach (Section section in formInstance.Sections)
            {
                foreach (Check check in section.Checks)
                {
                    foreach (RelatedFile relatedFile in check.UserImages)
                    {
                        RestoreImageFromPackage(package, relationships, relatedFile);
                    }
                }
            }

            RestoreImageFromPackage(package, relationships, formInstance.Signature);
        }

        private static void RestoreImageFromPackage(Package package, PackageRelationshipCollection relationships, RelatedFile relatedFile)
        {
            try
            {
                PackageRelationship packageRelationship = relationships.Single(relationship => relationship.TargetUri.OriginalString == relatedFile.StorageFilename);

                if (packageRelationship != null)
                {
                    string temporaryFilename = Path.GetTempFileName();

                    Uri relationshipUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), packageRelationship.TargetUri);

                    PackagePart relatedFilePackagePart = package.GetPart(relationshipUri);

                    using (FileStream fileStream = new FileStream(temporaryFilename, FileMode.Create))
                    {
                        CopyStream(relatedFilePackagePart.GetStream(), fileStream);
                    }

                    relatedFile.LocalFilename = temporaryFilename;
                }
            }
            catch (Exception) { }
        }

#endregion

        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

    }
}

