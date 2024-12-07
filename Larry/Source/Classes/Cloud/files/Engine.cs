namespace Larry.Source.Classes.Cloud.files
{
    public class Engine
    {
        public static string GetDefaultEngine()
        {
            return @"
[OnlineSubsystemMcp.Xmpp]
bUseSSL=false
ServerAddr=""ws://127.0.0.1:443""
ServerPort=443
bPrivateChatFriendsOnly=true
bResetPingTimeoutOnReceiveStanza=true
bUsePlainTextAuth=true

[OnlineSubsystemMcp.Xmpp Prod]
bUseSSL=false
ServerAddr=""ws://127.0.0.1:443""
ServerPort=443
Domain=127.0.0.1

[OnlineSubsystemMcp]
bUsePartySystemV2=false

[OnlineSubsystemMcp.OnlinePartySystemMcpAdapter]
bUsePartySystemV2=false

[OnlineSubsystem]
bHasVoiceEnabled=true

[OnlineSubsystemMcp.OnlineIdentityMcp]
bAutoLoginToXmpp=true
bShouldReconnectXmpp=true
bOfflineAccountToken=true
bOfflineClientToken=true
bVerifyAuthIncludesPermissions=true

[Voice]
bEnabled=true

[XMPP]
bEnableWebsockets=true
bEnabled=true

[/Script/Engine.InputSettings]
+ConsoleKeys=Tilde
+ConsoleKeys=F8

[/Script/FortniteGame.FortPlayerController]
TurboBuildInterval=0.005f
TurboBuildFirstInterval=0.005f
bClientSideEditPrediction=false

[HTTP.Curl]
bAllowSeekFunction=false

[LwsWebSocket]
bDisableCertValidation=true

[ConsoleVariables]
FortMatchmakingV2.ContentBeaconFailureCancelsMatchmaking=0
Fort.ShutdownWhenContentBeaconFails=0
FortMatchmakingV2.EnableContentBeacon=0
;TODM Fix for External Game Servers (Adrenaline, FS_GS, etc)
net.AllowAsyncLoading=0

[Core.Log]
LogEngine=Verbose
LogStreaming=Verbose
LogNetDormancy=Verbose
LogNetPartialBunch=Verbose
OodleHandlerComponentLog=Verbose
LogSpectatorBeacon=Verbose
PacketHandlerLog=Verbose
LogPartyBeacon=Verbose
LogNet=Verbose
LogBeacon=Verbose
LogNetTraffic=Verbose
LogDiscordRPC=Verbose
LogEOSSDK=Verbose
LogXmpp=Verbose
LogParty=Verbose
LogParty=Verbose
LogMatchmakingServiceClient=Verbose
LogScriptCore=Verbose
LogSkinnedMeshComp=Verbose
LogFortAbility=Verbose
LogContentBeacon=Verbose
LogPhysics=Verbose
LogStreaming=Error

[/Script/Qos.QosRegionManager]
NumTestsPerRegion=2
PingTimeout=3.0
!DatacenterDefinitions=ClearArray
+DatacenterDefinitions=(Id=""VA"", RegionId=""NAE"", bEnabled=true, Servers[0]=(Address=""ping-nae.ds.on.epicgames.com"", Port=22222))
+DatacenterDefinitions=(Id=""DE"", RegionId=""EU"", bEnabled=true, Servers[0]=(Address=""ping-eu.ds.on.epicgames.com"", Port=22222))
";
        }

    }
}
