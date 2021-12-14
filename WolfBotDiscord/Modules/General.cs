using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using WolfBotDiscord.Common;

namespace WolfBotDiscord.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        // Comando Ping, responde com Pong
        [Command("ping")]
        [Alias("p")] //apelido abreviado para o comando, pode ser add varios valores.         
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }
        
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
    }
}
