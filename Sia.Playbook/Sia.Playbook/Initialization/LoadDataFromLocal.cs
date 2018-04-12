using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Sia.Domain.Playbook;
using Sia.Shared.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public static class LoadDataFromLocal
    {
        public static void AddSeedDataFromLocal<T>(
            this Dictionary<long, T> index,
            ILogger logger,
            string path
        )
            where T: class, IEntity
        {
            FileInfo[] files = null;

            var directory = new DirectoryInfo(path);

            try
            {
                files = directory.GetFiles("*.json");
                index.AddSeedDataToDictionary(files.Select(TryDeserialize<T>(logger)), logger);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failure to read from path", new [] { path });
            }
        }

        private static Func<FileInfo, T> TryDeserialize<T>(ILogger logger)
            where T : class
        {
            T tryDeserialize(FileInfo file)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(file.FullName));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failure when trying to deserialize {file.Name} as json", new object[] { });
                }
                return null;
            }
            return tryDeserialize;
        }
    }
}
