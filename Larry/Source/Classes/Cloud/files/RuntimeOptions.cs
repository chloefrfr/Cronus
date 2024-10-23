namespace Larry.Source.Classes.Cloud.files
{
    public class RuntimeOptions
    {
        public static string GetDefaultRuntimeOptions()
        {
            return @"
[/Script/FortniteGame.FortRuntimeOptions]
!DisabledFrontendNavigationTabs=ClearArray
+DisabledFrontendNavigationTabs=(TabName=""Showdown"", TabState=EFortRuntimeOptionTabState::Hidden)
bEnableGlobalChat=true
bDisableGifting=false
bDisableGiftingPC=false
bDisableGiftingPS4=false
bDisableGiftingXB=false
bEnableInGameMatchmaking=true
bSkipTrailerMovie=true
bAlwaysPlayTrailerMovie=false
MaxPartySizeAthena=16
MaxPartySizeCampaign=16
MaxSquadSize=16
bAllowMimicingEmotes=true
!ExperimentalCohortPercent=ClearArray
+ExperimentalCohortPercent=(CohortPercent=100, ExperimentNum=20)

bShowStoreBanner=true
bEnableCatabaDynamicBackground=true
NewMtxStoreCohortSampleSet=100
+ExperimentalCohortPercent=(CohortPercent=100, ExperimentNum=14)
+ExperimentalCohortPercent=(CohortPercent=100, ExperimentNum=15)

ShowdownTournamentCacheExpirationHours=1
TournamentRefreshPlayerMaxRateSeconds=60
TournamentRefreshEventsMaxRateSeconds=60
TournamentRefreshPayoutMaxRateSeconds=60
";
        }

    }
}
