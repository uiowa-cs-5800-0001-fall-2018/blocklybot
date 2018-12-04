using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using NLog;

namespace BlockBot.Module.Aws.Services
{
    public class DynamoDbService
    {
        private readonly AmazonDynamoDBClient _client;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DynamoDbService(AWSCredentials credentials, RegionEndpoint awsRegion)
        {
            _client = new AmazonDynamoDBClient(credentials, awsRegion);
        }

        public async Task<CreateTableResponse> Create(Guid projectId)
        {
            try
            {
                CreateTableResponse response = await _client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = projectId.ToString(),
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement("sender", KeyType.HASH)
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition("sender", ScalarAttributeType.S)
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST,
                    
                });

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("There was an error creating the DynamoDb table.");
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

//        public void Read()
//        {
//        }
//
//        public void Update()
//        {
//        }

        public async Task<DeleteTableResponse> Delete(Guid projectId)
        {
            try
            {
                var response = await _client.DeleteTableAsync(projectId.ToString());

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("There was an error deleting the DynamoDb table.");
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}