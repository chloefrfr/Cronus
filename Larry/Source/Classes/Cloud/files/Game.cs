namespace Larry.Source.Classes.Cloud.files
{
    public class Game
    {
        public static string GetDefaultGame(int version)
        {
            string def = @"
[/Script/EngineSettings.GeneralProjectSettings]
ProjectID=(A=-2011270876,B=1182903154,C=-965786730,D=-1399474123)
ProjectName=Fortnite
ProjectDisplayedTitle=NSLOCTEXT(""Larry"", ""FortniteMainWindowTitle"", ""Larry"")
ProjectVersion=1.0.0
CompanyName=Epic Games, Inc.
CompanyDistinguishedName=""CN=Epic Games, O=Epic Games, L=Cary, S=North Carolina, C=US""
CopyrightNotice=Copyright Epic Games, Inc. All Rights Reserved.
bUseBorderlessWindow=True

[VoiceChatManager]
bEnabled=true
bEnableOnLoadingScreen=true
bObtainJoinTokenFromPartyService=true
bAllowStateTransitionOnLoadingScreen=false
MaxRetries=5
RetryTimeJitter=1.0
RetryTimeBase=3.0
RetryTimeMultiplier=1.0
MaxRetryDelay=240.0
RequestJoinTokenTimeout=10.0
JoinChannelTimeout=120.0
VoiceChatImplementation=Vivox
NetworkTypePollingDelay=0.0
PlayJoinSoundRecentLeaverDelaySeconds=30.0
DefaultInputVolume=1.0
DefaultOutputVolume=1.0
JoinTimeoutRecoveryMethod=Reinitialize
JoinErrorWorkaroundMethod=ResetConnection
NetworkChangeRecoveryMethod=ResetConnection
bEnableBluetoothMicrophone=false
VideoPreferredFramerate=0
bEnableEOSReservedAudioStreams=true

[VoiceChat.EOS]
bEnabled=true

ReplayStreamerOverride=FortniteDSSReplayStreamer

[/Script/FortniteGame.FortPlayspaceGameState]
bUsePlayspaceSystem=true

[/Script/FortniteGame.FortGameStateAthena]
bAllowBuildingThroughBlockingObjects=true

[/Script/FortniteGame.FortGameInstance]
KairosMinSupportedAppVersion=20
bBattleRoyaleMatchmakingEnabled=true
!FrontEndPlaylistData=ClearArray
FrontEndPlaylistData=(PlaylistName=Playlist_DefaultSolo, PlaylistAccess=(bEnabled=True, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=false, CategoryIndex=0, bDisplayAsLimitedTime=false, DisplayPriority=3))
+FrontEndPlaylistData=(PlaylistName=Playlist_DefaultDuo, PlaylistAccess=(bEnabled=True, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=false, CategoryIndex=0, bDisplayAsLimitedTime=false, DisplayPriority=4))
+FrontEndPlaylistData=(PlaylistName=Playlist_DefaultSquad, PlaylistAccess=(bEnabled=True, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=false, CategoryIndex=0, bDisplayAsLimitedTime=false, DisplayPriority=6))
+FrontEndPlaylistData=(PlaylistName=Playlist_PlaygroundV2, PlaylistAccess=(bEnabled=False, bIsDefaultPlaylist=false, bVisibleWhenDisabled=true, bDisplayAsNew=true, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=15))
+FrontEndPlaylistData=(PlaylistName=Playlist_BattleLab, PlaylistAccess=(bEnabled=False, bIsDefaultPlaylist=false, bVisibleWhenDisabled=true, bDisplayAsNew=false, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=16))

; Arena
+FrontEndPlaylistData=(PlaylistName=Playlist_ShowdownAlt_Solo, PlaylistAccess=(bEnabled=True, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=true, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=17))
+FrontEndPlaylistData=(PlaylistName=Playlist_ShowdownAlt_Duos, PlaylistAccess=(bEnabled=False, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=true, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=19))
+ExperimentalBucketPercentList=(ExperimentNum=23,Name=""BattlePassPurchaseScreen"",BucketPercents=(0, 50, 50))

[/Script/FortniteGame.FortPlayerPawn]
NavLocationValidityDistance=500
MoveSoundStimulusBroadcastInterval=0.5
bCharacterPartsCastIndirectShadows=true

[/Script/FortniteGame.FortOnlineAccount]
bShouldAthenaQueryRecentPlayers=false
bDisablePurchasingOnRedemptionFailure=false

[/Script/FortniteGame.FortPlayerControllerAthena]
bNoInGameMatchmaking=true

[/Script/GameFeatures.GameFeaturesSubsystemSettings]
+DisabledPlugins=DiscoveryBrowser

[VoiceChat.EOS]
bEnabled=true

[/Script/Account.OnlineAccountCommon]
bEnableWaitingRoom=false

[/Script/FortniteGame.FortChatManager]
bShouldRequestGeneralChatRooms=true
bShouldJoinGlobalChat=true
bShouldJoinFounderChat=false
bIsAthenaGlobalChatEnabled=true
RecommendChatFailureDelay=30
RecommendChatBackoffMultiplier=2.0
RecommendChatRandomWindow=120.0
RecommendChatFailureCountCap=7

[OnlinePartyEmbeddedCommunication]
bRetainPartyStateFields=false
bPerformAutoPromotion=true
InviteNotificationDelaySeconds=1.0

[/Script/Party.SocialSettings]
bMustSendPrimaryInvites=true
bLeavePartyOnDisconnect=false
bSetDesiredPrivacyOnLocalPlayerBecomesLeader=false
DefaultMaxPartySize=16";


            if (version >= 4 && version <= 10)
            {
                def = def.Replace(
                    @"+FrontEndPlaylistData=(PlaylistName=Playlist_BattleLab, PlaylistAccess=(bEnabled=False, bIsDefaultPlaylist=false, bVisibleWhenDisabled=true, bDisplayAsNew=false, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=16))",
                    @"+FrontEndPlaylistData=(PlaylistName=Playlist_Playground, PlaylistAccess=(bEnabled=True, bIsDefaultPlaylist=true, bVisibleWhenDisabled=false, bDisplayAsNew=true, CategoryIndex=1, bDisplayAsLimitedTime=false, DisplayPriority=16))"
                );
            }

            return def;
        }
    }
}
