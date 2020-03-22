using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YTUploader.Dto;

namespace YTUploader
{
    public class YTWrapper
    {
        private string _videoId;
        public bool Pronto { get; set; }    

        public string VideoId => _videoId;

        public async Task Upload(VideoDto dados)
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }
            
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = dados.Nome;
            video.Snippet.Description = dados.Descricao;
            video.Snippet.Tags = new string[] { "Teste", "Desenvolvimento" };
            video.Snippet.CategoryId = dados.Categoria.ToString(); // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
            //var filePath = @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

            using (var fileStream = new MemoryStream(dados.Conteudo))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }
 
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                break;

                case UploadStatus.Failed:
                Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine(video.Id);
            _videoId = video.Id;
            Pronto = true;
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}