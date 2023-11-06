using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using deviocourse;

var schemaConfig = new SchemaRegistryConfig
{
    Url = "http://localhost:8081"
};

var schemaRegistry = new CachedSchemaRegistryClient(schemaConfig);

var config = new ConsumerConfig
{
    GroupId = "devio",
    BootstrapServers = "localhost:9092"
};

var deserializer = new AvroDeserializer<learning>(schemaRegistry).AsSyncOverAsync();
using var consumer = new ConsumerBuilder<string, learning>(config).SetValueDeserializer(deserializer).Build();

const string topicName = "deviocourse";
consumer.Subscribe(topicName);

var result = consumer.Consume();

Console.WriteLine($"Message {result.Message.Key} with value -> {result.Message.Value.id}");