using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Amazon.Lambda;
using Amazon.Runtime;
using BlockBot.AwsServices.Models;
using NLog;

namespace BlockBot.AwsServices.Services
{
    public class ApiGatewayService
    {
        private readonly AmazonAPIGatewayClient _client;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ApiGatewayService(AWSCredentials credentials, RegionEndpoint awsRegion)
        {
            _client = new AmazonAPIGatewayClient(credentials, awsRegion);
        }

        public async Task<ApiGatewayRestApi> CreateApiGateway(string apiName, string apiDescription)
        {
            try
            {
                CreateRestApiResponse restApi = await _client.CreateRestApiAsync(new CreateRestApiRequest
                {
                    Name = apiName,
                    Description = apiDescription,
                    EndpointConfiguration = new EndpointConfiguration{Types = {"REGIONAL"}}
                });
                if (restApi.HttpStatusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Error creating REST API named '{apiName}'.");
                }

                return new ApiGatewayRestApi
                {
                    RestApiId = restApi.Id,
                    RestApi = restApi
                };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<ApiGatewayRestApiDelete> DeleteApiGateway(string restApiId)
        {
            try
            {
                // TODO see if we need to delete other resources, or just root REST API 

                DeleteRestApiResponse restApi = await _client.DeleteRestApiAsync(new DeleteRestApiRequest
                {
                    RestApiId = restApiId
                });
                if (restApi.HttpStatusCode != HttpStatusCode.Accepted)
                {
                    throw new Exception($"Error deleting REST API id '{restApiId}'.");
                }

                // TODO return delete responses
                return new ApiGatewayRestApiDelete
                {
                    RestApi = restApi
                };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<ApiGatewayResource> CreateResourceMappedToLambda(string restApiId, string resourceName, string functionArn, string roleArn)
        {
            try
            {
                string parentResourceId = string.Empty;
                GetResourcesResponse parentResources = await _client.GetResourcesAsync(new GetResourcesRequest
                {
                    RestApiId = restApiId
                });
                // TODO consider handling multiple pages of resources
                foreach (Resource pItem in parentResources.Items)
                {
                    if (pItem.Path == "/")
                    {
                        parentResourceId = pItem.Id;
                    }
                }

                if (string.IsNullOrEmpty(parentResourceId))
                {
                    throw new Exception($"Could not calculate root resource id for API Gateway Rest API id '{restApiId}'.");
                }

                CreateResourceResponse resource = await _client.CreateResourceAsync(new CreateResourceRequest
                {
                    PathPart = resourceName,
                    RestApiId = restApiId,
                    ParentId = parentResourceId
                });
                if (resource.HttpStatusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Could not create resource '{resourceName}' under parent resource id '{parentResourceId}' on REST API id '{restApiId}'.");
                }

                string httpMethod = "ANY";
                PutMethodResponse method = await _client.PutMethodAsync(new PutMethodRequest
                {
                    RestApiId = restApiId,
                    ResourceId = resource.Id,
                    HttpMethod = httpMethod,
                    AuthorizationType = "NONE",
                    ApiKeyRequired = false
                });
                if (method.HttpStatusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Could not configure HTTP method '{httpMethod}' for resource id '{resource.Id}' on REST API id '{restApiId}'.");
                }

                string uri =
                    $"arn:aws:apigateway:{_client.Config.RegionEndpoint.SystemName}:lambda:path/2015-03-31/functions/{functionArn}/invocations";
                PutIntegrationResponse integration = await _client.PutIntegrationAsync(new PutIntegrationRequest
                {
                    RestApiId = restApiId,
                    ResourceId = resource.Id,
                    HttpMethod = "ANY",
                    Type = IntegrationType.AWS_PROXY,
                    // TODO determine why this value must be POST
                    IntegrationHttpMethod = "POST", 
                    // 2015-03-31 is the publish date of the lambda API that's being used
                    Uri = uri,
                    Credentials = roleArn
                });
                if (integration.HttpStatusCode != HttpStatusCode.Created)
                {
                    // TODO populate exception message
                    throw new Exception($"Could not configure integration between resource id '{resource.Id}' on REST API id '{restApiId}' and Lambda URI '{uri}'.");
                }

                return new ApiGatewayResource
                {
                    RestApiId = restApiId,
                    ResourceId = resource.Id,
                    Resource = resource,
                    Method = method,
                    Integration = integration
                };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<ApiGatewayResourceDelete> DeleteResourceMappedToLambda(string restApiId, string resourceId)
        {
            try
            {
                DeleteIntegrationResponse integration = await _client.DeleteIntegrationAsync(new DeleteIntegrationRequest
                {
                    HttpMethod = "ANY",
                    RestApiId = restApiId,
                    ResourceId = resourceId
                });
                if (integration.HttpStatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception($"Error deleting integration for resource id '{resourceId}' on REST API id '{restApiId}'.");
                }

                DeleteMethodResponse method = await _client.DeleteMethodAsync(new DeleteMethodRequest
                {
                    HttpMethod = "ANY",
                    RestApiId = restApiId,
                    ResourceId = resourceId
                });
                if (method.HttpStatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception($"Error deleting method for resource id '{resourceId}' on REST API id '{restApiId}'.");
                }

                DeleteResourceResponse resource = await _client.DeleteResourceAsync(new DeleteResourceRequest
                {
                    RestApiId = restApiId,
                    ResourceId = resourceId
                });
                if (resource.HttpStatusCode != HttpStatusCode.Accepted)
                {
                    throw new Exception($"Error deleting resource id '{resourceId}' on REST API id '{restApiId}'.");
                }

                return new ApiGatewayResourceDelete
                {
                    Integration = integration,
                    Method = method,
                    Resource = resource
                };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<CreateDeploymentResponse> DeployRestApi(string restApiId, string deploymentStageName = "default")
        {
            try
            {
                CreateDeploymentResponse deployment = await _client.CreateDeploymentAsync(new CreateDeploymentRequest
                {
                    RestApiId = restApiId,
                    StageName = deploymentStageName
                });
                if (deployment.HttpStatusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Error creating deployment for stage name '{deploymentStageName}' on REST API id '{restApiId}'.");
                }

                return deployment;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}
