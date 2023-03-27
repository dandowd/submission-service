using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace IntegrationTest;

public class IntegrationDbClient : AmazonDynamoDBClient
{
    public IntegrationDbClient()
        : base(
            new BasicAWSCredentials("access_key_id", "secret_key"),
            new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000", UseHttp = true }
        ) { }
}
