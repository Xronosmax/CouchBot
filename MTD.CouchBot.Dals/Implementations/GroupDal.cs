﻿using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using MTD.CouchBot.Domain.Dtos.Discord;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MTD.CouchBot.Dals.Implementations
{
    public class GroupDal : IGroupDal
    {
        private readonly IConfiguration _configuration;

        public GroupDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GuildGroup> GetGuildGroupByGuildIdAndName(string guildId, string name)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroups WHERE GuildId = @GuildId AND Name = @GuildName";

                return await connection.QueryFirstOrDefaultAsync<GuildGroup>(query, new { GuildId = guildId, GuildName = name });
            }
        }

        public async Task<List<GuildGroup>> GetGuildGroupsByGuildId(string guildId)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroups WHERE GuildId = @GuildId";

                return (await connection.QueryAsync<GuildGroup>(query, new { GuildId = guildId })).ToList();
            }
        }

        public async Task CreateGuildGroup(GuildGroup guildGroup)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.InsertAsync(guildGroup);
            }
        }

        public async Task UpdateGuildGroup(GuildGroup guildGroup)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.UpdateAsync(guildGroup);
            }
        }

        public async Task DeleteGuildGroup(GuildGroup guildGroup)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.DeleteAsync(guildGroup);
            }
        }
    }
}