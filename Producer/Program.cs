using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using deviocourse;

var schemaConfig = new SchemaRegistryConfig
{
    Url = "http://localhost:8081"
};

var schemaRegistry = new CachedSchemaRegistryClient(schemaConfig);

var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

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

const string topicName = "deviocourse";
var result = await producer.ProduceAsync(topicName, message);

Console.WriteLine($"Message sent to topic {topicName} with offset {result.Offset}");