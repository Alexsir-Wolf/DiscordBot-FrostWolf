using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WolfBotDiscord.Modules
{
    public class Moderation : ModuleBase
    {
        //DELETAR MENSAGENS !info + quantidade
        [Command("limpar")]
        [Alias("apagarmensagens")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var messege = await Context.Channel.SendMessageAsync($"{messages.Count()} Mensagens apagadas com sucesso!");
            await Task.Delay(3000);//ms
            await messege.DeleteAsync();
        }
    }
}
