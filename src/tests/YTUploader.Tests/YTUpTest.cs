using System;
using System.Threading.Tasks;
using Xunit;
using YTUploader;
using YTUploader.Dto;

namespace YTUploader.Tests
{
    public class YTUpYTest
    {
        [Fact]
        public async Task Teste_DeveEnviarVideoContaTeste()
        {
            Console.WriteLine("Inicio do teste");
            VideoDto dados = new VideoDto()
            {
                Nome = "Teste Upload",
                Categoria = 22,
                Descricao = "Teste de Descrição de Upload",
                Conteudo = new byte[0]
            };

            Console.WriteLine(dados);

            var videoUploader = new YTEnvio();
            await videoUploader.Upload(dados);
            Console.WriteLine(videoUploader.Pronto);

            Assert.True(videoUploader.Pronto);
            Assert.NotEqual(string.Empty, videoUploader.VideoId);
        }
    }
}
