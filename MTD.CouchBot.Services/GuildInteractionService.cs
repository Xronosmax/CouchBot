﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Services
{
    public class GuildInteractionService
    {
        private readonly DiscordSocketClient _discord;
        private readonly LoggingService _loggingService;
        private readonly IGuildManager _guildManager;
        private readonly IGroupManager _groupManager;

        public GuildInteractionService(DiscordSocketClient discord, LoggingService loggingService, IGuildManager guildManager, IGroupManager groupManager)
        {
            _discord = discord;
            _loggingService = loggingService;
            _guildManager = guildManager;
            _groupManager = groupManager;
        }

        public void Init()
        {
            _discord.JoinedGuild += _discord_JoinedGuild;
            _discord.LeftGuild += _discord_LeftGuild;
        }

        private async Task _discord_LeftGuild(SocketGuild arg)
        {
            var guild = await _guildManager.GetGuildById(arg.Id);

            if (guild != null)
            {
                await _guildManager.DeleteGuild(guild);
            }

            var guildConfiguration = await _guildManager.GetGuildConfigurationByGuildId(arg.Id);

            if (guild != null)
            {
                await _guildManager.DeleteGuildConfiguration(guildConfiguration);
            }

            var defaultGroup = await _groupManager.GetGuildGroupByGuildIdAndName(arg.Id, "Default");

            if (defaultGroup != null)
            {
                await _groupManager.DeleteGuildGroup(defaultGroup);
            }
        }

        private async Task _discord_JoinedGuild(SocketGuild arg)
        {
            var guild = await _guildManager.GetGuildById(arg.Id);

            if (guild == null)
            {
                await _guildManager.CreateGuild(new Guild
                {
                    GuildId = Cryptography.Encrypt(arg.Id.ToString()),
                    OwnerId = Cryptography.Encrypt(arg.OwnerId.ToString()),
                    CreatedDate = DateTime.UtcNow
                });
            }

            var guildConfiguration = await _guildManager.GetGuildConfigurationByGuildId(arg.Id);

            if (guildConfiguration == null)
            {
                await _guildManager.CreateGuildConfiguration(new GuildConfiguration
                {
                    GuildId = Cryptography.Encrypt(arg.Id.ToString()),
                    LanguageCode = "en-US"
                });
            }

            var defaultGroup = await _groupManager.GetGuildGroupByGuildIdAndName(arg.Id, "Default");

            if (defaultGroup == null)
            {
                await _groupManager.CreateGuildGroup(new GuildGroup
                {
                    GuildId = Cryptography.Encrypt(arg.Id.ToString()),
                    Name = "Default"
                });
            }
        }
    }
}