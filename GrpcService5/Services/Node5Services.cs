using Grpc.Core;
using GrpcService5.DataAccess;
using GrpcService5.Protos;
using SupportService.Models;

namespace GrpcService5.Services
{
    public class Node5Services : NodeService.NodeServiceBase
    {
        private readonly AppDbContext _dbcontext;
        public Node5Services(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        private readonly List<string> peers = new List<string>
        {
       "127.0.0.1:7244",
        "127.0.0.1:7082",
        "127.0.0.1:7245",
        "127.0.0.1:7279",
       // "127.0.0.1:7006"
        };

        public override Task<castMessageReply> BroadcastMessage(castMessageRequest request, ServerCallContext context)
        {
            request.Content = "Request Node 5";

            foreach (var peer in peers)
            {
                var channel = new Channel(peer, ChannelCredentials.Insecure);
                var client = new NodeService.NodeServiceClient(channel);
                client.BroadcastMessage(request);
            }

            var rep = new castMessageReply();
            rep.Content = "Respones Node 5";
            return Task.FromResult(rep);
        }

        public override async Task<createValueReply> CreateValue(createValueRequest value, ServerCallContext context)
        {
            try
            {
                var val = new valueItems
                {
                    Id = value.Id,
                    Values = value.Value

                };
                var res = _dbcontext.Add(val);
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {

                throw;
            }

            return await Task.FromResult(new createValueReply
            {
                Id = value.Id,

            });
        }

        public override async Task<readValueReply> ReadValue(readValueRequest value, ServerCallContext context)
        {
            var result = new valueItems();
            try
            {

                result = _dbcontext.Set<valueItems>().FindAsync(value.Id).Result;

            }
            catch (Exception e)
            {

                throw;
            }

            return await Task.FromResult(new readValueReply
            {
                Id = result.Id,
                Value = result.Values,

            });
        }


        public override async Task<createNodeReply> JoinNode(createNodeRequest value, ServerCallContext context)
        {
            try
            {
                var val = new nodeItems
                {
                    Id = value.Id,
                    Name = value.Name,
                    Role = value.Role,

                };
                var res = _dbcontext.Add(val);
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {

                throw;
            }

            return await Task.FromResult(new createNodeReply
            {
                Id = value.Id,

            });
        }

        public override async Task<IEnumerable<readNodeReply>> ReadNode(readNodeRequest value, IServerStreamWriter<readNodeReply> responseStream, ServerCallContext context)
        {
            var result = new List<nodeItems>();
            var retrunVal = new List<readNodeReply>();
            try
            {

                result = _dbcontext.Set<nodeItems>().ToList();

            }
            catch (Exception e)
            {

                throw;
            }

            foreach (var item in result)
            {
                var data = new readNodeReply
                {
                    Id = item.Id,
                    Name = item.Name,
                    Role = item.Role,

                };

                await responseStream.WriteAsync(data);
            }


            return retrunVal;
        }
    }
}
