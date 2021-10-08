using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;
using Microsoft.Graph;
using Microsoft.Identity.Client;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Volo.Abp.DependencyInjection;

namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    public class SharePointBlobProvider : BlobProviderBase, ITransientDependency
    {
        protected ISharePointBlobNameCalculator AzureBlobNameCalculator { get; }

        public SharePointBlobProvider(ISharePointBlobNameCalculator azureBlobNameCalculator)
        {
            AzureBlobNameCalculator = azureBlobNameCalculator;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var blobName = AzureBlobNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetSharePointConfiguration();
            var graphClient = await GetGraphServiceClientAsync(args, blobName);

            if (!args.OverrideExisting && await BlobExistsAsync(graphClient, args, blobName))
            {
                throw new BlobAlreadyExistsException($"Saving BLOB '{args.BlobName}' does already exists in the container '{GetContainerName(args)}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }



            //if (configuration.CreateContainerIfNotExists)
            //{
            //    await CreateContainerIfNotExists(args);
            //}
            //var path = Path.GetDirectoryName(args.BlobName);
            //var fileName = Path.GetFileName(args.BlobName);
            //await CreateFolderIfNotExists(path, graphClient);



            IDriveItemCreateUploadSessionRequest uploadSessionRequest;
            //SharePointSites

            //if (string.IsNullOrEmpty(path))
            //{
            //    uploadSessionRequest = GetDrive(graphClient).Root.ItemWithPath(fileName).CreateUploadSession().Request();
            //}
            //else
            //{
            //    //var drive = graphClient.Sites[siteId].Drives[driveId];
            //    var drive = graphClient.Sites.Root.Drive;
            //    //uploadSessionRequest = drive.Items[storeFolderId].ItemWithPath(fileName).CreateUploadSession().Request();
            //    uploadSessionRequest = drive.Items[storeFolderId].ItemWithPath(fileName).CreateUploadSession().Request();
            //}

            uploadSessionRequest = GetDrive(graphClient).Root.ItemWithPath(args.BlobName).CreateUploadSession().Request();

            var regex = new System.Text.RegularExpressions.Regex(@"/microsoft\.graph\.createUploadSession$");
            var reqUrl = uploadSessionRequest.RequestUrl;
            typeof(DriveItemCreateUploadSessionRequest)
                .GetProperty(nameof(DriveItemCreateUploadSessionRequest.RequestUrl))
                .GetSetMethod(nonPublic: true)
                .Invoke(
                    uploadSessionRequest,
                    new[]
                    {
                            regex.Replace(reqUrl, "/createUploadSession")
                    }
                );

            UploadSession uploadSession = await uploadSessionRequest.PostAsync();

            var maxChunkSize = 320 * 1024; // 320 KB - Change this to your chunk size. 5MB is the default.
            var provider = new ChunkedUploadProvider(uploadSession, graphClient, args.BlobStream, maxChunkSize);

            // Setup the chunk request necessities
            var chunkRequests = provider.GetUploadChunkRequests();
            var trackedExceptions = new List<Exception>();
            Microsoft.Graph.DriveItem itemResult = null;

            //upload the chunks
            foreach (var request in chunkRequests)
            {
                // Do your updates here: update progress bar, etc.
                // ...
                // Send chunk request
                var result = await provider.GetChunkRequestResponseAsync(request, trackedExceptions);

                if (result.UploadSucceeded)
                {
                    itemResult = result.ItemResponse;
                }
            }

            // Check that upload succeeded
            if (itemResult == null)
            {
                // Retry the upload
                // ...
            }

        }

        private IDriveRequestBuilder GetDrive(GraphServiceClient graphClient)
        {
            return graphClient.Sites.Root.Drive;
        }

        private async Task CreateFolderIfNotExists(string path, GraphServiceClient graphClient)
        {

            var drive = GetDrive(graphClient);
            var folderRequest = drive.Root.ItemWithPath(path);
            var folder = await folderRequest.Request().GetAsync();
            if (folder == null)
            {
                // create path
                foreach (var p in path.Split('/'))
                {
                    folderRequest = drive.Root.ItemWithPath(p);
                    folder = await folderRequest.Request().GetAsync();
                    if (folder == null)
                    {
                        await drive.Root.Children.Request().AddAsync(new DriveItem()
                        {
                            Name = p,
                        });
                    }
                }
            }
        }

        public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var blobName = AzureBlobNameCalculator.Calculate(args);

            var graphClient = await GetGraphServiceClientAsync(args, blobName);

            if (await BlobExistsAsync(graphClient, args, blobName))
            {
                await GetDrive(graphClient).Root.ItemWithPath(args.BlobName)
                    .Request()
                    .DeleteAsync();
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var blobName = AzureBlobNameCalculator.Calculate(args);
            var graphClient = await GetGraphServiceClientAsync(args, blobName);

            return await BlobExistsAsync(graphClient, args, blobName);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var blobName = AzureBlobNameCalculator.Calculate(args);
            var graphClient = await GetGraphServiceClientAsync(args, blobName);
            //var item = await GetDrive(graphClient).Root.ItemWithPath(args.BlobName)
            //    .Request()
            //    .GetAsync();

            var filestream = await GetDrive(graphClient).Root.ItemWithPath(args.BlobName)
                            .Content.Request().GetAsync();

            var memoryStream = new MemoryStream();
            await filestream.CopyToAsync(memoryStream);
            return memoryStream;
        }

        public override async Task<List<string>> GetListAsync(BlobProviderGetArgs args)
        {
            var blobName = AzureBlobNameCalculator.Calculate(args);
            var graphClient = await GetGraphServiceClientAsync(args, blobName);

            var children = await graphClient.Sites["root"].Drive.Root.Children
                .Request()
                .GetAsync();

            return children.Select(f => GetFullPath(f.ParentReference.Path, f.Name)).ToList();
        }

        private string GetFullPath(string path, string name)
        {
            return path.Substring(path.IndexOf(':')) + "/" + name;
        }

        protected virtual async Task<GraphServiceClient> GetGraphServiceClientAsync(BlobProviderArgs args, string blobName)
        {
            var accessToken = await GetTokenAsync(args);

            GraphServiceClient graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        // Set bearer authentication on header
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                    }));

            return graphClient;
        }

        //protected virtual async Task CreateContainerIfNotExists(BlobProviderArgs args)
        //{
        //    var blobContainerClient = GetBlobContainerClient(args);
        //    await blobContainerClient.CreateIfNotExistsAsync();
        //}

        private async Task<bool> BlobExistsAsync(GraphServiceClient graphClient, BlobProviderArgs args, string blobName)
        {
            // Make sure Blob Container exists.
            //return await ContainerExistsAsync(GetBlobContainerClient(args)) &&
            //(await GetBlobClientAsync(args, blobName).ExistsAsync()).Value;

            var item = await GetDrive(graphClient).Root.ItemWithPath(args.BlobName)
               .Request()
               .GetAsync();

            return item != null;
        }

        private static string GetContainerName(BlobProviderArgs args)
        {
            var configuration = args.Configuration.GetSharePointConfiguration();
            return configuration.ContainerName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : configuration.ContainerName;
        }

        //private static async Task<bool> ContainerExistsAsync(BlobContainerClient blobContainerClient)
        //{
        //    return (await blobContainerClient.ExistsAsync()).Value;
        //}


        private async Task<string> GetTokenAsync(BlobProviderArgs args)
        {

            var configuration = args.Configuration.GetSharePointConfiguration();
            //AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            // You can run this sample using ClientSecret or Certificate. The code will differ only when instantiating the IConfidentialClientApplication
            //bool isUsingClientSecret = AppUsesClientSecret(config);

            // Even if this is a console application here, a daemon application is a confidential client application
            IConfidentialClientApplication app;

            //if (isUsingClientSecret)
            {
                app = ConfidentialClientApplicationBuilder.Create(configuration.ClientId)
                    .WithClientSecret(configuration.ClientSecret)
                    .WithAuthority(new Uri(configuration.Authority))
                    .Build();
            }

            //else
            //{
            //X509Certificate2 certificate = ReadCertificate(args.Configuration.CertificateName);
            //app = ConfidentialClientApplicationBuilder.Create(args.Configuration.ClientId)
            //    .WithCertificate(certificate)
            //    .WithAuthority(new Uri(args.Configuration.Authority))
            //    .Build();
            //}

            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator. 
            string[] scopes = new string[] { $"{configuration.ApiUrl}.default" };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();

            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected

                throw new Exception("Scope provided is not supported", ex);
            }
            return result.AccessToken;
        }

        //private static X509Certificate2 ReadCertificate(string certificateName)
        //{
        //    if (string.IsNullOrWhiteSpace(certificateName))
        //    {
        //        throw new ArgumentException("certificateName should not be empty. Please set the CertificateName setting in the appsettings.json", "certificateName");
        //    }
        //    CertificateDescription certificateDescription = CertificateDescription.FromStoreWithDistinguishedName(certificateName);
        //    DefaultCertificateLoader defaultCertificateLoader = new DefaultCertificateLoader();
        //    defaultCertificateLoader.LoadIfNeeded(certificateDescription);
        //    return certificateDescription.Certificate;
        //}
    }
}
