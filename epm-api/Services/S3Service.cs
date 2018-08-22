using Amazon.S3;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketName = "ethereumpackagemanager";

        // will keep injection in for now - will remove if it stays like this forever 
        public S3Service(IAmazonS3 client)
        {
            // this._client = client.Config;
            // as aws seem to not bring my region back even though it is in the config
            // we have to create a new instance of amazons3client :(
            this._client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
        }

        public IAmazonS3 GetClient()
        {
            return this._client;
        }

        public string GetBucketName()
        {
            return this._bucketName;
        }
    }
}
