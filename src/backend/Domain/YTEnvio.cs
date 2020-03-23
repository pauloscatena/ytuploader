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
    public class YTEnvio
    {
        private string _videoId;
        public bool Pronto { get; set; }    

        public string VideoId => _videoId;

        public async Task Upload(VideoDto dados)
        {
            UserCredential credencial;

            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credencial = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }
            
            var servicoYoutube = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credencial,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var privacidade = string.Empty;

            switch(dados.Privacidade)
            {
                case EPrivacidade.Privado:
                    privacidade = "private";
                    break;
                case EPrivacidade.Publico:
                    privacidade = "public";
                    break;
                case EPrivacidade.NaoListado:
                    privacidade = "unlisted";
                    break;
            }

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = dados.Nome;
            video.Snippet.Description = dados.Descricao;
            video.Snippet.Tags = dados.Tags.ToArray();
            video.Snippet.CategoryId = dados.Categoria.ToString(); // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = privacidade; // or "private" or "public"
            //var filePath = @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

            using (var fileStream = new MemoryStream(dados.Conteudo))
            {
                var videosInsertRequest = servicoYoutube.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }
 
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progresso)
        {
            switch (progresso.Status)
            {
                case UploadStatus.Uploading:
                Console.WriteLine("{0} bytes enviados.", progresso.BytesSent);
                break;

                case UploadStatus.Failed:
                Console.WriteLine("Ocorreu um erro impedindo o upload do video.\n{0}", progresso.Exception);
                break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine(video.Id);
            _videoId = video.Id;
            Pronto = true;
            Console.WriteLine("Video id '{0}' enviado com sucesso.", video.Id);
        }
    }
}