using Amazon.DynamoDBv2.Model;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace IntegrationTest;

public class DbContainer : IAsyncLifetime
{
    private string _containerId = String.Empty;

    private async Task CreateTable()
    {
        const string tableName = "Submissions";

        var client = new IntegrationDbClient();

        await client.CreateTableAsync(
            new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH" }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            }
        );
    }

    private async Task CreateContainer()
    {
        var config = new HostConfig
        {
            PortBindings = new Dictionary<string, IList<PortBinding>>
            {
                {
                    "8000/tcp",
                    new List<PortBinding> { new PortBinding { HostPort = "8000" } }
                }
            }
        };

        var createContainerParameters = new CreateContainerParameters
        {
            Image = "amazon/dynamodb-local:latest",
            HostConfig = config,
            Env = new List<string>
            {
                "AWS_ACCESS_KEY_ID=access_key_id",
                "AWS_SECRET_ACCESS_KEY=secret_key",
            },
            Cmd = new List<string> { "-jar", "DynamoDBLocal.jar", "-inMemory" }
        };

        var dockerClient = new DockerClientConfiguration().CreateClient();

        var response = await dockerClient.Containers.CreateContainerAsync(
            createContainerParameters
        );

        _containerId = response.ID;

        await dockerClient.Containers.StartContainerAsync(
            response.ID,
            new ContainerStartParameters()
        );
    }

    public async Task InitializeAsync()
    {
        await CreateContainer();
        await CreateTable();
    }

    public async Task DisposeAsync()
    {
        var dockerClient = new DockerClientConfiguration().CreateClient();

        await dockerClient.Containers.StopContainerAsync(
            _containerId,
            new ContainerStopParameters()
        );

        await dockerClient.Containers.RemoveContainerAsync(
            _containerId,
            new ContainerRemoveParameters()
        );
    }
}
