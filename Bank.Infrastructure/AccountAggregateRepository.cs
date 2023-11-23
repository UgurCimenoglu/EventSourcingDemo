using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bank.Domain.BankAccount;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure
{
    public class AccountAggregateRepository
    {
        private readonly EventStoreClient _client;

        public AccountAggregateRepository(EventStoreClient client)
        {
            _client = client;
        }

        private string GetStreamName(AccountId accountId) => $"Account-{accountId}";
        private string GetSnapshotName(AccountId accountId) => $"AccountSnapshot-{accountId}";

        public async Task<Account> GetAsync(AccountId accountId, CancellationToken cancellationToken = default)
        {
            var snapshot = await LoadSnapShotAsync(accountId);
            var account = snapshot == null ? new Account(accountId) : new Account(accountId, snapshot);

            var events = _client.ReadStreamAsync
            (
                Direction.Forwards,
                GetStreamName(accountId),
                snapshot == null ? StreamPosition.Start : StreamPosition.FromInt64(snapshot.Version + 1),
                cancellationToken: cancellationToken
            );

            if (await events.ReadState == ReadState.StreamNotFound)
            {
                return account;
            }

            await foreach (var @event in events)
            {
                var data = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
                account.LoadChanges(
                    @event.OriginalEventNumber.ToInt64(),
                    @event.Event.EventType switch
                    {
                        nameof(AccountCreated) => JsonSerializer.Deserialize<AccountCreated>(data),
                        nameof(MoneyDeposited) => JsonSerializer.Deserialize<MoneyDeposited>(data),
                        nameof(MoneyWithdrawn) => JsonSerializer.Deserialize<MoneyWithdrawn>(data),
                    }
                );
            }
            return account;
        }

        private async Task<AccountSnapshot?> LoadSnapShotAsync(AccountId accountId)
        {
            var events = _client.ReadStreamAsync(
                Direction.Backwards,
                GetSnapshotName(accountId),
                StreamPosition.End,
                maxCount: 1
            );
            if (await events.ReadState == ReadState.StreamNotFound)
            {
                return null;
            }

            var lastEvent = await events.ElementAtAsync(0);

            var data = Encoding.UTF8.GetString(lastEvent.Event.Data.ToArray());
            return JsonSerializer.Deserialize<AccountSnapshot>(data);
        }

        public async Task SaveAsync(Account account, CancellationToken cancellationToken = default)
        {
            if (!account.GetChanges().Any())
            {
                return;
            }
            var result = await _client.AppendToStreamAsync
             (
                 GetStreamName(account.Id),
                 StreamRevision.FromInt64(account.Version),
                 account.GetChanges().Select(change => new EventData(Uuid.NewUuid(), change.GetType().Name, JsonSerializer.SerializeToUtf8Bytes(change))),
                 cancellationToken: cancellationToken
             );

            if (result.NextExpectedStreamRevision.ToInt64() % 5 == 0)
            {
                await AppendSnapshotAsync(account, result.NextExpectedStreamRevision.ToInt64());
            }
        }

        private async Task AppendSnapshotAsync(Account account, long version)
        {
            await _client.AppendToStreamAsync(
                GetSnapshotName(account.Id),
                StreamState.Any,
                new EventData[] { new EventData(Uuid.NewUuid(), "snapshot", JsonSerializer.SerializeToUtf8Bytes(new AccountSnapshot { State = account.State, Version = version })) }
            );
        }
    }
}
