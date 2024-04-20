using Grpc.Core;
using Grpc.Net.Client;
using GrpcService4.Protos;


namespace Node4.Services
{
    public class Node4ServiceProxy
    {
        private readonly NodeService.NodeServiceClient _client;

        public Node4ServiceProxy()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7279");
            _client = new NodeService.NodeServiceClient(channel);
        }

        public createValueReply CreateValue(createValueRequest request)
        {
            return _client.CreateValue(request);
        }

        public readValueReply ReadValue(readValueRequest request)
        {
            return _client.ReadValue(request);
        }

        public createNodeReply JoinNode(createNodeRequest request)
        {
            return _client.JoinNode(request);
        }

        public async Task<List<readNodeReply>> ReadNode(readNodeRequest request)
        {
            var responseList = new List<readNodeReply>();
            using (var call = _client.ReadNode(request))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    responseList.Add(responseStream.Current);
                }
            }
            return responseList;
        }


        public async Task<List<readNodeReply>> ReadNodeStream(readNodeRequest request)
        {
            var stream = _client.ReadNode(request);
            var nodes = new List<readNodeReply>();
            await foreach (var node in stream.ResponseStream.ReadAllAsync())
            {
                nodes.Add(node);
            }
            return nodes;
        }

        public castMessageReply BroadcastMessage(castMessageRequest request)
        {
            return _client.BroadcastMessage(request);
        }
    }
}
