﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Commands
{
    [Group("Set")]
    public class SetCommands : Command
    {
        private readonly IGuildManager _guildManager;

        public SetCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration) : base(translations, guildManager, groupManager, configuration)
        {
            _guildManager = guildManager;
        }

        [Command("Language")]
        public async Task Language(string languageCode)
        {
            if (!IsOwner)
            {
                return;
            }

            var guildConfiguration = await GetGuildConfiguration();

            guildConfiguration.LanguageCode = languageCode;
            await _guildManager.UpdateGuildConfiguration(guildConfiguration);
            await Context.Channel.SendMessageAsync($"{(await GetTranslation()).SetCommands.Language}: {languageCode}");
        }
    }
}