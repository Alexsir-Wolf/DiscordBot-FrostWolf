using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Reflection;
using Victoria;
using Victoria.EventArgs;
using WolfBotDiscord.Common;

namespace WolfBotDiscord.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _configuration;
        private readonly LavaNode _lavaNode;


        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration, LavaNode lavaNode)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _configuration = configuration;
            _lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessegeReceived;
            _service.CommandExecuted += OnCommandExecuted;
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _client.Ready += OnReadyAsync;

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
            {
                return;
            }

           await commandContext.Channel.SendMessageAsync($"Aconteceu o seguinte erro: \n" + result.ErrorReason);
        }

        private async Task OnMessegeReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        // AUTO PLAY
        public async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if (!args.Reason.ShouldPlayNext())
            {
                return;
            }

            var player = args.Player;

            if (!args.Player.Queue.TryDequeue(out var queueable))
            {
                await args.Player.TextChannel.SendMessageAsync("Fim da Playlist");        
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await args.Player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            var builder = new WolfBotEmbedBuilder()
                  .WithTitle($"Tocando agora: {track.Title}")
                  .AddField(" use !pause", "para pausar", true)
                  .AddField(" use !resume", "para continuar", true)
                  .WithCurrentTimestamp();
            var embed = builder.Build();
            await player.TextChannel.SendMessageAsync(null, false, embed);
        }
    }
}
