﻿using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Commands
{
    [Group("Twitch")]
    public class TwitchCommands : Command
    {
        private readonly ITwitchManager _twitchManager;
        private readonly IGroupManager _groupManager;
        private readonly IChannelManager _channelManager;

        public TwitchCommands(List<Translation> translations, IGuildManager guildManager, 
            IGroupManager groupManager, IConfiguration configuration, ITwitchManager twitchManager, IChannelManager channelManager) : base(translations, guildManager, groupManager, configuration)
        {
            _twitchManager = twitchManager;
            _channelManager = channelManager;
            _groupManager = groupManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string loginName)
        {
            var stringBuilder = new StringBuilder();
            var response = await _twitchManager.GetTwitchUserByLoginName(loginName);

            if(response != null)
            {
                stringBuilder.AppendLine($"Name: {response.Users[0].DisplayName}");
                stringBuilder.AppendLine($"Login: {response.Users[0].Login}");
                stringBuilder.AppendLine($"View Count: {response.Users[0].ViewCount}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a Twitch channel with that name does not exist.");
            }

            var builder = new EmbedBuilder();
            builder.Description = stringBuilder.ToString();

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("Add")]
        public async Task Add(string loginName)
        {
            await Add(loginName, "Default");
        }

        [Command("Add")]
        public async Task Add(string loginName, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var twitchChannel = await _twitchManager.GetTwitchUserByLoginName(loginName);

            if (twitchChannel == null || twitchChannel.Users.Count < 1)
            {
                await Context.Channel.SendMessageAsync("Sorry, a Twitch channel with that name does not exist.");
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            var groupChannel =
                await _channelManager.GetChannelByGuildGroupIdAndChannelId(group.Id, twitchChannel.Users[0].Id);

            if (groupChannel != null)
            {
                await Context.Channel.SendMessageAsync("Sorry, a this Twitch channel has already been added to this group.");
                return;
            }

            await _channelManager.AddChannel(new GuildGroupChannel
            { 
                ChannelId = twitchChannel.Users[0].Id,
                GuildGroupId = group.Id,
                Platform = Platform.Twitch
            });

            await Context.Channel.SendMessageAsync("This new Twitch channel has been added.");
        }
    }
}