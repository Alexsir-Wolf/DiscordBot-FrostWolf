using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using Victoria;
using WolfBotDiscord.Common;
using Discord;
using System;

namespace WolfBotDiscord.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        #region COMANDO PING
        [Command("ping")]
        [Alias("p")] //apelido abreviado para o comando, pode ser add varios valores.         
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }

        [Command("data")]
        public async Task Data()
        {
            await Context.Channel.SendMessageAsync(DateTime.Now.ToString("dddd, dd MMMM yyyy"));
        }
        #endregion

        #region COMANDO INFOPMAÇÕES DO SERVER
        [Command("server")]
        public async Task Server()
        {
            var builder = new WolfBotEmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"Informações sobre {Context.Guild.Name}")
                //.WithDescription("Informações sobre o servidor: ")   
                .AddField("Criado em ", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Membros ", (Context.Guild as SocketGuild).MemberCount + " membros", true)
                .AddField("Online ", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Online).Count() + " membros", true);   
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        #endregion

        #region COMANDO INFORMAÇÕES DO USUÁRIO
        // Puxa info dos usuarios e mostra em um card
        [Command("info")]
        public async Task InfoAsync(SocketGuildUser socketGuildUser = null)
        {
            if (socketGuildUser == null)
            {
                var builder = new WolfBotEmbedBuilder()
                  .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                  .WithTitle($"Informações sobre {Context.User.Username}")
                  .AddField("Nome do usuário", Context.User.Username, true)//TRUE = info na mesma linha.
                  .AddField("#", Context.User.Discriminator, true)
                  .AddField("Criado em: ", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                  .AddField("Entrou no server em: ", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                  .AddField("Cargo", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new WolfBotEmbedBuilder()
                  .WithThumbnailUrl(socketGuildUser.GetAvatarUrl() ?? socketGuildUser.GetDefaultAvatarUrl())
                  .WithTitle($"Informações sobre {socketGuildUser.Username}")
                  .AddField("Nome do usuário", socketGuildUser.Username, true)//TRUE = info na mesma linha.
                  .AddField("#", socketGuildUser.Discriminator, true)
                  .AddField("Criado em: ", socketGuildUser.CreatedAt.ToString("dd/MM/yyyy"), true)
                  .AddField("Entrou no server em: ", socketGuildUser.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                  .AddField("Cargo", string.Join(" ", socketGuildUser.Roles.Select(x => x.Mention)))
                  .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
        #endregion

        #region COMANDO AJUDA NOS COMANDOS
        [Command("help")]
        [Alias("ajuda", "h")]
        public async Task HelpAsync()
        {
            var builder = new WolfBotEmbedBuilder()
              .WithThumbnailUrl(Context.Guild.IconUrl)
              .WithTitle($"Ajuda")
              .AddField("Hello there!", "Eu sou o Frost, seu lobo ajudante!")
              .AddField("!ping", "retorna um Pong!")
              .AddField("!play + **musica ou podcast**", "procura musica ou pocast. Repetir o comando, add musicas para serem tocadas em seguida.")
              .AddField("!pause", "pause na musica.")
              .AddField("!resume", "resume a musica do momento do pause.")
              .AddField("!prox", "pula para a proxima musica da lista")
              .AddField("!limpar + quantidade", "deleta mensagens do chat")
              .WithCurrentTimestamp();
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        #endregion

        #region COMANDO MELHOR GAME
        [Command("melhorgame")]
        public async Task BestGame()
        {
            await Context.Channel.SendMessageAsync("https://www.elderscrollsonline.com/en-us/home");
        }
        #endregion
    }
}
