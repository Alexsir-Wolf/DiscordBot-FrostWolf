//using System;
//using System.Collections.Concurrent;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Addons.Hosting;
//using Discord.Commands;
//using Discord.WebSocket;
//using Victoria;
//using Victoria.EventArgs;
//using WolfBotDiscord.Common;

//namespace WolfBotDiscord.Services
//{
//	public class MusicHandler : InitializedService
//	{
//		private readonly LavaNode _lavaNode;
//		private readonly DiscordSocketClient _client;
//		private readonly IServiceProvider _provider;
//		private readonly CommandService _service;
//		private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

//		public MusicHandler(LavaNode lavaNode, DiscordSocketClient client, IServiceProvider provider, CommandService service)
//		{
//			_lavaNode = lavaNode;
//			_client = client;
//			_provider = provider;
//			_service = service;
//			_disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();
//		}

//		public override async Task InitializeAsync(CancellationToken cancellationToken)
//		{
//			_client.Ready += OnReadyAsync;

//			//_lavaNode.OnTrackStarted += OnTrackStarted;
//			_lavaNode.OnTrackEnded += OnTrackEnded;

//			await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
//		}

//		private async Task OnReadyAsync()
//		{
//			if (!_lavaNode.IsConnected)
//			{
//				await _lavaNode.ConnectAsync();
//			}
//		}

//		private async Task OnTrackEnded(TrackEndedEventArgs args)
//		{
//			if (!args.Reason.ShouldPlayNext())
//			{
//				return;
//			}

//			var player = args.Player;
//			if (!player.Queue.TryDequeue(out var queueable))
//			{
//				//await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
//				_ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(300));
//				return;
//			}

//			if (!(queueable is LavaTrack track))
//			{
//				await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
//				return;
//			}

//			await player.PlayAsync(track);
//			var builder = new WolfBotEmbedBuilder()
//				  .WithTitle($"Tocando agora: {track.Title}")
//				  .AddField(" use !pause", "para pausar", true)
//				  .AddField(" use !resume", "para continuar", true)
//				  .WithCurrentTimestamp();
//			var embed = builder.Build();

//			await player.TextChannel.SendMessageAsync(null, false, embed);
//		}

//		private async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
//		{
//			if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value))
//			{
//				value = new CancellationTokenSource();
//				_disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
//			}
//			else if (value.IsCancellationRequested)
//			{
//				_disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
//				value = _disconnectTokens[player.VoiceChannel.Id];
//			}

//			// await player.TextChannel.SendMessageAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...");
//			var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
//			if (isCancelled)
//			{
//				return;
//			}

//			await _lavaNode.LeaveAsync(player.VoiceChannel);
//			//await player.TextChannel.SendMessageAsync("Bye Bye.");
//		}
//	}
//}