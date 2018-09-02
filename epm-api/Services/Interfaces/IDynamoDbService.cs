using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using epm_api.Enums;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IDynamoDbService
    {
        IAmazonDynamoDB GetClient();
        Task<Document> PutItemAsync(DynamoDbTablesEnum dynamoDbtable, Document document);
        Task<Document> PutItemAsync(DynamoDbTablesEnum dynamoDbtable, Document document, PutItemOperationConfig config);
        Task<Document> GetItemAsync(DynamoDbTablesEnum dynamoDbTable, Primitive primaryKey);
        Task<Document> GetItemAsync(DynamoDbTablesEnum dynamoDbTable, Primitive primaryKey, GetItemOperationConfig config);
        Task<Document> DeleteItemAsync(DynamoDbTablesEnum dynamoDbTable, Primitive primaryKey);
        Task<Document> DeleteItemAsync(DynamoDbTablesEnum dynamoDbTable, Primitive primaryKey, DeleteItemOperationConfig config);
        Task<Document> UpdateItemAsync(DynamoDbTablesEnum dynamoDbTable, Document document);
        Task<Document> UpdateItemAsync(DynamoDbTablesEnum dynamoDbTable, Document document, UpdateItemOperationConfig config);
    }
}