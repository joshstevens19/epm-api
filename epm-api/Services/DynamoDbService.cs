using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using epm_api.Entities;
using epm_api.Enums;
using epm_api.Extentions;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class DynamoDbService : IDynamoDbService
    {
        private readonly IAmazonDynamoDB _client;
        private readonly DynamoDBContext _context;

        public DynamoDbService()
        {
            this._client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            // may use context instead as per - https://aws.amazon.com/blogs/developer/dynamodb-apis/
            // may be better working with proper types 
            this._context = new DynamoDBContext(this._client);
        }

        public IAmazonDynamoDB GetClient()
        {
            return this._client;
        }

        /// <summary>
        /// Loads the table
        /// </summary>
        /// <param name="table">The table which needs to be loaded</param>
        /// <returns></returns>
        private Table LoadTable(DynamoDbTablesEnum table)
        {
            return Table.LoadTable(this.GetClient(), table.DisplayName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> PutItemAsync<T>(T entity)
        {
            await this._context.SaveAsync(entity);
            return entity;
        }

        public async Task<Document> PutItemAsync(DynamoDbTablesEnum dynamoDbtable,
                                                 Document document)
        {
            return await this.PutItemAsync(dynamoDbtable, document, null);
        }

        public async Task<Document> PutItemAsync(DynamoDbTablesEnum dynamoDbtable,
                                                 Document document,
                                                 PutItemOperationConfig config)
        {
            Table table = this.LoadTable(dynamoDbtable);
            return await table.PutItemAsync(document, null);
        }

        public async Task<T> GetItemAsync<T>(string primaryKey)
        {
            return await this._context.LoadAsync<T>(primaryKey);
        }

        public async Task<Document> GetItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                 Primitive primaryKey)
        {
            return await this.GetItemAsync(dynamoDbTable, primaryKey, null);
        }

        public async Task<Document> GetItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                 Primitive primaryKey,
                                                 GetItemOperationConfig config)
        {
            Table table = this.LoadTable(dynamoDbTable);
            return await table.GetItemAsync(primaryKey, config);
        }

        public async Task DeleteItemAsync<T>(string primaryKey)
        {
            await this._context.DeleteAsync<T>(primaryKey);
        }

        public async Task<Document> DeleteItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                    Primitive primaryKey)
        {
            return await this.DeleteItemAsync(dynamoDbTable, primaryKey, null);
        }

        public async Task<Document> DeleteItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                    Primitive primaryKey,
                                                    DeleteItemOperationConfig config)
        {
            Table table = this.LoadTable(dynamoDbTable);
            return await table.DeleteItemAsync(primaryKey, config);
        }

        public async Task<Document> UpdateItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                    Document document)
        {
            return await this.UpdateItemAsync(dynamoDbTable, document, null);
        }

        public async Task<Document> UpdateItemAsync(DynamoDbTablesEnum dynamoDbTable,
                                                    Document document,
                                                    UpdateItemOperationConfig config)
        {
            Table table = this.LoadTable(dynamoDbTable);
            return await table.UpdateItemAsync(document, config);
        }

        // examples to reference if i need it 

        //public async void PushDataIntoTableTest()
        //{
        //    Table table = Table.LoadTable(this._client, "PackageDetails");

        //    var book = new Document();
        //    book["Id"] = 101;
        //    book["Title"] = "Book 101 Title";
        //    book["PackageName"] = "11-11-11-11";

        //    // example of list
        //    var relatedItems = new DynamoDBList();
        //    relatedItems.Add(341);
        //    relatedItems.Add(472);
        //    relatedItems.Add(649);

        //    // Create a condition expression for the optional conditional put operation.
        //    Expression expr = new Expression();
        //    expr.ExpressionStatement = "ISBN = :val";
        //    expr.ExpressionAttributeValues[":val"] = "55-55-55-55";

        //    PutItemOperationConfig config = new PutItemOperationConfig()
        //    {
        //        // Optional parameter.
        //        ConditionalExpression = expr
        //    };


        //    await table.PutItemAsync(book);
        //}

        //public async Task<Document> GetSomeAttributesFromItem()
        //{
        //    // Configuration object that specifies optional parameters.
        //    GetItemOperationConfig config = new GetItemOperationConfig()
        //    {
        //        AttributesToGet = new List<string>() { "Id", "Title" },
        //    };

        //    Table table = Table.LoadTable(this._client, "PackageDetails");

        //    Document doc = await table.GetItemAsync("11-11-11-11", config);

        //    return doc;
        //}

        //public async void DeleteExample()
        //{
        //    DeleteItemOperationConfig config = new DeleteItemOperationConfig
        //    {
        //        // Return the deleted item.
        //        ReturnValues = ReturnValues.AllOldAttributes
        //    };

        //    Table table = Table.LoadTable(this._client, "PackageDetails");

        //    Document document = await table.DeleteItemAsync("blah", config);
        //}

        //public async void UpdateExample()
        //{
        //    var book = new Document();
        //    book["Id"] = "blah";
        //    // List of attribute updates.
        //    // The following replaces the existing authors list.
        //    book["Authors"] = new List<string> { "Author x", "Author y" };
        //    book["newAttribute"] = "New Value";
        //    book["ISBN"] = null; // Remove it.

        //    // Optional parameters.
        //    UpdateItemOperationConfig config = new UpdateItemOperationConfig
        //    {
        //        // Get updated item in response.
        //        ReturnValues = ReturnValues.AllNewAttributes
        //    };

        //    Table table = Table.LoadTable(this._client, "PackageDetails");

        //    Document updatedBook = await table.UpdateItemAsync(book, config);
        //}
    }
}
