using System.Text;
using EventStore.Client;

namespace Bank.API
{
    public class AccountAggregateSubscriber : IHostedService
    {
        private readonly EventStoreClient _eventStoreClient;
        private StreamSubscription _subscription;
        public AccountAggregateSubscriber(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {

            var checkpoint = GetCheckpoint();

            _subscription = await _eventStoreClient.SubscribeToAllAsync(
                checkpoint == null ? FromAll.Start : FromAll.After(checkpoint.Value),
                (_, @event, _) =>
                {
                    var data = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
                    Console.WriteLine(data);

                    SaveCheckpoint(@event.OriginalPosition);
                    return Task.CompletedTask;
                },
                filterOptions: new SubscriptionFilterOptions(StreamFilter.Prefix("Account-"))
            );
        }

        private Position? GetCheckpoint()
        {
            using (var sr = File.OpenText("checkpoint.txt"))
            {
                var position = sr.ReadToEnd().Split(";", StringSplitOptions.RemoveEmptyEntries);

                return position.Length > 0
                    ? new Position(Convert.ToUInt64(position[0]), Convert.ToUInt64(position[1]))
                    : null;
            }
        }

        private void SaveCheckpoint(Position? position)
        {
            using (var sw = File.CreateText("checkpoint.txt"))
            {
                sw.WriteLine("{0};{1}", position.Value.CommitPosition, position.Value.PreparePosition);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
