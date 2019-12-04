namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter 
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone 
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong 
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var writerDto = JsonConvert.DeserializeObject<ImportWriterDto[]>(jsonString);

            var sb = new StringBuilder();
            List<Writer> writers = new List<Writer>();

            for (int i = 0; i < writerDto.Length; i++)
            {
                if (IsValid(writerDto[i]))
                {
                    var writer = new Writer()
                    {
                        Name = writerDto[i].Name,
                        Pseudonym = writerDto[i].Pseudonym
                    };

                    writers.Add(writer);
                    sb.AppendLine($"Imported {writer.Name}");
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context
                .Writers
                .AddRange(writers);

            context
                .SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var serializerSettings = new JsonSerializerSettings();
            //serializerSettings
            //    .Converters
            //    .Add(
            //    new IsoDateTimeConverter()
            //    { DateTimeFormat = "dd/MM/yyyy" , Culture = CultureInfo.InvariantCulture});

            var producersAlbums = JsonConvert
                .DeserializeObject<ImportProducerDto[]>(jsonString,serializerSettings);

            StringBuilder sb = new StringBuilder();
            List<Producer> producers = new List<Producer>();

            for (int i = 0; i < producersAlbums.Length; i++)
            {
                if (!IsValid(producersAlbums[i]))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                else
                {
                    Producer producer = new Producer()
                    {
                        Name = producersAlbums[i].Name,
                        PhoneNumber = producersAlbums[i].PhoneNumber,
                        Pseudonym = producersAlbums[i].Pseudonym,
                    };

                    List<Album> albums = new List<Album>();
                    int validAlbumsCount = producersAlbums[i].Albums.Length;

                    for (int j = 0; j < producersAlbums[i].Albums.Length; j++)
                    {
                        if (!IsValid(producersAlbums[i].Albums[j]))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        else
                        {
                            Album album = new Album()
                            {
                                Producer = producer,
                                Name = producersAlbums[i].Albums[j].Name,
                                ReleaseDate = DateTime
                                .ParseExact(
                                    producersAlbums[i].Albums[j].ReleaseDate,
                                    "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture)
                            };

                            albums.Add(album);
                        }
                    }

                    if (albums.Count == validAlbumsCount)
                    {
                        context
                            .Albums
                            .AddRange(albums);

                        producers.Add(producer);

                        if (string.IsNullOrEmpty(producersAlbums[i].PhoneNumber))
                        {
                            sb.AppendLine($"Imported {producer.Name} with no phone number produces {albums.Count} albums");
                        }
                        else
                        {
                            sb.AppendLine($"Imported {producer.Name} with phone: {producer.PhoneNumber} produces {albums.Count} albums");
                        }
                    }
                }
            }

            context
                .Producers
                .AddRange(producers);

            context
                .SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(
                typeof(ImportSongDto[]),
                new XmlRootAttribute("Songs"));

            var deserialziedObjects = (ImportSongDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            var albumIds = context
                .Albums
                .Select(a => a.Id)
                .ToList();

            var writersIds = context
                .Writers
                .Select(w => w.Id)
                .ToList();

            List<string> genres = new List<string>() { "Blues", "Rap", "PopMusic", "Rock","Jazz"};
            List<Song> songs = new List<Song>();


            foreach (var dto in deserialziedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (dto.AlbumId == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                int albumid = (int)dto.AlbumId;

                if (!albumIds.Contains(albumid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!writersIds.Contains(dto.WriterId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!genres.Contains(dto.Genre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                Song song = new Song()
                {
                    Name = dto.Name,
                    Duration = TimeSpan.Parse(dto.Duration),
                    CreatedOn = DateTime.ParseExact(dto.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Genre = (Genre)Enum.Parse(typeof(Genre), dto.Genre, true),
                    AlbumId = dto.AlbumId,
                    WriterId = dto.WriterId,
                    Price = dto.Price
                };

                songs.Add(song);
                sb.AppendLine($"Imported {song.Name} ({song.Genre} genre) with duration {song.Duration}");
            }

            context
                .Songs
                .AddRange(songs);

            context
                .SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var res = Validator.TryValidateObject(obj, validator, validationRes, true);

            return res;
        }
    }
}