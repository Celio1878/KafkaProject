using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using deviocourse;

string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL") ?? "";
string topic = Environment.GetEnvironmentVariable("TOPIC_NAME") ?? "";
string bootstrapServer = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVER") ?? "";

var schemaConfig = new SchemaRegistryConfig { Url = schemaRegistryUrl };
var schemaRegistry = new CachedSchemaRegistryClient(schemaConfig);

var config = new ProducerConfig { BootstrapServers = bootstrapServer };

var avroSerializer = new AvroSerializer<learning>(schemaRegistry);
var producerBuilder = new ProducerBuilder<string, learning>(config);
producerBuilder.SetValueSerializer(avroSerializer);
using var producer = producerBuilder.Build();

Message<string, learning> message = new Message<string, learning>
{
    Key = Guid.NewGuid().ToString(),
    Value = new learning
    {
        id = Guid.NewGuid().ToString(),
        describe = "New Schema",
        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    }
};

string topicName = topic;
var result = await producer.ProduceAsync(topicName, message);

Console.WriteLine($"Message sent to topic {topicName} with offset {result.Offset}");