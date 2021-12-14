using Discord;

namespace WolfBotDiscord.Common
{    
    // Embed Builder tema customizado   
    class WolfBotEmbedBuilder : EmbedBuilder 
    {
        public WolfBotEmbedBuilder()
        {
            WithColor(new Color(0, 166, 255));
        }
    }
}
