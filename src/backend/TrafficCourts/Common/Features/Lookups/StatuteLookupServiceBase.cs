using Microsoft.Extensions.Logging;
using System.Text;
using TrafficCourts.Domain.Models;
using TrafficCourts.OrdsDataService.Justin;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Common.Features.Lookups
{
    public abstract class StatuteLookupServiceBase<T> : IStatuteLookupService
    {
        private readonly IStatuteRepository _repository;
        private readonly IFusionCache _cache;
        private readonly ILogger<StatuteLookupServiceBase<T>> _logger;

        public StatuteLookupServiceBase(IStatuteRepository repository, IFusionCache cache, ILogger<StatuteLookupServiceBase<T>> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract string GetCacheKey();

        public async Task<TrafficCourts.Domain.Models.Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(section))
            {
                return null;
            }

            if (!LegalSection.TryParse(section, out LegalSection? legalSection))
            {
                _logger.LogInformation("Could not parse section {Section}", section);
                return null;
            }

            List<OrdsDataService.Justin.Statute> items = await GetStatutesAsync(cancellationToken);

            var query = items
                .AsQueryable()
                .Where(_ => _.stat_section_txt == legalSection.Section);

            if (!string.IsNullOrWhiteSpace(legalSection.Subsection))
            {
                query = query.Where(_ => _.stat_sub_section_txt == legalSection.Subsection);
            }

            if (!string.IsNullOrWhiteSpace(legalSection.Paragraph))
            {
                query = query.Where(_ => _.stat_paragraph_txt == legalSection.Paragraph.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(legalSection.Subparagraph))
            {
                query = query.Where(_ => _.stat_sub_paragraph_txt == legalSection.Subparagraph.ToLower());
            }

            var sections = query.ToList();
            if (sections.Count == 0)
            {
                _logger.LogInformation("Could not find matching section {Section}", section);
                return null;
            }

            if (sections.Count > 1)
            {
                _logger.LogInformation("{Count} sections were returned matching {Section}, returning first value", sections.Count, section);
            }

            return ToDomainModel(sections[0], new StringBuilder());
        }

        public async Task<TrafficCourts.Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken)
        {
            // statutes will already be sorted by statuteId
            List<TrafficCourts.OrdsDataService.Justin.Statute> statutes = await GetStatutesAsync(cancellationToken);

            var index = statutes.BinarySearch(
                new TrafficCourts.OrdsDataService.Justin.Statute { stat_id = statuteId },
                new StatuteIdComparer());

            if (index >= 0)
            {
                return ToDomainModel(statutes[index], new StringBuilder());
            }

            // new statute? try fetching the statute from ORDS without cache
            var statute = await _repository.GetAsync(statuteId, cancellationToken);

            if (statute is not null)
            {
                // clear the cache because the statute was not found in the cache
                await _cache.RemoveAsync(GetCacheKey(), token: cancellationToken);
                return ToDomainModel(statute, new StringBuilder());
            }
            
            return null; // not found
        }

        public async Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(CancellationToken cancellationToken)
        {
            List<TrafficCourts.OrdsDataService.Justin.Statute> items = await GetStatutesAsync(cancellationToken);

            StringBuilder buffer = new StringBuilder();

            List<Domain.Models.Statute> models = items.Select(_ => ToDomainModel(_, buffer)).ToList();

            return models;
        }

        private TrafficCourts.Domain.Models.Statute ToDomainModel(TrafficCourts.OrdsDataService.Justin.Statute statute, StringBuilder buffer)
        {
            return new TrafficCourts.Domain.Models.Statute(
                /*Id*/ statute.stat_id.ToString(),
                /*ActCode*/ statute.act_cd,
                /*SectionText*/ statute.stat_section_txt,
                /*SubsectionText*/ statute.stat_sub_section_txt ?? string.Empty,
                /*ParagraphText*/ statute.stat_paragraph_txt ?? string.Empty,
                /*SubparagraphText*/ statute.stat_sub_paragraph_txt ?? string.Empty,
                /*Code*/ GetCode(statute, buffer),
                /*ShortDescriptionText*/statute.stat_short_description_txt ?? statute.stat_description_txt,
                /*DescriptionText*/ statute.stat_description_txt
            );
        }

        private async Task<List<TrafficCourts.OrdsDataService.Justin.Statute>> GetStatutesAsync(CancellationToken cancellationToken)
        {
            var key = GetCacheKey();

            // cache the data directly fetched from ORDS
            var statutes = await _cache.GetOrSetAsync<List<TrafficCourts.OrdsDataService.Justin.Statute>>(
                key,
                ct => _repository.GetListAsync(ct),
                options => options.SetDuration(TimeSpan.FromHours(1)),
                cancellationToken);

            return statutes;
        }

        private class StatuteIdComparer : IComparer<TrafficCourts.OrdsDataService.Justin.Statute>
        {
            public int Compare(TrafficCourts.OrdsDataService.Justin.Statute? x, TrafficCourts.OrdsDataService.Justin.Statute? y)
            {
                if (x is null)
                {
                    return y is null ? 0 : -1;
                }

                if (y is null)
                {
                    return 1;
                }

                return x.stat_id.CompareTo(y.stat_id);
            }
        }

        private static string GetCode(TrafficCourts.OrdsDataService.Justin.Statute statute, StringBuilder buffer)
        {
            buffer.Append(statute.stat_section_txt);

            if (!string.IsNullOrWhiteSpace(statute.stat_sub_section_txt))
            {

                buffer.Append('(');
                buffer.Append(statute.stat_sub_section_txt);
                buffer.Append(')');
            }

            if (!string.IsNullOrWhiteSpace(statute.stat_paragraph_txt))
            {

                buffer.Append('(');
                buffer.Append(statute.stat_paragraph_txt);
                buffer.Append(')');
            }

            var code = buffer.ToString();
            buffer.Clear();
            return code;
        }
    }
}
