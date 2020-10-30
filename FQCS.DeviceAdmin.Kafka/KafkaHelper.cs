using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FQCS.DeviceAdmin.Kafka
{
    public static class KafkaHelper
    {
        public static IConsumer<Null, string> GetPlainConsumer(string server,
            string groupId, string username, string password)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
                SaslUsername = username,
                SaslPassword = password,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslPlaintext
            };
            var newOrderConsumer = new ConsumerBuilder<Null, string>(config).Build();
            return newOrderConsumer;
        }

        public static IProducer<Null, string> GetPlainProducer(string server,
            string username, string password)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server,
                SaslUsername = username,
                SaslPassword = password,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslPlaintext
            };
            var producer = new ProducerBuilder<Null, string>(config).Build();
            return producer;
        }

    }
}
