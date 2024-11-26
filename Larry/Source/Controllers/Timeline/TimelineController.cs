using Larry.Source.Classes.Timeline;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Larry.Source.Utilities.Parsers;
using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Timeline
{
    [ApiController]
    [Route("/fortnite/api/calendar/v1/")]
    public class TimelineController : Controller
    {
        [HttpGet("timeline")]
        public async Task<IActionResult> GetCalendar()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var useragent = Request.Headers["User-Agent"].ToString();
            string nextDayMidnight = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");

            if (string.IsNullOrEmpty(useragent))
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing header 'User-Agent'.", timestamp));
            }

            var uahelper = UserAgentParser.Parse(useragent);
            if (uahelper == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Failed to parse User-Agent.", timestamp));
            }

            var activeEvents = await TimelineManager.CreateTimelineAsync(useragent);

            var channels = new Channels
            {
                clientMatchmaking = new ClientMatchmaking
                {
                    states = new List<object>(),
                    cacheExpire = nextDayMidnight
                },
                communityVotes = new CommunityVotes
                {
                    states = new List<VoteState>
                    {
                        new VoteState
                        {
                            validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            activeEvents = new List<string>(),
                            state = new ElectionState
                            {
                                electionId = string.Empty,
                                candidates = new List<string>(),
                                electionEnds = nextDayMidnight,
                                numWinners = 1,
                                wipeNumber = 1,
                                winnerStateHours = 1,
                                offers = new List<string>()
                            }
                        }
                    },
                    cacheExpire = nextDayMidnight
                },
                clientEvents = new ClientEvents
                {
                    states = new List<EventState>
                    {
                        new EventState
                        {
                            validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            activeEvents = activeEvents,
                            state = new EventInfo
                            {
                                activeStorefronts = new List<string>(),
                                eventNamedWeights = new Dictionary<string, int>(),
                                seasonNumber = uahelper.Season,
                                seasonTemplateId = $"AthenaSeason:athenaseason{uahelper.Season}",
                                matchXpBonusPoints = 0,
                                seasonBegin = DateTime.Parse("2020-01-01T00:00:00.000Z").ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                seasonEnd = DateTime.Parse("9999-01-01T00:00:00.000Z").ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                seasonDisplayedEnd = DateTime.Parse("9999-01-01T00:00:00.000Z").ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                weeklyStoreEnd = nextDayMidnight,
                                stwEventStoreEnd = nextDayMidnight,
                                stwWeeklyStoreEnd = nextDayMidnight,
                                sectionStoreEnds = new Dictionary<string, DateTime>(),
                                dailyStoreEnd = nextDayMidnight
                            }
                        }
                    },
                    cacheExpire = nextDayMidnight
                }
            };

            var response = new
            {
                channels,
                eventsTimeOffsetHrs = 0,
                cacheIntervalMins = 10,
                currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return Ok(response);
        }
    }
}
