using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

using Victoria.EventArgs;

namespace WolfBotDiscord.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }


        //JOIN
        [Command("Join")]
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


        // SAIR DE SALA/CHAT
        [Command("leave")]
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


        //PLAY + AUTO JOIN  
        [Command("Play")]
        public async Task PlayAsync([Remainder] string query)
        {
            var voiceState = Context.User as IVoiceState;
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Digite a música:");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Entrei no chat : {voiceState.VoiceChannel.Name}!");
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"Não consegui encontrar nada para `{ query}`.");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                await ReplyAsync($"Na lista: {track.Title}");

            }
            else
            {
                var track = searchResponse.Tracks[0];

                await player.PlayAsync(track);

                await ReplyAsync($"Tocando agora: {track.Title}");
            }

        }


        //PROXIMA MUSICA
        [Command("proxima")]
        [Alias("prox", "next")]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
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
                await ReplyAsync($"Proxima! Tocando agora: {player.Track.Title}");
            }
        }


        //PAUSAR MUSICAS
        [Command("pause")]
        [Alias("pausa")]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
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

                await player.PauseAsync();
                await ReplyAsync($"Pausa na musica : {player.Track.Title}");
            }
        }


        //RESUMIR A PARTIR DA PAUSA
        [Command("resume")]
        [Alias("conti", "continuar")]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
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
                await ReplyAsync($"Continuando a musica : {player.Track.Title}");
            }

        }

    }
}




