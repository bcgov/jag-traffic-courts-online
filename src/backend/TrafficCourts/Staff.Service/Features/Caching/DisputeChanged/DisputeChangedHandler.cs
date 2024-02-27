using MediatR;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Features.Caching.DisputeChanged
{
    public class DisputeChangedHandler : INotificationHandler<DisputeChangedNotification>
    {
        private readonly IFusionCache _cache;

        public DisputeChangedHandler(IFusionCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task Handle(DisputeChangedNotification notification, CancellationToken cancellationToken)
        {
            await ClearDisputeListItemCacheAsync(cancellationToken);
        }

        private async Task ClearDisputeListItemCacheAsync(CancellationToken cancellationToken)
        {
            await _cache.ExpireAsync(Keys.Dispute.DisputeListItems, null, cancellationToken).ConfigureAwait(false);
        }
    }
}
