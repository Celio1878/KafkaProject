using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using deviocourse;

string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL") ?? "";
string groupId = Environment.GetEnvironmentVariable("GROUP_ID") ?? "";
string topic = Environment.GetEnvironmentVariable("TOPIC_NAME") ?? "";
string bootstrapServer = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVER") ?? "";

var schemaConfig = new SchemaRegistryConfig { Url = schemaRegistryUrl };
var schemaRegistry = new CachedSchemaRegistryClient(schemaConfig);

var config = new ConsumerConfig
{
    GroupId = groupId,
    BootstrapServers = bootstrapServer
};

var deserializer = new AvroDeserializer<learning>(schemaRegistry).AsSyncOverAsync();
using var consumer = new ConsumerBuilder<string, learning>(config).SetValueDeserializer(deserializer).Build();

string topicName = topic;
consumer.Subscribe(topicName);

var result = consumer.Consume();

Console.WriteLine($"Message {result.Message.Key} with value -> {result.Message.Value.id}");