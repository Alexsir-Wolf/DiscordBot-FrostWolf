using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using WolfBotDiscord.Common;

namespace WolfBotDiscord.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        #region COMANDO JOIN
        [Command("Join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Já estou conectado em um canal de voz!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado em um canal de voz!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Entrei no chat : {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }
        #endregion

        #region COMANDO PARA SAIR DA SALA
        [Command("leave", RunMode = RunMode.Async)]
        [Alias("sair")]
        public async Task Leave()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var voiceState = Context.User as IVoiceState;
                try
                {
                    await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                    await ReplyAsync($"Pulei fora do chat : {voiceState.VoiceChannel.Name}!");
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }
        }
        #endregion

        #region PLAY E AUTO JOIN, REPETIR COMANDO ADD MUSICAS NA LISTA 
        [Command("Play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string query)
        {
            var voiceState = Context.User as IVoiceState;
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Entrei no chat : {voiceState.VoiceChannel.Name}!");
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed || searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"Não consegui encontrar nada para `{ query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);

                var builder = new WolfBotEmbedBuilder()
                 .WithTitle($"Na lista: {track.Title}")
                 .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var track = searchResponse.Tracks[0];
                var builder = new WolfBotEmbedBuilder()
                  .WithTitle($"Tocando agora: {track.Title}")
                  .AddField(" use !pause", "para pausar", true)
                  .AddField(" use !resume", "para continuar", true)
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);

                await player.PlayAsync(track);
            }
        }
        #endregion

        #region COMANDO PROXIMA MUSICA DA LISTA
        [Command("proxima", RunMode = RunMode.Async)]
        [Alias("prox", "next")]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado em um canal de voz!");
                return;
            }
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (voiceState.VoiceChannel != player.VoiceChannel)
                {
                    await ReplyAsync("Nós precisamos estar no mesmo canal de voz!");
                    return;
                }
                if (player.Queue.Count == 0)
                {
                    await ReplyAsync("Não há mais musicas na lista!");
                    return;
                }
                await player.SkipAsync();
                var builder = new WolfBotEmbedBuilder()
                      .WithTitle($"Tocando agora: {player.Track.Title}")
                      .AddField(" use !pause", "para pausar", true)
                      .AddField(" use !resume", "para continuar", true)
                      .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
        #endregion

        #region COMANDO PAUSAR MUSICAS
        [Command("pause", RunMode = RunMode.Async)]
        [Alias("pausa")]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado em um canal de voz!");
                return;
            }
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (voiceState.VoiceChannel != player.VoiceChannel)
                {
                    await ReplyAsync("Nós precisamos estar no mesmo canal de voz!");
                    return;
                }
                if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Paused)
                {
                    await ReplyAsync("Musica pausada!");
                }
                if (player.Queue.Count == 0)
                {
                    await ReplyAsync("Não há mais musicas para pausar!");
                    return;
                }
                await player.PauseAsync();
                var builder = new WolfBotEmbedBuilder()
                  .WithTitle($"Pausa em: {player.Track.Title}")
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
        #endregion

        #region COMANDO RESUMIR MUSICAS PAUSADAS
        [Command("resume", RunMode = RunMode.Async)]
        [Alias("conti", "continuar")]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado em um canal de voz!");
                return;
            }
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (voiceState.VoiceChannel != player.VoiceChannel)
                {
                    await ReplyAsync("Nós precisamos estar no mesmo canal de voz!");
                    return;
                }
                if (player.PlayerState == PlayerState.Playing)
                {
                    await ReplyAsync("Continando...");
                }

                await player.ResumeAsync();
                var builder = new WolfBotEmbedBuilder()
                  .WithTitle($"Continuando : {player.Track.Title}")
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
        #endregion

        #region COMANDO PARAR MSUICAS  
        [Command("stop", RunMode = RunMode.Async)]
        [Alias("parar")]
        public async Task Stop()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado em um canal de voz!");
                return;
            }
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (voiceState.VoiceChannel != player.VoiceChannel)
                {
                    await ReplyAsync("Nós precisamos estar no mesmo canal de voz!");
                    return;
                }
                if (player.PlayerState == PlayerState.Stopped)
                {
                    await ReplyAsync("Musica pausada!");
                }
                if (player.Queue.Count == 0)
                {
                    await ReplyAsync("Não há mais musicas para pausar!");
                    return;
                }

                await player.StopAsync();
                var builder = new WolfBotEmbedBuilder()
                  .WithTitle($"Musica parada: {player.Track.Title}")
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
        #endregion
    }
}





