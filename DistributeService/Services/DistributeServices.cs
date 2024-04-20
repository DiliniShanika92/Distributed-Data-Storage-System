using Grpc.Core;
using Grpc.Net.Client;
using GrpcService1;
using SupportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DistributeService.Services
{
    public class DistributeServices : IDistributeServices
    {
        public string stringValue { get; set; }

        private List<nodeItems> nodes = new List<nodeItems>();

        public string NodeName { get; set; }
        public int NodeId { get; set; }
        private UdpClient udpClient;

        public DistributeServices()
        {

        }

        public void ProcessStoreString(string value)
        {
            stringValue = value;

            string firstTenCharacters = stringValue.Substring(0, Math.Min(stringValue.Length, 10));
            string hashValue = GenerateHashValue(value);

            //When a new node joins 

            NodeName = "Node" + new Random().Next(1000, 9999); // Random name generation
            NodeId = new Random().Next(0, 5); // Unique ID from 0 to 4


            //NewNodeJoin(NodeName, NodeId);

            //send a broadcast message to all
            BroadcastMessage(NodeName, NodeId);


            int result = CalculateRecipientNode(firstTenCharacters, nodes.Count);

            ReceiverStoreDataAsync(hashValue, stringValue, result, firstTenCharacters);



        }
        public string GenerateHashValue(string inputString)
        {
            // Implement hash function
            return GetHash(inputString).Substring(0, 10); // First 10 characters

        }


        public static int CalculateRecipientNode(string input, int numNodes)
        {
            try
            {
                int sum = 0;
                for (int i = 0; i < Math.Min(input.Length, 10); i++)
                {
                    sum += (int)input[i];
                }
                return sum % numNodes;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task ReceiverStoreDataAsync(string hashValue, string stringValue, int id, string firstTenCharacters)
        {
            nodeItems node = new nodeItems();

             List<string> addresses = new List<string>
            {
                "127.0.0.1:7244",
                "127.0.0.1:7082",
                "127.0.0.1:7245",
                "127.0.0.1:7279",
                "127.0.0.1:7006"
            };

            if (id != 0) 
            {
                node = nodes[id - 1];
            }

            if (node.Role == "receiver ")
            {
                
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                //using var channel = GrpcChannel.ForAddress("https://127.0.0.1:7244");

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                // Use the custom handler with the gRPC channel
                using var channel = GrpcChannel.ForAddress(addresses[id - 1], new GrpcChannelOptions
                {
                    HttpClient = new HttpClient(handler)
                });

                var client = new NodeService.NodeServiceClient(channel);

                var request = new createValueRequest
                {

                    Id = firstTenCharacters,
                    Value = stringValue,

                };
                var reply = await client.CreateValueAsync(request);

            }
            else 
            {

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                // Use the custom handler with the gRPC channel
                using var channel = GrpcChannel.ForAddress(addresses[id - 1], new GrpcChannelOptions
                {
                    HttpClient = new HttpClient(handler)
                });

                var client = new NodeService.NodeServiceClient(channel);

                var request = new createValueRequest
                {

                    Id = hashValue,
                    Value = stringValue,

                };
                var reply = await client.CreateValueAsync(request);

            }

        }

        public void ReceiverRetrieveData(string hashValue)
        {
            // Retrieve data based on the hash value
        }

        private string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private async void BroadcastMessage(string nodeName, int nodeId)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //using var channel = GrpcChannel.ForAddress("https://127.0.0.1:7244");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // Use the custom handler with the gRPC channel
            using var channel = GrpcChannel.ForAddress("https://127.0.0.1:7244", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });

            var client = new NodeService.NodeServiceClient(channel);

            var request = new castMessageRequest { Content = "Hello from client" };
            var reply = await client.BroadcastMessageAsync(request);

            Console.WriteLine($"Response: {reply.Content}");
        }

        
    }
}
