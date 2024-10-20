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
            var timestamp = DateTime.UtcNow.ToString("o");
            var useragent = Request.Headers["User-Agent"].ToString();
            string nextDay = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (useragent == null)
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
                    cacheExpire = nextDay
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
                                electionEnds = nextDay,
                                numWinners = 1,
                                wipeNumber = 1,
                                winnerStateHours = 1,
                                offers = new List<string>()
                            }
                        }
                    },
                    cacheExpire = nextDay
                },
                clientEvents = new ClientEvents
                {
                    states = new List<EventState>
                    {
                        new EventState
                        {
                            validFrom = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                            activeEvents = activeEvents,
                            state = new EventInfo
                            {
                                activeStorefronts = new List<string>(),
                                eventNamedWeights = new Dictionary<string, int>(),
                                seasonNumber = uahelper.Season,
                                seasonTemplateId = $"AthenaSeason:athenaseason{uahelper.Season}",
                                matchXpBonusPoints = 0,
                                seasonBegin = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                seasonEnd = DateTime.Parse("9999-01-01T00:00:00Z"),
                                seasonDisplayedEnd = DateTime.Parse("9999-01-01T00:00:00Z"),
                                weeklyStoreEnd = nextDay,
                                stwEventStoreEnd = nextDay,
                                stwWeeklyStoreEnd = nextDay,
                                sectionStoreEnds = new Dictionary<string, DateTime>(),
                                dailyStoreEnd = nextDay
                            }
                        }
                    },
                    cacheExpire = nextDay
                }
            };

            var response = new
            {
                channels,
                eventsTimeOffsetHrs = 0,
                cacheIntervalMins = 10,
                currentTime = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return Ok(response);
        }
    }
}
