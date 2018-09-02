using System;
using Amazon.DynamoDBv2.DocumentModel;
using epm_api.Enums;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Xunit;

namespace epmApiTests
{
    public class DynamoDbServiceTests
    {
        private readonly IDynamoDbService _dynamoDbService;
        public DynamoDbServiceTests()
        {
            this._dynamoDbService = new DynamoDbService();
        }

        [Fact]
        public async void CreateAUser()
        {
            Document user = new Document
            {
                ["EmailAddress"] = "hello@hotmail.co.uk",
                ["Password"] = "boby123891011"
            };

            // Create a condition expression for the optional conditional put operation.
            Expression expr = new Expression
            {
                ExpressionStatement = "EmailAddress != :pk",
                ExpressionAttributeValues = { [":pk"] = "hello@hotmail.co.uk" }
            };

            PutItemOperationConfig config = new PutItemOperationConfig()
            {
                ConditionalExpression = expr
            };

            await this._dynamoDbService.PutItemAsync(DynamoDbTablesEnum.Users, user, config);
        }

        [Fact]
        public async void GetUserFromEmail()
        {
            Document doc = await this._dynamoDbService.GetItemAsync(DynamoDbTablesEnum.Users, "hell3o@hotmail.co.uk");
            Assert.True(true);
        }
    }
}
